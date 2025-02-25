namespace KPMG.Pulse.Back.Accounting.Mandate.Function;

using System.Text.Json;
using Azure.Messaging.ServiceBus;
using KPMG.Pulse.Back.Accounting.Mandate.Function.Interfaces;
using KPMG.Pulse.Back.Accounting.Mandate.Function.Models;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

public class MandateEventPublisher : IMandateEventPublisher
{
    private readonly ILogger<MandateEventPublisher> _logger;
    private readonly IAzureClientFactory<ServiceBusSender> _azureClientFactory;
    private readonly string _serviceBusMandateTopic;

    public MandateEventPublisher(
        ILogger<MandateEventPublisher> logger,
        IOptions<ServiceBusOptions> serviceBusOptions,
        IAzureClientFactory<ServiceBusSender> azureClientFactory)
    {
        _logger = logger;
        _serviceBusMandateTopic = serviceBusOptions.Value.ServiceBusMandateTopic;
        _azureClientFactory = azureClientFactory;
    }

    public async Task PublishEventAsync<TEvent>(TEvent message)
    {
        var sender = _azureClientFactory.CreateClient(_serviceBusMandateTopic);
        var messageBody = JsonSerializer.Serialize(message);
        var serviceBusMessage = new ServiceBusMessage(messageBody)
        {
            ContentType = "application/json",
            ApplicationProperties = { ["EventType"] = typeof(TEvent).Name}
        };

        await sender.SendMessageAsync(serviceBusMessage);
        _logger.LogInformation("Message sent: {Message}", messageBody);
    }
}