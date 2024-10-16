// <copyright file="ServiceRegistrationTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

using KPMG.Constellation.Portal.Client;
using KPMG.Pulse.Back.Accounting.Mandate.Adapters;
using KPMG.Pulse.Back.Accounting.Mandate.Application;
using KPMG.Pulse.Back.Accounting.Mandate.Formio.Client;
using KPMG.Pulse.Back.Accounting.Mandate.Formio.Client.Http;
using KPMG.Pulse.Back.Accounting.Mandate.JeDeclare.Client;
using KPMG.Pulse.Back.Accounting.Mandate.JeDeclare.Client.Http;
using KPMG.Pulse.Back.Accounting.Mandate.Notifications;
using KPMG.Pulse.Back.Accounting.Mandate.Portal;
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
            sc.AddMandateSql(opt => opt.ConnectionString = "a");
            sc.AddMandateJeDeclare(opt =>
            {
                opt.BaseUri = new Uri("https://recette.jedeclare.com/webservice/gestion/");
                opt.Login = "loginT";
                opt.Password = "passwordT";
                opt.JdcCompteId = "jdcCompteIdT";
                opt.HistoryDateEnabledBanks = "HistoryDateEnabledBanks";
            });
            sc.AddMandateFormio(opt =>
            {
                opt.BaseUri = new Uri("https://toto.com");
                opt.FormioApiKey = "test";
            });

            sc.AddHttpContextAccessor();
            sc.AddSingleton<IConfiguration>(configuration);
            sc.AddPortailApi((ConfigurationManager)configuration);
            sc.AddNotificationsApi(new Action<NotificationOptions>(options => options.BaseUrl = "https://notifications"));
            sc.AddServiceBusConfiguration(configuration);

            var sp = sc.BuildServiceProvider();

            // Make sure we don't forget services ; exclude services from Microsoft (IOption, ...)
            sc.Count(s => s.ServiceType.FullName?.StartsWith("KPMG") ?? false).Should().Be(25);

            // Test all services ; number of tests below should match the number of services above
            sp.GetService<IBankManager>().Should().NotBeNull();
            sp.GetService<IBbanManager>().Should().NotBeNull();
            sp.GetService<IMandateManager>().Should().NotBeNull();
            sp.GetService<ICompanyManager>().Should().NotBeNull();
            sp.GetService<IAsposeHelper>().Should().NotBeNull();
            sp.GetService<IGuidGenerator>().Should().NotBeNull();
            sp.GetService<IFormioManager>().Should().NotBeNull();
            sp.GetService<IFakeDataManager>().Should().NotBeNull();
            sp.GetService<IDatabaseService>().Should().NotBeNull();
            sp.GetService<IJeDeclareService>().Should().NotBeNull();
            sp.GetService<IPortalManager>().Should().NotBeNull();
            sp.GetService<IFormioService>().Should().NotBeNull();
            sp.GetService<IMandateRepository>().Should().NotBeNull();
            sp.GetService<IJeDeclareClientFactory>().Should().NotBeNull();
            sp.GetService<IJeDeclareClient>().Should().NotBeNull();
            sp.GetService<IFormioClientFactory>().Should().NotBeNull();
            sp.GetService<IFormioClient>().Should().NotBeNull();
            sp.GetService<IFormioManager>().Should().NotBeNull();
            sp.GetService<IPortalClientFactory>().Should().NotBeNull();
            sp.GetService<Portal.IAuthenticationContext>().Should().NotBeNull();
            sp.GetService<IPortalProvider>().Should().NotBeNull();
            sp.GetService<INotificationsService>().Should().NotBeNull();
            sp.GetService<INotificationsProvider>().Should().NotBeNull();
        }

        [Fact]
        public void AddNotificationsApi_NullService_ThrowsArgumentNullException()
        {
            IServiceCollection services = null!;
            ConfigurationManager configuration = new ConfigurationManager();
            Action act = () => services.AddNotificationsApi(new Action<NotificationOptions>(options => options.BaseUrl = "https://notifications"));
            act.Should().Throw<ArgumentNullException>();
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
        public void AddMandateFormio_NullService_ThrowsArgumentNullException()
        {
            IServiceCollection services = null!;
            ConfigurationManager configuration = new ConfigurationManager();
            Action act = () => services.AddMandateFormio(opt =>
            {
                opt.BaseUri = new Uri("https://toto.com");
                opt.FormioApiKey = "test";
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

        [Fact]
        public void AddPortailApi_NullService_ThrowsArgumentNullException()
        {
            IServiceCollection services = null!;
            ConfigurationManager configuration = new ConfigurationManager();
            Action act = () => services.AddPortailApi(configuration);
            act.Should().Throw<ArgumentNullException>();
        }
    }
}
