// <copyright file="ServiceRegistration.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Notifications
{
    using KPMG.Pulse.Back.Accounting.Mandate.Application;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;

    public static class ServiceRegistration
    {
        public static IServiceCollection AddNotificationsApi(this IServiceCollection services, Action<NotificationOptions> options)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }
            services.AddOptions().Configure(options);

            //services.AddNotificationsClient(c => c.BaseUri = new Uri(configuration["MANDATE_NOTIFICATION_V2_API_URL"]!));

            services.AddSingleton<INotificationsProvider, NotificationsProvider>();

            return services;
        }
    }
}
