// <copyright file="RoleEventsFunctionTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.AzureFunctions.Tests;

using Azure.Messaging.ServiceBus;
using global::Pulse.Back.Events.IntegrationEvents.EventsData;
using global::Pulse.Back.Events.IntegrationEvents;
using KPMG.Pulse.Back.Accounting.Mandate.Function.Functions;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using KPMG.Pulse.Back.Accounting.Mandate.Client;
using KPMG.Pulse.Back.Accounting.Mandate.Function;

public class RoleEventsFunctionTest
{
    private readonly Mock<IEventsFunctionManager> mockManager = new Mock<IEventsFunctionManager>(MockBehavior.Strict);
    private readonly RoleEventsFunction function;

    public RoleEventsFunctionTest()
    {
        this.function = new RoleEventsFunction(new Mock<ILogger<RoleEventsFunction>>().Object, this.mockManager.Object);
    }

    [Fact]
    public async Task RoleCreatedEvent_ShouldProcessMessageCorrectly()
    {
        // Arrange
        var roleCreatedEvent = new RoleCreatedEvent(new RoleCreatedEventData
        {
            AccountId = 1,
            ContactId = 2,
        });
        var messageBody = JsonConvert.SerializeObject(roleCreatedEvent);
        var message = ServiceBusModelFactory.ServiceBusReceivedMessage(
                   body: BinaryData.FromString(messageBody),
                   messageId: "123",
                   contentType: "application/json");

        var messageActions = new Mock<ServiceBusMessageActions>();

        this.mockManager.Setup(_ => _.CreateRoleByEventAsync(It.Is<AccountContact>(_ => _.AccountId == roleCreatedEvent.Data.AccountId && _.ContactId == roleCreatedEvent.Data.ContactId)))
                        .Returns(Task.CompletedTask);

        // Act
        await this.function.RoleCreatedEvent(message, messageActions.Object);

        // Assert
        this.mockManager.Verify(m => m.CreateRoleByEventAsync(It.Is<AccountContact>(_ => _.AccountId == 1 && _.ContactId == 2)), Times.Once);
        messageActions.Verify(m => m.CompleteMessageAsync(message, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RoleDeletedEvent_ShouldProcessMessageCorrectly()
    {
        // Arrange
        var roleDeletedEvent = new RoleDeletedEvent(new RoleDeletedEventData
        {
            AccountId = 1,
            ContactId = 2,
        });
        var messageBody = JsonConvert.SerializeObject(roleDeletedEvent);
        var message = ServiceBusModelFactory.ServiceBusReceivedMessage(
                    body: BinaryData.FromString(messageBody),
                    messageId: "123",
                    contentType: "application/json");

        var messageActions = new Mock<ServiceBusMessageActions>();
        this.mockManager.Setup(_ => _.DeleteRoleByEventAsync(It.Is<AccountContact>(_ => _.AccountId == roleDeletedEvent.Data.AccountId && _.ContactId == roleDeletedEvent.Data.ContactId)))
                        .Returns(Task.CompletedTask);

        // Act
        await this.function.RunRoleDeletedEvent(message, messageActions.Object);

        // Assert
        this.mockManager.Verify(m => m.DeleteRoleByEventAsync(It.Is<AccountContact>(_ => _.AccountId == 1 && _.ContactId == 2)), Times.Once);
        messageActions.Verify(m => m.CompleteMessageAsync(message, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RoleDeletedEvent_WithInvalidData_ShouldNotProcessMessage()
    {
        // Arrange
        var roleDeletedEvent = new RoleDeletedEvent(null!);
        var messageBody = JsonConvert.SerializeObject(roleDeletedEvent);
        var message = ServiceBusModelFactory.ServiceBusReceivedMessage(
                           body: BinaryData.FromString(messageBody),
                           messageId: "123",
                           contentType: "application/json");

        var messageActions = new Mock<ServiceBusMessageActions>();

        // Act
        await this.function.RunRoleDeletedEvent(message, messageActions.Object);

        // Assert
        this.mockManager.Verify(m => m.DeleteRoleByEventAsync(It.Is<AccountContact>(_ => _.AccountId == 1 && _.ContactId == 2)), Times.Never);
        messageActions.Verify(m => m.CompleteMessageAsync(message, It.IsAny<CancellationToken>()), Times.Never);
    }
}
