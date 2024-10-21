// <copyright file="ServiceRegistrationTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Sql.Implementation.Tests
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public class ServiceRegistrationTest
    {
        [Fact]
        public void AddMandateSql_ShouldAddServicesToCollection()
        {
            // Arrange
            var services = new ServiceCollection();

            var inMemorySettings = new Dictionary<string, string> {
                {"DbConnectionString", "Server=(localdb)\\mssqllocaldb;Database=TestDb;Trusted_Connection=True;"}
            };
            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            // Act
            ServiceRegistration.AddMandateSql(services, configuration);

            // Assert
            services.Should().ContainSingle(service =>
                service.ServiceType == typeof(IMandateRepository) &&
                service.ImplementationType == typeof(SqlMandateRepository) &&
                service.Lifetime == ServiceLifetime.Scoped);
        }
    }
}
