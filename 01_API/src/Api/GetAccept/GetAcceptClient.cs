// <copyright file="GetAcceptClient.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.AspNetCore.GetAccept;

using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using KPMG.Pulse.Back.Accounting.Mandate.Application.Interfaces;
using KPMG.Pulse.Back.Accounting.Mandate.Application.Models;
using Microsoft.Extensions.Options;

/// <inheritdoc />
public sealed class GetAcceptClient(
    HttpClient httpClient,
    IOptions<GetAcceptOptions> options,
    ILogger<GetAcceptClient> logger) : IGetAcceptClient
{
    private const string DocumentType = "other";
    private const string SignerRole = "signer";
    private static readonly TimeSpan AccessTokenRefreshSkew = TimeSpan.FromMinutes(1);
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private readonly SemaphoreSlim authenticationLock = new(1, 1);
    private string? accessToken;
    private DateTimeOffset accessTokenExpiresAt;

    /// <inheritdoc />
    public async Task<GetAcceptMandateSignatureResponse> SendMandateForSignatureAsync(
        GetAcceptMandateSignatureRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var accessToken = await AuthenticateAsync();
        var document = await CreateDocumentAsync(request, accessToken);
        var signatureUrl = await GetSignatureUrlAsync(document.Id, accessToken);

        return new GetAcceptMandateSignatureResponse(document.Id, signatureUrl);
    }

    /// <inheritdoc />
    public async Task<GetAcceptDocumentStatusResponse> GetDocumentStatusAsync(string signatureRequestId)
    {
        if (string.IsNullOrWhiteSpace(signatureRequestId))
        {
            throw new ArgumentException("The GetAccept signature request identifier is required.", nameof(signatureRequestId));
        }

        var accessToken = await AuthenticateAsync();
        using var httpRequest = new HttpRequestMessage(
            HttpMethod.Get,
            $"v1/documents/{Uri.EscapeDataString(signatureRequestId)}");
        httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        using var response = await httpClient.SendAsync(httpRequest);
        response.EnsureSuccessStatusCode();

        var document = await response.Content.ReadFromJsonAsync<GetAcceptDocumentStatusPayload>()
            ?? throw new InvalidOperationException("GetAccept document status response was empty.");

        return new GetAcceptDocumentStatusResponse(document.Status, document.DownloadUrl);
    }

    /// <inheritdoc />
    public async Task<GetAcceptSignedDocument> DownloadSignedDocumentAsync(string signedDocumentUrl)
    {
        if (string.IsNullOrWhiteSpace(signedDocumentUrl))
        {
            throw new ArgumentException("The GetAccept signed document URL is required.", nameof(signedDocumentUrl));
        }

        using var httpRequest = new HttpRequestMessage(HttpMethod.Get, signedDocumentUrl);
        if (RequiresGetAcceptAuthorization(signedDocumentUrl))
        {
            var bearerToken = await AuthenticateAsync();
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
        }

        using var response = await httpClient.SendAsync(httpRequest);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsByteArrayAsync();
        var contentType = response.Content.Headers.ContentType?.MediaType ?? "application/pdf";
        var fileName = response.Content.Headers.ContentDisposition?.FileNameStar
            ?? response.Content.Headers.ContentDisposition?.FileName?.Trim('"')
            ?? "mandate-sepa-signed.pdf";

        return new GetAcceptSignedDocument(content, contentType, fileName);
    }

    private async Task<string> AuthenticateAsync()
    {
        EnsureConfigured();

        if (HasValidAccessToken())
        {
            return accessToken!;
        }

        await authenticationLock.WaitAsync();
        try
        {
            if (HasValidAccessToken())
            {
                return accessToken!;
            }

            using var response = await httpClient.PostAsJsonAsync(
                "v1/auth",
                new GetAcceptAuthRequest(options.Value.Email, options.Value.Password));

            response.EnsureSuccessStatusCode();
            var auth = await response.Content.ReadFromJsonAsync<GetAcceptAuthResponse>()
                ?? throw new InvalidOperationException("GetAccept authentication response was empty.");

            if (string.IsNullOrWhiteSpace(auth.AccessToken))
            {
                throw new InvalidOperationException("GetAccept authentication response did not include an access token.");
            }

            accessToken = auth.AccessToken;
            accessTokenExpiresAt = DateTimeOffset.UtcNow.AddSeconds(auth.ExpiresIn);
            return accessToken;
        }
        finally
        {
            authenticationLock.Release();
        }
    }

    private bool HasValidAccessToken()
    {
        return !string.IsNullOrWhiteSpace(accessToken)
            && accessTokenExpiresAt > DateTimeOffset.UtcNow.Add(AccessTokenRefreshSkew);
    }

    private bool RequiresGetAcceptAuthorization(string signedDocumentUrl)
    {
        if (!Uri.TryCreate(signedDocumentUrl, UriKind.Absolute, out var downloadUri))
        {
            return true;
        }

        return httpClient.BaseAddress is null
            || string.Equals(downloadUri.Host, httpClient.BaseAddress.Host, StringComparison.OrdinalIgnoreCase);
    }

    private async Task<GetAcceptDocumentResponse> CreateDocumentAsync(
        GetAcceptMandateSignatureRequest request,
        string accessToken)
    {
        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "v1/documents")
        {
            Content = JsonContent.Create(new GetAcceptCreateDocumentRequest(
                "Mandat SEPA",
                DocumentType,
                Convert.ToBase64String(request.FileContent),
                request.FileName,
                true,
                true,
                [
                    new GetAcceptRecipientRequest(
                        request.Recipient.Email,
                        request.Recipient.FirstName,
                        request.Recipient.LastName,
                        SignerRole,
                        false,
                        false)
                ]))
        };
        httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        using var response = await httpClient.SendAsync(httpRequest);
        response.EnsureSuccessStatusCode();

        var document = await response.Content.ReadFromJsonAsync<GetAcceptDocumentResponse>()
            ?? throw new InvalidOperationException("GetAccept document creation response was empty.");

        if (string.IsNullOrWhiteSpace(document.Id))
        {
            throw new InvalidOperationException("GetAccept document creation response did not include a document id.");
        }

        return document;
    }

    private async Task<string> GetSignatureUrlAsync(string documentId, string accessToken)
    {
        var maxAttempts = Math.Max(1, options.Value.SignatureUrlMaxAttempts);
        var retryDelay = TimeSpan.FromMilliseconds(Math.Max(0, options.Value.SignatureUrlRetryDelayMilliseconds));

        for (var attempt = 1; attempt <= maxAttempts; attempt++)
        {
            using var httpRequest = new HttpRequestMessage(
                HttpMethod.Get,
                $"v1/documents/{Uri.EscapeDataString(documentId)}/recipients");
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            using var response = await httpClient.SendAsync(httpRequest);
            response.EnsureSuccessStatusCode();

            var recipientsPayload = await response.Content.ReadAsStringAsync();
            var recipientsResponse = JsonSerializer.Deserialize<GetAcceptRecipientsResponse>(recipientsPayload, JsonOptions)
                ?? throw new InvalidOperationException("GetAccept recipients response was empty.");

            if (logger.IsEnabled(LogLevel.Debug))
            {
                var diagnosticPayload = JsonSerializer.Serialize(
                    new
                    {
                        Recipients = recipientsResponse.Recipients.Select(recipient => new
                        {
                            recipient.Role,
                            recipient.Status,
                            HasDocumentUrl = !string.IsNullOrWhiteSpace(recipient.DocumentUrl)
                        })
                    },
                    JsonOptions);

                logger.LogDebug(
                    "GetAccept recipients diagnostic for document {DocumentId} on attempt {Attempt}/{MaxAttempts}. Payload: {RecipientsPayload}",
                    documentId,
                    attempt,
                    maxAttempts,
                    diagnosticPayload);
            }

            var signatureUrl = recipientsResponse.Recipients
                .Where(recipient => string.Equals(recipient.Role, SignerRole, StringComparison.OrdinalIgnoreCase))
                .Select(recipient => recipient.DocumentUrl)
                .FirstOrDefault(candidate => !string.IsNullOrWhiteSpace(candidate));

            if (!string.IsNullOrWhiteSpace(signatureUrl))
            {
                return signatureUrl;
            }

            if (attempt < maxAttempts)
            {
                await Task.Delay(retryDelay);
            }
        }

        logger.LogError(
            "GetAccept recipients response for document {DocumentId} did not contain a signer document URL after {MaxAttempts} attempts.",
            documentId,
            maxAttempts);
        throw new InvalidOperationException("GetAccept recipients response did not include a signer document URL.");
    }

    private void EnsureConfigured()
    {
        if (string.IsNullOrWhiteSpace(options.Value.Email)
            || string.IsNullOrWhiteSpace(options.Value.Password))
        {
            throw new InvalidOperationException("GetAccept credentials are not configured.");
        }
    }

    private sealed record GetAcceptAuthRequest(
        [property: JsonPropertyName("email")] string Email,
        [property: JsonPropertyName("password")] string Password);

    private sealed record GetAcceptAuthResponse(
        [property: JsonPropertyName("access_token")] string AccessToken,
        [property: JsonPropertyName("expires_in")] int ExpiresIn);

    private sealed record GetAcceptCreateDocumentRequest(
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("type")] string Type,
        [property: JsonPropertyName("file_content")] string FileContent,
        [property: JsonPropertyName("file_name")] string FileName,
        [property: JsonPropertyName("is_signing")] bool IsSigning,
        [property: JsonPropertyName("is_automatic_sending")] bool IsAutomaticSending,
        [property: JsonPropertyName("recipients")] IReadOnlyCollection<GetAcceptRecipientRequest> Recipients);

    private sealed record GetAcceptRecipientRequest(
        [property: JsonPropertyName("email")] string Email,
        [property: JsonPropertyName("first_name")] string FirstName,
        [property: JsonPropertyName("last_name")] string LastName,
        [property: JsonPropertyName("role")] string Role,
        [property: JsonPropertyName("verify_sms_sign")] bool VerifySmsSign = false,
        [property: JsonPropertyName("verify_sms_open")] bool VerifySmsOpen = false);

    private sealed record GetAcceptDocumentResponse(
        [property: JsonPropertyName("id")] string Id);

    private sealed record GetAcceptDocumentStatusPayload(
        [property: JsonPropertyName("status")] string? Status,
        [property: JsonPropertyName("download_url")] string? DownloadUrl);

    private sealed record GetAcceptRecipientsResponse(
        [property: JsonPropertyName("recipients")] IReadOnlyCollection<GetAcceptRecipientResponse> Recipients);

    private sealed record GetAcceptRecipientResponse(
        [property: JsonPropertyName("role")] string Role,
        [property: JsonPropertyName("status")] string? Status,
        [property: JsonPropertyName("document_url")] string? DocumentUrl);
}
