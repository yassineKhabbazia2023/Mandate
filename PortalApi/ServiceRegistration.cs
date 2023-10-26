// <copyright file="ServiceRegistration.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.PortalApi
{
    using Microsoft.Extensions.DependencyInjection;

    public static class ServiceRegistration
    {
        public static IServiceCollection AddPortailApi(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddSingleton<IAuthenticationContext, AuthenticationContext>();

            return services;
        }
    }
}