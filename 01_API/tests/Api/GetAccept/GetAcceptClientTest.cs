// <copyright file="GetAcceptClientTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.AspNetCore.Tests.GetAccept;

using System.Net;
using System.Text.Json;
using KPMG.Pulse.Back.Accounting.Mandate.Application.Models;
using KPMG.Pulse.Back.Accounting.Mandate.AspNetCore.GetAccept;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

/// <summary>
/// Unit tests for <see cref="GetAcceptClient"/>.
/// </summary>
public sealed class GetAcceptClientTest
{
    /// <summary>
    /// Verifies the full GetAccept call sequence and the document creation payload contract.
    /// </summary>
    [Fact]
    public async Task SendMandateForSignatureAsync_WhenGetAcceptReturnsSignerUrl_ReturnsDocumentIdAndSignatureUrl()
    {
        var handler = new RecordingHandler(
            request =>
            {
                if (request.RequestUri!.PathAndQuery == "/v1/auth")
                {
                    return JsonResponse("""{"access_token":"token-123","expires_in":3600}""");
                }

                if (request.RequestUri!.PathAndQuery == "/v1/documents")
                {
                    return JsonResponse("""{"id":"doc-123"}""");
                }

                if (request.RequestUri!.PathAndQuery == "/v1/documents/doc-123/recipients")
                {
                    return JsonResponse(
                        """{"recipients":[{"role":"viewer","document_url":"https://viewer.test"},{"role":"signer","document_url":"https://signature.test"}]}""");
                }

                return new HttpResponseMessage(HttpStatusCode.NotFound);
            });
        var client = CreateClient(handler);

        var result = await client.SendMandateForSignatureAsync(new GetAcceptMandateSignatureRequest(
            [1, 2, 3],
            "mandat.pdf",
            new SepaRecipient("jean.dupont@test.fr", "Jean", "Dupont")));

        result.SignatureRequestId.Should().Be("doc-123");
        result.SignatureUrl.Should().Be("https://signature.test");
        handler.Requests.Should().HaveCount(3);
        handler.Requests[0].Method.Should().Be(HttpMethod.Post);
        handler.Requests[0].RequestUri!.PathAndQuery.Should().Be("/v1/auth");
        handler.Requests[1].Authorization!.Scheme.Should().Be("Bearer");
        handler.Requests[1].Authorization!.Parameter.Should().Be("token-123");
        handler.Requests[2].RequestUri!.PathAndQuery.Should().Be("/v1/documents/doc-123/recipients");

        var documentPayload = JsonDocument.Parse(handler.Requests[1].Body);
        documentPayload.RootElement.GetProperty("type").GetString().Should().Be("other");
        documentPayload.RootElement.GetProperty("file_content").GetString().Should().Be(Convert.ToBase64String([1, 2, 3]));
        documentPayload.RootElement.GetProperty("file_name").GetString().Should().Be("mandat.pdf");
        documentPayload.RootElement.GetProperty("is_signing").GetBoolean().Should().BeTrue();
        documentPayload.RootElement.GetProperty("is_automatic_sending").GetBoolean().Should().BeTrue();
        var recipient = documentPayload.RootElement.GetProperty("recipients").EnumerateArray().Single();
        recipient.GetProperty("email").GetString().Should().Be("jean.dupont@test.fr");
        recipient.GetProperty("first_name").GetString().Should().Be("Jean");
        recipient.GetProperty("last_name").GetString().Should().Be("Dupont");
        recipient.GetProperty("role").GetString().Should().Be("signer");
        recipient.GetProperty("verify_sms_sign").GetBoolean().Should().BeFalse();
    }

