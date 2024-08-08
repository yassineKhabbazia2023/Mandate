// <copyright file="ContactEventsFunctionTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.AzureFunctions.Tests;

using Azure.Messaging.ServiceBus;
using global::Pulse.Back.Events.IntegrationEvents;
using global::Pulse.Back.Events.IntegrationEvents.EventsData;
using KPMG.Pulse.Back.Accounting.Mandate.Function;
using KPMG.Pulse.Back.Accounting.Mandate.Function.Functions;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

internal class ContactCreatedMessageBuilder
{
    private ContactStateEventData contactStateEventData;

    public ContactCreatedMessageBuilder()
    {
        this.contactStateEventData = new ContactStateEventData
        {
            ContactId = 1,
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            Type = "Collaborator",
        };
    }

    public ContactCreatedMessageBuilder WithContactId(int id)
    {
        this.contactStateEventData.ContactId = id;
        return this;
    }

    public ContactCreatedMessageBuilder WithCollaboratorContact()
    {
        this.contactStateEventData.Type = "Collaborator";
        return this;
    }

    public ContactCreatedMessageBuilder WithCustomerContact()
    {
        this.contactStateEventData.Type = "Customer";
        return this;
    }

    public (ServiceBusReceivedMessage Message, ContactCreatedEvent ContactCreatedEvent) Build()
    {
        ContactCreatedEvent contactCreatedEvent = new ContactCreatedEvent(this.contactStateEventData);
        return (ServiceBusModelFactory.ServiceBusReceivedMessage(
            body: BinaryData.FromString(JsonConvert.SerializeObject(contactCreatedEvent)),
            messageId: "123",
            contentType: "application/json"), contactCreatedEvent);
    }
}

internal class ContactRemovedMessageBuilder
{
    private ContactRemovedEventData contactRemovedData;

    public ContactRemovedMessageBuilder()
    {
        this.contactRemovedData = new ContactRemovedEventData { ContactId = 2, };
    }

    public ContactRemovedMessageBuilder WithContactId(int id)
    {
        this.contactRemovedData.ContactId = id;
        return this;
    }

    public (ServiceBusReceivedMessage Message, ContactRemovedEvent ContactRemovedEvent) Build()
    {
        var contactRemovedEvent = new ContactRemovedEvent(this.contactRemovedData);
        return (ServiceBusModelFactory.ServiceBusReceivedMessage(
            body: BinaryData.FromString(JsonConvert.SerializeObject(contactRemovedEvent)),
            messageId: "123",
            contentType: "application/json"), contactRemovedEvent);
    }
}

internal class ContactUpdatedMessageBuilder
{
    private ContactStateEventData contactStateEventData;

    public ContactUpdatedMessageBuilder()
    {
        this.contactStateEventData = new ContactStateEventData
        {
            ContactId = 1,
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            Type = "Collaborator",
        };
    }

    public ContactUpdatedMessageBuilder WithCustomerContact()
    {
        this.contactStateEventData.Type = "Customer";
        return this;
    }

    public ContactUpdatedMessageBuilder WithCollaboratorContact()
    {
        this.contactStateEventData.Type = "Collaborator";
        return this;
    }

    public ContactUpdatedMessageBuilder WithNullContactEventData()
    {
        this.contactStateEventData = null!;
        return this;
    }

    public (ServiceBusReceivedMessage Message, ContactUpdatedEvent ContactUpdatedEvent) Build()
    {
        var contactUpdatedEvent = new ContactUpdatedEvent(this.contactStateEventData);
        return (ServiceBusModelFactory.ServiceBusReceivedMessage(
                           body: BinaryData.FromString(JsonConvert.SerializeObject(contactUpdatedEvent)),
                           messageId: "123",
                           contentType: "application/json"), contactUpdatedEvent);
    }
}

public class ContactEventsFunctionTests
{
    private readonly Mock<ILogger<ContactEventsFunction>> loggerMock;
    private readonly Mock<IEventsFunctionManager> managerMock;
    private readonly ContactEventsFunction function;
    private readonly Mock<ServiceBusMessageActions> messageActionsMock;

    public ContactEventsFunctionTests()
    {
        this.loggerMock = new Mock<ILogger<ContactEventsFunction>>();
        this.managerMock = new Mock<IEventsFunctionManager>(MockBehavior.Strict);
        this.function = new ContactEventsFunction(this.loggerMock.Object, this.managerMock.Object);
        this.messageActionsMock = new Mock<ServiceBusMessageActions>();
    }



