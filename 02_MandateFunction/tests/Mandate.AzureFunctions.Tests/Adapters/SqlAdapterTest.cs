// <copyright file="SqlAdapterTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.AzureFunctions.Tests;

using KPMG.Pulse.Back.Accounting.Mandate.Function;
using KPMG.Pulse.Back.Accounting.Mandate.Sql;
using Microsoft.Identity.Client;
using Notifications.Commons.WebApi;

public class SqlAdapterTest
{
    [Fact]
    public async Task CreateContactByEventAsync_WhenCalled_ShouldCreateContact()
    {
        // Arrange
        var contact = new Contact(1, "firstName", "lastName", "email", true);

        var mockMandateRepository = new Mock<IMandateRepository>();
        var sqlAdapter = new SqlAdapter(mockMandateRepository.Object);

        // Act
        await sqlAdapter.CreateContactByEventAsync(contact);

        // Assert
        mockMandateRepository.Verify(x => x.CreateContactByEventAsync(It.IsAny<CollaboratorDb>()), Times.Once);
    }

    [Fact]
    public async Task UpdateContactByEventAsync_WhenContactNotNull_ShouldUpdateContact()
    {
        // Arrange
        var contact = new Contact(1, "firstName", "lastName", "email", true);
        var mockMandateRepository = new Mock<IMandateRepository>();
        var sqlAdapter = new SqlAdapter(mockMandateRepository.Object);

        // Act
        await sqlAdapter.UpdateContactByEventAsync(contact);

        // Assert
        mockMandateRepository.Verify(x => x.UpdateContactByEventAsync(It.Is<CollaboratorDb>(c => c.Id == 1 && c.FirstName == "firstName")), Times.Once);
    }