    /// <summary>
    /// Verifies that GetAccept credentials are required before any HTTP call.
    /// </summary>
    [Fact]
    public async Task SendMandateForSignatureAsync_WhenCredentialsAreMissing_ThrowsBeforeHttpCall()
    {
        var handler = new RecordingHandler(_ => new HttpResponseMessage(HttpStatusCode.OK));
        var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.getaccept.test/") };
        var client = new GetAcceptClient(
            httpClient,
            Options.Create(new GetAcceptOptions { BaseUrl = "https://api.getaccept.test/" }),
            NullLogger<GetAcceptClient>.Instance);

        var act = () => client.SendMandateForSignatureAsync(new GetAcceptMandateSignatureRequest(
            [1, 2, 3],
            "mandat.pdf",
            new SepaRecipient("jean.dupont@test.fr", "Jean", "Dupont")));

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("GetAccept credentials are not configured.");
        handler.Requests.Should().BeEmpty();
    }

    /// <summary>
    /// Verifies that missing signer document URLs fail explicitly.
    /// </summary>
    [Fact]
    public async Task SendMandateForSignatureAsync_WhenSignerUrlIsMissing_Throws()
    {
        var handler = new RecordingHandler(
            request =>
            {
                if (request.RequestUri!.PathAndQuery == "/v1/auth")
                {
                    return JsonResponse("""{"access_token":"token-123","expires_in":3600}""");
                }

                if (request.RequestUri!.PathAndQuery == "/v1/documents")
                {
                    return JsonResponse("""{"id":"doc-123"}""");
                }

                if (request.RequestUri!.PathAndQuery == "/v1/documents/doc-123/recipients")
                {
                    return JsonResponse("""{"recipients":[{"role":"viewer","document_url":"https://viewer.test"}]}""");
                }

                return new HttpResponseMessage(HttpStatusCode.NotFound);
            });
        var client = CreateClient(handler);

        var act = () => client.SendMandateForSignatureAsync(new GetAcceptMandateSignatureRequest(
            [1, 2, 3],
            "mandat.pdf",
            new SepaRecipient("jean.dupont@test.fr", "Jean", "Dupont")));

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("GetAccept recipients response did not include a signer document URL.");
    }

    /// <summary>
    /// Verifies that a null authentication payload fails explicitly.
    /// </summary>
    [Fact]
    public async Task SendMandateForSignatureAsync_WhenAuthResponseIsNull_Throws()
    {
        var handler = new RecordingHandler(
            request =>
            {
                if (request.RequestUri!.PathAndQuery == "/v1/auth")
                {
                    return JsonResponse("null");
                }

                return new HttpResponseMessage(HttpStatusCode.NotFound);
            });
        var client = CreateClient(handler);

        var act = () => client.SendMandateForSignatureAsync(new GetAcceptMandateSignatureRequest(
            [1, 2, 3],
            "mandat.pdf",
            new SepaRecipient("jean.dupont@test.fr", "Jean", "Dupont")));

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("GetAccept authentication response was empty.");
    }

    /// <summary>
    /// Verifies that an empty access token fails explicitly.
    /// </summary>
    [Fact]
    public async Task SendMandateForSignatureAsync_WhenAuthTokenIsMissing_Throws()
    {
        var handler = new RecordingHandler(
            request =>
            {
                if (request.RequestUri!.PathAndQuery == "/v1/auth")
                {
                    return JsonResponse("""{"access_token":"","expires_in":3600}""");
                }

                return new HttpResponseMessage(HttpStatusCode.NotFound);
            });
        var client = CreateClient(handler);

        var act = () => client.SendMandateForSignatureAsync(new GetAcceptMandateSignatureRequest(
            [1, 2, 3],
            "mandat.pdf",
            new SepaRecipient("jean.dupont@test.fr", "Jean", "Dupont")));

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("GetAccept authentication response did not include an access token.");
    }