    [Fact]
    public async Task RunContactCreatedEvent_ValidMessage_ShouldProcessSuccessfully()
    {
        // Arrange
        var (message, contactCreatedEvent) = new ContactCreatedMessageBuilder().Build();
        this.managerMock.Setup(_ => _.CreateContactByEventAsync(It.Is<Contact>(_ => _.Id == contactCreatedEvent.Data.ContactId)))
                        .Returns(Task.CompletedTask);

        // Act
        await this.function.RunContactCreatedEventAsync(message, this.messageActionsMock.Object);

        // Assert
        this.managerMock.Verify(m => m.CreateContactByEventAsync(It.Is<Contact>(c => c.Id == contactCreatedEvent.Data.ContactId && c.Email == contactCreatedEvent.Data.Email)), Times.Once);
        this.messageActionsMock.Verify(m => m.CompleteMessageAsync(message, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RunContactCreatedEvent_WithCustomerContact_ShouldIgnoreTheEvent()
    {
        // Arrange
        var (message, contactCreatedEvent) = new ContactCreatedMessageBuilder().WithCustomerContact().Build();

        this.managerMock.Setup(_ => _.CreateContactByEventAsync(It.Is<Contact>(_ => _.Id == contactCreatedEvent.Data.ContactId)))
                        .Returns(Task.CompletedTask);

        // Act
        await this.function.RunContactCreatedEventAsync(message, this.messageActionsMock.Object);

        // Assert
        this.managerMock.Verify(m => m.CreateContactByEventAsync(It.Is<Contact>(c => c.Id == contactCreatedEvent.Data.ContactId && c.Email == contactCreatedEvent.Data.Email)), Times.Never);
        this.messageActionsMock.Verify(m => m.CompleteMessageAsync(message, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RunContactUpdatedEvent_WithCustomerContact_ShouldIgnoreTheEvent()
    {
        // Arrange
        var (message, contactCreatedEvent) = new ContactUpdatedMessageBuilder().WithCustomerContact().Build();

        this.managerMock.Setup(_ => _.CreateContactByEventAsync(It.Is<Contact>(_ => _.Id == contactCreatedEvent.Data.ContactId)))
                        .Returns(Task.CompletedTask);

        // Act
        await this.function.RunContactCreatedEventAsync(message, this.messageActionsMock.Object);

        // Assert
        this.managerMock.Verify(m => m.CreateContactByEventAsync(It.Is<Contact>(c => c.Id == contactCreatedEvent.Data.ContactId && c.Email == contactCreatedEvent.Data.Email)), Times.Never);
        this.messageActionsMock.Verify(m => m.CompleteMessageAsync(message, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RunContactCreatedEvent_WithCollaboratorContact_ShouldProcessTheEvent()
    {
        // Arrange
        var (message, contactCreatedEvent) = new ContactCreatedMessageBuilder().WithCollaboratorContact().Build();

        this.managerMock.Setup(_ => _.CreateContactByEventAsync(It.Is<Contact>(_ => _.Id == contactCreatedEvent.Data.ContactId)))
                        .Returns(Task.CompletedTask);

        // Act
        await this.function.RunContactCreatedEventAsync(message, this.messageActionsMock.Object);

        // Assert
        this.managerMock.Verify(m => m.CreateContactByEventAsync(It.Is<Contact>(c => c.Id == contactCreatedEvent.Data.ContactId && c.Email == contactCreatedEvent.Data.Email)), Times.Once);
        this.messageActionsMock.Verify(m => m.CompleteMessageAsync(message, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RunContactUpdatedEvent_WithCollaboratorContact_ShouldProcessTheEvent()
    {
        // Arrange
        var (message, contactCreatedEvent) = new ContactUpdatedMessageBuilder().WithCollaboratorContact().Build();

        this.managerMock.Setup(_ => _.CreateContactByEventAsync(It.Is<Contact>(_ => _.Id == contactCreatedEvent.Data.ContactId)))
                        .Returns(Task.CompletedTask);

        // Act
        await this.function.RunContactCreatedEventAsync(message, this.messageActionsMock.Object);

        // Assert
        this.managerMock.Verify(m => m.CreateContactByEventAsync(It.Is<Contact>(c => c.Id == contactCreatedEvent.Data.ContactId && c.Email == contactCreatedEvent.Data.Email)), Times.Once);
        this.messageActionsMock.Verify(m => m.CompleteMessageAsync(message, It.IsAny<CancellationToken>()), Times.Once);
    }


    [Fact]
    public async Task RunContactCreatedEvent_InValidMessage_ShouldProcessSuccessfully()
    {
        // Arrange
        var (message, contactCreatedEvent) = new ContactCreatedMessageBuilder().WithContactId(0).Build();

        this.managerMock.Setup(_ => _.CreateContactByEventAsync(It.Is<Contact>(_ => _.Id == contactCreatedEvent.Data.ContactId)))
                        .Returns(Task.CompletedTask);

        // Act
        await this.function.RunContactCreatedEventAsync(message, this.messageActionsMock.Object);

        // Assert
        this.managerMock.Verify(m => m.CreateContactByEventAsync(It.Is<Contact>(c => c.Id == contactCreatedEvent.Data.ContactId && c.Email == contactCreatedEvent.Data.Email)), Times.Never);
        this.messageActionsMock.Verify(m => m.CompleteMessageAsync(message, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RunContactDeletedEvent_ValidMessage_ShouldProcessSuccessfully()
    {
        // Arrange
        var (message, contactRemovedEvent) = new ContactRemovedMessageBuilder().Build();
        var contactData = contactRemovedEvent.Data;

        this.managerMock.Setup(_ => _.DeleteContactByEventAsync(contactData.ContactId))
                        .Returns(Task.CompletedTask);

        // Act
        await this.function.RunContactDeletedEventAsync(message, this.messageActionsMock.Object);

        // Assert
        this.managerMock.Verify(m => m.DeleteContactByEventAsync(contactData.ContactId), Times.Once);
        this.messageActionsMock.Verify(m => m.CompleteMessageAsync(message, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RunContactDeletedEvent_InValidMessage_ShouldProcessSuccessfully()
    {
        // Arrange
        var (message, contactRemovedEvent) = new ContactRemovedMessageBuilder().WithContactId(0).Build();
        var contactData = contactRemovedEvent.Data;

        this.managerMock.Setup(_ => _.DeleteContactByEventAsync(contactData.ContactId))
                        .Returns(Task.CompletedTask);

        // Act
        await this.function.RunContactDeletedEventAsync(message, this.messageActionsMock.Object);

        // Assert
        this.managerMock.Verify(m => m.DeleteContactByEventAsync(contactRemovedEvent.Data.ContactId), Times.Never);
        this.messageActionsMock.Verify(m => m.CompleteMessageAsync(message, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RunContactUpdatedEvent_ValidMessage_ShouldProcessSuccessfully()
    {
        // Arrange
        var (message, contactUpdatedEvent) = new ContactUpdatedMessageBuilder().Build();
        var contactData = contactUpdatedEvent.Data;

        this.managerMock.Setup(_ => _.UpdateContactByEventAsync(It.Is<Contact>(_ => _.Id == contactData.ContactId)))
                        .Returns(Task.CompletedTask);

        // Act
        await this.function.RunContactUpdatedEventAsync(message, this.messageActionsMock.Object);

        // Assert
        this.managerMock.Verify(m => m.UpdateContactByEventAsync(It.Is<Contact>(c => c.Id == contactData.ContactId && c.Email == contactData.Email)), Times.Once);
        this.messageActionsMock.Verify(m => m.CompleteMessageAsync(message, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RunContactUpdatedEvent_InValidMessage_ShouldProcessSuccessfully()
    {
        // Arrange
        var (message, contactUpdatedEvent) = new ContactUpdatedMessageBuilder().WithNullContactEventData().Build();

        this.managerMock.Setup(_ => _.UpdateContactByEventAsync(It.IsAny<Contact>()))
                        .Returns(Task.CompletedTask);

        // Act
        await this.function.RunContactUpdatedEventAsync(message, this.messageActionsMock.Object);

        // Assert
        this.managerMock.Verify(m => m.UpdateContactByEventAsync(It.IsAny<Contact>()), Times.Never);
        this.messageActionsMock.Verify(m => m.CompleteMessageAsync(message, It.IsAny<CancellationToken>()), Times.Once);
    }
}
