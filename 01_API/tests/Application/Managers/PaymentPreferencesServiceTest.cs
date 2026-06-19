// <copyright file="PaymentPreferencesServiceTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Application.Tests.Managers;

using KPMG.Pulse.Back.Accounting.Mandate;
using KPMG.Pulse.Back.Accounting.Mandate.Application;
using KPMG.Pulse.Back.Accounting.Mandate.Application.Managers;
using KPMG.Pulse.Back.Accounting.Mandate.Application.Strategies;
using Microsoft.Extensions.Logging.Abstractions;

/// <summary>
/// Unit tests for <see cref="PaymentPreferencesService"/>.
/// </summary>
public sealed class PaymentPreferencesServiceTest
{
    /// <summary>
    /// Verifies that an existing account without a preference returns a null payment type.
    /// </summary>
    [Fact]
    public async Task GetAsync_WhenAccountExistsWithoutPreference_ReturnsNullPaymentType()
    {
        var databaseService = new Mock<IDatabaseService>();
        databaseService.Setup(candidate => candidate.AccountExistsAsync(42)).ReturnsAsync(true);
        databaseService.Setup(candidate => candidate.GetPaymentPreferenceByAccountIdAsync(42)).ReturnsAsync((PaymentPreference?)null);
        var service = CreateService(databaseService.Object);

        var result = await service.GetAsync(42);

        result.AccountFound.Should().BeTrue();
        result.PaymentType.Should().BeNull();
    }

    /// <summary>
    /// Verifies that an existing OTHER preference is returned.
    /// </summary>
    [Fact]
    public async Task GetAsync_WhenOtherPreferenceExists_ReturnsOther()
    {
        var databaseService = new Mock<IDatabaseService>();
        databaseService.Setup(candidate => candidate.AccountExistsAsync(42)).ReturnsAsync(true);
        databaseService.Setup(candidate => candidate.GetPaymentPreferenceByAccountIdAsync(42))
            .ReturnsAsync(new PaymentPreference(0, 42, (int)PaymentPreferenceType.Other, DateTime.UtcNow, "user@test.fr"));
        var service = CreateService(databaseService.Object);

        var result = await service.GetAsync(42);

        result.AccountFound.Should().BeTrue();
        result.PaymentType.Should().Be(PaymentPreferenceType.Other);
    }

    /// <summary>
    /// Verifies that an existing preference reset to null is returned as unselected.
    /// </summary>
    [Fact]
    public async Task GetAsync_WhenPreferencePaymentTypeIsNull_ReturnsNullPaymentType()
    {
        var databaseService = new Mock<IDatabaseService>();
        databaseService.Setup(candidate => candidate.AccountExistsAsync(42)).ReturnsAsync(true);
        databaseService.Setup(candidate => candidate.GetPaymentPreferenceByAccountIdAsync(42))
            .ReturnsAsync(new PaymentPreference(0, 42, null, DateTime.UtcNow, "user@test.fr"));
        var service = CreateService(databaseService.Object);

        var result = await service.GetAsync(42);

        result.AccountFound.Should().BeTrue();
        result.PaymentType.Should().BeNull();
    }

    /// <summary>
    /// Verifies that setting OTHER inserts a new preference when none exists.
    /// </summary>
    [Fact]
    public async Task SetOtherAsync_WhenPreferenceDoesNotExist_InsertsOtherPreference()
    {
        PaymentPreference? savedPreference = null;
        var databaseService = new Mock<IDatabaseService>();
        databaseService.Setup(candidate => candidate.AccountExistsAsync(42)).ReturnsAsync(true);
        databaseService.Setup(candidate => candidate.GetPaymentPreferenceByAccountIdAsync(42)).ReturnsAsync((PaymentPreference?)null);
        databaseService.Setup(candidate => candidate.SavePaymentPreferenceAsync(It.IsAny<PaymentPreference>()))
            .Callback<PaymentPreference>(preference => savedPreference = preference)
            .Returns(Task.CompletedTask);
        var service = CreateService(databaseService.Object);

        var result = await service.SetOtherAsync(42, "user@test.fr");

        result.Should().BeTrue();
        savedPreference.Should().NotBeNull();
        savedPreference!.AccountId.Should().Be(42);
        savedPreference.PaymentType.Should().Be((int)PaymentPreferenceType.Other);
        savedPreference.CreatedBy.Should().Be("user@test.fr");
    }

