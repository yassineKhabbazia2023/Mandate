// <copyright file="HttpMandateClientFactoryTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Client.Http.Tests.Clients
{
    using Kpmg.Constellation.Net.Http;
    using KPMG.Pulse.Back.Accounting.Mandate.Client.Http.Clients;
    using KPMG.Pulse.Back.Accounting.Mandate.Client.Http.Options;
    using Microsoft.Extensions.Options;

    public class HttpMandateClientFactoryTest
    {
        private readonly Mock<IOptions<MandateClientOptions>> mockOptions;
        private readonly Mock<IHttpClientFactory> mockFactory;
        private readonly MandateClientOptions options;

        public HttpMandateClientFactoryTest()
        {
            this.mockOptions = new Mock<IOptions<MandateClientOptions>>();
            this.mockFactory = new Mock<IHttpClientFactory>();

            this.options = new MandateClientOptions { BaseUri = new Uri("http://example.com") };
            this.mockOptions.Setup(o => o.Value).Returns(this.options);
        }

        [Fact]
        public void Create_WithoutUserToken_ReturnsHttpMandateClient()
        {
            // Arrange
            var factory = new HttpMandateClientFactory(this.mockOptions.Object, this.mockFactory.Object);

            // Act
            var client = factory.Create();

            // Assert
            client.Should().BeAssignableTo<IMandateClient>();
        }

        [Fact]
        public void Create_WithUserToken_ReturnsHttpMandateClientWithAuthentication()
        {
            // Arrange
            var factory = new HttpMandateClientFactory(this.mockOptions.Object, this.mockFactory.Object);
            var userToken = "testToken";

            // Act
            var client = factory.Create(userToken);

            // Assert
            client.Should().BeAssignableTo<IMandateClient>();
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_IfOptionsIsNull()
        {
            // Arrange, Act & Assert
            Action act = () =>
            {
                var factory = new HttpMandateClientFactory(null!, this.mockFactory.Object);
            };

            act.Should().Throw<ArgumentNullException>()
               .And.ParamName.Should().Be("options");
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_IfFactoryIsNull()
        {
            // Arrange, Act & Assert
            Action act = () =>
            {
                var factory = new HttpMandateClientFactory(this.mockOptions.Object, null!);
            };

            act.Should().Throw<ArgumentNullException>()
               .And.ParamName.Should().Be("factory");
        }
    }
}
