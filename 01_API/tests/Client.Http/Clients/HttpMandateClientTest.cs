// <copyright file="HttpMandateClientTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Client.Http.Tests
{
    using System.Net;
    using System.Text.Json.Nodes;
    using Kpmg.Constellation.Net.Http;
    using Newtonsoft.Json;

    public class HttpMandateClientTest
    {
        [Fact]
        public void Constructor()
        {
            var httpFactory = new Mock<Kpmg.Constellation.Net.Http.IHttpClientFactory>(MockBehavior.Strict);

            var client = new HttpMandateClient(
                baseUri: new Uri("http://test.test"),
                authentication: Mock.Of<Kpmg.Constellation.Net.Http.HttpClientAuthentication>(),
                clientFactory: httpFactory.Object);

            client.Should().NotBeNull();
        }

        [Fact]
        public void Constructor_BaseUriNullException()
        {
            var httpFactory = new Mock<Kpmg.Constellation.Net.Http.IHttpClientFactory>(MockBehavior.Strict);

            Action action = () =>
            {
                _ = new HttpMandateClient(
                baseUri: null!,
                authentication: Mock.Of<Kpmg.Constellation.Net.Http.HttpClientAuthentication>(),
                clientFactory: httpFactory.Object);
            };

            action.Should().ThrowExactly<ArgumentNullException>().WithMessage("Value cannot be null. (Parameter 'baseUri')");
        }

        [Fact]
        public void Constructor_FactoryNullException()
        {
            Action action = () =>
            {
                _ = new HttpMandateClient(
                baseUri: new Uri("http://test.test"),
                authentication: Mock.Of<Kpmg.Constellation.Net.Http.HttpClientAuthentication>(),
                clientFactory: null!);
            };

            action.Should().ThrowExactly<ArgumentNullException>().WithMessage("Value cannot be null. (Parameter 'clientFactory')");
        }

        [Fact]
        public async Task GetRecoveryAsync()
        {
            var authentication = new BearerHttpClientAuthentication("tTest");

            var collectionSummary = new CollectionSummary(
                Guid.Empty,
                "1234567890",
                "Weyland Corporation",
                "Crédit Agricole",
                "98765432101",
                new DateTime(2023, 10, 1, 0, 0, 0, DateTimeKind.Utc),
                new DateTime(2023, 10, 2, 0, 0, 0, DateTimeKind.Utc),
                10);

            var serializedCollectionSummary = JsonNode.Parse(JsonConvert.SerializeObject(collectionSummary)) !.ToJsonString();
            var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(serializedCollectionSummary, Encoding.UTF8, "application/json"),
            };
            var client = new Mock<IHttpClient>(MockBehavior.Strict);
            client.Setup(c => c.SendAsync(It.IsAny<HttpRequestMessage>()))
                .Callback<HttpRequestMessage>(message =>
                {
                    message.Method.Should().Be(HttpMethod.Post);
                    message.RequestUri.Should().Be("mandate/recovery");
                })
                .ReturnsAsync(httpResponse)
                .Verifiable();
            client.Setup(c => c.Dispose())
                .Verifiable();

            var httpFactory = new Mock<Kpmg.Constellation.Net.Http.IHttpClientFactory>(MockBehavior.Strict);
            httpFactory.Setup(h => h.Create(It.IsAny<Uri>(), It.IsAny<Kpmg.Constellation.Net.Http.HttpClientAuthentication>()))
                .Callback<Uri, HttpClientAuthentication>((uri, auth) =>
                {
                    uri.Should().BeEquivalentTo(new Uri("http://test.test"));
                    auth.Should().BeEquivalentTo(authentication);
                })
                .Returns(client.Object)
                .Verifiable();

            var httpMandateClient = new HttpMandateClient(
                baseUri: new Uri("http://test.test"),
                authentication: authentication,
                clientFactory: httpFactory.Object);

            var rib = new Bban(
                bankCode: "bankCodeT",
                branchCode: "branchCodeT",
                accountNumber: "accountNumberT",
                checkDigits: "checkDigitsT");

            var result = await httpMandateClient.GetRecoveryAsync(rib);

            result.Should().BeEquivalentTo(collectionSummary);
        }
    }
}
