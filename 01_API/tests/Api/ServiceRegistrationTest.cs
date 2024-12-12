// <copyright file="ServiceRegistrationTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

using KPMG.Pulse.Back.Accounting.Mandate.Adapters;
using KPMG.Pulse.Back.Accounting.Mandate.Application;
using KPMG.Pulse.Back.Accounting.Mandate.JeDeclare.Client;
using KPMG.Pulse.Back.Accounting.Mandate.JeDeclare.Client.Http;
using KPMG.Pulse.Back.Accounting.Mandate.Sql;
using KPMG.Pulse.Back.Accounting.Mandate.Sql.Implementation;
using Mandate.Messaging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace KPMG.Pulse.Back.Accounting.Mandate.AspNetCore.Tests
{
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
                new KeyValuePair<string, string?>("MANDATE_NOTIFICATION_V2_API_URL", "https://notifications"),
                new KeyValuePair<string, string?>("ServiceBusQueueCreateQueueName", "big-queue"),
                new KeyValuePair<string, string?>("hubServiceBus:fullyQualifiedNamespace", "ahah"),
                new KeyValuePair<string, string?>("hubServiceBus:clientId", "watashi"),
            };

            var configuration = new ConfigurationManager()
                .AddInMemoryCollection(variables)
                .Build();

            var sc = new ServiceCollection();
            sc.AddMandateApplication(opt =>
            {
                opt.MandateCancellationSubject = "MandateCancellationSubject";
                opt.MandateCancellationTemplateName = "MandateCancellationTemplateName";
                opt.MandateCancellationFromEmail = "MandateCancellationFromEmail";
                opt.MandateCancellationToEmail = "MandateCancellationToEmail";
                opt.MandateCancellationCcEmails = new List<string>();
                opt.MandateUploadedSubject = "MandateUploadedSubject";
                opt.MandateUploadedTemplateName = "MandateUploadedTemplateName";
                opt.MandateUploadedFromEmail = "MandateUploadedFromEmail";
                opt.MandateUploadedToEmail = "MandateUploadedToEmail";
                opt.MandateUploadedCcEmails = new List<string>();
            });
            sc.AddMandateAdapters();
            sc.AddMandateSql(configuration);
            sc.AddMandateJeDeclare(opt =>
            {
                opt.BaseUri = new Uri("https://recette.jedeclare.com/webservice/gestion/");
                opt.Login = "loginT";
                opt.Password = "passwordT";
                opt.JdcCompteId = "jdcCompteIdT";
                opt.HistoryDateEnabledBanks = "HistoryDateEnabledBanks";
            });

            sc.AddHttpContextAccessor();
            sc.AddSingleton<IConfiguration>(configuration);
            sc.AddServiceBusConfiguration(configuration);

            var sp = sc.BuildServiceProvider();

            // Make sure we don't forget services ; exclude services from Microsoft (IOption, ...)
            sc.Count(s => s.ServiceType.FullName?.StartsWith("KPMG") ?? false).Should().Be(21);

            // Test all services ; number of tests below should match the number of services above
            sp.GetService<IBankManager>().Should().NotBeNull();
            sp.GetService<IBbanManager>().Should().NotBeNull();
            sp.GetService<IMandateManager>().Should().NotBeNull();
            sp.GetService<ICompanyManager>().Should().NotBeNull();
            sp.GetService<IAsposeHelper>().Should().NotBeNull();
            sp.GetService<IGuidGenerator>().Should().NotBeNull();
            sp.GetService<IFakeDataManager>().Should().NotBeNull();
            sp.GetService<IDatabaseService>().Should().NotBeNull();
            sp.GetService<IJeDeclareService>().Should().NotBeNull();
            sp.GetService<IPortalManager>().Should().NotBeNull();
            sp.GetService<IMandateRepository>().Should().NotBeNull();
            sp.GetService<IJeDeclareClientFactory>().Should().NotBeNull();
            sp.GetService<IJeDeclareClient>().Should().NotBeNull();
            sp.GetService<INotificationsService>().Should().NotBeNull();
        }


        [Fact]
        public void AddMandateJeDeclare_NullService_ThrowsArgumentNullException()
        {
            IServiceCollection services = null!;
            ConfigurationManager configuration = new ConfigurationManager();
            Action act = () => services.AddMandateJeDeclare(opt =>
            {
                opt.BaseUri = new Uri("https://recette.jedeclare.com/webservice/gestion/");
                opt.Login = "loginT";
                opt.Password = "passwordT";
                opt.JdcCompteId = "jdcCompteIdT";
                opt.HistoryDateEnabledBanks = "HistoryDateEnabledBanks";
            });

            act.Should().Throw<ArgumentNullException>();
        }
        
        [Fact]
        public void AddMandateAdapters_NullService_ThrowsArgumentNullException()
        {
            IServiceCollection services = null!;
            ConfigurationManager configuration = new ConfigurationManager();
            Action act = () => services.AddMandateAdapters();

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddMandateApplication_NullService_ThrowsArgumentNullException()
        {
            IServiceCollection services = null!;
            ConfigurationManager configuration = new ConfigurationManager();
            Action act = () => services.AddMandateApplication(opt =>
            {
                opt.MandateCancellationSubject = "MandateCancellationSubject";
                opt.MandateCancellationTemplateName = "MandateCancellationTemplateName";
                opt.MandateCancellationFromEmail = "MandateCancellationFromEmail";
                opt.MandateCancellationToEmail = "MandateCancellationToEmail";
                opt.MandateCancellationCcEmails = new List<string>();
                opt.MandateUploadedSubject = "MandateUploadedSubject";
                opt.MandateUploadedTemplateName = "MandateUploadedTemplateName";
                opt.MandateUploadedFromEmail = "MandateUploadedFromEmail";
                opt.MandateUploadedToEmail = "MandateUploadedToEmail";
                opt.MandateUploadedCcEmails = new List<string>();
            });

            act.Should().Throw<ArgumentNullException>();
        }
    }
}
