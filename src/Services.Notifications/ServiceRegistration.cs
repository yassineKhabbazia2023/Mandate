// <copyright file="ServiceRegistration.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Notifications
{
    using Kpmg.Constellation.Notifications.V2.Client.Http;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public static class ServiceRegistration
    {
        public static IServiceCollection AddNotificationsApi(this IServiceCollection services, ConfigurationManager configuration)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddNotificationsClient(c => c.BaseUri = new Uri(configuration["MANDATE_NOTIFICATION_V2_API_URL"]!));

            services.AddSingleton<INotificationsProvider, NotificationsProvider>();

            return services;
        }
    }
}
