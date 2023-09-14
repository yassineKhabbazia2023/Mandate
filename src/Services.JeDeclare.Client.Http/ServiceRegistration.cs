// <copyright file="ServiceRegistration.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.JeDeclare.Client.Http
{
    using Microsoft.Extensions.DependencyInjection;

    public static class ServiceRegistration
    {
        public static IServiceCollection AddMandateJeDeclare(this IServiceCollection services, Action<JeDeclareOptions> options)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddOptions().Configure(options);

            services.AddConstellationHttpClient();
            services.AddScoped<IJeDeclareClientFactory, HttpJeDeclareClientFactory>();

            return services;
        }
    }
}
