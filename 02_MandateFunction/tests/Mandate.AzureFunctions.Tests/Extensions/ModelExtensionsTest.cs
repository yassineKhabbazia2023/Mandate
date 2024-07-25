// <copyright file="ModelExtensionsTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.AzureFunctions.Tests;

using global::Pulse.Back.Events.IntegrationEvents.EventsData;
using KPMG.Pulse.Back.Accounting.Mandate.Function;

public class ModelExtensionsTest
{
    [Fact]
    public void ToModel_ShouldConvertContactStateEventDataToCollaborator()
    {
        // Arrange
        var contactStateEventData = new ContactStateEventData
        {
            ContactId = 1,
            Email = "test@example.com",
            FirstName = "John",
            LastName = "Doe",
        };

        // Act
        var result = contactStateEventData.ToModel();

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(contactStateEventData.ContactId);
        result.Email.Should().Be(contactStateEventData.Email);
        result.FirstName.Should().Be(contactStateEventData.FirstName);
        result.LastName.Should().Be(contactStateEventData.LastName);
    }

    [Fact]
    public void ToModel_ShouldConvertRoleCreatedEventDataToCompanyCollaborator()
    {
        // Arrange
        var roleCreatedEventData = new RoleCreatedEventData
        {
            AccountId = 1,
            ContactId = 2,
        };

        // Act
        var result = roleCreatedEventData.ToModel();

        // Assert
        result.Should().NotBeNull();
        result.AccountId.Should().Be(roleCreatedEventData.AccountId);
        result.ContactId.Should().Be(roleCreatedEventData.ContactId);
    }

    [Fact]
    public void ToModel_ShouldConvertRoleDeletedEventDataToCompanyCollaborator()
    {
        // Arrange
        var roleDeletedEventData = new RoleDeletedEventData
        {
            AccountId = 1,
            ContactId = 2,
        };

        // Act
        var result = roleDeletedEventData.ToModel();

        // Assert
        result.Should().NotBeNull();
        result.AccountId.Should().Be(roleDeletedEventData.AccountId);
        result.ContactId.Should().Be(roleDeletedEventData.ContactId);
    }

    [Fact]
    public void ToModel_ShouldConvertAccountStateEventDataToCompany()
    {
        // Arrange
        var accountStateEventData = new AccountStateEventData
        {
            AccountId = 1,
            LegalName = "Test Company",
            AccountNumber = "123456",
            SiretNumber = "123456789 12345",
        };

        // Act
        var result = accountStateEventData.ToModel();

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(accountStateEventData.AccountId);
        result.Name.Should().Be(accountStateEventData.LegalName);
        result.SiretNumber.Should().Be(accountStateEventData.SiretNumber.Trim().Replace(" ", string.Empty));
    }

    [Fact]
    public void ToModel_WithSiretNumberLongerThan14Characters_ShouldThrowArgumentException()
    {
        // Arrange
        var accountStateEventData = new AccountStateEventData
        {
            AccountId = 1,
            LegalName = "Test Company",
            AccountNumber = "123456",
            SiretNumber = "123456789 123456789",
        };

        // Act
        Action act = () => accountStateEventData.ToModel();

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void ToModel_ShouldThrowArgumentNullException_WhenRoleCreatedEventDataIsNull()
    {
        // Arrange
        RoleCreatedEventData roleCreatedEventData = null!;

        // Act
        Action act = () => roleCreatedEventData.ToModel();

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void ToModel_ShouldThrowArgumentNullException_WhenRoleDeletedEventDataIsNull()
    {
        // Arrange
        RoleDeletedEventData roleDeletedEventData = null!;

        // Act
        Action act = () => roleDeletedEventData.ToModel();

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void ToModel_ShouldThrowArgumentNullException_WhenAccountStateEventDataIsNull()
    {
        // Arrange
        AccountStateEventData accountStateEventData = null!;

        // Act
        Action act = () => accountStateEventData.ToModel();

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }
}
