// <copyright file="MandateClientServiceCollectionExtensionsTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Client.Http.Tests
{
    using KPMG.Pulse.Back.Accounting.Mandate.Client.Http.Options;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;

    public class MandateClientServiceCollectionExtensionsTest
    {
        [Fact]
        public void AddMandateClient_ShouldRegisterRequiredServices()
        {
            // Arrange
            var services = new ServiceCollection();
            Action<MandateClientOptions> configureOptions = options => { /* configure options here */ };

            // Act
            MandateClientServiceCollectionExtensions.AddMandateClient(services, configureOptions);

            // Assert
            services.Should().Contain(service => service.ServiceType == typeof(IConfigureOptions<MandateClientOptions>));
            services.Should().Contain(service => service.ServiceType == typeof(IMandateClientFactory) && service.Lifetime == ServiceLifetime.Singleton);
        }

        [Fact]
        public void AddMandateClient_ShouldThrowArgumentNullException_WhenServicesIsNull()
        {
            // Arrange, Act & Assert
            Action act = () => MandateClientServiceCollectionExtensions.AddMandateClient(null!, _ => { });

            // Assert
            act.Should().Throw<ArgumentNullException>()
               .And.ParamName.Should().Be("services");
        }

        [Fact]
        public void AddMandateClient_ShouldThrowArgumentNullException_WhenConfigureOptionsIsNull()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act & Assert
            Action act = () => MandateClientServiceCollectionExtensions.AddMandateClient(services, null!);

            // Assert
            act.Should().Throw<ArgumentNullException>()
               .And.ParamName.Should().Be("configureOptions");
        }
    }
}
