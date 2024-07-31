// <copyright file="AccountEventsFunctionTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.AzureFunctions.Tests;

using Azure.Messaging.ServiceBus;
using KPMG.Pulse.Back.Accounting.Mandate.Function.Functions;
using KPMG.Pulse.Back.Accounting.Mandate.Function;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using global::Pulse.Back.Events.IntegrationEvents;
using Newtonsoft.Json;
using KPMG.Pulse.Back.Accounting.Mandate.Client;
using global::Pulse.Back.Events.IntegrationEvents.EventsData;
using Microsoft.EntityFrameworkCore.ChangeTracking;

internal class AccountCreatedEventBuilder
{
    private AccountStateEventData? accountStateEventData;

    public AccountCreatedEventBuilder()
    {
        accountStateEventData =
                   new AccountStateEventData()
                   {
                       AccountId = 1,
                       AccountNumber = "accountNumber",
                       AccountGlobalUniqueId = Guid.NewGuid(),
                       LegalName = "Test",
                   };
    }

    public AccountCreatedEventBuilder WithoutEventData()
    {
        accountStateEventData = null;
        return this;
    }


    public ServiceBusReceivedMessage Build()
    {
        return ServiceBusModelFactory.ServiceBusReceivedMessage(
           body: BinaryData.FromString(JsonConvert.SerializeObject(new AccountCreatedEvent(accountStateEventData!))),
           messageId: "123",
           contentType: "application/json");
    }
}

internal class AccountUpdatedEventBuilder
{
    private AccountStateEventData? accountStateEventData;

    public AccountUpdatedEventBuilder()
    {
        accountStateEventData =
                   new AccountStateEventData()
                   {
                       AccountId = 1,
                       AccountNumber = "accountNumber",
                       AccountGlobalUniqueId = Guid.NewGuid(),
                       LegalName = "Test",
                   };
    }

    public AccountUpdatedEventBuilder WithoutEventData()
    {
        accountStateEventData = null;
        return this;
    }


    public ServiceBusReceivedMessage Build()
    {
        return ServiceBusModelFactory.ServiceBusReceivedMessage(
           body: BinaryData.FromString(JsonConvert.SerializeObject(new AccountUpdatedEvent(accountStateEventData!))),
           messageId: "123",
           contentType: "application/json");
    }
}

internal class AccountDeletedEventBuilder
{
    private AccountRemovedEventData accountRemovedEventData;

    public AccountDeletedEventBuilder()
    {

        accountRemovedEventData = new AccountRemovedEventData { AccountId = 1 };
    }

    public AccountDeletedEventBuilder WithAccountId(int accountId)
    {
        accountRemovedEventData.AccountId = accountId;
        return this;
    }


    public (ServiceBusReceivedMessage message, AccountRemovedEvent accountRemovedEvent) Build()
    {
        // Arrange
        var accountEvent = new AccountRemovedEvent(accountRemovedEventData);

        return (ServiceBusModelFactory.ServiceBusReceivedMessage(
                   body: BinaryData.FromString(JsonConvert.SerializeObject(accountEvent)),
                   messageId: "123",
                   contentType: "application/json"), accountEvent);
    }
}
public class AccountEventsFunctionTest
{
    private readonly Mock<ILogger<AccountEventsFunction>> logger;
    private readonly Mock<IEventsFunctionManager> manager;
    private readonly Mock<ServiceBusMessageActions> messageActions;
    private readonly AccountEventsFunction function;

    public AccountEventsFunctionTest()
    {
        logger = new Mock<ILogger<AccountEventsFunction>>();
        manager = new Mock<IEventsFunctionManager>();
        messageActions = new Mock<ServiceBusMessageActions>();
        function = new AccountEventsFunction(logger.Object, manager.Object);
    }