    [Fact]
    public async Task UpdateContactByEventAsync_WhenContactNull_ShouldNotUpdateContact()
    {
        // Arrange
        var mockMandateRepository = new Mock<IMandateRepository>();
        var sqlAdapter = new SqlAdapter(mockMandateRepository.Object);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => sqlAdapter.UpdateContactByEventAsync(null!));
    }

    [Fact]
    public async Task GetContactByIdAsync_WithValidContactId_ReturnsContact()
    {
        // Arrange
        int contactId = 1;
        var contact = new Contact(1, "firstName", "lastName", "email", true);
        CollaboratorDb dbContact = contact.ToSql();

        var mockMandateRepository = new Mock<IMandateRepository>();
        mockMandateRepository.Setup(r => r.GetActiveContactByIdAsync(contactId)).ReturnsAsync(dbContact);

        var sqlAdapter = new SqlAdapter(mockMandateRepository.Object);

        // Act
        Contact? result = await sqlAdapter.GetActiveContactByIdAsync(contactId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(contactId);
        result.FirstName.Should().Be("firstName");
    }

    [Fact]
    public async Task GetContactEventByIdAsync_WithValidContactId_ReturnsContact()
    {
        // Arrange
        int contactId = 1;
        var contact = new Contact(1, "firstName", "lastName", "email", false);
        CollaboratorDb dbContact = contact.ToSql();

        var mockMandateRepository = new Mock<IMandateRepository>();
        mockMandateRepository.Setup(r => r.GetContactByIdAsync(contactId)).ReturnsAsync(dbContact);

        var sqlAdapter = new SqlAdapter(mockMandateRepository.Object);

        // Act
        Contact? result = await sqlAdapter.GetContactByIdAsync(contactId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(contactId);
        result.FirstName.Should().Be("firstName");
    }

    [Fact]
    public async Task GetAccountByIdAsync_ReturnsAccount_WhenAccountExists()
    {
        // Arrange
        int accountId = 1;
        var mockMandateRepository = new Mock<IMandateRepository>();
        var expectedAccount = new Account(accountId, "name", "siretNumber", "accountNumber", true);
        mockMandateRepository.Setup(x => x.GetActiveAccountByIdAsync(accountId)).ReturnsAsync(expectedAccount.ToSql());
        var sqlAdapter = new SqlAdapter(mockMandateRepository.Object);

        // Act
        var result = await sqlAdapter.GetActiveAccountByIdAsync(accountId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(accountId);
        result.Name.Should().Be("name");
    }

    [Fact]
    public async Task GetAccountEventByIdAsync_ReturnsAccount_WhenAccountExists()
    {
        // Arrange
        int accountId = 1;
        var mockMandateRepository = new Mock<IMandateRepository>();
        var expectedAccount = new Account(accountId, "name", "siretNumber", "accountNumber", false);
        mockMandateRepository.Setup(x => x.GetAccountByIdAsync(accountId)).ReturnsAsync(expectedAccount.ToSql());
        var sqlAdapter = new SqlAdapter(mockMandateRepository.Object);

        // Act
        var result = await sqlAdapter.GetAccountByIdAsync(accountId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(accountId);
        result.Name.Should().Be("name");
    }

    [Fact]
    public async Task GetAccountByIdAsync_ReturnsNull_WhenAccountDoesNotExist()
    {
        // Arrange
        int accountId = 1;
        var mockMandateRepository = new Mock<IMandateRepository>();
        mockMandateRepository.Setup(x => x.GetActiveAccountByIdAsync(accountId)).ReturnsAsync((CompanyDb?)null);
        var sqlAdapter = new SqlAdapter(mockMandateRepository.Object);

        // Act
        var result = await sqlAdapter.GetActiveAccountByIdAsync(accountId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateRoleByEventAsync_ValidAccountContact_CreatesRole()
    {
        // Arrange
        var accountContact = new AccountContact(1, 2);
        var mockMandateRepository = new Mock<IMandateRepository>();
        var sqlAdapter = new SqlAdapter(mockMandateRepository.Object);

        // Act
        await sqlAdapter.CreateRoleByEventAsync(accountContact);

        // Assert
        mockMandateRepository.Verify(x => x.CreateRoleAsync(It.IsAny<CompanyCollaboratorDb>()), Times.Once);
    }

    [Fact]
    public async Task DeleteRoleByEventAsync_ShouldDeleteRole()
    {
        // Arrange
        var accountContact = new AccountContact(1, 2);
        var mockMandateRepository = new Mock<IMandateRepository>();
        var sqlAdapter = new SqlAdapter(mockMandateRepository.Object);

        // Act
        await sqlAdapter.DeleteRoleByEventAsync(accountContact);

        // Assert
        mockMandateRepository.Verify(x => x.DeleteRoleAsync(It.IsAny<CompanyCollaboratorDb>()), Times.Once);
    }

    [Fact]
    public async Task GetAccountContactByAccountIdAndContactIdAsync_ValidIds_ReturnsAccountContact()
    {
        // Arrange
        var accountId = 1;
        var contactId = 1;
        var expectedAccountContact = new AccountContact(accountId, contactId);
        var expectedDbAccountContact = expectedAccountContact.ToSql();

        var mockMandateRepository = new Mock<IMandateRepository>();
        mockMandateRepository.Setup(x => x.GetAccountContactByAccountIdAndContactIdAsync(accountId, contactId))
            .ReturnsAsync(expectedDbAccountContact);

        var sqlAdapter = new SqlAdapter(mockMandateRepository.Object);

        // Act
        var result = await sqlAdapter.GetAccountContactByAccountIdAndContactIdAsync(accountId, contactId);

        // Assert
        result.Should().NotBeNull();
        result!.AccountId.Should().Be(accountId);
        result.ContactId.Should().Be(contactId);
    }

    [Fact]
    public async Task GetAccountContactByAccountIdAndContactIdAsync_InvalidIds_ReturnsNull()
    {
        // Arrange
        var accountId = 1;
        var contactId = 1;

        var mockMandateRepository = new Mock<IMandateRepository>();
        mockMandateRepository.Setup(x => x.GetAccountContactByAccountIdAndContactIdAsync(accountId, contactId))
            .ReturnsAsync((CompanyCollaboratorDb?)null);

        var sqlAdapter = new SqlAdapter(mockMandateRepository.Object);

        // Act
        var result = await sqlAdapter.GetAccountContactByAccountIdAndContactIdAsync(accountId, contactId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateAccountByEventAsync_WhenValidAccount_ShouldCreateAccount()
    {
        // Arrange
        var mockMandateRepository = new Mock<IMandateRepository>();
        var sqlAdapter = new SqlAdapter(mockMandateRepository.Object);
        var account = new Account(1, "name", "siretNumber", "accountNumber", true);

        // Act
        await sqlAdapter.CreateAccountByEventAsync(account);

        // Assert
        mockMandateRepository.Verify(x => x.CreateCompanyAsync(It.Is<CompanyDb>(a => a.Id == account.Id && a.Name == account.Name && a.IsActive)), Times.Once);
    }

    [Fact]
    public async Task CreateAccountByEventAsync_WhenNullAccount_ShouldThrowArgumentNullException()
    {
        // Arrange
        var mockMandateRepository = new Mock<IMandateRepository>();
        var sqlAdapter = new SqlAdapter(mockMandateRepository.Object);

        // Act
        async Task Act() => await sqlAdapter.CreateAccountByEventAsync(null!);

        // Assert
        await Assert.ThrowsAsync<ArgumentNullException>(Act);
        mockMandateRepository.Verify(x => x.CreateCompanyAsync(It.IsAny<CompanyDb>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAccountByEventAsync_DeletesAccount()
    {
        // Arrange
        var mockMandateRepository = new Mock<IMandateRepository>();
        var sqlAdapter = new SqlAdapter(mockMandateRepository.Object);
        var account = new Account(1, "name", "siretNumber", "accountNumber", true);

        // Act
        await sqlAdapter.DeleteAccountByEventAsync(account);

        // Assert
        mockMandateRepository.Verify(repo => repo.UpdateCompanyAsync(It.Is<CompanyDb>(a => a.Id == account.Id && !a.IsActive)), Times.Once);
    }

    [Fact]
    public async Task DeleteAccountByEventAsync_NullAccount_ThrowsException()
    {
        // Arrange
        var mockMandateRepository = new Mock<IMandateRepository>();
        var sqlAdapter = new SqlAdapter(mockMandateRepository.Object);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => sqlAdapter.DeleteAccountByEventAsync(null!));
    }

    [Fact]
    public async Task UpdateAccountByEventAsync_WhenCalled_ShouldUpdateAccount()
    {
        // Arrange
        var mockMandateRepository = new Mock<IMandateRepository>();
        var sqlAdapter = new SqlAdapter(mockMandateRepository.Object);
        var account = new Account(1, "name", "siretNumber", "accountNumber", true);

        // Act
        await sqlAdapter.UpdateAccountByEventAsync(account);

        // Assert
        mockMandateRepository.Verify(x => x.UpdateCompanyAsync(It.IsAny<CompanyDb>()), Times.Once);
    }
}
