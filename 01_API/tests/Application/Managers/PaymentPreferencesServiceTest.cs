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
    /// Verifies that a missing account returns not found without resolving any strategy.
    /// </summary>
    [Fact]
    public async Task GetAsync_WhenAccountDoesNotExist_ReturnsAccountNotFound()
    {
        var paymentPreferenceStore = new Mock<IPaymentPreferenceStore>();
        paymentPreferenceStore.Setup(candidate => candidate.AccountExistsAsync(42)).ReturnsAsync(false);
        var readStrategy = new Mock<IPaymentPreferenceReadStrategy>(MockBehavior.Strict);
        var service = CreateService(
            paymentPreferenceStore.Object,
            Mock.Of<ISepaMandateStore>(),
            Mock.Of<ISepaMandatePdfGenerator>(),
            Mock.Of<IGetAcceptClient>(),
            [new OtherPaymentPreferenceStrategy(), new SepaPaymentPreferenceStrategy()],
            [readStrategy.Object]);

        var result = await service.GetAsync(42);

        result.AccountFound.Should().BeFalse();
        result.PaymentType.Should().BeNull();
        readStrategy.Verify(candidate => candidate.GetAsync(It.IsAny<int>(), It.IsAny<PaymentPreference?>()), Times.Never);
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
    /// Verifies that reading SEPA uses the SEPA synchronization read strategy.
    /// </summary>
    [Fact]
    public async Task GetAsync_WhenSepaPreferenceExists_UsesSepaReadStrategy()
    {
        var paymentPreferenceStore = new Mock<IPaymentPreferenceStore>();
        var defaultReadStrategy = new Mock<IPaymentPreferenceReadStrategy>(MockBehavior.Strict);
        var sepaReadStrategy = new Mock<IPaymentPreferenceReadStrategy>();
        paymentPreferenceStore.Setup(candidate => candidate.AccountExistsAsync(42)).ReturnsAsync(true);
        paymentPreferenceStore.Setup(candidate => candidate.GetByAccountIdAsync(42))
            .ReturnsAsync(new PaymentPreference(7, 42, (int)PaymentPreferenceType.MandateSepa, DateTime.UtcNow, "user@test.fr"));
        defaultReadStrategy.Setup(candidate => candidate.Supports(It.IsAny<PaymentPreferenceType?>())).Returns(false);
        sepaReadStrategy.Setup(candidate => candidate.Supports(PaymentPreferenceType.MandateSepa)).Returns(true);
        sepaReadStrategy
            .Setup(candidate => candidate.GetAsync(42, It.IsAny<PaymentPreference?>()))
            .ReturnsAsync(new PaymentPreferenceResult(true, PaymentPreferenceType.MandateSepa));
        var service = CreateService(
            paymentPreferenceStore.Object,
            Mock.Of<ISepaMandateStore>(),
            Mock.Of<ISepaMandatePdfGenerator>(),
            Mock.Of<IGetAcceptClient>(),
            [new OtherPaymentPreferenceStrategy(), new SepaPaymentPreferenceStrategy()],
            [defaultReadStrategy.Object, sepaReadStrategy.Object]);

        var result = await service.GetAsync(42);

        result.PaymentType.Should().Be(PaymentPreferenceType.MandateSepa);
        sepaReadStrategy.Verify(candidate => candidate.GetAsync(42, It.IsAny<PaymentPreference?>()), Times.Once);
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
    /// Verifies that marking sent to Akuiteo returns false for an unknown account.
    /// </summary>
    [Fact]
    public async Task MarkSentToAkuiteoAsync_WhenAccountDoesNotExist_ReturnsFalse()
    {
        var paymentPreferenceStore = new Mock<IPaymentPreferenceStore>();
        var sepaMandateStore = new Mock<ISepaMandateStore>();
        paymentPreferenceStore.Setup(candidate => candidate.AccountExistsAsync(42)).ReturnsAsync(false);
        var service = CreateService(paymentPreferenceStore.Object, sepaMandateStore.Object, Mock.Of<ISepaMandatePdfGenerator>(), Mock.Of<IGetAcceptClient>());

        var result = await service.MarkSentToAkuiteoAsync(42);

        result.Should().BeFalse();
        sepaMandateStore.Verify(candidate => candidate.MarkSentToAkuiteoAsync(It.IsAny<int>(), It.IsAny<DateTime>()), Times.Never);
    }

    /// <summary>
    /// Verifies that marking sent to Akuiteo delegates to the SEPA mandate store.
    /// </summary>
    [Fact]
    public async Task MarkSentToAkuiteoAsync_WhenAccountExists_DelegatesToMandateStore()
    {
        var paymentPreferenceStore = new Mock<IPaymentPreferenceStore>();
        var sepaMandateStore = new Mock<ISepaMandateStore>();
        paymentPreferenceStore.Setup(candidate => candidate.AccountExistsAsync(42)).ReturnsAsync(true);
        sepaMandateStore.Setup(candidate => candidate.MarkSentToAkuiteoAsync(42, It.IsAny<DateTime>())).ReturnsAsync(true);
        var service = CreateService(paymentPreferenceStore.Object, sepaMandateStore.Object, Mock.Of<ISepaMandatePdfGenerator>(), Mock.Of<IGetAcceptClient>());

        var result = await service.MarkSentToAkuiteoAsync(42);

        result.Should().BeTrue();
        sepaMandateStore.Verify(candidate => candidate.MarkSentToAkuiteoAsync(42, It.IsAny<DateTime>()), Times.Once);
    }

    /// <summary>
    /// Verifies that saving the signed mandate document identifier returns false when the identifier is blank.
    /// </summary>
    [Fact]
    public async Task SaveSignedMandateDocumentIdAsync_WhenIdentifierIsBlank_ReturnsFalse()
    {
        var paymentPreferenceStore = new Mock<IPaymentPreferenceStore>(MockBehavior.Strict);
        var sepaMandateStore = new Mock<ISepaMandateStore>(MockBehavior.Strict);
        var service = CreateService(paymentPreferenceStore.Object, sepaMandateStore.Object, Mock.Of<ISepaMandatePdfGenerator>(), Mock.Of<IGetAcceptClient>());

        var result = await service.SaveSignedMandateDocumentIdAsync(42, string.Empty);

        result.Should().BeFalse();
    }

    /// <summary>
    /// Verifies that saving the signed mandate document identifier returns false for an unknown account.
    /// </summary>
    [Fact]
    public async Task SaveSignedMandateDocumentIdAsync_WhenAccountDoesNotExist_ReturnsFalse()
    {
        var paymentPreferenceStore = new Mock<IPaymentPreferenceStore>();
        var sepaMandateStore = new Mock<ISepaMandateStore>();
        paymentPreferenceStore.Setup(candidate => candidate.AccountExistsAsync(42)).ReturnsAsync(false);
        var service = CreateService(paymentPreferenceStore.Object, sepaMandateStore.Object, Mock.Of<ISepaMandatePdfGenerator>(), Mock.Of<IGetAcceptClient>());

        var result = await service.SaveSignedMandateDocumentIdAsync(42, "456");

        result.Should().BeFalse();
        sepaMandateStore.Verify(candidate => candidate.SaveSignedMandateDocumentIdAsync(It.IsAny<int>(), It.IsAny<string>()), Times.Never);
    }

    /// <summary>
    /// Verifies that saving the signed mandate document identifier delegates to the SEPA mandate store.
    /// </summary>
    [Fact]
    public async Task SaveSignedMandateDocumentIdAsync_WhenAccountExists_DelegatesToMandateStore()
    {
        var paymentPreferenceStore = new Mock<IPaymentPreferenceStore>();
        var sepaMandateStore = new Mock<ISepaMandateStore>();
        paymentPreferenceStore.Setup(candidate => candidate.AccountExistsAsync(42)).ReturnsAsync(true);
        sepaMandateStore.Setup(candidate => candidate.SaveSignedMandateDocumentIdAsync(42, "456")).ReturnsAsync(true);
        var service = CreateService(paymentPreferenceStore.Object, sepaMandateStore.Object, Mock.Of<ISepaMandatePdfGenerator>(), Mock.Of<IGetAcceptClient>());

        var result = await service.SaveSignedMandateDocumentIdAsync(42, "456");

        result.Should().BeTrue();
        sepaMandateStore.Verify(candidate => candidate.SaveSignedMandateDocumentIdAsync(42, "456"), Times.Once);
    }

    /// <summary>
    /// Verifies that the SEPA read strategy returns MANDATE_SEPA without GetAccept calls when the mandate is already signed and sent.
    /// </summary>
    [Fact]
    public async Task SepaReadStrategy_WhenMandateIsAlreadySignedAndSent_ReturnsMandateSepa()
    {
        var sepaMandateStore = new Mock<ISepaMandateStore>();
        var getAcceptClient = new Mock<IGetAcceptClient>();
        sepaMandateStore
            .Setup(store => store.GetLatestByAccountIdAsync(42))
            .ReturnsAsync(CreateSepaMandate(SepaMandateSignatureStatus.Signed, true));
        var strategy = new SepaSynchronizationPaymentPreferenceReadStrategy(
            sepaMandateStore.Object,
            getAcceptClient.Object,
            NullLogger<SepaSynchronizationPaymentPreferenceReadStrategy>.Instance);

        var result = await strategy.GetAsync(42, new PaymentPreference(7, 42, (int)PaymentPreferenceType.MandateSepa, DateTime.UtcNow, "user@test.fr"));

        result.PaymentType.Should().Be(PaymentPreferenceType.MandateSepa);
        result.SignedMandatePdf.Should().BeNull();
        getAcceptClient.Verify(client => client.GetDocumentStatusAsync(It.IsAny<string>()), Times.Never);
    }

    /// <summary>
    /// Verifies that the SEPA read strategy keeps a signed but not yet sent mandate resumable.
    /// </summary>
    [Fact]
    public async Task SepaReadStrategy_WhenMandateIsAlreadySignedButNotSent_ReturnsSignedPdf()
    {
        var sepaMandateStore = new Mock<ISepaMandateStore>();
        var getAcceptClient = new Mock<IGetAcceptClient>();
        sepaMandateStore
            .Setup(store => store.GetLatestByAccountIdAsync(42))
            .ReturnsAsync(CreateSepaMandate(SepaMandateSignatureStatus.Signed));
        getAcceptClient
            .Setup(client => client.GetDocumentStatusAsync("doc-123"))
            .ReturnsAsync(new GetAcceptDocumentStatusResponse("signed", "https://download.test/signed.pdf"));
        getAcceptClient
            .Setup(client => client.DownloadSignedDocumentAsync("https://download.test/signed.pdf"))
            .ReturnsAsync(new GetAcceptSignedDocument([1, 2, 3], "application/pdf", "signed.pdf"));
        var strategy = new SepaSynchronizationPaymentPreferenceReadStrategy(
            sepaMandateStore.Object,
            getAcceptClient.Object,
            NullLogger<SepaSynchronizationPaymentPreferenceReadStrategy>.Instance);

        var result = await strategy.GetAsync(42, new PaymentPreference(7, 42, (int)PaymentPreferenceType.MandateSepa, DateTime.UtcNow, "user@test.fr"));

        result.PaymentType.Should().Be(PaymentPreferenceType.MandateSepa);
        result.AccountId.Should().Be(42);
        result.RibDocumentId.Should().Be(123);
        result.SignedMandatePdf.Should().Equal([1, 2, 3]);
        sepaMandateStore.Verify(store => store.UpdateSignatureStatusAsync(It.IsAny<int>(), It.IsAny<SepaMandateSignatureStatus>()), Times.Never);
        getAcceptClient.Verify(client => client.GetDocumentStatusAsync("doc-123"), Times.Once);
        getAcceptClient.Verify(client => client.DownloadSignedDocumentAsync("https://download.test/signed.pdf"), Times.Once);
    }

    /// <summary>
    /// Verifies that the SEPA read strategy reports null when no mandate row exists.
    /// </summary>
    [Fact]
    public async Task SepaReadStrategy_WhenMandateDoesNotExist_ReturnsNullPaymentType()
    {
        var sepaMandateStore = new Mock<ISepaMandateStore>();
        var getAcceptClient = new Mock<IGetAcceptClient>();
        sepaMandateStore
            .Setup(store => store.GetLatestByAccountIdAsync(42))
            .ReturnsAsync((SepaMandate?)null);
        var strategy = new SepaSynchronizationPaymentPreferenceReadStrategy(
            sepaMandateStore.Object,
            getAcceptClient.Object,
            NullLogger<SepaSynchronizationPaymentPreferenceReadStrategy>.Instance);

        var result = await strategy.GetAsync(42, new PaymentPreference(7, 42, (int)PaymentPreferenceType.MandateSepa, DateTime.UtcNow, "user@test.fr"));

        result.PaymentType.Should().BeNull();
        getAcceptClient.Verify(client => client.GetDocumentStatusAsync(It.IsAny<string>()), Times.Never);
    }

    /// <summary>
    /// Verifies that the SEPA read strategy reports null when the mandate has no signature request identifier.
    /// </summary>
    [Fact]
    public async Task SepaReadStrategy_WhenSignatureRequestIdIsMissing_ReturnsNullPaymentType()
    {
        var sepaMandateStore = new Mock<ISepaMandateStore>();
        var getAcceptClient = new Mock<IGetAcceptClient>();
        sepaMandateStore
            .Setup(store => store.GetLatestByAccountIdAsync(42))
            .ReturnsAsync(CreateSepaMandate(SepaMandateSignatureStatus.Sent, false, null));
        var strategy = new SepaSynchronizationPaymentPreferenceReadStrategy(
            sepaMandateStore.Object,
            getAcceptClient.Object,
            NullLogger<SepaSynchronizationPaymentPreferenceReadStrategy>.Instance);

        var result = await strategy.GetAsync(42, new PaymentPreference(7, 42, (int)PaymentPreferenceType.MandateSepa, DateTime.UtcNow, "user@test.fr"));

        result.PaymentType.Should().BeNull();
        getAcceptClient.Verify(client => client.GetDocumentStatusAsync(It.IsAny<string>()), Times.Never);
    }

    /// <summary>
    /// Verifies that the SEPA read strategy refreshes GetAccept status and returns null while the signature is not signed.
    /// </summary>
    [Fact]
    public async Task SepaReadStrategy_WhenGetAcceptStatusIsNotSigned_ReturnsNullAndUpdatesStatus()
    {
        var sepaMandateStore = new Mock<ISepaMandateStore>();
        var getAcceptClient = new Mock<IGetAcceptClient>();
        sepaMandateStore
            .Setup(store => store.GetLatestByAccountIdAsync(42))
            .ReturnsAsync(CreateSepaMandate(SepaMandateSignatureStatus.Sent));
        getAcceptClient
            .Setup(client => client.GetDocumentStatusAsync("doc-123"))
            .ReturnsAsync(new GetAcceptDocumentStatusResponse("sent", null));
        var strategy = new SepaSynchronizationPaymentPreferenceReadStrategy(
            sepaMandateStore.Object,
            getAcceptClient.Object,
            NullLogger<SepaSynchronizationPaymentPreferenceReadStrategy>.Instance);

        var result = await strategy.GetAsync(42, new PaymentPreference(7, 42, (int)PaymentPreferenceType.MandateSepa, DateTime.UtcNow, "user@test.fr"));

        result.PaymentType.Should().BeNull();
        sepaMandateStore.Verify(store => store.UpdateSignatureStatusAsync(5, SepaMandateSignatureStatus.Sent), Times.Once);
        getAcceptClient.Verify(client => client.DownloadSignedDocumentAsync(It.IsAny<string>()), Times.Never);
    }

    /// <summary>
    /// Verifies that the SEPA read strategy downloads and returns the signed PDF when GetAccept status becomes signed.
    /// </summary>
    [Fact]
    public async Task SepaReadStrategy_WhenGetAcceptStatusBecomesSigned_ReturnsSignedPdf()
    {
        var sepaMandateStore = new Mock<ISepaMandateStore>();
        var getAcceptClient = new Mock<IGetAcceptClient>();
        sepaMandateStore
            .Setup(store => store.GetLatestByAccountIdAsync(42))
            .ReturnsAsync(CreateSepaMandate(SepaMandateSignatureStatus.Sent));
        getAcceptClient
            .Setup(client => client.GetDocumentStatusAsync("doc-123"))
            .ReturnsAsync(new GetAcceptDocumentStatusResponse("signed", "https://download.test/signed.pdf"));
        getAcceptClient
            .Setup(client => client.DownloadSignedDocumentAsync("https://download.test/signed.pdf"))
            .ReturnsAsync(new GetAcceptSignedDocument([1, 2, 3], "application/pdf", "signed.pdf"));
        var strategy = new SepaSynchronizationPaymentPreferenceReadStrategy(
            sepaMandateStore.Object,
            getAcceptClient.Object,
            NullLogger<SepaSynchronizationPaymentPreferenceReadStrategy>.Instance);

        var result = await strategy.GetAsync(42, new PaymentPreference(7, 42, (int)PaymentPreferenceType.MandateSepa, DateTime.UtcNow, "user@test.fr"));

        result.PaymentType.Should().Be(PaymentPreferenceType.MandateSepa);
        result.AccountId.Should().Be(42);
        result.RibDocumentId.Should().Be(123);
        result.SignedMandatePdf.Should().Equal([1, 2, 3]);
        result.SignedMandateContentType.Should().Be("application/pdf");
        result.SignedMandateFileName.Should().Be("signed.pdf");
        sepaMandateStore.Verify(store => store.UpdateSignatureStatusAsync(5, SepaMandateSignatureStatus.Signed), Times.Once);
    }

    /// <summary>
    /// Verifies that the SEPA read strategy does not return upload payload when the mandate was already sent to Akuiteo.
    /// </summary>
    [Fact]
    public async Task SepaReadStrategy_WhenGetAcceptStatusBecomesSignedButMandateWasSentToAkuiteo_ReturnsMandateSepaWithoutPdf()
    {
        var sepaMandateStore = new Mock<ISepaMandateStore>();
        var getAcceptClient = new Mock<IGetAcceptClient>();
        sepaMandateStore
            .Setup(store => store.GetLatestByAccountIdAsync(42))
            .ReturnsAsync(CreateSepaMandate(SepaMandateSignatureStatus.Sent, true));
        getAcceptClient
            .Setup(client => client.GetDocumentStatusAsync("doc-123"))
            .ReturnsAsync(new GetAcceptDocumentStatusResponse("signed", "https://download.test/signed.pdf"));
        var strategy = new SepaSynchronizationPaymentPreferenceReadStrategy(
            sepaMandateStore.Object,
            getAcceptClient.Object,
            NullLogger<SepaSynchronizationPaymentPreferenceReadStrategy>.Instance);

        var result = await strategy.GetAsync(42, new PaymentPreference(7, 42, (int)PaymentPreferenceType.MandateSepa, DateTime.UtcNow, "user@test.fr"));

        result.PaymentType.Should().Be(PaymentPreferenceType.MandateSepa);
        result.SignedMandatePdf.Should().BeNull();
        result.RibDocumentId.Should().BeNull();
        sepaMandateStore.Verify(store => store.UpdateSignatureStatusAsync(5, SepaMandateSignatureStatus.Signed), Times.Once);
        getAcceptClient.Verify(client => client.DownloadSignedDocumentAsync(It.IsAny<string>()), Times.Never);
    }

    /// <summary>
    /// Verifies that the SEPA read strategy throws when a signed mandate has no download URL.
    /// </summary>
    [Fact]
    public async Task SepaReadStrategy_WhenSignedDocumentUrlIsMissing_Throws()
    {
        var sepaMandateStore = new Mock<ISepaMandateStore>();
        var getAcceptClient = new Mock<IGetAcceptClient>();
        sepaMandateStore
            .Setup(store => store.GetLatestByAccountIdAsync(42))
            .ReturnsAsync(CreateSepaMandate(SepaMandateSignatureStatus.Sent));
        getAcceptClient
            .Setup(client => client.GetDocumentStatusAsync("doc-123"))
            .ReturnsAsync(new GetAcceptDocumentStatusResponse("signed", null));
        var strategy = new SepaSynchronizationPaymentPreferenceReadStrategy(
            sepaMandateStore.Object,
            getAcceptClient.Object,
            NullLogger<SepaSynchronizationPaymentPreferenceReadStrategy>.Instance);

        var act = () => strategy.GetAsync(42, new PaymentPreference(7, 42, (int)PaymentPreferenceType.MandateSepa, DateTime.UtcNow, "user@test.fr"));

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("GetAccept document doc-123 is signed but did not include a download_url.");
        sepaMandateStore.Verify(store => store.UpdateSignatureStatusAsync(5, SepaMandateSignatureStatus.Signed), Times.Once);
    }

    /// <summary>
    /// Verifies that GetAccept failures propagate without updating the mandate.
    /// </summary>
    [Fact]
    public async Task SepaReadStrategy_WhenGetAcceptStatusFails_ThrowsWithoutUpdatingStatus()
    {
        var sepaMandateStore = new Mock<ISepaMandateStore>();
        var getAcceptClient = new Mock<IGetAcceptClient>();
        sepaMandateStore
            .Setup(store => store.GetLatestByAccountIdAsync(42))
            .ReturnsAsync(CreateSepaMandate(SepaMandateSignatureStatus.Sent));
        getAcceptClient
            .Setup(client => client.GetDocumentStatusAsync("doc-123"))
            .ThrowsAsync(new HttpRequestException("GetAccept failed."));
        var strategy = new SepaSynchronizationPaymentPreferenceReadStrategy(
            sepaMandateStore.Object,
            getAcceptClient.Object,
            NullLogger<SepaSynchronizationPaymentPreferenceReadStrategy>.Instance);

        var act = () => strategy.GetAsync(42, new PaymentPreference(7, 42, (int)PaymentPreferenceType.MandateSepa, DateTime.UtcNow, "user@test.fr"));

        await act.Should().ThrowAsync<HttpRequestException>();
        sepaMandateStore.Verify(store => store.UpdateSignatureStatusAsync(It.IsAny<int>(), It.IsAny<SepaMandateSignatureStatus>()), Times.Never);
    }

    /// <summary>
    /// Verifies that the SEPA read strategy only supports MANDATE_SEPA payment types.
    /// </summary>
    [Theory]
    [InlineData(PaymentPreferenceType.MandateSepa, true)]
    [InlineData(PaymentPreferenceType.Other, false)]
    public void SepaReadStrategy_Supports_ReturnsExpectedValue(PaymentPreferenceType paymentType, bool expected)
    {
        var strategy = new SepaSynchronizationPaymentPreferenceReadStrategy(
            Mock.Of<ISepaMandateStore>(),
            Mock.Of<IGetAcceptClient>(),
            NullLogger<SepaSynchronizationPaymentPreferenceReadStrategy>.Instance);

        var result = strategy.Supports(paymentType);

        result.Should().Be(expected);
    }

    /// <summary>
    /// Verifies that the SEPA read strategy does not support a null payment type.
    /// </summary>
    [Fact]
    public void SepaReadStrategy_Supports_WhenPaymentTypeIsNull_ReturnsFalse()
    {
        var strategy = new SepaSynchronizationPaymentPreferenceReadStrategy(
            Mock.Of<ISepaMandateStore>(),
            Mock.Of<IGetAcceptClient>(),
            NullLogger<SepaSynchronizationPaymentPreferenceReadStrategy>.Instance);

        var result = strategy.Supports(null);

        result.Should().BeFalse();
    }

    /// <summary>
    /// Verifies the GetAccept status field mapping used by SEPA synchronization.
    /// </summary>
    [Theory]
    [InlineData("sent", SepaMandateSignatureStatus.Sent)]
    [InlineData("signed", SepaMandateSignatureStatus.Signed)]
    [InlineData("viewed", SepaMandateSignatureStatus.Viewed)]
    [InlineData("draft", SepaMandateSignatureStatus.Draft)]
    [InlineData("processing", SepaMandateSignatureStatus.Processing)]
    [InlineData("sealed", SepaMandateSignatureStatus.Sealed)]
    [InlineData("reviewed", SepaMandateSignatureStatus.Reviewed)]
    [InlineData("rejected", SepaMandateSignatureStatus.Rejected)]
    [InlineData("recalled", SepaMandateSignatureStatus.Recalled)]
    [InlineData(" SIGNED ", SepaMandateSignatureStatus.Signed)]
    [InlineData(null, SepaMandateSignatureStatus.Processing)]
    [InlineData("unknown", SepaMandateSignatureStatus.Processing)]
    public void MapGetAcceptStatus_WhenStatusIsProvided_ReturnsInternalStatus(
        string? status,
        SepaMandateSignatureStatus expected)
    {
        var result = SepaSynchronizationPaymentPreferenceReadStrategy.MapGetAcceptStatus(status);

        result.Should().Be(expected);
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
            [new DefaultPaymentPreferenceReadStrategy(), Mock.Of<IPaymentPreferenceReadStrategy>(strategy => strategy.Supports(PaymentPreferenceType.MandateSepa) == true)],
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
            [new DefaultPaymentPreferenceReadStrategy(), Mock.Of<IPaymentPreferenceReadStrategy>(strategy => strategy.Supports(PaymentPreferenceType.MandateSepa) == true)],
            NullLogger<PaymentPreferencesService>.Instance);
    }

    /// <summary>
    /// Creates a payment preferences service with explicit strategy collections.
    /// </summary>
    /// <param name="paymentPreferenceStore">The payment preference store.</param>
    /// <param name="sepaMandateStore">The SEPA mandate store.</param>
    /// <param name="pdfGenerator">The SEPA PDF generator.</param>
    /// <param name="getAcceptClient">The GetAccept client.</param>
    /// <param name="paymentPreferenceStrategies">The write strategies.</param>
    /// <param name="paymentPreferenceReadStrategies">The read strategies.</param>
    /// <returns>The service.</returns>
    private static PaymentPreferencesService CreateService(
        IPaymentPreferenceStore paymentPreferenceStore,
        ISepaMandateStore sepaMandateStore,
        ISepaMandatePdfGenerator pdfGenerator,
        IGetAcceptClient getAcceptClient,
        IEnumerable<IPaymentPreferenceStrategy> paymentPreferenceStrategies,
        IEnumerable<IPaymentPreferenceReadStrategy> paymentPreferenceReadStrategies)
    {
        return new PaymentPreferencesService(
            paymentPreferenceStore,
            sepaMandateStore,
            pdfGenerator,
            getAcceptClient,
            paymentPreferenceStrategies,
            paymentPreferenceReadStrategies,
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

    /// <summary>
    /// Creates a SEPA mandate with the provided signature status.
    /// </summary>
    /// <param name="signatureStatus">The signature status.</param>
    /// <returns>The SEPA mandate.</returns>
    private static SepaMandate CreateSepaMandate(
        SepaMandateSignatureStatus signatureStatus,
        bool isSentToAkuiteo = false,
        string? signatureRequestId = "doc-123")
    {
        return new SepaMandate(
            5,
            42,
            123,
            "Jean Dupont",
            "FR7630006000011234567890189",
            "AGRIFRPP",
            "10 rue de Paris",
            signatureRequestId,
            "https://signature.test",
            signatureStatus,
            isSentToAkuiteo,
            isSentToAkuiteo ? DateTime.UtcNow : null,
            DateTime.UtcNow,
            "user@test.fr");
    }
}



