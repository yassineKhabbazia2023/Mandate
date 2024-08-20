// <copyright file="ServiceRegistration.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace Mandate.Messaging
{
    using Azure.Identity;
    using Azure.Messaging.ServiceBus;
    using KPMG.Pulse.Back.Accounting.Mandate.Application.Interfaces;
    using Mandate.Messaging.Models;
    using Mandate.Messaging.Providers;
    using Microsoft.Extensions.Azure;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public static class ServiceRegistration
    {
        /// <summary>
        /// Extension to configure Azure Service Bus.
        /// </summary>
        /// <param name="services">IServiceCollection./param>.
        /// <param name="configuration">configuration.</param>.
        public static IServiceCollection AddServiceBusConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            if (configuration is null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            ArgumentException.ThrowIfNullOrEmpty(configuration["ServiceBusQueueCreateQueueName"]);
            ArgumentException.ThrowIfNullOrEmpty(configuration["hubServiceBus:fullyQualifiedNamespace"]);
            ArgumentException.ThrowIfNullOrEmpty(configuration["hubServiceBus:clientId"]);

            services.Configure<ServiceBusOptions>(opt =>
            {
                opt.ServiceBusCreateMandateQueueName = configuration["ServiceBusQueueCreateQueueName"]!;
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
                        !.CreateSender(configuration["ServiceBusQueueCreateQueueName"]))
                .WithName(configuration["ServiceBusQueueCreateQueueName"]);
            });
            services.AddScoped<IEventPublisher, EventPublisher>();

            return services;
        }
    }
}
