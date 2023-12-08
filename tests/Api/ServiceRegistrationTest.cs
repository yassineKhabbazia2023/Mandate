// <copyright file="ServiceRegistrationTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.AspNetCore.Tests
{

    using KPMG.Pulse.Back.Accounting.Mandate.Adapters;
    using KPMG.Pulse.Back.Accounting.Mandate.Application;
    using KPMG.Pulse.Back.Accounting.Mandate.Formio.Client.Http;
    using KPMG.Pulse.Back.Accounting.Mandate.JeDeclare.Client.Http;
    using KPMG.Pulse.Back.Accounting.Mandate.Portal;
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
            sc.AddMandateApplication();
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
            sc.Count(s => s.ServiceType.FullName?.StartsWith("KPMG") ?? false).Should().Be(20);

            // Test all services ; number of tests below should match the number of services above
            sp.GetService<IBankManager>().Should().NotBeNull();
            sp.GetService<IBbanManager>().Should().NotBeNull();
            sp.GetService<IMandateManager>().Should().NotBeNull();
            sp.GetService<ICompanyManager>().Should().NotBeNull();
            sp.GetService<IAsposeHelper>().Should().NotBeNull();
            sp.GetService<IGuidGenerator>().Should().NotBeNull();
            sp.GetService<IFormIoManager>().Should().NotBeNull();
            sp.GetService<IFakeDataManager>().Should().NotBeNull();
        }
    }
}
