// <copyright file="SepaMandateRepositoryTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Sql.Implementation.Tests.Repositories;

using KPMG.Pulse.Back.Accounting.Mandate.Sql;
using KPMG.Pulse.Back.Accounting.Mandate.Sql.Implementation.Tests;
using KPMG.Pulse.Back.Accounting.Mandate.Sql.Implementation.Tests.Tools;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Integration tests for <see cref="SepaMandateRepository"/>.
/// </summary>
public sealed class SepaMandateRepositoryTest : SqlServerTestBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SepaMandateRepositoryTest"/> class.
    /// </summary>
    /// <param name="sqlServerFixture">The SQL server fixture.</param>
    public SepaMandateRepositoryTest(SqlServerFixture sqlServerFixture)
        : base(sqlServerFixture)
    {
    }

    /// <summary>
    /// Verifies that a new preference is inserted with the SEPA mandate row.
    /// </summary>
    [Fact]
    public async Task SaveWithPaymentPreferenceAsync_WhenPreferenceIsNew_InsertsBothRows()
    {
        await using var context = new MandateContext(CreateContextOptions());
        await context.Company.AddAsync(CreateCompany(42));
        await context.SaveChangesAsync();
        var repository = new SepaMandateRepository(context);

        await repository.SaveWithPaymentPreferenceAsync(
            new SepaMandateDb
            {
                AccountId = 42,
                AccountHolder = "Jean Dupont",
                Address = "10 rue de Paris",
                Iban = "FR7630006000011234567890189",
                Bic = "AGRIFRPP",
                RibDocumentId = 99,
                SignatureRequestId = "doc-123",
                SignatureUrl = "https://signature.test",
                SignatureStatus = 1,
                IsSentToAkuiteo = false,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "user@test.fr"
            },
            new PaymentPreferenceDb
            {
                AccountId = 42,
                PaymentType = 2,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "user@test.fr"
            });

        context.SepaMandates.Should().ContainSingle();
        context.PaymentPreferences.Should().ContainSingle();
        context.SepaMandates.Single().SignatureUrl.Should().Be("https://signature.test");
        context.PaymentPreferences.Single().PaymentType.Should().Be(2);
    }

    /// <summary>
    /// Verifies that an existing preference is updated rather than inserted again.
    /// </summary>
    [Fact]
    public async Task SaveWithPaymentPreferenceAsync_WhenPreferenceExists_UpdatesPreference()
    {
        await using var context = new MandateContext(CreateContextOptions());
        await context.Company.AddAsync(CreateCompany(42));
        var preference = new PaymentPreferenceDb
        {
            AccountId = 42,
            PaymentType = 2,
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            CreatedBy = "old@test.fr"
        };
        await context.PaymentPreferences.AddAsync(preference);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();
        var repository = new SepaMandateRepository(context);

        await repository.SaveWithPaymentPreferenceAsync(
            new SepaMandateDb
            {
                AccountId = 42,
                AccountHolder = "Jean Dupont",
                Address = "10 rue de Paris",
                Iban = "FR7630006000011234567890189",
                Bic = "AGRIFRPP",
                RibDocumentId = 99,
                SignatureStatus = 1,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "user@test.fr"
            },
            new PaymentPreferenceDb
            {
                Id = preference.Id,
                AccountId = 42,
                PaymentType = null,
                CreatedAt = preference.CreatedAt,
                CreatedBy = preference.CreatedBy
            });

        context.SepaMandates.Should().ContainSingle();
        context.PaymentPreferences.Should().ContainSingle(candidate => candidate.Id == preference.Id && candidate.PaymentType == null);
    }

    /// <summary>
    /// Verifies that a null SEPA mandate throws.
    /// </summary>
    [Fact]
    public async Task SaveWithPaymentPreferenceAsync_WhenSepaMandateIsNull_Throws()
    {
        await using var context = new MandateContext(CreateContextOptions());
        var repository = new SepaMandateRepository(context);

        Func<Task> act = async () => await repository.SaveWithPaymentPreferenceAsync(null!, new PaymentPreferenceDb());

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    /// <summary>
    /// Verifies that a null payment preference throws.
    /// </summary>
    [Fact]
    public async Task SaveWithPaymentPreferenceAsync_WhenPaymentPreferenceIsNull_Throws()
    {
        await using var context = new MandateContext(CreateContextOptions());
        var repository = new SepaMandateRepository(context);

        Func<Task> act = async () => await repository.SaveWithPaymentPreferenceAsync(new SepaMandateDb(), null!);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    /// <summary>
    /// Verifies that the latest SEPA mandate is returned for the account.
    /// </summary>
    [Fact]
    public async Task GetLatestByAccountIdAsync_WhenSeveralMandatesExist_ReturnsLatestRow()
    {
        await using var context = new MandateContext(CreateContextOptions());
        await context.Company.AddAsync(CreateCompany(42));
        await context.SepaMandates.AddRangeAsync(
            new SepaMandateDb
            {
                AccountId = 42,
                AccountHolder = "Jean Dupont",
                Address = "10 rue de Paris",
                Iban = "FR7630006000011234567890189",
                Bic = "AGRIFRPP",
                RibDocumentId = 99,
                SignatureRequestId = "doc-older",
                SignatureUrl = "https://older.test",
                SignatureStatus = 1,
                IsSentToAkuiteo = false,
                CreatedAt = DateTime.UtcNow.AddMinutes(-2),
                CreatedBy = "user@test.fr"
            },
            new SepaMandateDb
            {
                AccountId = 42,
                AccountHolder = "Jean Dupont",
                Address = "10 rue de Paris",
                Iban = "FR7630006000011234567890189",
                Bic = "AGRIFRPP",
                RibDocumentId = 100,
                SignatureRequestId = null,
                SignatureUrl = null,
                SignatureStatus = 6,
                IsSentToAkuiteo = true,
                SentToAkuiteoAt = null,
                SignedMandateDocumentId = null,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "user@test.fr"
            });
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();
        var repository = new SepaMandateRepository(context);

        var result = await repository.GetLatestByAccountIdAsync(42);

        result.Should().NotBeNull();
        result!.RibDocumentId.Should().Be(100);
        result.SignatureRequestId.Should().BeNull();
        result.SignatureUrl.Should().BeNull();
        result.IsSentToAkuiteo.Should().BeTrue();
        result.SentToAkuiteoAt.Should().BeNull();
        result.SignedMandateDocumentId.Should().BeNull();
    }

    /// <summary>
    /// Verifies that updating the signature status updates the targeted mandate row.
    /// </summary>
    [Fact]
    public async Task UpdateSignatureStatusAsync_WhenMandateExists_UpdatesSignatureStatus()
    {
        await using var context = new MandateContext(CreateContextOptions());
        await context.Company.AddAsync(CreateCompany(42));
        var mandate = new SepaMandateDb
        {
            AccountId = 42,
            AccountHolder = "Jean Dupont",
            Address = "10 rue de Paris",
            Iban = "FR7630006000011234567890189",
            Bic = "AGRIFRPP",
            RibDocumentId = 99,
            SignatureStatus = 4,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "user@test.fr"
        };
        await context.SepaMandates.AddAsync(mandate);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();
        var repository = new SepaMandateRepository(context);

        await repository.UpdateSignatureStatusAsync(mandate.Id, 6);

        var persisted = await context.SepaMandates.SingleAsync(candidate => candidate.Id == mandate.Id);
        persisted.SignatureStatus.Should().Be(6);
    }

    /// <summary>
    /// Verifies that marking a mandate as sent updates the latest mandate for the account.
    /// </summary>
    [Fact]
    public async Task MarkSentToAkuiteoAsync_WhenMandateExists_UpdatesLatestMandate()
    {
        await using var context = new MandateContext(CreateContextOptions());
        await context.Company.AddAsync(CreateCompany(42));
        await context.SepaMandates.AddRangeAsync(
            CreateMandateRow(42, 99, "doc-older", 4, false, null),
            CreateMandateRow(42, 100, "doc-latest", 4, false, null));
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();
        var repository = new SepaMandateRepository(context);
        var sentAt = DateTime.UtcNow;

        var result = await repository.MarkSentToAkuiteoAsync(42, sentAt);

        result.Should().BeTrue();
        var mandates = await context.SepaMandates.OrderBy(candidate => candidate.Id).ToListAsync();
        mandates[0].IsSentToAkuiteo.Should().BeFalse();
        mandates[0].SentToAkuiteoAt.Should().BeNull();
        mandates[1].IsSentToAkuiteo.Should().BeTrue();
        mandates[1].SentToAkuiteoAt.Should().Be(sentAt);
    }

    /// <summary>
    /// Verifies that marking a mandate as sent returns false when no row exists.
    /// </summary>
    [Fact]
    public async Task MarkSentToAkuiteoAsync_WhenMandateDoesNotExist_ReturnsFalse()
    {
        await using var context = new MandateContext(CreateContextOptions());
        await context.Company.AddAsync(CreateCompany(42));
        await context.SaveChangesAsync();
        var repository = new SepaMandateRepository(context);

        var result = await repository.MarkSentToAkuiteoAsync(42, DateTime.UtcNow);

        result.Should().BeFalse();
    }

    /// <summary>
    /// Verifies that saving the signed mandate document identifier updates the latest mandate for the account.
    /// </summary>
    [Fact]
    public async Task SaveSignedMandateDocumentIdAsync_WhenMandateExists_UpdatesLatestMandate()
    {
        await using var context = new MandateContext(CreateContextOptions());
        await context.Company.AddAsync(CreateCompany(42));
        await context.SepaMandates.AddRangeAsync(
            CreateMandateRow(42, 99, "doc-older", 4, false, null),
            CreateMandateRow(42, 100, "doc-latest", 6, true, DateTime.UtcNow.AddMinutes(-5)));
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();
        var repository = new SepaMandateRepository(context);

        var result = await repository.SaveSignedMandateDocumentIdAsync(42, "456");

        result.Should().BeTrue();
        var mandates = await context.SepaMandates.OrderBy(candidate => candidate.Id).ToListAsync();
        mandates[0].SignedMandateDocumentId.Should().BeNull();
        mandates[1].SignedMandateDocumentId.Should().Be("456");
        mandates[1].IsSentToAkuiteo.Should().BeTrue();
        mandates[1].SignatureStatus.Should().Be(6);
    }

    /// <summary>
    /// Verifies that saving the signed mandate document identifier returns false when no row exists.
    /// </summary>
    [Fact]
    public async Task SaveSignedMandateDocumentIdAsync_WhenMandateDoesNotExist_ReturnsFalse()
    {
        await using var context = new MandateContext(CreateContextOptions());
        await context.Company.AddAsync(CreateCompany(42));
        await context.SaveChangesAsync();
        var repository = new SepaMandateRepository(context);

        var result = await repository.SaveSignedMandateDocumentIdAsync(42, "456");

        result.Should().BeFalse();
    }

    private DbContextOptions<MandateContext> CreateContextOptions()
    {
        return new DbContextOptionsBuilder<MandateContext>()
            .UseSqlServer(this._sqlServerFixture.ConnectionString + ";MultipleActiveResultSets=True")
            .Options;
    }

    private static CompanyDb CreateCompany(int accountId)
    {
        var company = EntityDbFactory.CompanyDb;
        company.Id = accountId;
        company.SiretNumber = accountId.ToString();
        company.ErpId = accountId.ToString();
        return company;
    }

    private static SepaMandateDb CreateMandateRow(
        int accountId,
        int ribDocumentId,
        string? signatureRequestId,
        int signatureStatus,
        bool isSentToAkuiteo,
        DateTime? sentToAkuiteoAt)
    {
        return new SepaMandateDb
        {
            AccountId = accountId,
            AccountHolder = "Jean Dupont",
            Address = "10 rue de Paris",
            Iban = "FR7630006000011234567890189",
            Bic = "AGRIFRPP",
            RibDocumentId = ribDocumentId,
            SignatureRequestId = signatureRequestId,
            SignatureUrl = signatureRequestId is null ? null : $"https://{signatureRequestId}.test",
            SignatureStatus = signatureStatus,
            IsSentToAkuiteo = isSentToAkuiteo,
            SentToAkuiteoAt = sentToAkuiteoAt,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "user@test.fr"
        };
    }
}
