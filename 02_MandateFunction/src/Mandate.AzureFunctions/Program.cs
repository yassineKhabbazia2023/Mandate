using System.Diagnostics.CodeAnalysis;
using KPMG.Pulse.Back.Accounting.Mandate.AzureFunctions;
using KPMG.Pulse.Back.Accounting.Mandate.Client.Http;
using KPMG.Pulse.Back.Accounting.Mandate.Function;
using Mandate.AzureFunctions.Interfaces;
using Mandate.AzureFunctions.Managers;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices((context, services) =>
    {
        var config = context.Configuration;
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        services.AddConstellationHttpClient();
        services.AddSystemAccountAuthenticationProvider<IConfiguration>((settings, configuration) =>
        {
            if (string.IsNullOrWhiteSpace(configuration["AuthClientId"]))
            {
                throw new InvalidOperationException($"Client Id for Identity Service is not definied (Missing setting: ConstellationClientId)");
            }

            if (string.IsNullOrWhiteSpace(configuration["AuthClientSecret"]))
            {
                throw new InvalidOperationException($"Client Secret for Identity Service is not definied (Missing setting: ConstellationSecret)");
            }

            if (string.IsNullOrWhiteSpace(configuration["AuthAudience"]))
            {
                throw new InvalidOperationException($"Scope for Identity Service is not definied (Missing setting: ConstellationAudience)");
            }

            if (string.IsNullOrWhiteSpace(configuration["AuthTenant"]))
            {
                throw new InvalidOperationException($"Tenant for Identity Service is not definied (Missing setting: ConstellationTenant)");
            }

            settings.Audience = configuration["AuthAudience"];
            settings.ClientId = configuration["AuthClientId"];
            settings.ClientSecret = configuration["AuthClientSecret"];
            settings.Tenant = configuration["AuthTenant"];
        });

        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();

        services.AddSingleton<IPreloadManager, PreloadManager>();
        services.AddSingleton<IMandateProvider, MandateProvider>();
        services.AddSingleton<IMandateFunctionManager, MandateFunctionManager>();

        services.AddMandateClient(options =>
        {
            options.BaseUri = new Uri(config["MANDATE_API_URL"]!);
        });
    })
    .Build();

host.Run();

[ExcludeFromCodeCoverage]
public partial class Program { }
