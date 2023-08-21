// <copyright file="ServiceCollectionExtensions.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.AspNetCore
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMandateApplication(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddScoped<IBbanManager, Application.BbanManager>();

            return services;
        }
    }
}
