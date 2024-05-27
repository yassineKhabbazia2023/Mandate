// <copyright file="RoleEventsFunction.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Function.Functions;

using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using global::Pulse.Back.Events.IntegrationEvents;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

public class RoleEventsFunction
{
    private readonly ILogger<RoleEventsFunction> logger;
    private readonly IEventsFunctionManager manager;

    public RoleEventsFunction(ILogger<RoleEventsFunction> logger, IEventsFunctionManager manager)
    {
        this.logger = logger;
        this.manager = manager;
    }

    [Function("RoleCreatedEventFunction")]
    public async Task RoleCreatedEvent(
    [ServiceBusTrigger("role", "role-created-mandate", Connection = "serviceBusNameSpace")]
    ServiceBusReceivedMessage message,
    ServiceBusMessageActions messageActions)
    {
        this.logger.LogInformation("Message ID: {Id}", message.MessageId);
        this.logger.LogInformation("Message Body: {Body}", message.Body);
        this.logger.LogInformation("Message Content-Type: {ContentType}", message.ContentType);

        var roleEvent = JsonConvert.DeserializeObject<RoleCreatedEvent>(message.Body.ToString());
        if (roleEvent!.Data == null || roleEvent.Data?.AccountId <= 0)
        {
            await messageActions.CompleteMessageAsync(message);
            return;
        }

        var roleEntity = roleEvent!.Data!.ToModel();

        await this.manager.CreateRoleByEventAsync(roleEntity!);

        // Complete the message
        await messageActions.CompleteMessageAsync(message);
    }

    [Function("RoleDeletedEventFunction")]
    public async Task RunRoleDeletedEvent(
    [ServiceBusTrigger("account", "role-deleted-mandate", Connection = "serviceBusNameSpace")]
    ServiceBusReceivedMessage message,
    ServiceBusMessageActions messageActions)
    {
        this.logger.LogInformation("Message ID: {Id}", message.MessageId);
        this.logger.LogInformation("Message Body: {Body}", message.Body);
        this.logger.LogInformation("Message Content-Type: {ContentType}", message.ContentType);

        var roleEvent = JsonConvert.DeserializeObject<RoleDeletedEvent>(message.Body.ToString());
        if (roleEvent!.Data == null || roleEvent.Data?.AccountId <= 0 || roleEvent.Data?.ContactId <= 0)
        {
            return;
        }

        await this.manager.DeleteRoleByEventAsync(roleEvent.Data!.ToModel());

        // Complete the message
        await messageActions.CompleteMessageAsync(message);
    }
}
