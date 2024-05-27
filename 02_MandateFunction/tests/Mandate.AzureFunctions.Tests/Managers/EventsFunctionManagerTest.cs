// <copyright file="EventsFunctionManagerTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.AzureFunctions.Tests;

using KPMG.Pulse.Back.Accounting.Mandate.Function;
using Microsoft.Extensions.Logging;

public class EventsFunctionManagerTest
{
    [Fact]
    public async Task DeleteContactByEventAsync_ContactExists_DeletesContact()
    {
        // Arrange
        var contactId = 1;
        var loggerMock = new Mock<ILogger<EventsFunctionManager>>();
        var mockSqlAdapter = new Mock<ISqlAdapter>();
        mockSqlAdapter.Setup(x => x.GetContactByIdAsync(contactId)).ReturnsAsync(new Contact(1, "firstName", "lastName", "email", true));
        var eventsFunctionManager = new EventsFunctionManager(loggerMock.Object, mockSqlAdapter.Object);

        // Act
        await eventsFunctionManager.DeleteContactByEventAsync(contactId);

        // Assert
        mockSqlAdapter.Verify(x => x.DeleteContactByEventAsync(It.IsAny<Contact>()), Times.Once);
        loggerMock.Verify(
        x => x.Log(
            LogLevel.Information,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((o, t) => string.Equals($"Contact with ID {contactId} successfully removed from database.", o.ToString(), StringComparison.InvariantCultureIgnoreCase)),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
        Times.Once);
    }

    [Fact]
    public async Task DeleteContactByEventAsync_ContactDoesNotExist_ReturnsError()
    {
        // Arrange
        var contactId = 1;
        var loggerMock = new Mock<ILogger<EventsFunctionManager>>();
        var mockSqlAdapter = new Mock<ISqlAdapter>();
        mockSqlAdapter.Setup(x => x.GetContactByIdAsync(contactId)).ReturnsAsync((Contact?)null);
        var eventsFunctionManager = new EventsFunctionManager(loggerMock.Object, mockSqlAdapter.Object);

        // Act
        await eventsFunctionManager.DeleteContactByEventAsync(contactId);

        // Assert
        mockSqlAdapter.Verify(x => x.DeleteContactByEventAsync(It.IsAny<Contact>()), Times.Never);
        loggerMock.Verify(
        x => x.Log(
            LogLevel.Error,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((o, t) => string.Equals($"Contact with ID {contactId} does not exists in database.", o.ToString(), StringComparison.InvariantCultureIgnoreCase)),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
        Times.Once);
    }

    [Fact]
    public async Task DeleteRoleByEventAsync_WhenRoleExists_ShouldDeleteRole()
    {
        // Arrange
        var accountContact = new AccountContact(1, 1);
        var mockSqlAdapter = new Mock<ISqlAdapter>();
        mockSqlAdapter.Setup(x => x.GetAccountContactByAccountIdAndContactIdAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(new AccountContact(1, 1));

        var loggerMock = Mock.Of<ILogger<EventsFunctionManager>>();
        var eventsFunctionManager = new EventsFunctionManager(loggerMock, mockSqlAdapter.Object);

        // Act
        await eventsFunctionManager.DeleteRoleByEventAsync(accountContact);

        // Assert
        mockSqlAdapter.Verify(x => x.DeleteRoleByEventAsync(accountContact), Times.Once);
    }

    [Fact]
    public async Task DeleteRoleByEventAsync_WhenRoleDoesNotExist_ShouldNotDeleteRole()
    {
        // Arrange
        var accountContact = new AccountContact(1, 1);
        var mockSqlAdapter = new Mock<ISqlAdapter>();
        mockSqlAdapter.Setup(x => x.GetAccountContactByAccountIdAndContactIdAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync((AccountContact?)null);

        var loggerMock = Mock.Of<ILogger<EventsFunctionManager>>();
        var eventsFunctionManager = new EventsFunctionManager(loggerMock, mockSqlAdapter.Object);

        // Act
        await eventsFunctionManager.DeleteRoleByEventAsync(accountContact);

        // Assert
        mockSqlAdapter.Verify(x => x.DeleteRoleByEventAsync(accountContact), Times.Never);
    }

    [Fact]
    public async Task UpdateAccountByEventAsync_ValidAccount_AccountUpdated()
    {
        // Arrange
        var accountId = 1;
        var account = new Account(1, "name", "siretnumber", "accountNumber", true);

        var mockSqlAdapter = new Mock<ISqlAdapter>();
        mockSqlAdapter.Setup(x => x.GetAccountByIdAsync(accountId)).ReturnsAsync(account);
        var loggerMock = new Mock<ILogger<EventsFunctionManager>>();

        var eventsFunctionManager = new EventsFunctionManager(loggerMock.Object, mockSqlAdapter.Object);

        // Act
        await eventsFunctionManager.UpdateAccountByEventAsync(account);

        // Assert
        mockSqlAdapter.Verify(x => x.UpdateAccountByEventAsync(account), Times.Once);
        loggerMock.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => string.Equals($"Account with ID {account.Id} successfully updated in database.", o.ToString(), StringComparison.InvariantCultureIgnoreCase)),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
    }

    [Fact]
    public async Task UpdateAccountByEventAsync_AccountNotFound_CreateNewAccount()
    {
        // Arrange
        var accountId = 1;
        var account = new Account(1, "name", "siretnumber", "accountNumber", true);

        var mockSqlAdapter = new Mock<ISqlAdapter>();
        mockSqlAdapter.Setup(x => x.GetAccountByIdAsync(accountId)).ReturnsAsync((Account?)null);
        var loggerMock = new Mock<ILogger<EventsFunctionManager>>();

        var eventsFunctionManager = new EventsFunctionManager(loggerMock.Object, mockSqlAdapter.Object);

        // Act
        await eventsFunctionManager.UpdateAccountByEventAsync(account);

        // Assert
        mockSqlAdapter.Verify(x => x.CreateAccountByEventAsync(account), Times.Once);
        loggerMock.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => string.Equals($"Account with ID {account.Id} successfully created in database.", o.ToString(), StringComparison.InvariantCultureIgnoreCase)),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
    }

    [Fact]
    public async Task UpdateAccountByEventAsync_InactiveAccount_LogWarning()
    {
        // Arrange
        var accountId = 1;
        var account = new Account(1, "name", "siretnumber", "accountNumber", false);

        var mockSqlAdapter = new Mock<ISqlAdapter>();
        mockSqlAdapter.Setup(x => x.GetAccountByIdAsync(accountId)).ReturnsAsync(account);
        var loggerMock = new Mock<ILogger<EventsFunctionManager>>();

        var eventsFunctionManager = new EventsFunctionManager(loggerMock.Object, mockSqlAdapter.Object);

        // Act
        await eventsFunctionManager.UpdateAccountByEventAsync(account);

        // Assert
        loggerMock.Verify(
        x => x.Log(
            LogLevel.Warning,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((o, t) => string.Equals("Since the value of IsActive in this entry is false, this update will deactivate the company in this database.", o.ToString(), StringComparison.InvariantCultureIgnoreCase)),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
        Times.Once);
    }

    [Fact]
    public async Task DeleteAccountByEventAsync_AccountExists_AccountRemovedFromDatabase()
    {
        // Arrange
        var accountId = 1;
        var mockSqlAdapter = new Mock<ISqlAdapter>();
        mockSqlAdapter.Setup(x => x.GetAccountByIdAsync(accountId)).ReturnsAsync(new Account(1, "name", "siretnumber", "accountNumber", true));

        var loggerMock = new Mock<ILogger<EventsFunctionManager>>();
        var eventsFunctionManager = new EventsFunctionManager(loggerMock.Object, mockSqlAdapter.Object);

        // Act
        await eventsFunctionManager.DeleteAccountByEventAsync(accountId);

        // Assert
        mockSqlAdapter.Verify(x => x.DeleteAccountByEventAsync(It.IsAny<Account>()), Times.Once);
        loggerMock.Verify(
        x => x.Log(
            LogLevel.Information,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((o, t) => string.Equals($"Account with ID {accountId} successfully removed from database.", o.ToString(), StringComparison.InvariantCultureIgnoreCase)),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
        Times.Once);
    }

    [Fact]
    public async Task DeleteAccountByEventAsync_AccountDoesNotExist_LogError()
    {
        // Arrange
        var accountId = 1;
        var mockSqlAdapter = new Mock<ISqlAdapter>();
        mockSqlAdapter.Setup(x => x.GetAccountByIdAsync(accountId)).ReturnsAsync((Account?)null);

        var loggerMock = new Mock<ILogger<EventsFunctionManager>>();
        var eventsFunctionManager = new EventsFunctionManager(loggerMock.Object, mockSqlAdapter.Object);

        // Act
        await eventsFunctionManager.DeleteAccountByEventAsync(accountId);

        // Assert
        mockSqlAdapter.Verify(x => x.DeleteAccountByEventAsync(It.IsAny<Account>()), Times.Never);
        loggerMock.Verify(
        x => x.Log(
            LogLevel.Error,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((o, t) => string.Equals($"Company with ID {accountId} does not exists in database.", o.ToString(), StringComparison.InvariantCultureIgnoreCase)),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
        Times.Once);
    }

    [Fact]
    public async Task CreateRoleByEventAsync_WhenAccountContactDoesNotExist_ShouldCreateRole()
    {
        // Arrange
        var mockSqlAdapter = new Mock<ISqlAdapter>();
        mockSqlAdapter.Setup(x => x.GetContactByIdAsync(It.IsAny<int>())).ReturnsAsync(new Contact(2, "firstName", "lastName", "email", true));
        mockSqlAdapter.Setup(x => x.GetAccountByIdAsync(It.IsAny<int>())).ReturnsAsync(new Account(1, "name", "siretnumber", "accountNumber", true));
        mockSqlAdapter.Setup(x => x.GetAccountContactByAccountIdAndContactIdAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync((AccountContact?)null);

        var eventsFunctionManager = new EventsFunctionManager(Mock.Of<ILogger<EventsFunctionManager>>(), mockSqlAdapter.Object);
        var accountContact = new AccountContact(1, 2);

        // Act
        await eventsFunctionManager.CreateRoleByEventAsync(accountContact);

        // Assert
        mockSqlAdapter.Verify(x => x.CreateRoleByEventAsync(accountContact), Times.Once);
    }

    [Fact]
    public async Task CreateRoleByEventAsync_WhenAccountContactExists_ShouldNotCreateRole()
    {
        // Arrange
        var mockSqlAdapter = new Mock<ISqlAdapter>();
        mockSqlAdapter.Setup(x => x.GetAccountContactByAccountIdAndContactIdAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(new AccountContact(1, 2));

        var eventsFunctionManager = new EventsFunctionManager(Mock.Of<ILogger<EventsFunctionManager>>(), mockSqlAdapter.Object);
        var accountContact = new AccountContact(1, 2);

        // Act
        await eventsFunctionManager.CreateRoleByEventAsync(accountContact);

        // Assert
        mockSqlAdapter.Verify(x => x.CreateRoleByEventAsync(accountContact), Times.Never);
    }

    [Fact]
    public async Task CreateRoleByEventAsync_WhenAccountNotExists_ShouldNotCreateRole()
    {
        // Arrange
        var mockSqlAdapter = new Mock<ISqlAdapter>();
        mockSqlAdapter.Setup(x => x.GetAccountByIdAsync(It.IsAny<int>())).ReturnsAsync((Account?)null);
        mockSqlAdapter.Setup(x => x.GetAccountContactByAccountIdAndContactIdAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync((AccountContact?)null);

        var eventsFunctionManager = new EventsFunctionManager(Mock.Of<ILogger<EventsFunctionManager>>(), mockSqlAdapter.Object);
        var accountContact = new AccountContact(1, 2);

        // Act
        await eventsFunctionManager.CreateRoleByEventAsync(accountContact);

        // Assert
        mockSqlAdapter.Verify(x => x.CreateRoleByEventAsync(accountContact), Times.Never);
    }

    [Fact]
    public async Task CreateRoleByEventAsync_WhenContactNotExists_ShouldNotCreateRole()
    {
        // Arrange
        var mockSqlAdapter = new Mock<ISqlAdapter>();
        mockSqlAdapter.Setup(x => x.GetContactByIdAsync(It.IsAny<int>())).ReturnsAsync((Contact?)null);
        mockSqlAdapter.Setup(x => x.GetAccountContactByAccountIdAndContactIdAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync((AccountContact?)null);

        var eventsFunctionManager = new EventsFunctionManager(Mock.Of<ILogger<EventsFunctionManager>>(), mockSqlAdapter.Object);
        var accountContact = new AccountContact(1, 2);

        // Act
        await eventsFunctionManager.CreateRoleByEventAsync(accountContact);

        // Assert
        mockSqlAdapter.Verify(x => x.CreateRoleByEventAsync(accountContact), Times.Never);
    }

    [Fact]
    public async Task CreateContactByEventAsync_ContactDoesNotExist_ShouldCreateNewContact()
    {
        // Arrange
        var contact = new Contact(1, "firstName", "lastName", "email", true);
        var mockSqlAdapter = new Mock<ISqlAdapter>();
        mockSqlAdapter.Setup(x => x.GetContactByIdAsync(contact.Id)).ReturnsAsync((Contact?)null);
        var loggerMock = new Mock<ILogger<EventsFunctionManager>>();
        var eventsFunctionManager = new EventsFunctionManager(loggerMock.Object, mockSqlAdapter.Object);

        // Act
        await eventsFunctionManager.CreateContactByEventAsync(contact);

        // Assert
        mockSqlAdapter.Verify(x => x.CreateContactByEventAsync(contact), Times.Once);
        loggerMock.Verify(
        x => x.Log(
            LogLevel.Information,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((o, t) => string.Equals($"Contact with ID {contact.Id} successfully created in database.", o.ToString(), StringComparison.InvariantCultureIgnoreCase)),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
        Times.Once);
    }

    [Fact]
    public async Task CreateContactByEventAsync_ContactExists_ShouldUpdateContact()
    {
        // Arrange
        var contact = new Contact(1, "firstName", "lastName", "email", true);
        var existingContact = new Contact(1, "firstName", "lastName", "email", true);
        var mockSqlAdapter = new Mock<ISqlAdapter>();
        mockSqlAdapter.Setup(x => x.GetContactByIdAsync(contact.Id)).ReturnsAsync(existingContact);
        var loggerMock = new Mock<ILogger<EventsFunctionManager>>();
        var eventsFunctionManager = new EventsFunctionManager(loggerMock.Object, mockSqlAdapter.Object);

        // Act
        await eventsFunctionManager.CreateContactByEventAsync(contact);

        // Assert
        mockSqlAdapter.Verify(x => x.UpdateContactByEventAsync(contact), Times.Once);
        loggerMock.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => string.Equals($"Contact with ID {contact.Id} successfully updated in database.", o.ToString(), StringComparison.InvariantCultureIgnoreCase)),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
    }

    [Fact]
    public async Task UpdateContactByEventAsync_ContactExistsInDatabase_ContactUpdated()
    {
        // Arrange
        var contactId = 1;
        var contact = new Contact(1, "firstName", "lastName", "email", true);
        var dbCollab = new Contact(1, "firstName", "lastName", "email", true);

        var sqlAdapterMock = new Mock<ISqlAdapter>();
        sqlAdapterMock.Setup(x => x.GetContactByIdAsync(contactId)).ReturnsAsync(dbCollab);

        var loggerMock = new Mock<ILogger<EventsFunctionManager>>();

        var eventsFunctionManager = new EventsFunctionManager(loggerMock.Object, sqlAdapterMock.Object);

        // Act
        await eventsFunctionManager.UpdateContactByEventAsync(contact);

        // Assert
        sqlAdapterMock.Verify(x => x.UpdateContactByEventAsync(contact), Times.Once);
        loggerMock.Verify(
                        x => x.Log(
                            LogLevel.Information,
                            It.IsAny<EventId>(),
                            It.Is<It.IsAnyType>((o, t) => string.Equals($"Contact with ID {contact.Id} successfully updated in database.", o.ToString(), StringComparison.InvariantCultureIgnoreCase)),
                            It.IsAny<Exception>(),
                            It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                        Times.Once);
    }

    [Fact]
    public async Task UpdateContactByEventAsync_ContactDoesNotExistInDatabase_NewContactCreated()
    {
        // Arrange
        var contactId = 1;
        var contact = new Contact(1, "firstName", "lastName", "email", true);

        var sqlAdapterMock = new Mock<ISqlAdapter>();
        sqlAdapterMock.Setup(x => x.GetContactByIdAsync(contactId)).ReturnsAsync((Contact?)null);

        var loggerMock = new Mock<ILogger<EventsFunctionManager>>();

        var eventsFunctionManager = new EventsFunctionManager(loggerMock.Object, sqlAdapterMock.Object);

        // Act
        await eventsFunctionManager.UpdateContactByEventAsync(contact);

        // Assert
        sqlAdapterMock.Verify(x => x.CreateContactByEventAsync(contact), Times.Once);
        loggerMock.Verify(
                        x => x.Log(
                            LogLevel.Information,
                            It.IsAny<EventId>(),
                            It.Is<It.IsAnyType>((o, t) => string.Equals($"Contact with ID {contact.Id} successfully updated in database.", o.ToString(), StringComparison.InvariantCultureIgnoreCase)),
                            It.IsAny<Exception>(),
                            It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                        Times.Never);
    }

    [Fact]
    public async Task UpdateContactByEventAsync_InactiveContact_LogsWarning()
    {
        // Arrange
        var contactId = 1;
        var contact = new Contact(1, "firstName", "lastName", "email", false);
        var dbCollab = new Contact(1, "firstName", "lastName", "email", true);

        var sqlAdapterMock = new Mock<ISqlAdapter>();
        sqlAdapterMock.Setup(x => x.GetContactByIdAsync(contactId)).ReturnsAsync(dbCollab);

        var loggerMock = new Mock<ILogger<EventsFunctionManager>>();

        var eventsFunctionManager = new EventsFunctionManager(loggerMock.Object, sqlAdapterMock.Object);

        // Act
        await eventsFunctionManager.UpdateContactByEventAsync(contact);

        // Assert
        loggerMock.Verify(
        x => x.Log(
            LogLevel.Warning,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((o, t) => string.Equals("Since the value of IsActive in this entry is false, this update will deactivate the contact in this database.", o.ToString(), StringComparison.InvariantCultureIgnoreCase)),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
        Times.Once);
    }

    [Fact]
    public async Task CreateAccountByEventAsync_AccountDoesNotExist_AccountCreated()
    {
        // Arrange
        var account = new Account(1, "name", "siretnumber", "accountNumber", true);
        var mockSqlAdapter = new Mock<ISqlAdapter>();
        mockSqlAdapter.Setup(x => x.GetAccountByIdAsync(It.IsAny<int>())).ReturnsAsync((Account?)null);
        var loggerMock = new Mock<ILogger<EventsFunctionManager>>();
        var eventsFunctionManager = new EventsFunctionManager(loggerMock.Object, mockSqlAdapter.Object);

        // Act
        await eventsFunctionManager.CreateAccountByEventAsync(account);

        // Assert
        mockSqlAdapter.Verify(x => x.GetAccountByIdAsync(account.Id), Times.Once);
        mockSqlAdapter.Verify(x => x.CreateAccountByEventAsync(account), Times.Once);
        loggerMock.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => string.Equals($"Account with ID {account.Id} successfully created in database.", o.ToString(), StringComparison.InvariantCultureIgnoreCase)),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
    }

    [Fact]
    public async Task CreateAccountByEventAsync_AccountAlreadyExists_AccountNotCreated()
    {
        // Arrange
        var account = new Account(1, "name", "siretnumber", "accountNumber", true);
        var mockSqlAdapter = new Mock<ISqlAdapter>();
        mockSqlAdapter.Setup(x => x.GetAccountByIdAsync(It.IsAny<int>())).ReturnsAsync(account);
        var loggerMock = new Mock<ILogger<EventsFunctionManager>>();
        var eventsFunctionManager = new EventsFunctionManager(loggerMock.Object, mockSqlAdapter.Object);

        // Act
        await eventsFunctionManager.CreateAccountByEventAsync(account);

        // Assert
        mockSqlAdapter.Verify(x => x.GetAccountByIdAsync(account.Id), Times.Exactly(2));
        mockSqlAdapter.Verify(x => x.CreateAccountByEventAsync(account), Times.Never);
        loggerMock.Verify(
        x => x.Log(
            LogLevel.Error,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((o, t) => string.Equals($"Account with ID {account.Id} already exists in database. Start updating...", o.ToString(), StringComparison.InvariantCultureIgnoreCase)),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
        Times.Once);
        loggerMock.Verify(
        x => x.Log(
            LogLevel.Information,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((o, t) => string.Equals($"Account with ID {account.Id} successfully created in database.", o.ToString(), StringComparison.InvariantCultureIgnoreCase)),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
        Times.Never);
    }

    [Fact]
    public async Task UpdateAccountByEventAsync_ShouldUpdateAccount_WhenAccountExists()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<EventsFunctionManager>>();
        var sqlAdapterMock = new Mock<ISqlAdapter>();
        var account = new Account(1, "name", "siretnumber", "accountNumber", true);
        var dbCompany = new Account(1, "name", "siretnumber", "accountNumber", true);

        sqlAdapterMock.Setup(x => x.GetAccountByIdAsync(account.Id)).ReturnsAsync(dbCompany);
        var eventsFunctionManager = new EventsFunctionManager(loggerMock.Object, sqlAdapterMock.Object);

        // Act
        await eventsFunctionManager.UpdateAccountByEventAsync(account);

        // Assert
        sqlAdapterMock.Verify(x => x.UpdateAccountByEventAsync(account), Times.Once);
        loggerMock.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => string.Equals($"Account with ID {account.Id} successfully updated in database.", o.ToString(), StringComparison.InvariantCultureIgnoreCase)),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
    }

    [Fact]
    public async Task UpdateAccountByEventAsync_ShouldCreateAccount_WhenAccountDoesNotExist()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<EventsFunctionManager>>();
        var sqlAdapterMock = new Mock<ISqlAdapter>();
        var account = new Account(1, "name", "siretnumber", "accountNumber", true);

        sqlAdapterMock.Setup(x => x.GetAccountByIdAsync(account.Id)).ReturnsAsync((Account?)null);
        var eventsFunctionManager = new EventsFunctionManager(loggerMock.Object, sqlAdapterMock.Object);

        // Act
        await eventsFunctionManager.UpdateAccountByEventAsync(account);

        // Assert
        sqlAdapterMock.Verify(x => x.CreateAccountByEventAsync(account), Times.Once);
        loggerMock.Verify(
                        x => x.Log(
                            LogLevel.Information,
                            It.IsAny<EventId>(),
                            It.Is<It.IsAnyType>((o, t) => string.Equals($"Account with ID {account.Id} successfully updated in database.", o.ToString(), StringComparison.InvariantCultureIgnoreCase)),
                            It.IsAny<Exception>(),
                            It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                        Times.Never);
    }

    [Fact]
    public async Task UpdateAccountByEventAsync_ShouldLogWarning_WhenAccountIsActiveIsFalse()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<EventsFunctionManager>>();
        var sqlAdapterMock = new Mock<ISqlAdapter>();
        var account = new Account(1, "name", "siretnumber", "accountNumber", true);
        var dbCompany = new Account(1, "name", "siretnumber", "accountNumber", false);

        sqlAdapterMock.Setup(x => x.GetAccountByIdAsync(account.Id)).ReturnsAsync(dbCompany);
        var eventsFunctionManager = new EventsFunctionManager(loggerMock.Object, sqlAdapterMock.Object);

        // Act
        await eventsFunctionManager.UpdateAccountByEventAsync(account);

        // Assert
        loggerMock.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => string.Equals("Since the value of IsActive in this entry is false, this update will deactivate the company in this database.", o.ToString(), StringComparison.InvariantCultureIgnoreCase)),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
    }
}
