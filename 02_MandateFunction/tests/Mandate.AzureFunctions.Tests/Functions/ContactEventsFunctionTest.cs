// <copyright file="ContactEventsFunctionTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.AzureFunctions.Tests;

using Azure.Messaging.ServiceBus;
using global::Pulse.Back.Events.IntegrationEvents;
using global::Pulse.Back.Events.IntegrationEvents.EventsData;
using KPMG.Pulse.Back.Accounting.Mandate.Client;
using KPMG.Pulse.Back.Accounting.Mandate.Function;
using KPMG.Pulse.Back.Accounting.Mandate.Function.Functions;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

public class ContactEventsFunctionTests
{
    private readonly Mock<ILogger<ContactEventsFunction>> loggerMock;
    private readonly Mock<IEventsFunctionManager> managerMock;
    private readonly ContactEventsFunction function;
    private readonly Mock<ServiceBusMessageActions> messageActionsMock;

    private ServiceBusReceivedMessage message;

    public ContactEventsFunctionTests()
    {
        this.loggerMock = new Mock<ILogger<ContactEventsFunction>>();
        this.managerMock = new Mock<IEventsFunctionManager>(MockBehavior.Strict);
        this.function = new ContactEventsFunction(this.loggerMock.Object, this.managerMock.Object);
        this.messageActionsMock = new Mock<ServiceBusMessageActions>();

        // Setup a basic message
        this.message = ServiceBusModelFactory.ServiceBusReceivedMessage(
            body: BinaryData.FromString(JsonConvert.SerializeObject(new ContactStateEventData
            {
                ContactId = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
            })),
            messageId: "123",
            contentType: "application/json");
    }

