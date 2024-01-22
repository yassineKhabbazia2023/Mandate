// <copyright file="Startup.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(KPMG.Pulse.Back.Accounting.Mandate.AzureFunctions.Startup))]

namespace KPMG.Pulse.Back.Accounting.Mandate.AzureFunctions
{
    [ExcludeFromCodeCoverage]
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddConstellationHttpClient();
            builder.Services.AddSystemAccountAuthenticationProvider<IConfiguration>((settings, configuration) =>
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

            builder.Services.AddApplicationInsightsTelemetry();

            builder.Services.AddSingleton<IPreloadManager, PreloadManager>();
            builder.Services.AddSingleton<IMandateProvider, MandateProvider>();
        }
    }
}
