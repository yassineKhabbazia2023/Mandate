// <copyright file="PaymentPreferencesSqlAdapterSepaTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Adapters.Tests;

using KPMG.Pulse.Back.Accounting.Mandate;
using KPMG.Pulse.Back.Accounting.Mandate.Adapters;
using KPMG.Pulse.Back.Accounting.Mandate.Sql;
using Moq;

/// <summary>
/// Unit tests for SEPA mandate methods on <see cref="PaymentPreferencesSqlAdapter"/>.
/// </summary>
public sealed class PaymentPreferencesSqlAdapterSepaTest
{
    /// <summary>
    /// Verifies that the SEPA mandate and payment preference models are mapped and delegated.
    /// </summary>
    [Fact]
    public async Task SaveWithPaymentPreferenceAsync_WhenProvided_MapsDatabaseModels()
    {
        SepaMandateDb? savedMandate = null;
        PaymentPreferenceDb? savedPreference = null;
        var createdAt = DateTime.UtcNow;
        var sentToAkuiteoAt = DateTime.UtcNow.AddMinutes(-5);
        var repository = new Mock<ISepaMandateRepository>();
        repository
            .Setup(candidate => candidate.SaveWithPaymentPreferenceAsync(
                It.IsAny<SepaMandateDb>(),
                It.IsAny<PaymentPreferenceDb>()))
            .Callback<SepaMandateDb, PaymentPreferenceDb>((mandate, preference) =>
            {
                savedMandate = mandate;
                savedPreference = preference;
            })
            .Returns(Task.CompletedTask);
        var adapter = CreateAdapter(repository.Object);
        var sepaMandate = new SepaMandate(
            7,
            42,
            99,
            "Jean Dupont",
            "FR7630006000011234567890189",
            "AGRIFRPP",
            "10 rue de Paris",
            "doc-123",
            "https://signature.test",
            SepaMandateSignatureStatus.Processing,
            true,
            sentToAkuiteoAt,
            createdAt,
            "user@test.fr");
        var paymentPreference = new PaymentPreference(11, 42, 2, createdAt, "user@test.fr");

        await adapter.SaveWithPaymentPreferenceAsync(sepaMandate, paymentPreference);

        savedMandate.Should().NotBeNull();
        savedMandate!.Id.Should().Be(7);
        savedMandate.AccountId.Should().Be(42);
        savedMandate.RibDocumentId.Should().Be(99);
        savedMandate.AccountHolder.Should().Be("Jean Dupont");
        savedMandate.Iban.Should().Be("FR7630006000011234567890189");
        savedMandate.Bic.Should().Be("AGRIFRPP");
        savedMandate.Address.Should().Be("10 rue de Paris");
        savedMandate.SignatureRequestId.Should().Be("doc-123");
        savedMandate.SignatureUrl.Should().Be("https://signature.test");
        savedMandate.SignatureStatus.Should().Be((int)SepaMandateSignatureStatus.Processing);
        savedMandate.IsSentToAkuiteo.Should().BeTrue();
        savedMandate.SentToAkuiteoAt.Should().Be(sentToAkuiteoAt);
        savedMandate.SignedMandateDocumentId.Should().BeNull();
        savedMandate.CreatedAt.Should().Be(createdAt);
        savedMandate.CreatedBy.Should().Be("user@test.fr");

        savedPreference.Should().NotBeNull();
        savedPreference!.Id.Should().Be(11);
        savedPreference.AccountId.Should().Be(42);
        savedPreference.PaymentType.Should().Be(2);
        savedPreference.CreatedAt.Should().Be(createdAt);
        savedPreference.CreatedBy.Should().Be("user@test.fr");
    }

