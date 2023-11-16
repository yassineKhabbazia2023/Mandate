// <copyright file="ServiceRegistration.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Portal
{
    using KPMG.Constellation.Portal.Client.Http;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public static class ServiceRegistration
    {
        public static IServiceCollection AddPortailApi(this IServiceCollection services, ConfigurationManager configuration)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddPortalClient(c => c.BaseUri = new Uri(configuration["PORTAL_API_URL"] !));

            services.AddSingleton<IAuthenticationContext, AuthenticationContext>();
            services.AddSingleton<IPortalProvider, PortalProvider>();

            return services;
        }
    }
}