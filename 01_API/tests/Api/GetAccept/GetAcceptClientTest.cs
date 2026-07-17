// <copyright file="GetAcceptClientTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.AspNetCore.Tests.GetAccept;

using System.Net;
using System.Text.Json;
using KPMG.Pulse.Back.Accounting.Mandate.Application.Models;
using KPMG.Pulse.Back.Accounting.Mandate.AspNetCore.GetAccept;
using Microsoft.Extensions.Logging;
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
    /// Verifies that recipient retrieval is retried until GetAccept returns the signer document URL.
    /// </summary>
    [Fact]
    public async Task SendMandateForSignatureAsync_WhenSignerUrlBecomesAvailable_ReturnsSignatureUrl()
    {
        const string pendingPayload = """{"recipients":[{"role":"signer","status":"added","email":"jean.dupont@test.fr","document_url":""}]}""";
        const string readyPayload = """{"recipients":[{"role":"signer","status":"sent","email":"jean.dupont@test.fr","document_url":"https://signature.test"}]}""";
        const string pendingDiagnosticPayload = """{"recipients":[{"role":"signer","status":"added","hasDocumentUrl":false}]}""";
        const string readyDiagnosticPayload = """{"recipients":[{"role":"signer","status":"sent","hasDocumentUrl":true}]}""";
        var recipientsAttempts = 0;
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
                    recipientsAttempts++;
                    return JsonResponse(recipientsAttempts == 1 ? pendingPayload : readyPayload);
                }

                return new HttpResponseMessage(HttpStatusCode.NotFound);
            });
        var logger = new Mock<ILogger<GetAcceptClient>>();
        logger.Setup(candidate => candidate.IsEnabled(LogLevel.Debug)).Returns(true);
        var client = CreateClient(handler, logger.Object, signatureUrlMaxAttempts: 3);

        var result = await client.SendMandateForSignatureAsync(new GetAcceptMandateSignatureRequest(
            [1, 2, 3],
            "mandat.pdf",
            new SepaRecipient("jean.dupont@test.fr", "Jean", "Dupont")));

        result.SignatureRequestId.Should().Be("doc-123");
        result.SignatureUrl.Should().Be("https://signature.test");
        recipientsAttempts.Should().Be(2);
        logger.Verify(candidate => candidate.Log(
            LogLevel.Debug,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((state, _) => state.ToString()!.Contains(pendingDiagnosticPayload, StringComparison.Ordinal)),
            null,
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
        logger.Verify(candidate => candidate.Log(
            LogLevel.Debug,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((state, _) => state.ToString()!.Contains(readyDiagnosticPayload, StringComparison.Ordinal)),
            null,
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
        logger.Verify(candidate => candidate.Log(
            It.IsAny<LogLevel>(),
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((state, _) => state.ToString()!.Contains("jean.dupont@test.fr", StringComparison.Ordinal)
                || state.ToString()!.Contains("https://signature.test", StringComparison.Ordinal)),
            It.IsAny<Exception?>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Never);
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
        const string recipientsPayload = """{"recipients":[{"role":"viewer","status":"sent","email":"viewer@test.fr","document_url":"https://viewer.test"}]}""";
        const string diagnosticPayload = """{"recipients":[{"role":"viewer","status":"sent","hasDocumentUrl":true}]}""";
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
                    return JsonResponse(recipientsPayload);
                }

                return new HttpResponseMessage(HttpStatusCode.NotFound);
            });
        var logger = new Mock<ILogger<GetAcceptClient>>();
        logger.Setup(candidate => candidate.IsEnabled(LogLevel.Debug)).Returns(true);
        var client = CreateClient(handler, logger.Object, signatureUrlMaxAttempts: 3);

        var act = () => client.SendMandateForSignatureAsync(new GetAcceptMandateSignatureRequest(
            [1, 2, 3],
            "mandat.pdf",
            new SepaRecipient("jean.dupont@test.fr", "Jean", "Dupont")));

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("GetAccept recipients response did not include a signer document URL.");
        handler.Requests.Count(request => request.RequestUri.PathAndQuery == "/v1/documents/doc-123/recipients")
            .Should().Be(3);
        logger.Verify(candidate => candidate.Log(
            LogLevel.Debug,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((state, _) => state.ToString()!.Contains(diagnosticPayload, StringComparison.Ordinal)),
            null,
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Exactly(3));
        logger.Verify(candidate => candidate.Log(
            LogLevel.Error,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((state, _) => state.ToString() ==
                "GetAccept recipients response for document doc-123 did not contain a signer document URL after 3 attempts."),
            null,
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
        logger.Verify(candidate => candidate.Log(
            It.IsAny<LogLevel>(),
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((state, _) => state.ToString()!.Contains("viewer@test.fr", StringComparison.Ordinal)
                || state.ToString()!.Contains("https://viewer.test", StringComparison.Ordinal)),
            It.IsAny<Exception?>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Never);
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

    /// <summary>
    /// Verifies that document status retrieval authenticates and maps the payload.
    /// </summary>
    [Fact]
    public async Task GetDocumentStatusAsync_WhenResponseIsValid_ReturnsStatusAndDownloadUrl()
    {
        var handler = new RecordingHandler(
            request =>
            {
                if (request.RequestUri!.PathAndQuery == "/v1/auth")
                {
                    return JsonResponse("""{"access_token":"token-123","expires_in":3600}""");
                }

                if (request.RequestUri!.PathAndQuery == "/v1/documents/doc-123")
                {
                    return JsonResponse("""{"status":"signed","download_url":"https://download.test/signed.pdf"}""");
                }

                return new HttpResponseMessage(HttpStatusCode.NotFound);
            });
        var client = CreateClient(handler);

        var result = await client.GetDocumentStatusAsync("doc-123");

        result.Status.Should().Be("signed");
        result.SignedDocumentUrl.Should().Be("https://download.test/signed.pdf");
        handler.Requests.Should().HaveCount(2);
        handler.Requests[1].Authorization!.Scheme.Should().Be("Bearer");
        handler.Requests[1].Authorization!.Parameter.Should().Be("token-123");
        handler.Requests[1].RequestUri!.PathAndQuery.Should().Be("/v1/documents/doc-123");
    }

    /// <summary>
    /// Verifies that an empty document status payload fails explicitly.
    /// </summary>
    [Fact]
    public async Task GetDocumentStatusAsync_WhenResponseIsNull_Throws()
    {
        var handler = new RecordingHandler(
            request =>
            {
                if (request.RequestUri!.PathAndQuery == "/v1/auth")
                {
                    return JsonResponse("""{"access_token":"token-123","expires_in":3600}""");
                }

                if (request.RequestUri!.PathAndQuery == "/v1/documents/doc-123")
                {
                    return JsonResponse("null");
                }

                return new HttpResponseMessage(HttpStatusCode.NotFound);
            });
        var client = CreateClient(handler);

        var act = () => client.GetDocumentStatusAsync("doc-123");

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("GetAccept document status response was empty.");
    }

    /// <summary>
    /// Verifies that a missing signature request identifier is rejected.
    /// </summary>
    [Fact]
    public async Task GetDocumentStatusAsync_WhenSignatureRequestIdIsMissing_Throws()
    {
        var client = CreateClient(new RecordingHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)));

        var act = () => client.GetDocumentStatusAsync(string.Empty);

        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("The GetAccept signature request identifier is required.*");
    }

    /// <summary>
    /// Verifies that non-success document status responses bubble up as HTTP failures.
    /// </summary>
    [Fact]
    public async Task GetDocumentStatusAsync_WhenGetAcceptReturnsNonSuccess_Throws()
    {
        var handler = new RecordingHandler(
            request => request.RequestUri!.PathAndQuery == "/v1/auth"
                ? JsonResponse("""{"access_token":"token-123","expires_in":3600}""")
                : new HttpResponseMessage(HttpStatusCode.BadGateway));
        var client = CreateClient(handler);

        var act = () => client.GetDocumentStatusAsync("doc-123");

        await act.Should().ThrowAsync<HttpRequestException>();
    }

    /// <summary>
    /// Verifies that signed document download does not authenticate again for pre-signed external URLs and preserves response metadata.
    /// </summary>
    [Fact]
    public async Task DownloadSignedDocumentAsync_WhenResponseIsPresignedExternalUrl_ReturnsContentMetadataWithoutAuthorizationHeader()
    {
        var handler = new RecordingHandler(
            request =>
            {
                if (request.RequestUri!.AbsoluteUri == "https://download.test/signed.pdf")
                {
                    return new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new ByteArrayContent([1, 2, 3])
                        {
                            Headers =
                            {
                                ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/pdf"),
                                ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                                {
                                    FileNameStar = "signed-file.pdf"
                                }
                            }
                        }
                    };
                }

                return new HttpResponseMessage(HttpStatusCode.NotFound);
            });
        var client = CreateClient(handler);

        var result = await client.DownloadSignedDocumentAsync("https://download.test/signed.pdf");

        result.Content.Should().Equal([1, 2, 3]);
        result.ContentType.Should().Be("application/pdf");
        result.FileName.Should().Be("signed-file.pdf");
        handler.Requests.Should().ContainSingle();
        handler.Requests[0].Authorization.Should().BeNull();
        handler.Requests[0].RequestUri!.AbsoluteUri.Should().Be("https://download.test/signed.pdf");
    }

    /// <summary>
    /// Verifies that signed document download falls back to default metadata when headers are absent.
    /// </summary>
    [Fact]
    public async Task DownloadSignedDocumentAsync_WhenResponseHeadersAreMissing_UsesDefaults()
    {
        var handler = new RecordingHandler(
            _ => new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent([4, 5, 6])
            });
        var client = CreateClient(handler);

        var result = await client.DownloadSignedDocumentAsync("https://download.test/no-headers");

        result.Content.Should().Equal([4, 5, 6]);
        result.ContentType.Should().Be("application/pdf");
        result.FileName.Should().Be("mandate-sepa-signed.pdf");
        handler.Requests.Should().ContainSingle();
        handler.Requests[0].Authorization.Should().BeNull();
    }

    /// <summary>
    /// Verifies that a missing signed document URL is rejected.
    /// </summary>
    [Fact]
    public async Task DownloadSignedDocumentAsync_WhenSignedDocumentUrlIsMissing_Throws()
    {
        var client = CreateClient(new RecordingHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)));

        var act = () => client.DownloadSignedDocumentAsync(string.Empty);

        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("The GetAccept signed document URL is required.*");
    }

    /// <summary>
    /// Verifies that non-success signed document responses bubble up as HTTP failures.
    /// </summary>
    [Fact]
    public async Task DownloadSignedDocumentAsync_WhenGetAcceptReturnsNonSuccess_Throws()
    {
        var handler = new RecordingHandler(
            _ => new HttpResponseMessage(HttpStatusCode.BadGateway));
        var client = CreateClient(handler);

        var act = () => client.DownloadSignedDocumentAsync("https://download.test/signed.pdf");

        await act.Should().ThrowAsync<HttpRequestException>();
    }

    /// <summary>
    /// Verifies that signed document download authenticates for GetAccept-hosted URLs.
    /// </summary>
    [Fact]
    public async Task DownloadSignedDocumentAsync_WhenResponseIsHostedByGetAccept_UsesAuthorizationHeader()
    {
        var handler = new RecordingHandler(
            request =>
            {
                if (request.RequestUri!.PathAndQuery == "/v1/auth")
                {
                    return JsonResponse("""{"access_token":"token-123","expires_in":3600}""");
                }

                if (request.RequestUri!.AbsoluteUri == "https://api.getaccept.test/v1/documents/doc-123/download")
                {
                    return new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new ByteArrayContent([7, 8, 9])
                    };
                }

                return new HttpResponseMessage(HttpStatusCode.NotFound);
            });
        var client = CreateClient(handler);

        var result = await client.DownloadSignedDocumentAsync("https://api.getaccept.test/v1/documents/doc-123/download");

        result.Content.Should().Equal([7, 8, 9]);
        handler.Requests.Should().HaveCount(2);
        handler.Requests[1].Authorization!.Scheme.Should().Be("Bearer");
        handler.Requests[1].Authorization!.Parameter.Should().Be("token-123");
    }

    /// <summary>
    /// Verifies that the authenticated token is reused across sequential GetAccept calls on the same client instance.
    /// </summary>
    [Fact]
    public async Task GetAcceptClient_WhenSeveralOperationsRunSequentially_ReusesAuthenticationToken()
    {
        var handler = new RecordingHandler(
            request =>
            {
                if (request.RequestUri!.PathAndQuery == "/v1/auth")
                {
                    return JsonResponse("""{"access_token":"token-123","expires_in":3600}""");
                }

                if (request.RequestUri!.PathAndQuery == "/v1/documents/doc-123")
                {
                    return JsonResponse("""{"status":"signed","download_url":"https://download.test/signed.pdf"}""");
                }

                if (request.RequestUri!.AbsoluteUri == "https://download.test/signed.pdf")
                {
                    return new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new ByteArrayContent([1, 2, 3])
                    };
                }

                return new HttpResponseMessage(HttpStatusCode.NotFound);
            });
        var client = CreateClient(handler);

        var status = await client.GetDocumentStatusAsync("doc-123");
        var document = await client.DownloadSignedDocumentAsync(status.SignedDocumentUrl!);

        status.Status.Should().Be("signed");
        document.Content.Should().Equal([1, 2, 3]);
        handler.Requests.Should().HaveCount(3);
        handler.Requests.Count(request => request.RequestUri.PathAndQuery == "/v1/auth").Should().Be(1);
        handler.Requests[1].RequestUri!.PathAndQuery.Should().Be("/v1/documents/doc-123");
        handler.Requests[1].Authorization!.Scheme.Should().Be("Bearer");
        handler.Requests[1].Authorization!.Parameter.Should().Be("token-123");
        handler.Requests[2].RequestUri!.AbsoluteUri.Should().Be("https://download.test/signed.pdf");
        handler.Requests[2].Authorization.Should().BeNull();
    }

    private static GetAcceptClient CreateClient(
        RecordingHandler handler,
        ILogger<GetAcceptClient>? logger = null,
        int signatureUrlMaxAttempts = 1,
        int signatureUrlRetryDelayMilliseconds = 0)
    {
        var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.getaccept.test/") };
        return new GetAcceptClient(
            httpClient,
            Options.Create(new GetAcceptOptions
            {
                BaseUrl = "https://api.getaccept.test/",
                Email = "sender@test.fr",
                Password = "password",
                SignatureUrlMaxAttempts = signatureUrlMaxAttempts,
                SignatureUrlRetryDelayMilliseconds = signatureUrlRetryDelayMilliseconds
            }),
            logger ?? NullLogger<GetAcceptClient>.Instance);
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
