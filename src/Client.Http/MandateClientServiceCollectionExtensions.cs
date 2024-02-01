// <copyright file="MandateClientServiceCollectionExtensions.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Client.Http
{
    using KPMG.Pulse.Back.Accounting.Mandate.Client.Http.Clients;
    using KPMG.Pulse.Back.Accounting.Mandate.Client.Http.Options;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Contains extensions methods to add the support of the <see cref="HttpPortalClient"/> in the <see cref="IServiceCollection"/>.
    /// </summary>
    public static class MandateClientServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the required registrations for <c>Constellation Portal Service</c> client use.
        /// </summary>
        /// <param name="services"><see cref="IServiceCollection"/> which the registration will be performed on.</param>
        /// <param name="configureOptions">The action used to configure the <see cref="PortalClientOptions"/>.</param>
        /// <returns>The IServiceCollection passed as <paramref name="services"/> so that additional calls can be chained.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="services"/> or <paramref name="configureOptions"/> is null.</exception>
        public static IServiceCollection AddMandateClient(this IServiceCollection services, Action<MandateClientOptions> configureOptions)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (configureOptions is null)
            {
                throw new ArgumentNullException(nameof(configureOptions));
            }

            services.Configure(configureOptions);

            services.AddConstellationHttpClient();
            services.AddSingleton<IMandateClientFactory, HttpMandateClientFactory>();

            return services;
        }
    }
}