    [Fact]
    public async Task RunContactCreatedEvent_ValidMessage_ShouldProcessSuccessfully()
    {
        // Arrange
        var contactData = new ContactStateEventData
        {
            ContactId = 1,
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
        };
        var contactCreatedEvent = new ContactCreatedEvent(contactData);
        this.message = ServiceBusModelFactory.ServiceBusReceivedMessage(
            body: BinaryData.FromString(JsonConvert.SerializeObject(contactCreatedEvent)),
            messageId: "123",
            contentType: "application/json");

        this.managerMock.Setup(_ => _.CreateContactByEventAsync(It.Is<Contact>(_ => _.Id == contactData.ContactId)))
                        .Returns(Task.CompletedTask);

        // Act
        await this.function.RunContactCreatedEventAsync(this.message, this.messageActionsMock.Object);

        // Assert
        this.managerMock.Verify(m => m.CreateContactByEventAsync(It.Is<Contact>(c => c.Id == contactData.ContactId && c.Email == contactData.Email)), Times.Once);
        this.messageActionsMock.Verify(m => m.CompleteMessageAsync(this.message, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RunContactCreatedEvent_InValidMessage_ShouldProcessSuccessfully()
    {
        // Arrange
        var contactData = new ContactStateEventData
        {
            ContactId = 1,
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
        };
        var contactCreatedEvent = new ContactCreatedEvent(null!);
        this.message = ServiceBusModelFactory.ServiceBusReceivedMessage(
            body: BinaryData.FromString(JsonConvert.SerializeObject(contactCreatedEvent)),
            messageId: "123",
            contentType: "application/json");

        this.managerMock.Setup(_ => _.CreateContactByEventAsync(It.Is<Contact>(_ => _.Id == contactData.ContactId)))
                        .Returns(Task.CompletedTask);

        // Act
        await this.function.RunContactCreatedEventAsync(this.message, this.messageActionsMock.Object);

        // Assert
        this.managerMock.Verify(m => m.CreateContactByEventAsync(It.Is<Contact>(c => c.Id == contactData.ContactId && c.Email == contactData.Email)), Times.Never);
        this.messageActionsMock.Verify(m => m.CompleteMessageAsync(this.message, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RunContactDeletedEvent_ValidMessage_ShouldProcessSuccessfully()
    {
        // Arrange
        var contactData = new ContactRemovedEventData
        {
            ContactId = 1,
        };
        var contactRemovedEvent = new ContactRemovedEvent(contactData);
        this.message = ServiceBusModelFactory.ServiceBusReceivedMessage(
                    body: BinaryData.FromString(JsonConvert.SerializeObject(contactRemovedEvent)),
                    messageId: "123",
                    contentType: "application/json");

        this.managerMock.Setup(_ => _.DeleteContactByEventAsync(contactData.ContactId))
                        .Returns(Task.CompletedTask);

        // Act
        await this.function.RunContactDeletedEventAsync(this.message, this.messageActionsMock.Object);

        // Assert
        this.managerMock.Verify(m => m.DeleteContactByEventAsync(contactRemovedEvent.Data.ContactId), Times.Once);
        this.messageActionsMock.Verify(m => m.CompleteMessageAsync(this.message, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RunContactDeletedEvent_InValidMessage_ShouldProcessSuccessfully()
    {
        // Arrange
        var contactData = new ContactRemovedEventData
        {
            ContactId = 0,
        };
        var contactRemovedEvent = new ContactRemovedEvent(contactData);
        this.message = ServiceBusModelFactory.ServiceBusReceivedMessage(
                    body: BinaryData.FromString(JsonConvert.SerializeObject(contactRemovedEvent)),
                    messageId: "123",
                    contentType: "application/json");

        this.managerMock.Setup(_ => _.DeleteContactByEventAsync(contactData.ContactId))
                        .Returns(Task.CompletedTask);

        // Act
        await this.function.RunContactDeletedEventAsync(this.message, this.messageActionsMock.Object);

        // Assert
        this.managerMock.Verify(m => m.DeleteContactByEventAsync(contactRemovedEvent.Data.ContactId), Times.Never);
        this.messageActionsMock.Verify(m => m.CompleteMessageAsync(this.message, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RunContactUpdatedEvent_ValidMessage_ShouldProcessSuccessfully()
    {
        // Arrange
        var contactData = new ContactStateEventData
        {
            ContactId = 1,
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
        };
        var contactUpdatedEvent = new ContactUpdatedEvent(contactData);
        this.message = ServiceBusModelFactory.ServiceBusReceivedMessage(
                           body: BinaryData.FromString(JsonConvert.SerializeObject(contactUpdatedEvent)),
                           messageId: "123",
                           contentType: "application/json");

        this.managerMock.Setup(_ => _.UpdateContactByEventAsync(It.Is<Contact>(_ => _.Id == contactData.ContactId)))
                        .Returns(Task.CompletedTask);

        // Act
        await this.function.RunContactUpdatedEventAsync(this.message, this.messageActionsMock.Object);

        // Assert
        this.managerMock.Verify(m => m.UpdateContactByEventAsync(It.Is<Contact>(c => c.Id == 1 && c.Email == contactData.Email)), Times.Once);
        this.messageActionsMock.Verify(m => m.CompleteMessageAsync(this.message, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RunContactUpdatedEvent_InValidMessage_ShouldProcessSuccessfully()
    {
        // Arrange
        var contactUpdatedEvent = new ContactUpdatedEvent(null!);
        this.message = ServiceBusModelFactory.ServiceBusReceivedMessage(
                           body: BinaryData.FromString(JsonConvert.SerializeObject(contactUpdatedEvent)),
                           messageId: "123",
                           contentType: "application/json");

        this.managerMock.Setup(_ => _.UpdateContactByEventAsync(It.IsAny<Contact>()))
                        .Returns(Task.CompletedTask);

        // Act
        await this.function.RunContactUpdatedEventAsync(this.message, this.messageActionsMock.Object);

        // Assert
        this.managerMock.Verify(m => m.UpdateContactByEventAsync(It.IsAny<Contact>()), Times.Never);
        this.messageActionsMock.Verify(m => m.CompleteMessageAsync(this.message, It.IsAny<CancellationToken>()), Times.Once);
    }
}
