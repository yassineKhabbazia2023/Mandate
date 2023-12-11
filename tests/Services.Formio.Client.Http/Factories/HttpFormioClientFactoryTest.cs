// <copyright file="HttpFormioClientFactoryTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Formio.Client.Http.Tests
{
    using Microsoft.Extensions.Options;

    public class HttpFormioClientFactoryTest
    {
        [Fact]
        public void Constructor_CaseOptionsIsNull()
        {
            var factory = new Mock<Kpmg.Constellation.Net.Http.IHttpClientFactory>(MockBehavior.Strict);

            Action act = () =>
            {
                _ = new HttpFormioClientFactory(null!, factory.Object);
            };

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Constructor_CaseFactoryIsNull()
        {
            var options = new FormioOptions()
            {
                BaseUri = new Uri("https://toto.com"),
                FormioApiKey = "formioApiKeyT",
            };

            var formioOptions = Options.Create(options);

            Action act = () =>
            {
                _ = new HttpFormioClientFactory(formioOptions, null !);
            };

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Create()
        {
            var options = new FormioOptions()
            {
                BaseUri = new Uri("https://toto.com"),
                FormioApiKey = "formioApiKeyT",
            };

            var formioOptions = Options.Create(options);

            var headers = new HttpRequestMessage().Headers;

            var client = new Mock<Kpmg.Constellation.Net.Http.IHttpClient>(MockBehavior.Strict);
            client.Setup(c => c.DefaultRequestHeaders)
                .Returns(headers)
                .Verifiable();

            var factory = new Mock<Kpmg.Constellation.Net.Http.IHttpClientFactory>(MockBehavior.Strict);
            factory.Setup(h => h.Create(It.IsAny<Uri>(), It.IsAny<Kpmg.Constellation.Net.Http.HttpClientAuthentication>()))
                .Callback<Uri, Kpmg.Constellation.Net.Http.HttpClientAuthentication>((uri, auth) =>
                {
                    uri.Should().BeEquivalentTo(new Uri("https://toto.com"));
                    auth.Should().BeNull();
                })
                .Returns(client.Object)
                .Verifiable();

            var httpFormioclientFactory = new HttpFormioClientFactory(formioOptions, factory.Object);
            var httpClient = httpFormioclientFactory.Create();

            client.VerifyAll();
            factory.VerifyAll();
        }

        [Fact]
        public void Create_CaseUserAuthToken()
        {
            var options = new FormioOptions()
            {
                BaseUri = new Uri("https://toto.com"),
                FormioApiKey = "formioApiKeyT",
            };

            var formioOptions = Options.Create(options);

            var headers = new HttpRequestMessage().Headers;

            var client = new Mock<Kpmg.Constellation.Net.Http.IHttpClient>(MockBehavior.Strict);
            client.Setup(c => c.DefaultRequestHeaders)
                .Returns(headers)
                .Verifiable();
            client.Setup(c => c.EnsureHttpClientCreated())
                .Verifiable();

            var factory = new Mock<Kpmg.Constellation.Net.Http.IHttpClientFactory>(MockBehavior.Strict);
            factory.Setup(h => h.Create(It.IsAny<Uri>(), It.IsAny<Kpmg.Constellation.Net.Http.HttpClientAuthentication>()))
                .Callback<Uri, Kpmg.Constellation.Net.Http.HttpClientAuthentication>((uri, auth) =>
                {
                    uri.Should().BeEquivalentTo(new Uri("https://toto.com"));
                    auth.Should().BeNull();
                })
                .Returns(client.Object)
                .Verifiable();

            var httpFormioclientFactory = new HttpFormioClientFactory(formioOptions, factory.Object);
            var formioAuthToken = new FormioAuthToken()
            {
                Type = FormioTokenType.User,
                Value = "valueT",
            };

            var httpClient = httpFormioclientFactory.Create(formioAuthToken);
            client.Object.DefaultRequestHeaders.GetValues("x-jwt-token").Single().Should().Be("valueT");

            // idigao
            client.VerifyAll();
            factory.VerifyAll();
        }

        [Fact]
        public void Create_CaseAppAuthToken()
        {
            var options = new FormioOptions()
            {
                BaseUri = new Uri("https://toto.com"),
                FormioApiKey = "formioApiKeyT",
            };

            var formioOptions = Options.Create(options);

            var headers = new HttpRequestMessage().Headers;

            var client = new Mock<Kpmg.Constellation.Net.Http.IHttpClient>(MockBehavior.Strict);
            client.Setup(c => c.DefaultRequestHeaders)
                .Returns(headers)
                .Verifiable();
            client.Setup(c => c.EnsureHttpClientCreated())
                .Verifiable();

            var factory = new Mock<Kpmg.Constellation.Net.Http.IHttpClientFactory>(MockBehavior.Strict);
            factory.Setup(h => h.Create(It.IsAny<Uri>(), It.IsAny<Kpmg.Constellation.Net.Http.HttpClientAuthentication>()))
                .Callback<Uri, Kpmg.Constellation.Net.Http.HttpClientAuthentication>((uri, auth) =>
                {
                    uri.Should().BeEquivalentTo(new Uri("https://toto.com"));
                    auth.Should().BeNull();
                })
                .Returns(client.Object)
                .Verifiable();

            var httpFormioclientFactory = new HttpFormioClientFactory(formioOptions, factory.Object);
            var formioAuthToken = new FormioAuthToken()
            {
                Type = FormioTokenType.App,
                Value = "valueM",
            };

            var httpClient = httpFormioclientFactory.Create(formioAuthToken);
            client.Object.DefaultRequestHeaders.GetValues("x-token").Single().Should().Be("formioApiKeyT");

            // idigao
            client.VerifyAll();
            factory.VerifyAll();
        }
    }
}
