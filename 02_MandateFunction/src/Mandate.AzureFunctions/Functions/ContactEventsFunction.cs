// <copyright file="ContactEventsFunction.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Function.Functions;

using Azure.Messaging.ServiceBus;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using global::Pulse.Back.Events.IntegrationEvents;
using Microsoft.Azure.Functions.Worker;

public class ContactEventsFunction
{
    private readonly ILogger<ContactEventsFunction> logger;
    private readonly IEventsFunctionManager manager;

    public ContactEventsFunction(ILogger<ContactEventsFunction> logger, IEventsFunctionManager manager)
    {
        this.logger = logger;
        this.manager = manager;
    }

    [Function("ContactCreatedEventFunction")]
    public async Task RunContactCreatedEventAsync(
[ServiceBusTrigger("contact", "contact-created-mandate", Connection = "serviceBusNameSpace")]
ServiceBusReceivedMessage message,
ServiceBusMessageActions messageActions)
    {
        this.logger.LogInformation("Message ID: {Id}", message.MessageId);
        this.logger.LogInformation("Message Body: {Body}", message.Body);
        this.logger.LogInformation("Message Content-Type: {ContentType}", message.ContentType);

        var contactEvent = JsonConvert.DeserializeObject<ContactCreatedEvent>(message.Body.ToString());
        if (contactEvent!.Data == null || contactEvent.Data?.ContactId <= 0)
        {
            // Complete the message
            await messageActions.CompleteMessageAsync(message);
            return;
        }

        var contactEntity = contactEvent!.Data!.ToModel();

        await this.manager.CreateContactByEventAsync(contactEntity!);

        // Complete the message
        await messageActions.CompleteMessageAsync(message);
    }

    [Function("ContactDeletedEventFunction")]
    public async Task RunContactDeletedEventAsync(
[ServiceBusTrigger("contact", "contact-removed-mandate", Connection = "serviceBusNameSpace")]
ServiceBusReceivedMessage message,
ServiceBusMessageActions messageActions)
    {
        this.logger.LogInformation("Message ID: {Id}", message.MessageId);
        this.logger.LogInformation("Message Body: {Body}", message.Body);
        this.logger.LogInformation("Message Content-Type: {ContentType}", message.ContentType);

        var contactEvent = JsonConvert.DeserializeObject<ContactRemovedEvent>(message.Body.ToString());
        if (contactEvent!.Data == null || contactEvent.Data?.ContactId <= 0)
        {
            // Complete the message
            await messageActions.CompleteMessageAsync(message);
            return;
        }

        await this.manager.DeleteContactByEventAsync(contactEvent.Data!.ContactId);

        // Complete the message
        await messageActions.CompleteMessageAsync(message);
    }

    [Function("ContactUpdatedEventFunction")]
    public async Task RunContactUpdatedEventAsync(
[ServiceBusTrigger("contact", "contact-updated-mandate", Connection = "serviceBusNameSpace")]
ServiceBusReceivedMessage message,
ServiceBusMessageActions messageActions)
    {
        this.logger.LogInformation("Message ID: {Id}", message.MessageId);
        this.logger.LogInformation("Message Body: {Body}", message.Body);
        this.logger.LogInformation("Message Content-Type: {ContentType}", message.ContentType);

        var contactEvent = JsonConvert.DeserializeObject<ContactUpdatedEvent>(message.Body.ToString());

        if (contactEvent!.Data == null || contactEvent.Data?.ContactId <= 0)
        {
            // Complete the message
            await messageActions.CompleteMessageAsync(message);
            return;
        }

        var contactEntity = contactEvent.Data!.ToModel();

        await this.manager.UpdateContactByEventAsync(contactEntity!);

        // Complete the message
        await messageActions.CompleteMessageAsync(message);
    }
}