    /// <summary>
    /// Verifies that setting OTHER updates an existing preference.
    /// </summary>
    [Fact]
    public async Task SetOtherAsync_WhenPreferenceExists_UpdatesPaymentTypeToOther()
    {
        var existing = new PaymentPreference(7, 42, (int)PaymentPreferenceType.MandateSepa, DateTime.UtcNow, "user@test.fr");
        var databaseService = new Mock<IDatabaseService>();
        databaseService.Setup(candidate => candidate.AccountExistsAsync(42)).ReturnsAsync(true);
        databaseService.Setup(candidate => candidate.GetPaymentPreferenceByAccountIdAsync(42)).ReturnsAsync(existing);
        databaseService.Setup(candidate => candidate.SavePaymentPreferenceAsync(existing)).Returns(Task.CompletedTask);
        var service = CreateService(databaseService.Object);

        var result = await service.SetOtherAsync(42, "user@test.fr");

        result.Should().BeTrue();
        existing.PaymentType.Should().Be((int)PaymentPreferenceType.Other);
        databaseService.Verify(candidate => candidate.SavePaymentPreferenceAsync(existing), Times.Once);
    }

    /// <summary>
    /// Verifies that setting OTHER returns false when the account does not exist.
    /// </summary>
    [Fact]
    public async Task SetOtherAsync_WhenAccountDoesNotExist_ReturnsFalse()
    {
        var databaseService = new Mock<IDatabaseService>();
        databaseService.Setup(candidate => candidate.AccountExistsAsync(42)).ReturnsAsync(false);
        var service = CreateService(databaseService.Object);

        var result = await service.SetOtherAsync(42, "user@test.fr");

        result.Should().BeFalse();
        databaseService.Verify(candidate => candidate.SavePaymentPreferenceAsync(It.IsAny<PaymentPreference>()), Times.Never);
    }

    /// <summary>
    /// Verifies that resetting an existing preference clears the payment type.
    /// </summary>
    [Fact]
    public async Task ResetAsync_WhenPreferenceExists_SetsPaymentTypeToNull()
    {
        var existing = new PaymentPreference(7, 42, (int)PaymentPreferenceType.Other, DateTime.UtcNow, "user@test.fr");
        var databaseService = new Mock<IDatabaseService>();
        databaseService.Setup(candidate => candidate.AccountExistsAsync(42)).ReturnsAsync(true);
        databaseService.Setup(candidate => candidate.GetPaymentPreferenceByAccountIdAsync(42)).ReturnsAsync(existing);
        databaseService.Setup(candidate => candidate.SavePaymentPreferenceAsync(existing)).Returns(Task.CompletedTask);
        var service = CreateService(databaseService.Object);

        var result = await service.ResetAsync(42);

        result.Should().BeTrue();
        existing.PaymentType.Should().BeNull();
        databaseService.Verify(candidate => candidate.SavePaymentPreferenceAsync(existing), Times.Once);
    }

    /// <summary>
    /// Verifies that reset returns false when the account has no payment preference row.
    /// </summary>
    [Fact]
    public async Task ResetAsync_WhenPreferenceDoesNotExist_ReturnsFalse()
    {
        var databaseService = new Mock<IDatabaseService>();
        databaseService.Setup(candidate => candidate.AccountExistsAsync(42)).ReturnsAsync(true);
        databaseService.Setup(candidate => candidate.GetPaymentPreferenceByAccountIdAsync(42)).ReturnsAsync((PaymentPreference?)null);
        var service = CreateService(databaseService.Object);

        var result = await service.ResetAsync(42);

        result.Should().BeFalse();
        databaseService.Verify(candidate => candidate.SavePaymentPreferenceAsync(It.IsAny<PaymentPreference>()), Times.Never);
    }

    /// <summary>
    /// Verifies that reset returns false when the account does not exist.
    /// </summary>
    [Fact]
    public async Task ResetAsync_WhenAccountDoesNotExist_ReturnsFalse()
    {
        var databaseService = new Mock<IDatabaseService>();
        databaseService.Setup(candidate => candidate.AccountExistsAsync(42)).ReturnsAsync(false);
        var service = CreateService(databaseService.Object);

        var result = await service.ResetAsync(42);

        result.Should().BeFalse();
        databaseService.Verify(candidate => candidate.SavePaymentPreferenceAsync(It.IsAny<PaymentPreference>()), Times.Never);
    }

    /// <summary>
    /// Creates a payment preferences service.
    /// </summary>
    /// <param name="databaseService">The database service mock.</param>
    /// <returns>The service.</returns>
    private static PaymentPreferencesService CreateService(IDatabaseService databaseService)
    {
        return new PaymentPreferencesService(
            databaseService,
            [new OtherPaymentPreferenceStrategy()],
            NullLogger<PaymentPreferencesService>.Instance);
    }
}
