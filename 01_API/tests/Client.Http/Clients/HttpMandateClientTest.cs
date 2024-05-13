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

            var collectionBankInfo = new Client.CollectionBankInfo(
                bankName: "Crédit Agricole",
                accountNumber: "98765432101",
                jdcPartnership: 2);

            var collectionSummary = new CollectionSummary(
                id: Guid.Empty,
                erpId: "1234567890",
                companyName: "Weyland Corporation",
                collectionBankInfo: collectionBankInfo,
                creationDate: new DateTime(2023, 10, 1, 0, 0, 0, DateTimeKind.Utc),
                modificationDate: new DateTime(2023, 10, 2, 0, 0, 0, DateTimeKind.Utc),
                statusCode: 20);

            var serializedCollectionSummary = JsonNode.Parse(JsonConvert.SerializeObject(collectionSummary))!.ToJsonString();
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

        [Fact]
        public async Task GetTechnicalCollectionSummaryAsync()
        {
            var authentication = new BearerHttpClientAuthentication("tTest");

            var technicalCollectionSummary = new TechnicalCollectionSummary(
                Guid.Empty,
                "12345",
                "12347",
                new BankDetails(
                    "99999",
                    "00000",
                    "77340082511",
                    "99"),
                20);

            PagedTechnicalMandate paged = new PagedTechnicalMandate(
                new List<TechnicalCollectionSummary>() { technicalCollectionSummary });

            var serializedPageSummary = JsonNode.Parse(JsonConvert.SerializeObject(paged))!.ToJsonString();
            var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(serializedPageSummary, Encoding.UTF8, "application/json"),
            };
            var client = new Mock<IHttpClient>(MockBehavior.Strict);
            client.Setup(c => c.SendAsync(It.IsAny<HttpRequestMessage>()))
                .Callback<HttpRequestMessage>(message =>
                {
                    message.Method.Should().Be(HttpMethod.Get);
                    message.RequestUri!.ToString().Should().StartWith("mandate/technical");
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

            var result = await httpMandateClient.GetTechnicalCollectionSummaryAsync(0, 100, new List<int> { 1, 3 });

            result.Data[0].Should().BeEquivalentTo(technicalCollectionSummary);
        }

        [Fact]
        public async Task RefreshMandatsStatusesAsync()
        {
            var authentication = new BearerHttpClientAuthentication("tTest");

            var technicalCollectionSummary = new TechnicalCollectionSummary(
                Guid.Empty,
                "12345",
                "12347",
                new BankDetails(
                    "99999",
                    "00000",
                    "77340082511",
                    "99"),
                20);

            var technicalCollectionSummaryList = new List<TechnicalCollectionSummary>() { technicalCollectionSummary };

            var serialized = JsonNode.Parse(JsonConvert.SerializeObject(true))!.ToJsonString();
            var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(serialized, Encoding.UTF8, "application/json"),
            };
            var client = new Mock<IHttpClient>(MockBehavior.Strict);
            client.Setup(c => c.SendAsync(It.IsAny<HttpRequestMessage>()))
                .Callback<HttpRequestMessage>(message =>
                {
                    message.Method.Should().Be(HttpMethod.Post);
                    message.RequestUri!.ToString().Should().StartWith("mandate/refresh-mandates-statuses");
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

            var result = await httpMandateClient.RefreshMandatsStatusesAsync(technicalCollectionSummaryList);

            result.Should().Be(true);
        }

        [Fact]
        public async Task RecoveryFormIoAsync()
        {
            var authentication = new BearerHttpClientAuthentication("tTest");

            var collectionBankInfo = new CollectionBankInfo(
                bankName: "bank",
                accountNumber: "12345678910",
                jdcPartnership: 2);

            var collectionSummary = new CollectionSummary(
                id: Guid.Empty,
                erpId: "12345",
                companyName: "Name",
                collectionBankInfo: collectionBankInfo,
                creationDate: DateTime.UtcNow,
                modificationDate: DateTime.UtcNow,
                statusCode: 20);

            PagedRecoveryMandate page = new PagedRecoveryMandate(1, new List<CollectionSummary>() { collectionSummary });

            var serialized = JsonNode.Parse(JsonConvert.SerializeObject(page))!.ToJsonString();
            var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(serialized, Encoding.UTF8, "application/json"),
            };
            var client = new Mock<IHttpClient>(MockBehavior.Strict);
            client.Setup(c => c.SendAsync(It.IsAny<HttpRequestMessage>()))
                .Callback<HttpRequestMessage>(message =>
                {
                    message.Method.Should().Be(HttpMethod.Post);
                    message.RequestUri!.ToString().Should().StartWith("mandate/recovery-form-io");
                    message.RequestUri!.ToString().Should().Be("mandate/recovery-form-io?skip=0&limit=1");
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

            var result = await httpMandateClient.RecoveryFormIoAsync(0, 1);

            result.Should().BeEquivalentTo(page);
        }
    }
}
