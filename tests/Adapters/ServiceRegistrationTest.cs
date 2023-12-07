// <copyright file="ServiceRegistrationTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Adapters.Tests
{
    using KPMG.Constellation.Portal.Client;
    using KPMG.Pulse.Back.Accounting.Mandate.Formio.Client;
    using KPMG.Pulse.Back.Accounting.Mandate.Formio.Client.Http;
    using KPMG.Pulse.Back.Accounting.Mandate.JeDeclare.Client;
    using KPMG.Pulse.Back.Accounting.Mandate.JeDeclare.Client.Http;
    using KPMG.Pulse.Back.Accounting.Mandate.Portal;
    using KPMG.Pulse.Back.Accounting.Mandate.Sql;
    using KPMG.Pulse.Back.Accounting.Mandate.Sql.Implementation;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public class ServiceRegistrationTest
    {
        [Fact]
        public void Register()
        {
            var variables = new[]
            {
                new KeyValuePair<string, string?>("PORTAL_API_URL", "https://chat"),
                new KeyValuePair<string, string?>("ConstellationClientId", "ConstellationClientId"),
                new KeyValuePair<string, string?>("ConstellationSecret", "ConstellationSecret"),
                new KeyValuePair<string, string?>("ConstellationAudience", "ConstellationAudience"),
                new KeyValuePair<string, string?>("ConstellationTenant", "ConstellationTenant"),
            };

            var configuration = new ConfigurationManager()
                .AddInMemoryCollection(variables)
                .Build();

            var sc = new ServiceCollection();
            sc.AddMandateAdapters();
            sc.AddMandateSql(opt => opt.ConnectionString = "a");
            sc.AddMandateJeDeclare(opt =>
            {
                opt.BaseUri = new Uri("https://recette.jedeclare.com/webservice/gestion/");
                opt.Login = "loginT";
                opt.Password = "passwordT";
            });
            sc.AddMandateFormio(opt =>
            {
                opt.BaseUri = new Uri("https://toto.com");
                opt.FormioApiKey = "test";
            });

            sc.AddHttpContextAccessor();
            sc.AddSingleton<IConfiguration>(configuration);
            sc.AddPortailApi((ConfigurationManager)configuration);

            var sp = sc.BuildServiceProvider();

            // Make sure we don't forget services ; exclude services from Microsoft (IOption, ...)
            sc.Count(s => s.ServiceType.FullName?.StartsWith("KPMG") ?? false).Should().Be(12);

            // Test all services ; number of tests below should match the number of services above
            sp.GetService<IDatabaseService>().Should().NotBeNull();
            sp.GetService<IMandateRepository>().Should().NotBeNull();
            sp.GetService<IJeDeclareClientFactory>().Should().NotBeNull();
            sp.GetService<IJeDeclareClient>().Should().NotBeNull();
            sp.GetService<IJeDeclareService>().Should().NotBeNull();
            sp.GetService<IFormioClientFactory>().Should().NotBeNull();
            sp.GetService<IPortalProvider>().Should().NotBeNull();
            sp.GetService<IPortalClientFactory>().Should().NotBeNull();
            sp.GetService<IAuthenticationContext>().Should().NotBeNull();
            sp.GetService<IPortalManager>().Should().NotBeNull();
            sp.GetService<IFormioClient>().Should().NotBeNull();
            sp.GetService<IFormIoService>().Should().NotBeNull();
        }
    }
}