    [Fact]
    public async Task RunAccountCreatedEventAsync_InValidMessage_ShouldCompleteMessage()
    {
        // Arrange
        ServiceBusReceivedMessage message = new AccountCreatedEventBuilder().WithoutEventData().Build();

        messageActions.Setup(x => x.CompleteMessageAsync(message, It.IsAny<CancellationToken>()));

        // Act
        await function.RunAccountCreatedEventAsync(message, messageActions.Object);

        // Assert
        manager.Verify(x => x.CreateAccountByEventAsync(It.IsAny<Account>()), Times.Never);
        messageActions.Verify(x => x.CompleteMessageAsync(message, It.IsAny<CancellationToken>()), Times.Once);
    }

   

    [Fact]
    public async Task RunAccountCreatedEventAsync_ValidMessage_ShouldCompleteMessage()
    {
        // Arrange

        var message = new AccountCreatedEventBuilder().Build();

        messageActions.Setup(x => x.CompleteMessageAsync(message, It.IsAny<CancellationToken>()));

        // Act
        await function.RunAccountCreatedEventAsync(message, messageActions.Object);

        // Assert
        manager.Verify(x => x.CreateAccountByEventAsync(It.IsAny<Account>()), Times.Once);
        messageActions.Verify(x => x.CompleteMessageAsync(message, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RunAccountDeletedEventAsync_InValidMessage_ShouldCompleteMessage()
    {
        // Arrange
        var (message, accountEvent) = new AccountDeletedEventBuilder().WithAccountId(0).Build();

        manager.Setup(x => x.DeleteAccountByEventAsync(accountEvent.Data.AccountId));
        messageActions.Setup(x => x.CompleteMessageAsync(message, It.IsAny<CancellationToken>()));

        // Act
        await function.RunAccountDeletedEventAsync(message, messageActions.Object);

        // Assert
        manager.Verify(x => x.DeleteAccountByEventAsync(accountEvent.Data.AccountId), Times.Never);
        messageActions.Verify(x => x.CompleteMessageAsync(message, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RunAccountDeletedEventAsync_ValidMessage_ShouldCompleteMessage()
    {
        // Arrange
        var (message, accountEvent) = new AccountDeletedEventBuilder().Build();

        manager.Setup(x => x.DeleteAccountByEventAsync(accountEvent.Data.AccountId));
        messageActions.Setup(x => x.CompleteMessageAsync(message, It.IsAny<CancellationToken>()));

        // Act
        await function.RunAccountDeletedEventAsync(message, messageActions.Object);

        // Assert
        manager.Verify(x => x.DeleteAccountByEventAsync(accountEvent.Data.AccountId), Times.Once);
        messageActions.Verify(x => x.CompleteMessageAsync(message, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RunAccountUpdatedEventAsync_ValidMessage_ShouldCompleteMessage()
    {
        // Arrange
        var message = new AccountUpdatedEventBuilder().Build();

        manager.Setup(x => x.UpdateAccountByEventAsync(It.IsAny<Account>()));
        messageActions.Setup(x => x.CompleteMessageAsync(message, It.IsAny<CancellationToken>()));

        // Act
        await function.RunAccountUpdatedEventAsync(message, messageActions.Object);

        // Assert
        manager.Verify(x => x.UpdateAccountByEventAsync(It.IsAny<Account>()), Times.Once);
        messageActions.Verify(x => x.CompleteMessageAsync(message, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RunAccountUpdatedEventAsync_InValidMessage_ShouldCompleteMessage()
    {
        // Arrange
        var message = new AccountUpdatedEventBuilder().WithoutEventData().Build();

        manager.Setup(x => x.UpdateAccountByEventAsync(It.IsAny<Account>()));
        messageActions.Setup(x => x.CompleteMessageAsync(message, It.IsAny<CancellationToken>()));

        // Act
        await function.RunAccountUpdatedEventAsync(message, messageActions.Object);

        // Assert
        manager.Verify(x => x.UpdateAccountByEventAsync(It.IsAny<Account>()), Times.Never);
        messageActions.Verify(x => x.CompleteMessageAsync(message, It.IsAny<CancellationToken>()), Times.Once);
    }
}
