// <copyright file="ServiceRegistration.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Adapters
{
    using KPMG.Pulse.Back.Accounting.Mandate.Application.Interfaces;
    using Microsoft.Extensions.DependencyInjection;

    public static class ServiceRegistration
    {
        public static IServiceCollection AddMandateAdapters(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddScoped<IDatabaseService, SqlAdapter>();
            services.AddScoped<IPaymentPreferenceStore, PaymentPreferencesSqlAdapter>();
            services.AddScoped<ISepaMandateStore, PaymentPreferencesSqlAdapter>();
            services.AddScoped<IPaymentPreferenceCleanupStore, PaymentPreferencesSqlAdapter>();
            services.AddScoped<IJeDeclareService, JeDeclareAdapter>();
            services.AddScoped<INotificationsService, NotificationsAdapter>();

            return services;
        }
    }
}
