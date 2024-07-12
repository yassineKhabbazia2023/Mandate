// <copyright file="AccountEventsFunction.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Function.Functions;

using Azure.Messaging.ServiceBus;
using global::Pulse.Back.Events.IntegrationEvents;
using KPMG.Pulse.Back.Accounting.Mandate.Function;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Threading.Tasks;

public class AccountEventsFunction
{
    private readonly ILogger<AccountEventsFunction> logger;
    private readonly IEventsFunctionManager manager;

    public AccountEventsFunction(ILogger<AccountEventsFunction> logger, IEventsFunctionManager manager)
    {
        this.logger = logger;
        this.manager = manager;
    }

    [Function("AccountCreatedEventFunction")]
    public async Task RunAccountCreatedEventAsync(
[ServiceBusTrigger("account", "account-created-mandate", Connection = "serviceBusNameSpace")]
ServiceBusReceivedMessage message,
ServiceBusMessageActions messageActions)
    {
        this.logger.LogInformation("Message ID: {Id}", message.MessageId);
        this.logger.LogInformation("Message Body: {Body}", message.Body);
        this.logger.LogInformation("Message Content-Type: {ContentType}", message.ContentType);

        var accountEvent = JsonConvert.DeserializeObject<AccountCreatedEvent>(message.Body.ToString());
        if (accountEvent!.Data == null || accountEvent.Data?.AccountId <= 0)
        {
            await messageActions.CompleteMessageAsync(message);
            return;
        }

        var accountEntity = accountEvent!.Data!.ToModel();

        await this.manager.CreateAccountByEventAsync(accountEntity!);

        // Complete the message
        await messageActions.CompleteMessageAsync(message);
    }

    [Function("AccountDeletedEventFunction")]
    public async Task RunAccountDeletedEventAsync(
[ServiceBusTrigger("account", "account-removed-mandate", Connection = "serviceBusNameSpace")]
ServiceBusReceivedMessage message,
ServiceBusMessageActions messageActions)
    {
        this.logger.LogInformation("Message ID: {Id}", message.MessageId);
        this.logger.LogInformation("Message Body: {Body}", message.Body);
        this.logger.LogInformation("Message Content-Type: {ContentType}", message.ContentType);

        var accountEvent = JsonConvert.DeserializeObject<AccountRemovedEvent>(message.Body.ToString());
        if (accountEvent!.Data == null || accountEvent.Data?.AccountId <= 0)
        {
            await messageActions.CompleteMessageAsync(message);
            return;
        }

        await this.manager.DeleteAccountByEventAsync(accountEvent.Data!.AccountId);

        // Complete the message
        await messageActions.CompleteMessageAsync(message);
    }

    [Function("AccountUpdatedEventFunction")]
    public async Task RunAccountUpdatedEventAsync(
[ServiceBusTrigger("account", "account-updated-mandate", Connection = "serviceBusNameSpace")]
ServiceBusReceivedMessage message,
ServiceBusMessageActions messageActions)
    {
        this.logger.LogInformation("Message ID: {Id}", message.MessageId);
        this.logger.LogInformation("Message Body: {Body}", message.Body);
        this.logger.LogInformation("Message Content-Type: {ContentType}", message.ContentType);

        var accountEvent = JsonConvert.DeserializeObject<AccountUpdatedEvent>(message.Body.ToString());

        if (accountEvent!.Data == null || accountEvent.Data?.AccountId <= 0)
        {
            await messageActions.CompleteMessageAsync(message);
            return;
        }

        var accountEntity = accountEvent.Data!.ToModel();

        await this.manager.UpdateAccountByEventAsync(accountEntity!);

        // Complete the message
        await messageActions.CompleteMessageAsync(message);
    }
}
