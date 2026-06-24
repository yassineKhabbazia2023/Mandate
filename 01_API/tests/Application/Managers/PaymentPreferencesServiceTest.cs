// <copyright file="PaymentPreferencesServiceTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Application.Tests.Managers;

using KPMG.Pulse.Back.Accounting.Mandate;
using KPMG.Pulse.Back.Accounting.Mandate.Application;
using KPMG.Pulse.Back.Accounting.Mandate.Application.Managers;
using KPMG.Pulse.Back.Accounting.Mandate.Application.Interfaces;
using KPMG.Pulse.Back.Accounting.Mandate.Application.Models;
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
        var paymentPreferenceStore = new Mock<IPaymentPreferenceStore>();
        paymentPreferenceStore.Setup(candidate => candidate.AccountExistsAsync(42)).ReturnsAsync(true);
        paymentPreferenceStore.Setup(candidate => candidate.GetByAccountIdAsync(42)).ReturnsAsync((PaymentPreference?)null);
        var service = CreateService(paymentPreferenceStore.Object);

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
        var paymentPreferenceStore = new Mock<IPaymentPreferenceStore>();
        paymentPreferenceStore.Setup(candidate => candidate.AccountExistsAsync(42)).ReturnsAsync(true);
        paymentPreferenceStore.Setup(candidate => candidate.GetByAccountIdAsync(42))
            .ReturnsAsync(new PaymentPreference(0, 42, (int)PaymentPreferenceType.Other, DateTime.UtcNow, "user@test.fr"));
        var service = CreateService(paymentPreferenceStore.Object);

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
        var paymentPreferenceStore = new Mock<IPaymentPreferenceStore>();
        paymentPreferenceStore.Setup(candidate => candidate.AccountExistsAsync(42)).ReturnsAsync(true);
        paymentPreferenceStore.Setup(candidate => candidate.GetByAccountIdAsync(42))
            .ReturnsAsync(new PaymentPreference(0, 42, null, DateTime.UtcNow, "user@test.fr"));
        var service = CreateService(paymentPreferenceStore.Object);

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
        var paymentPreferenceStore = new Mock<IPaymentPreferenceStore>();
        paymentPreferenceStore.Setup(candidate => candidate.AccountExistsAsync(42)).ReturnsAsync(true);
        paymentPreferenceStore.Setup(candidate => candidate.GetByAccountIdAsync(42)).ReturnsAsync((PaymentPreference?)null);
        paymentPreferenceStore.Setup(candidate => candidate.SaveAsync(It.IsAny<PaymentPreference>()))
            .Callback<PaymentPreference>(preference => savedPreference = preference)
            .Returns(Task.CompletedTask);
        var service = CreateService(paymentPreferenceStore.Object);

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
        var paymentPreferenceStore = new Mock<IPaymentPreferenceStore>();
        paymentPreferenceStore.Setup(candidate => candidate.AccountExistsAsync(42)).ReturnsAsync(true);
        paymentPreferenceStore.Setup(candidate => candidate.GetByAccountIdAsync(42)).ReturnsAsync(existing);
        paymentPreferenceStore.Setup(candidate => candidate.SaveAsync(existing)).Returns(Task.CompletedTask);
        var service = CreateService(paymentPreferenceStore.Object);

        var result = await service.SetOtherAsync(42, "user@test.fr");

        result.Should().BeTrue();
        existing.PaymentType.Should().Be((int)PaymentPreferenceType.Other);
        paymentPreferenceStore.Verify(candidate => candidate.SaveAsync(existing), Times.Once);
    }

    /// <summary>
    /// Verifies that setting OTHER returns false when the account does not exist.
    /// </summary>
    [Fact]
    public async Task SetOtherAsync_WhenAccountDoesNotExist_ReturnsFalse()
    {
        var paymentPreferenceStore = new Mock<IPaymentPreferenceStore>();
        paymentPreferenceStore.Setup(candidate => candidate.AccountExistsAsync(42)).ReturnsAsync(false);
        var service = CreateService(paymentPreferenceStore.Object);

        var result = await service.SetOtherAsync(42, "user@test.fr");

        result.Should().BeFalse();
        paymentPreferenceStore.Verify(candidate => candidate.SaveAsync(It.IsAny<PaymentPreference>()), Times.Never);
    }

    /// <summary>
    /// Verifies that setting SEPA generates a mandate, sends it to GetAccept, and persists the mandate and payment preference.
    /// </summary>
    [Fact]
    public async Task SetSepaAsync_WhenAccountExists_PersistsSepaMandateAndPaymentPreference()
    {
        SepaMandate? savedMandate = null;
        PaymentPreference? savedPreference = null;
        var command = CreateSepaCommand();
        var paymentPreferenceStore = new Mock<IPaymentPreferenceStore>();
        var sepaMandateStore = new Mock<ISepaMandateStore>();
        var pdfGenerator = new Mock<ISepaMandatePdfGenerator>();
        var getAcceptClient = new Mock<IGetAcceptClient>();
        paymentPreferenceStore.Setup(candidate => candidate.AccountExistsAsync(42)).ReturnsAsync(true);
        paymentPreferenceStore.Setup(candidate => candidate.GetAccountNumberAsync(42)).ReturnsAsync("1001102412");
        paymentPreferenceStore.Setup(candidate => candidate.GetByAccountIdAsync(42)).ReturnsAsync((PaymentPreference?)null);
        sepaMandateStore
            .Setup(candidate => candidate.SaveWithPaymentPreferenceAsync(
                It.IsAny<SepaMandate>(),
                It.IsAny<PaymentPreference>()))
            .Callback<SepaMandate, PaymentPreference>((mandate, preference) =>
            {
                savedMandate = mandate;
                savedPreference = preference;
            })
            .Returns(Task.CompletedTask);
        pdfGenerator
            .Setup(candidate => candidate.GenerateAsync(It.Is<SepaMandatePdfData>(data =>
                data.AccountHolder == command.AccountHolder
                && data.AccountNumber == "1001102412"
                && data.Address == command.Address
                && data.AddressLine2 == command.AddressLine2
                && data.City == command.City
                && data.Country == command.Country
                && data.PostalCode == command.PostalCode
                && data.Iban == command.Iban
                && data.Bic == command.Bic)))
            .ReturnsAsync([1, 2, 3]);
        getAcceptClient
            .Setup(candidate => candidate.SendMandateForSignatureAsync(It.Is<GetAcceptMandateSignatureRequest>(request =>
                request.FileContent.SequenceEqual(new byte[] { 1, 2, 3 })
                && request.Recipient.Email == command.Recipient.Email
                && request.Recipient.FirstName == command.Recipient.FirstName
                && request.Recipient.LastName == command.Recipient.LastName)))
            .ReturnsAsync(new GetAcceptMandateSignatureResponse("doc-123", "https://signature.test"));
        var service = CreateService(paymentPreferenceStore.Object, sepaMandateStore.Object, pdfGenerator.Object, getAcceptClient.Object);

        var result = await service.SetSepaAsync(42, command);

        result.AccountFound.Should().BeTrue();
        result.SignatureUrl.Should().Be("https://signature.test");
        savedMandate.Should().NotBeNull();
        savedMandate!.AccountId.Should().Be(42);
        savedMandate.DocumentId.Should().Be(command.DocumentId);
        savedMandate.AccountHolder.Should().Be(command.AccountHolder);
        savedMandate.Iban.Should().Be(command.Iban);
        savedMandate.Bic.Should().Be(command.Bic);
        savedMandate.Address.Should().Be(command.Address);
        savedMandate.SignatureRequestId.Should().Be("doc-123");
        savedMandate.SignatureUrl.Should().Be("https://signature.test");
        savedMandate.SignatureStatus.Should().Be(SepaMandateSignatureStatus.Processing);
        savedMandate.IsSentToAkuiteo.Should().BeFalse();
        savedPreference.Should().NotBeNull();
        savedPreference!.PaymentType.Should().Be((int)PaymentPreferenceType.MandateSepa);
    }

    /// <summary>
    /// Verifies that setting SEPA returns false when the account does not exist.
    /// </summary>
    [Fact]
    public async Task SetSepaAsync_WhenAccountDoesNotExist_ReturnsAccountNotFound()
    {
        var paymentPreferenceStore = new Mock<IPaymentPreferenceStore>();
        var sepaMandateStore = new Mock<ISepaMandateStore>();
        var pdfGenerator = new Mock<ISepaMandatePdfGenerator>();
        var getAcceptClient = new Mock<IGetAcceptClient>();
        paymentPreferenceStore.Setup(candidate => candidate.AccountExistsAsync(42)).ReturnsAsync(false);
        var service = CreateService(paymentPreferenceStore.Object, sepaMandateStore.Object, pdfGenerator.Object, getAcceptClient.Object);

        var result = await service.SetSepaAsync(42, CreateSepaCommand());

        result.AccountFound.Should().BeFalse();
        result.SignatureUrl.Should().BeNull();
        pdfGenerator.Verify(candidate => candidate.GenerateAsync(It.IsAny<SepaMandatePdfData>()), Times.Never);
        getAcceptClient.Verify(candidate => candidate.SendMandateForSignatureAsync(It.IsAny<GetAcceptMandateSignatureRequest>()), Times.Never);
        sepaMandateStore.Verify(
            candidate => candidate.SaveWithPaymentPreferenceAsync(
                It.IsAny<SepaMandate>(),
                It.IsAny<PaymentPreference>()),
            Times.Never);
    }

    /// <summary>
    /// Verifies that setting SEPA updates an existing OTHER payment preference.
    /// </summary>
    [Fact]
    public async Task SetSepaAsync_WhenPreferenceExists_UpdatesPreferenceToMandateSepa()
    {
        PaymentPreference? savedPreference = null;
        var existing = new PaymentPreference(7, 42, (int)PaymentPreferenceType.Other, DateTime.UtcNow, "old@test.fr");
        var paymentPreferenceStore = new Mock<IPaymentPreferenceStore>();
        var sepaMandateStore = new Mock<ISepaMandateStore>();
        var pdfGenerator = new Mock<ISepaMandatePdfGenerator>();
        var getAcceptClient = new Mock<IGetAcceptClient>();
        paymentPreferenceStore.Setup(candidate => candidate.AccountExistsAsync(42)).ReturnsAsync(true);
        paymentPreferenceStore.Setup(candidate => candidate.GetAccountNumberAsync(42)).ReturnsAsync("1001102412");
        paymentPreferenceStore.Setup(candidate => candidate.GetByAccountIdAsync(42)).ReturnsAsync(existing);
        sepaMandateStore
            .Setup(candidate => candidate.SaveWithPaymentPreferenceAsync(
                It.IsAny<SepaMandate>(),
                existing))
            .Callback<SepaMandate, PaymentPreference>((_, preference) => savedPreference = preference)
            .Returns(Task.CompletedTask);
        pdfGenerator
            .Setup(candidate => candidate.GenerateAsync(It.IsAny<SepaMandatePdfData>()))
            .ReturnsAsync([1, 2, 3]);
        getAcceptClient
            .Setup(candidate => candidate.SendMandateForSignatureAsync(It.IsAny<GetAcceptMandateSignatureRequest>()))
            .ReturnsAsync(new GetAcceptMandateSignatureResponse("doc-123", "https://signature.test"));
        var service = CreateService(paymentPreferenceStore.Object, sepaMandateStore.Object, pdfGenerator.Object, getAcceptClient.Object);

        var result = await service.SetSepaAsync(42, CreateSepaCommand());

        result.AccountFound.Should().BeTrue();
        savedPreference.Should().BeSameAs(existing);
        existing.PaymentType.Should().Be((int)PaymentPreferenceType.MandateSepa);
    }

    /// <summary>
    /// Verifies that setting SEPA does not persist anything when PDF generation fails.
    /// </summary>
    [Fact]
    public async Task SetSepaAsync_WhenPdfGenerationFails_DoesNotCallGetAcceptOrPersist()
    {
        var paymentPreferenceStore = new Mock<IPaymentPreferenceStore>();
        var sepaMandateStore = new Mock<ISepaMandateStore>();
        var pdfGenerator = new Mock<ISepaMandatePdfGenerator>();
        var getAcceptClient = new Mock<IGetAcceptClient>();
        paymentPreferenceStore.Setup(candidate => candidate.AccountExistsAsync(42)).ReturnsAsync(true);
        paymentPreferenceStore.Setup(candidate => candidate.GetAccountNumberAsync(42)).ReturnsAsync("1001102412");
        paymentPreferenceStore.Setup(candidate => candidate.GetByAccountIdAsync(42)).ReturnsAsync((PaymentPreference?)null);
        pdfGenerator
            .Setup(candidate => candidate.GenerateAsync(It.IsAny<SepaMandatePdfData>()))
            .ThrowsAsync(new ArgumentException("A valid French IBAN is required.", "iban"));
        var service = CreateService(paymentPreferenceStore.Object, sepaMandateStore.Object, pdfGenerator.Object, getAcceptClient.Object);

        var act = () => service.SetSepaAsync(42, CreateSepaCommand());

        await act.Should().ThrowAsync<ArgumentException>();
        getAcceptClient.Verify(
            candidate => candidate.SendMandateForSignatureAsync(It.IsAny<GetAcceptMandateSignatureRequest>()),
            Times.Never);
        sepaMandateStore.Verify(
            candidate => candidate.SaveWithPaymentPreferenceAsync(
                It.IsAny<SepaMandate>(),
                It.IsAny<PaymentPreference>()),
            Times.Never);
    }

    /// <summary>
    /// Verifies that setting SEPA does not persist anything when GetAccept fails.
    /// </summary>
    [Fact]
    public async Task SetSepaAsync_WhenGetAcceptFails_DoesNotPersist()
    {
        var paymentPreferenceStore = new Mock<IPaymentPreferenceStore>();
        var sepaMandateStore = new Mock<ISepaMandateStore>();
        var pdfGenerator = new Mock<ISepaMandatePdfGenerator>();
        var getAcceptClient = new Mock<IGetAcceptClient>();
        paymentPreferenceStore.Setup(candidate => candidate.AccountExistsAsync(42)).ReturnsAsync(true);
        paymentPreferenceStore.Setup(candidate => candidate.GetAccountNumberAsync(42)).ReturnsAsync("1001102412");
        paymentPreferenceStore.Setup(candidate => candidate.GetByAccountIdAsync(42)).ReturnsAsync((PaymentPreference?)null);
        pdfGenerator
            .Setup(candidate => candidate.GenerateAsync(It.IsAny<SepaMandatePdfData>()))
            .ReturnsAsync([1, 2, 3]);
        getAcceptClient
            .Setup(candidate => candidate.SendMandateForSignatureAsync(It.IsAny<GetAcceptMandateSignatureRequest>()))
            .ThrowsAsync(new InvalidOperationException("GetAccept recipients response did not include a signer document URL."));
        var service = CreateService(paymentPreferenceStore.Object, sepaMandateStore.Object, pdfGenerator.Object, getAcceptClient.Object);

        var act = () => service.SetSepaAsync(42, CreateSepaCommand());

        await act.Should().ThrowAsync<InvalidOperationException>();
        sepaMandateStore.Verify(
            candidate => candidate.SaveWithPaymentPreferenceAsync(
                It.IsAny<SepaMandate>(),
                It.IsAny<PaymentPreference>()),
            Times.Never);
    }

    /// <summary>
    /// Verifies that resetting an existing preference clears the payment type.
    /// </summary>
    [Fact]
    public async Task ResetAsync_WhenPreferenceExists_SetsPaymentTypeToNull()
    {
        var existing = new PaymentPreference(7, 42, (int)PaymentPreferenceType.Other, DateTime.UtcNow, "user@test.fr");
        var paymentPreferenceStore = new Mock<IPaymentPreferenceStore>();
        paymentPreferenceStore.Setup(candidate => candidate.AccountExistsAsync(42)).ReturnsAsync(true);
        paymentPreferenceStore.Setup(candidate => candidate.GetByAccountIdAsync(42)).ReturnsAsync(existing);
        paymentPreferenceStore.Setup(candidate => candidate.SaveAsync(existing)).Returns(Task.CompletedTask);
        var service = CreateService(paymentPreferenceStore.Object);

        var result = await service.ResetAsync(42);

        result.Should().BeTrue();
        existing.PaymentType.Should().BeNull();
        paymentPreferenceStore.Verify(candidate => candidate.SaveAsync(existing), Times.Once);
    }

    /// <summary>
    /// Verifies that reset returns false when the account has no payment preference row.
    /// </summary>
    [Fact]
    public async Task ResetAsync_WhenPreferenceDoesNotExist_ReturnsFalse()
    {
        var paymentPreferenceStore = new Mock<IPaymentPreferenceStore>();
        paymentPreferenceStore.Setup(candidate => candidate.AccountExistsAsync(42)).ReturnsAsync(true);
        paymentPreferenceStore.Setup(candidate => candidate.GetByAccountIdAsync(42)).ReturnsAsync((PaymentPreference?)null);
        var service = CreateService(paymentPreferenceStore.Object);

        var result = await service.ResetAsync(42);

        result.Should().BeFalse();
        paymentPreferenceStore.Verify(candidate => candidate.SaveAsync(It.IsAny<PaymentPreference>()), Times.Never);
    }

    /// <summary>
    /// Verifies that reset returns false when the account does not exist.
    /// </summary>
    [Fact]
    public async Task ResetAsync_WhenAccountDoesNotExist_ReturnsFalse()
    {
        var paymentPreferenceStore = new Mock<IPaymentPreferenceStore>();
        paymentPreferenceStore.Setup(candidate => candidate.AccountExistsAsync(42)).ReturnsAsync(false);
        var service = CreateService(paymentPreferenceStore.Object);

        var result = await service.ResetAsync(42);

        result.Should().BeFalse();
        paymentPreferenceStore.Verify(candidate => candidate.SaveAsync(It.IsAny<PaymentPreference>()), Times.Never);
    }

    /// <summary>
    /// Creates a payment preferences service.
    /// </summary>
    /// <param name="paymentPreferenceStore">The payment preference store.</param>
    /// <returns>The service.</returns>
    private static PaymentPreferencesService CreateService(IPaymentPreferenceStore paymentPreferenceStore)
    {
        return new PaymentPreferencesService(
            paymentPreferenceStore,
            Mock.Of<ISepaMandateStore>(),
            Mock.Of<ISepaMandatePdfGenerator>(),
            Mock.Of<IGetAcceptClient>(),
            [new OtherPaymentPreferenceStrategy(), new SepaPaymentPreferenceStrategy()],
            NullLogger<PaymentPreferencesService>.Instance);
    }

    /// <summary>
    /// Creates a payment preferences service.
    /// </summary>
    /// <param name="paymentPreferenceStore">The payment preference store.</param>
    /// <param name="sepaMandateStore">The SEPA mandate store.</param>
    /// <param name="pdfGenerator">The SEPA PDF generator.</param>
    /// <param name="getAcceptClient">The GetAccept client.</param>
    /// <returns>The service.</returns>
    private static PaymentPreferencesService CreateService(
        IPaymentPreferenceStore paymentPreferenceStore,
        ISepaMandateStore sepaMandateStore,
        ISepaMandatePdfGenerator pdfGenerator,
        IGetAcceptClient getAcceptClient)
    {
        return new PaymentPreferencesService(
            paymentPreferenceStore,
            sepaMandateStore,
            pdfGenerator,
            getAcceptClient,
            [new OtherPaymentPreferenceStrategy(), new SepaPaymentPreferenceStrategy()],
            NullLogger<PaymentPreferencesService>.Instance);
    }

    /// <summary>
    /// Creates a SEPA command for tests.
    /// </summary>
    /// <returns>The command.</returns>
    private static SepaPaymentPreferenceCommand CreateSepaCommand()
    {
        return new SepaPaymentPreferenceCommand(
            123,
            "Jean Dupont",
            "10 rue de Paris",
            "Batiment A",
            "Paris",
            "France",
            "75008",
            "FR7630006000011234567890189",
            "AGRIFRPP",
            new SepaRecipient("jean.dupont@test.fr", "Jean", "Dupont"));
    }
}



