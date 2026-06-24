// <copyright file="ServiceRegistrationTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

using KPMG.Pulse.Back.Accounting.Mandate.Adapters;
using KPMG.Pulse.Back.Accounting.Mandate.Application;
using KPMG.Pulse.Back.Accounting.Mandate.Application.Interfaces;
using KPMG.Pulse.Back.Accounting.Mandate.Application.Models;
using KPMG.Pulse.Back.Accounting.Mandate.JeDeclare.Client;
using KPMG.Pulse.Back.Accounting.Mandate.JeDeclare.Client.Http;
using KPMG.Pulse.Back.Accounting.Mandate.Notifications;
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
            sc.AddSingleton<IGetAcceptClient, FakeGetAcceptClient>();
            sc.AddNotificationsApi(new Action<NotificationOptions>(options => options.BaseUrl = "https://notifications"));
            sc.AddServiceBusConfiguration(configuration);

            var sp = sc.BuildServiceProvider();

            // Make sure we don't forget services ; exclude services from Microsoft (IOption, ...)
            sc.Count(s => s.ServiceType.FullName?.StartsWith("KPMG") ?? false).Should().Be(29);

            // Test all services ; number of tests below should match the number of services above
            sp.GetService<IBankManager>().Should().NotBeNull();
            sp.GetService<IBbanManager>().Should().NotBeNull();
            sp.GetService<IMandateManager>().Should().NotBeNull();
            sp.GetService<ICompanyManager>().Should().NotBeNull();
            sp.GetService<IPdfTextReplacer>().Should().NotBeNull();
            sp.GetService<IPdfFormFieldFiller>().Should().NotBeNull();
            sp.GetService<IPdfHelper>().Should().NotBeNull();
            sp.GetService<IGuidGenerator>().Should().NotBeNull();
            sp.GetService<IFakeDataManager>().Should().NotBeNull();
            sp.GetService<IDatabaseService>().Should().NotBeNull();
            sp.GetService<IPaymentPreferenceStore>().Should().NotBeNull();
            sp.GetService<ISepaMandateStore>().Should().NotBeNull();
            sp.GetService<IJeDeclareService>().Should().NotBeNull();
            sp.GetService<IMandateRepository>().Should().NotBeNull();
            sp.GetService<IPaymentPreferencesService>().Should().NotBeNull();
            sp.GetService<IGetAcceptClient>().Should().NotBeNull();
            sp.GetService<IPaymentPreferenceStrategy>().Should().NotBeNull();
            sp.GetServices<IPaymentPreferenceStrategy>().Should().HaveCount(2);
            sp.GetService<ISepaMandateTemplateProvider>().Should().NotBeNull();
            sp.GetService<ISepaMandatePdfGenerator>().Should().NotBeNull();
            sp.GetService<IPaymentPreferenceRepository>().Should().NotBeNull();
            sp.GetService<ISepaMandateRepository>().Should().NotBeNull();
            sp.GetService<IJeDeclareClientFactory>().Should().NotBeNull();
            sp.GetService<IJeDeclareClient>().Should().NotBeNull();
            sp.GetService<INotificationsService>().Should().NotBeNull();
            sp.GetService<INotificationsProvider>().Should().NotBeNull();
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

        /// <summary>
        /// Test double used to satisfy SEPA payment preference dependencies in service registration tests.
        /// </summary>
        private sealed class FakeGetAcceptClient : IGetAcceptClient
        {
            /// <inheritdoc />
            public Task<GetAcceptMandateSignatureResponse> SendMandateForSignatureAsync(
                GetAcceptMandateSignatureRequest request)
            {
                return Task.FromResult(new GetAcceptMandateSignatureResponse("document-id", "https://signature"));
            }
        }
    }
}
