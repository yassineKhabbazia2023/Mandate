// <copyright file="SepaMandateDbTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Sql.Tests;

/// <summary>
/// Unit tests for <see cref="SepaMandateDb"/>.
/// </summary>
public sealed class SepaMandateDbTest
{
    /// <summary>
    /// Verifies the default values of the SEPA mandate database model.
    /// </summary>
    [Fact]
    public void Defaults()
    {
        var entity = new SepaMandateDb();

        entity.GetType().GetProperties().Length.Should().Be(15);
        entity.Id.Should().Be(0);
        entity.AccountId.Should().Be(0);
        entity.AccountHolder.Should().BeNull();
        entity.Address.Should().BeNull();
        entity.Iban.Should().BeNull();
        entity.Bic.Should().BeNull();
        entity.RibDocumentId.Should().Be(0);
        entity.SignatureRequestId.Should().BeNull();
        entity.SignatureUrl.Should().BeNull();
        entity.SignatureStatus.Should().Be(0);
        entity.IsSentToAkuiteo.Should().BeFalse();
        entity.SentToAkuiteoAt.Should().BeNull();
        entity.CreatedAt.Should().Be(default);
        entity.CreatedBy.Should().BeNull();
        entity.Account.Should().BeNull();
    }

    /// <summary>
    /// Verifies explicit values on the SEPA mandate database model.
    /// </summary>
    [Fact]
    public void Values()
    {
        var account = new CompanyDb { Id = 42, Name = "Company" };
        var sentToAkuiteoAt = DateTime.UtcNow;
        var createdAt = DateTime.UtcNow.AddMinutes(-1);

        var entity = new SepaMandateDb
        {
            Id = 7,
            AccountId = 42,
            AccountHolder = "Jean Dupont",
            Address = "10 rue de Paris",
            Iban = "FR7630006000011234567890189",
            Bic = "AGRIFRPP",
            RibDocumentId = 99,
            SignatureRequestId = "doc-123",
            SignatureUrl = "https://signature.test",
            SignatureStatus = 3,
            IsSentToAkuiteo = true,
            SentToAkuiteoAt = sentToAkuiteoAt,
            CreatedAt = createdAt,
            CreatedBy = "user@test.fr",
            Account = account
        };

        entity.Id.Should().Be(7);
        entity.AccountId.Should().Be(42);
        entity.AccountHolder.Should().Be("Jean Dupont");
        entity.Address.Should().Be("10 rue de Paris");
        entity.Iban.Should().Be("FR7630006000011234567890189");
        entity.Bic.Should().Be("AGRIFRPP");
        entity.RibDocumentId.Should().Be(99);
        entity.SignatureRequestId.Should().Be("doc-123");
        entity.SignatureUrl.Should().Be("https://signature.test");
        entity.SignatureStatus.Should().Be(3);
        entity.IsSentToAkuiteo.Should().BeTrue();
        entity.SentToAkuiteoAt.Should().Be(sentToAkuiteoAt);
        entity.CreatedAt.Should().Be(createdAt);
        entity.CreatedBy.Should().Be("user@test.fr");
        entity.Account.Should().BeSameAs(account);
    }
}