    /// <summary>
    /// Verifies that a null SEPA mandate throws.
    /// </summary>
    [Fact]
    public async Task SaveWithPaymentPreferenceAsync_WhenSepaMandateIsNull_Throws()
    {
        var adapter = CreateAdapter(Mock.Of<ISepaMandateRepository>());

        Func<Task> act = async () => await adapter.SaveWithPaymentPreferenceAsync(
            null!,
            new PaymentPreference(0, 0, null, DateTime.UtcNow, "user@test.fr"));

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    /// <summary>
    /// Verifies that a missing SEPA mandate maps to null.
    /// </summary>
    [Fact]
    public async Task GetLatestByAccountIdAsync_WhenMandateDoesNotExist_ReturnsNull()
    {
        var repository = new Mock<ISepaMandateRepository>();
        repository.Setup(candidate => candidate.GetLatestByAccountIdAsync(42)).ReturnsAsync((SepaMandateDb?)null);
        var adapter = CreateAdapter(repository.Object);

        var result = await adapter.GetLatestByAccountIdAsync(42);

        result.Should().BeNull();
    }

    /// <summary>
    /// Verifies that a database SEPA mandate maps to the domain model.
    /// </summary>
    [Fact]
    public async Task GetLatestByAccountIdAsync_WhenMandateExists_MapsDomainMandate()
    {
        var createdAt = DateTime.UtcNow;
        var sentToAkuiteoAt = createdAt.AddMinutes(-3);
        var repository = new Mock<ISepaMandateRepository>();
        repository.Setup(candidate => candidate.GetLatestByAccountIdAsync(42)).ReturnsAsync(new SepaMandateDb
        {
            Id = 7,
            AccountId = 42,
            RibDocumentId = 99,
            AccountHolder = "Jean Dupont",
            Iban = "FR7630006000011234567890189",
            Bic = "AGRIFRPP",
            Address = "10 rue de Paris",
            SignatureRequestId = "doc-123",
            SignatureUrl = "https://signature.test",
            SignatureStatus = (int)SepaMandateSignatureStatus.Signed,
            IsSentToAkuiteo = true,
            SentToAkuiteoAt = sentToAkuiteoAt,
            SignedMandateDocumentId = "456",
            CreatedAt = createdAt,
            CreatedBy = "user@test.fr"
        });
        var adapter = CreateAdapter(repository.Object);

        var result = await adapter.GetLatestByAccountIdAsync(42);

        result.Should().NotBeNull();
        result!.Id.Should().Be(7);
        result.AccountId.Should().Be(42);
        result.DocumentId.Should().Be(99);
        result.AccountHolder.Should().Be("Jean Dupont");
        result.SignatureRequestId.Should().Be("doc-123");
        result.SignatureUrl.Should().Be("https://signature.test");
        result.SignatureStatus.Should().Be(SepaMandateSignatureStatus.Signed);
        result.IsSentToAkuiteo.Should().BeTrue();
        result.SentToAkuiteoAt.Should().Be(sentToAkuiteoAt);
        result.SignedMandateDocumentId.Should().Be("456");
    }

    /// <summary>
    /// Verifies that signature status updates are delegated with the mapped integer value.
    /// </summary>
    [Fact]
    public async Task UpdateSignatureStatusAsync_WhenCalled_DelegatesMappedStatus()
    {
        var repository = new Mock<ISepaMandateRepository>();
        repository
            .Setup(candidate => candidate.UpdateSignatureStatusAsync(7, (int)SepaMandateSignatureStatus.Signed))
            .Returns(Task.CompletedTask);
        var adapter = CreateAdapter(repository.Object);

        await adapter.UpdateSignatureStatusAsync(7, SepaMandateSignatureStatus.Signed);

        repository.Verify(candidate => candidate.UpdateSignatureStatusAsync(7, (int)SepaMandateSignatureStatus.Signed), Times.Once);
    }

    /// <summary>
    /// Verifies that mark-sent is delegated to the repository.
    /// </summary>
    [Fact]
    public async Task MarkSentToAkuiteoAsync_WhenCalled_DelegatesToRepository()
    {
        var sentAt = DateTime.UtcNow;
        var repository = new Mock<ISepaMandateRepository>();
        repository.Setup(candidate => candidate.MarkSentToAkuiteoAsync(42, sentAt)).ReturnsAsync(true);
        var adapter = CreateAdapter(repository.Object);

        var result = await adapter.MarkSentToAkuiteoAsync(42, sentAt);

        result.Should().BeTrue();
        repository.Verify(candidate => candidate.MarkSentToAkuiteoAsync(42, sentAt), Times.Once);
    }

    /// <summary>
    /// Verifies that saving the signed mandate document identifier is delegated to the repository.
    /// </summary>
    [Fact]
    public async Task SaveSignedMandateDocumentIdAsync_WhenCalled_DelegatesToRepository()
    {
        var repository = new Mock<ISepaMandateRepository>();
        repository.Setup(candidate => candidate.SaveSignedMandateDocumentIdAsync(42, "456")).ReturnsAsync(true);
        var adapter = CreateAdapter(repository.Object);

        var result = await adapter.SaveSignedMandateDocumentIdAsync(42, "456");

        result.Should().BeTrue();
        repository.Verify(candidate => candidate.SaveSignedMandateDocumentIdAsync(42, "456"), Times.Once);
    }

    /// <summary>
    /// Verifies that a null payment preference throws.
    /// </summary>
    [Fact]
    public async Task SaveWithPaymentPreferenceAsync_WhenPaymentPreferenceIsNull_Throws()
    {
        var adapter = CreateAdapter(Mock.Of<ISepaMandateRepository>());

        Func<Task> act = async () => await adapter.SaveWithPaymentPreferenceAsync(
            new SepaMandate(
                7,
                42,
                99,
                "Jean Dupont",
                "FR7630006000011234567890189",
                "AGRIFRPP",
                "10 rue de Paris",
                null,
                null,
                SepaMandateSignatureStatus.Processing,
                false,
                null,
                DateTime.UtcNow,
                "user@test.fr"),
            null!);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    /// <summary>
    /// Creates the payment preference SQL adapter under test.
    /// </summary>
    /// <param name="repository">The SEPA mandate repository.</param>
    /// <returns>The adapter.</returns>
    private static PaymentPreferencesSqlAdapter CreateAdapter(ISepaMandateRepository repository)
    {
        return new PaymentPreferencesSqlAdapter(Mock.Of<IPaymentPreferenceRepository>(), repository);
    }
}