    /// <summary>
    /// Verifies that a null document creation payload fails explicitly.
    /// </summary>
    [Fact]
    public async Task SendMandateForSignatureAsync_WhenDocumentResponseIsNull_Throws()
    {
        var handler = new RecordingHandler(
            request =>
            {
                if (request.RequestUri!.PathAndQuery == "/v1/auth")
                {
                    return JsonResponse("""{"access_token":"token-123","expires_in":3600}""");
                }

                if (request.RequestUri!.PathAndQuery == "/v1/documents")
                {
                    return JsonResponse("null");
                }

                return new HttpResponseMessage(HttpStatusCode.NotFound);
            });
        var client = CreateClient(handler);

        var act = () => client.SendMandateForSignatureAsync(new GetAcceptMandateSignatureRequest(
            [1, 2, 3],
            "mandat.pdf",
            new SepaRecipient("jean.dupont@test.fr", "Jean", "Dupont")));

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("GetAccept document creation response was empty.");
    }

    /// <summary>
    /// Verifies that a missing document id fails explicitly.
    /// </summary>
    [Fact]
    public async Task SendMandateForSignatureAsync_WhenDocumentIdIsMissing_Throws()
    {
        var handler = new RecordingHandler(
            request =>
            {
                if (request.RequestUri!.PathAndQuery == "/v1/auth")
                {
                    return JsonResponse("""{"access_token":"token-123","expires_in":3600}""");
                }

                if (request.RequestUri!.PathAndQuery == "/v1/documents")
                {
                    return JsonResponse("""{"id":""}""");
                }

                return new HttpResponseMessage(HttpStatusCode.NotFound);
            });
        var client = CreateClient(handler);

        var act = () => client.SendMandateForSignatureAsync(new GetAcceptMandateSignatureRequest(
            [1, 2, 3],
            "mandat.pdf",
            new SepaRecipient("jean.dupont@test.fr", "Jean", "Dupont")));

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("GetAccept document creation response did not include a document id.");
    }

    /// <summary>
    /// Verifies that a null recipients payload fails explicitly.
    /// </summary>
    [Fact]
    public async Task SendMandateForSignatureAsync_WhenRecipientsResponseIsNull_Throws()
    {
        var handler = new RecordingHandler(
            request =>
            {
                if (request.RequestUri!.PathAndQuery == "/v1/auth")
                {
                    return JsonResponse("""{"access_token":"token-123","expires_in":3600}""");
                }

                if (request.RequestUri!.PathAndQuery == "/v1/documents")
                {
                    return JsonResponse("""{"id":"doc-123"}""");
                }

                if (request.RequestUri!.PathAndQuery == "/v1/documents/doc-123/recipients")
                {
                    return JsonResponse("null");
                }

                return new HttpResponseMessage(HttpStatusCode.NotFound);
            });
        var client = CreateClient(handler);

        var act = () => client.SendMandateForSignatureAsync(new GetAcceptMandateSignatureRequest(
            [1, 2, 3],
            "mandat.pdf",
            new SepaRecipient("jean.dupont@test.fr", "Jean", "Dupont")));

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("GetAccept recipients response was empty.");
    }

    private static GetAcceptClient CreateClient(RecordingHandler handler)
    {
        var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.getaccept.test/") };
        return new GetAcceptClient(
            httpClient,
            Options.Create(new GetAcceptOptions
            {
                BaseUrl = "https://api.getaccept.test/",
                Email = "sender@test.fr",
                Password = "password"
            }),
            NullLogger<GetAcceptClient>.Instance);
    }

    private static HttpResponseMessage JsonResponse(string json)
    {
        return new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")
        };
    }

    private sealed class RecordingHandler(Func<HttpRequestMessage, HttpResponseMessage> responder) : HttpMessageHandler
    {
        public List<RecordedRequest> Requests { get; } = [];

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var body = request.Content is null
                ? string.Empty
                : await request.Content.ReadAsStringAsync(cancellationToken);

            Requests.Add(new RecordedRequest(request.Method, request.RequestUri!, request.Headers.Authorization, body));
            return responder(request);
        }
    }

    private sealed record RecordedRequest(
        HttpMethod Method,
        Uri RequestUri,
        System.Net.Http.Headers.AuthenticationHeaderValue? Authorization,
        string Body);
}
