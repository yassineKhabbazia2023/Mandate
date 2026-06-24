// <copyright file="GetAcceptClient.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.AspNetCore.GetAccept;

using System.Net.Http.Headers;
using System.Net.Http.Json;
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

    private async Task<string> AuthenticateAsync()
    {
        EnsureConfigured();

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

        return auth.AccessToken;
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
        using var httpRequest = new HttpRequestMessage(
            HttpMethod.Get,
            $"v1/documents/{Uri.EscapeDataString(documentId)}/recipients");
        httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        using var response = await httpClient.SendAsync(httpRequest);
        response.EnsureSuccessStatusCode();

        var recipientsResponse = await response.Content.ReadFromJsonAsync<GetAcceptRecipientsResponse>()
            ?? throw new InvalidOperationException("GetAccept recipients response was empty.");

        var signatureUrl = recipientsResponse.Recipients
            .Where(recipient => string.Equals(recipient.Role, SignerRole, StringComparison.OrdinalIgnoreCase))
            .Select(recipient => recipient.DocumentUrl)
            .FirstOrDefault(candidate => !string.IsNullOrWhiteSpace(candidate));

        if (string.IsNullOrWhiteSpace(signatureUrl))
        {
            logger.LogError(
                "GetAccept recipients response for document {DocumentId} did not contain a signer document URL",
                documentId);
            throw new InvalidOperationException("GetAccept recipients response did not include a signer document URL.");
        }

        return signatureUrl;
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
        [property: JsonPropertyName("verify_sms_sign")] bool VerifySmsSign = false);

    private sealed record GetAcceptDocumentResponse(
        [property: JsonPropertyName("id")] string Id);

    private sealed record GetAcceptRecipientsResponse(
        [property: JsonPropertyName("recipients")] IReadOnlyCollection<GetAcceptRecipientResponse> Recipients);

    private sealed record GetAcceptRecipientResponse(
        [property: JsonPropertyName("role")] string Role,
        [property: JsonPropertyName("document_url")] string? DocumentUrl);
}
