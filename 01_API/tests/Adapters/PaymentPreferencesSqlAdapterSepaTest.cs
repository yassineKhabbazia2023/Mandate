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
