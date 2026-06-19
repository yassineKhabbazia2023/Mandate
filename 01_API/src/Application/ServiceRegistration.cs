// <copyright file="ServiceRegistration.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Application
{
    using KPMG.Pulse.Back.Accounting.Mandate.Application.Interfaces;
    using KPMG.Pulse.Back.Accounting.Mandate.Application.Managers;
    using KPMG.Pulse.Back.Accounting.Mandate.Application.Strategies;
    using Microsoft.Extensions.DependencyInjection;

    public static class ServiceRegistration
    {
        public static IServiceCollection AddMandateApplication(this IServiceCollection services, Action<MandateEmailOptions> options)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddOptions().Configure(options);

            services.AddScoped<IBankManager, BankManager>();
            services.AddScoped<IBbanManager, BbanManager>();
            services.AddScoped<IMandateManager, MandateManager>();
            services.AddScoped<ICompanyManager, CompanyManager>();
            services.AddScoped<IFakeDataManager, FakeDataManager>();
            services.AddScoped<IPdfTextReplacer, PdfSharpTextReplacer>();
            services.AddScoped<IPdfHelper, PdfHelper>();
            services.AddScoped<IGuidGenerator, GuidGenerator>();
            services.AddScoped<IEventManager, EventManager>();
            services.AddScoped<IPaymentPreferencesService, PaymentPreferencesService>();
            services.AddScoped<IPaymentPreferenceStrategy, OtherPaymentPreferenceStrategy>();

            return services;
        }
    }
}
