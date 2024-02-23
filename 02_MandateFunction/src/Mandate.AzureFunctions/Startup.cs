// <copyright file="Startup.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

using System;
using System.Diagnostics.CodeAnalysis;
using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using KPMG.Pulse.Back.Accounting.Mandate.Client.Http;
using Mandate.AzureFunctions.Interfaces;
using Mandate.AzureFunctions.Managers;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(KPMG.Pulse.Back.Accounting.Mandate.AzureFunctions.Startup))]

namespace KPMG.Pulse.Back.Accounting.Mandate.AzureFunctions
{
    [ExcludeFromCodeCoverage]
    public class Startup : FunctionsStartup
    {
        private static bool IsDevelopment
        {
            get
            {
                return string.Compare(
                    Environment.GetEnvironmentVariable("IsDevelopement"),
                    "true",
                    StringComparison.InvariantCultureIgnoreCase) == 0;
            }
        }

        public override void Configure(IFunctionsHostBuilder builder)
        {
            var config = builder.GetContext().Configuration;
            builder!.Services.AddConstellationHttpClient();
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
            builder.Services.AddSingleton<IMandateManager, MandateManager>();

            builder.Services.AddMandateClient(options =>
            {
                options.BaseUri = new Uri(config["MANDATE_API_URL"]);
            });
        }

        public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
        {
            var builtConfig = builder!.ConfigurationBuilder.Build();

            if (IsDevelopment)
            {
                var secretClient = new SecretClient(
                    new Uri($"https://{builtConfig["MANDATE_VAULT_NAME"]}.vault.azure.net/"),
                    new ClientSecretCredential(builtConfig["MANDATE_IDENTITYSERVICE_AAD_TENANT"], builtConfig["MANDATE_KEYVAULT_CLIENT_ID"], builtConfig["MANDATE_KEYVAULT_CLIENT_SECRET"]));

                builder.ConfigurationBuilder
                   .AddAzureKeyVault(secretClient, new KeyVaultSecretManager())
                   .AddEnvironmentVariables()
                   .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                   .Build();
            }
            else
            {
                builder.ConfigurationBuilder.AddAzureKeyVault(
                   new Uri($"https://{builtConfig["MANDATE_VAULT_NAME"]}.vault.azure.net/"),
                   new DefaultAzureCredential());
            }
        }
    }
}
