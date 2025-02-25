using Azure.Identity;
using Azure.Messaging.ServiceBus;
using KPMG.Pulse.Back.Accounting.Mandate.Function.Interfaces;
using KPMG.Pulse.Back.Accounting.Mandate.Function.Models;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace KPMG.Pulse.Back.Accounting.Mandate.Function.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddServiceBus(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<ServiceBusOptions>(opt =>
        {
            opt.ServiceBusMandateTopic = configuration["ServiceBusMandateTopic"]!;
        });

        services.AddAzureClients(builder =>
        {
            builder.AddServiceBusClientWithNamespace(configuration["hubServiceBus:fullyQualifiedNamespace"])
                .WithCredential(new DefaultAzureCredential(new DefaultAzureCredentialOptions
                {
                    ManagedIdentityClientId = configuration["hubServiceBus:clientId"],
                }));
            builder.AddClient<ServiceBusSender, ServiceBusClientOptions>((_, _, provider) =>
                    provider
                        !.GetService<ServiceBusClient>()
                        !.CreateSender(configuration["ServiceBusMandateTopic"]))
                .WithName(configuration["ServiceBusMandateTopic"]);
        });
        services.AddScoped<IMandateEventPublisher, MandateEventPublisher>();

        return services;
    }
}