// <copyright file="EventPublisher.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

using Azure.Messaging.ServiceBus;
using KPMG.Pulse.Back.Accounting.Mandate.Application.Interfaces;
using Mandate.Messaging.Models;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Mandate.Messaging.Providers
{
    /// <summary>
    /// EventPublisher.
    /// </summary>
    public class EventPublisher : IEventPublisher
    {
        private readonly IOptions<ServiceBusOptions> serviceBusOptions;
        private readonly IAzureClientFactory<ServiceBusSender> azureClientFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventPublisher"/> class.
        /// </summary>
        /// <param name="serviceBusOptions">serviceBusOptions.</param>
        /// <param name="azureClientFactory">azureClientFactory.</param>
        public EventPublisher(IOptions<ServiceBusOptions> serviceBusOptions, IAzureClientFactory<ServiceBusSender> azureClientFactory)
        {
            this.serviceBusOptions = serviceBusOptions;
            this.azureClientFactory = azureClientFactory;
        }

        /// <inheritdoc/>
        public async Task PublishToQueueAsync<T>(T message, string? correlationId = null, string? queueName = null)
        {
            var selectedQueue = queueName ?? this.serviceBusOptions.Value.ServiceBusCreateMandateQueueName;
            var sender = this.azureClientFactory.CreateClient(selectedQueue);
            var messageBody = JsonSerializer.Serialize(message);
            var serviceBusMessage = new ServiceBusMessage(messageBody)
            {
                ContentType = "application/json",
                CorrelationId = correlationId,
            };
            await sender.SendMessageAsync(serviceBusMessage);
        }
    }
}
