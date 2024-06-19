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

public class AccountEventsFunctionTest
{
    [Fact]
    public async Task RunAccountCreatedEventAsync_InValidMessage_ShouldCompleteMessage()
    {
        // Arrange
        var logger = new Mock<ILogger<AccountEventsFunction>>();
        var manager = new Mock<IEventsFunctionManager>();
        var messageActions = new Mock<ServiceBusMessageActions>();
        var accountEvent = new AccountCreatedEvent(null!);

        var message = ServiceBusModelFactory.ServiceBusReceivedMessage(
           body: BinaryData.FromString(JsonConvert.SerializeObject(accountEvent)),
           messageId: "123",
           contentType: "application/json");

        messageActions.Setup(x => x.CompleteMessageAsync(message, It.IsAny<CancellationToken>()));

        var function = new AccountEventsFunction(logger.Object, manager.Object);

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
        var logger = new Mock<ILogger<AccountEventsFunction>>();
        var manager = new Mock<IEventsFunctionManager>();
        var messageActions = new Mock<ServiceBusMessageActions>();
        var accountEvent = new AccountCreatedEvent(
                   new AccountStateEventData()
                   {
                       AccountId = 1,
                       AccountNumber = "accountNumber",
                       AccountGlobalUniqueId = Guid.NewGuid(),
                       LegalName = "Test",
                   });

        var message = ServiceBusModelFactory.ServiceBusReceivedMessage(
           body: BinaryData.FromString(JsonConvert.SerializeObject(accountEvent)),
           messageId: "123",
           contentType: "application/json");

        messageActions.Setup(x => x.CompleteMessageAsync(message, It.IsAny<CancellationToken>()));

        var function = new AccountEventsFunction(logger.Object, manager.Object);

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
        var logger = new Mock<ILogger<AccountEventsFunction>>();
        var manager = new Mock<IEventsFunctionManager>();
        var messageActions = new Mock<ServiceBusMessageActions>();
        var accountEvent = new AccountRemovedEvent(new AccountRemovedEventData { AccountId = 0 });

        var message = ServiceBusModelFactory.ServiceBusReceivedMessage(
                   body: BinaryData.FromString(JsonConvert.SerializeObject(accountEvent)),
                   messageId: "123",
                   contentType: "application/json");

        manager.Setup(x => x.DeleteAccountByEventAsync(accountEvent.Data.AccountId));
        messageActions.Setup(x => x.CompleteMessageAsync(message, It.IsAny<CancellationToken>()));

        var function = new AccountEventsFunction(logger.Object, manager.Object);

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
        var logger = new Mock<ILogger<AccountEventsFunction>>();
        var manager = new Mock<IEventsFunctionManager>();
        var messageActions = new Mock<ServiceBusMessageActions>();
        var accountEvent = new AccountRemovedEvent(new AccountRemovedEventData { AccountId = 1 });

        var message = ServiceBusModelFactory.ServiceBusReceivedMessage(
                   body: BinaryData.FromString(JsonConvert.SerializeObject(accountEvent)),
                   messageId: "123",
                   contentType: "application/json");

        manager.Setup(x => x.DeleteAccountByEventAsync(accountEvent.Data.AccountId));
        messageActions.Setup(x => x.CompleteMessageAsync(message, It.IsAny<CancellationToken>()));

        var function = new AccountEventsFunction(logger.Object, manager.Object);

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
        var logger = new Mock<ILogger<AccountEventsFunction>>();
        var manager = new Mock<IEventsFunctionManager>();
        var messageActions = new Mock<ServiceBusMessageActions>();
        var accountEvent = new AccountUpdatedEvent(
            new AccountStateEventData()
            {
                AccountId = 1,
                AccountNumber = "accountNumber",
                AccountGlobalUniqueId = Guid.NewGuid(),
                LegalName = "Test",
            });

        var message = ServiceBusModelFactory.ServiceBusReceivedMessage(
           body: BinaryData.FromString(JsonConvert.SerializeObject(accountEvent)),
           messageId: "123",
           contentType: "application/json");

        manager.Setup(x => x.UpdateAccountByEventAsync(It.IsAny<Account>()));
        messageActions.Setup(x => x.CompleteMessageAsync(message, It.IsAny<CancellationToken>()));

        var function = new AccountEventsFunction(logger.Object, manager.Object);

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
        var logger = new Mock<ILogger<AccountEventsFunction>>();
        var manager = new Mock<IEventsFunctionManager>();
        var messageActions = new Mock<ServiceBusMessageActions>();
        var accountEvent = new AccountUpdatedEvent(null!);

        var message = ServiceBusModelFactory.ServiceBusReceivedMessage(
           body: BinaryData.FromString(JsonConvert.SerializeObject(accountEvent)),
           messageId: "123",
           contentType: "application/json");

        manager.Setup(x => x.UpdateAccountByEventAsync(It.IsAny<Account>()));
        messageActions.Setup(x => x.CompleteMessageAsync(message, It.IsAny<CancellationToken>()));

        var function = new AccountEventsFunction(logger.Object, manager.Object);

        // Act
        await function.RunAccountUpdatedEventAsync(message, messageActions.Object);

        // Assert
        manager.Verify(x => x.UpdateAccountByEventAsync(It.IsAny<Account>()), Times.Never);
        messageActions.Verify(x => x.CompleteMessageAsync(message, It.IsAny<CancellationToken>()), Times.Once);
    }
}
