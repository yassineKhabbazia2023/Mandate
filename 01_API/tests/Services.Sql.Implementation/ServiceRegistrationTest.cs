// <copyright file="ServiceRegistrationTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Sql.Implementation.Tests
{
    using Microsoft.Extensions.DependencyInjection;

    public class ServiceRegistrationTest
    {
        [Fact]
        public void AddMandateSql_ShouldAddServicesToCollection()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            ServiceRegistration.AddMandateSql(services, options => { });

            // Assert
            services.Should().ContainSingle(service =>
                service.ServiceType == typeof(IMandateRepository) &&
                service.ImplementationType == typeof(SqlMandateRepository) &&
                service.Lifetime == ServiceLifetime.Scoped);
        }

        [Fact]
        public void AddMandateSql_ShouldThrowArgumentNullException_WhenServicesIsNull()
        {
            // Act
            Action act = () => ServiceRegistration.AddMandateSql(null!, options => { });

            // Assert
            act.Should().Throw<ArgumentNullException>().WithParameterName("services");
        }
    }
}
