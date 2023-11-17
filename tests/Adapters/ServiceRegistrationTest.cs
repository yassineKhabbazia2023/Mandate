// <copyright file="ServiceRegistrationTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Adapters.Tests
{
    using KPMG.Pulse.Back.Accounting.Mandate.Formio.Client;
    using KPMG.Pulse.Back.Accounting.Mandate.Formio.Client.Http;
    using KPMG.Pulse.Back.Accounting.Mandate.JeDeclare.Client;
    using KPMG.Pulse.Back.Accounting.Mandate.JeDeclare.Client.Http;
    using KPMG.Pulse.Back.Accounting.Mandate.Sql;
    using KPMG.Pulse.Back.Accounting.Mandate.Sql.Implementation;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;

    public class ServiceRegistrationTest
    {
        [Fact]
        public void Register()
        {
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

            var sp = sc.BuildServiceProvider();

            // Make sure we don't forget services ; exclude services from Microsoft (IOption, ...)
            sc.Count(s => s.ServiceType.FullName?.StartsWith("KPMG") ?? false).Should().Be(6);

            // Test all services ; number of tests below should match the number of services above
            sp.GetService<IDatabaseService>().Should().NotBeNull();
            sp.GetService<IMandateRepository>().Should().NotBeNull();
            sp.GetService<IJeDeclareClientFactory>().Should().NotBeNull();
            sp.GetService<IJeDeclareClient>().Should().NotBeNull();
            sp.GetService<IFormioClientFactory>().Should().NotBeNull();
            sp.GetService<IJeDeclareService>().Should().NotBeNull();
        }
    }
}
