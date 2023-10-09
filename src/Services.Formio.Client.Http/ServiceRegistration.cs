// <copyright file="ServiceRegistration.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Formio.Client.Http
{
    using Microsoft.Extensions.DependencyInjection;

    public static class ServiceRegistration
    {
        public static IServiceCollection AddMandateFormio(this IServiceCollection services, Action<FormioOptions> options)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddOptions().Configure(options);

            services.AddConstellationHttpClient();
            services.AddScoped<IFormioClientFactory, HttpFormioClientFactory>();

            return services;
        }
    }
}
