// <copyright file="ServiceRegistration.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.PortalApi
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    using KPMG.Constellation.Portal.Client.Http;

    public static class ServiceRegistration
    {
        public static IServiceCollection AddPortailApi(this IServiceCollection services, ConfigurationManager configuration)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddPortalClient(c => c.BaseUri = new Uri(configuration["PORTAL_API_URL"] !));

            services.AddSystemAccountAuthenticationProvider<IConfiguration>((settings, configuration) =>
            {
                if (string.IsNullOrWhiteSpace(configuration["ConstellationClientId"]))
                {
                    throw new InvalidOperationException($"Client Id for Identity Service is not definied (Missing setting: ConstellationClientId)");
                }

                if (string.IsNullOrWhiteSpace(configuration["ConstellationSecret"]))
                {
                    throw new InvalidOperationException($"Client Secret for Identity Service is not definied (Missing setting: ConstellationSecret)");
                }

                if (string.IsNullOrWhiteSpace(configuration["ConstellationAudience"]))
                {
                    throw new InvalidOperationException($"Scope for Identity Service is not definied (Missing setting: ConstellationAudience)");
                }

                if (string.IsNullOrWhiteSpace(configuration["ConstellationTenant"]))
                {
                    throw new InvalidOperationException($"Tenant for Identity Service is not definied (Missing setting: ConstellationTenant)");
                }

                settings.Audience = configuration["ConstellationAudience"];
                settings.ClientId = configuration["ConstellationClientId"];
                settings.ClientSecret = configuration["ConstellationSecret"];
                settings.Tenant = configuration["ConstellationTenant"];
            });

            services.AddSingleton<IAuthenticationContext, AuthenticationContext>();
            services.AddSingleton<IPortalProvider, PortalProvider>();

            return services;
        }
    }
}