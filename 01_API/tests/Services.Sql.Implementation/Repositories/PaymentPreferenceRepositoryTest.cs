// <copyright file="PaymentPreferenceRepositoryTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Sql.Implementation.Tests.Repositories;

using KPMG.Pulse.Back.Accounting.Mandate.Sql;
using KPMG.Pulse.Back.Accounting.Mandate.Sql.Implementation.Tests;
using KPMG.Pulse.Back.Accounting.Mandate.Sql.Implementation.Tests.Tools;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Integration tests for <see cref="PaymentPreferenceRepository"/>.
/// </summary>
public sealed class PaymentPreferenceRepositoryTest : SqlServerTestBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PaymentPreferenceRepositoryTest"/> class.
    /// </summary>
    /// <param name="sqlServerFixture">The SQL Server fixture.</param>
    public PaymentPreferenceRepositoryTest(SqlServerFixture sqlServerFixture)
        : base(sqlServerFixture)
    {
    }

    /// <summary>
    /// Verifies that account existence is checked against Mandate companies.
    /// </summary>
    [Fact]
    public async Task AccountExistsAsync_WhenCompanyExists_ReturnsTrue()
    {
        var accountId = 6201101;
        await using var context = new MandateContext(CreateContextOptions());
        await context.Company.AddAsync(CreateCompany(accountId));
        await context.SaveChangesAsync();
        var repository = new PaymentPreferenceRepository(context);

        var result = await repository.AccountExistsAsync(accountId);

        result.Should().BeTrue();
    }

    /// <summary>
    /// Verifies that a missing account returns false.
    /// </summary>
    [Fact]
    public async Task AccountExistsAsync_WhenCompanyDoesNotExist_ReturnsFalse()
    {
        await using var context = new MandateContext(CreateContextOptions());
        var repository = new PaymentPreferenceRepository(context);

        var result = await repository.AccountExistsAsync(6201199);

        result.Should().BeFalse();
    }

    /// <summary>
    /// Verifies that getting a preference returns the latest row without tracking it.
    /// </summary>
    [Fact]
    public async Task GetByAccountIdAsync_WhenPreferencesExist_ReturnsLatestPreferenceWithoutTracking()
    {
        var accountId = 6201102;
        await using var context = new MandateContext(CreateContextOptions());
        await context.Company.AddAsync(CreateCompany(accountId));
        await context.PaymentPreferences.AddAsync(new PaymentPreferenceDb
        {
            AccountId = accountId,
            PaymentType = 1,
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            CreatedBy = "old@test.fr"
        });
        await context.PaymentPreferences.AddAsync(new PaymentPreferenceDb
        {
            AccountId = accountId,
            PaymentType = 2,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "new@test.fr"
        });
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();
        var repository = new PaymentPreferenceRepository(context);

        var result = await repository.GetByAccountIdAsync(accountId);

        result.Should().NotBeNull();
        result!.PaymentType.Should().Be(2);
        context.ChangeTracker.Entries<PaymentPreferenceDb>().Should().BeEmpty();
    }

    /// <summary>
    /// Verifies that saving a new preference inserts it.
    /// </summary>
    [Fact]
    public async Task SaveAsync_WhenPreferenceIsNew_InsertsPreference()
    {
        var accountId = 6201103;
        await using var context = new MandateContext(CreateContextOptions());
        await context.Company.AddAsync(CreateCompany(accountId));
        await context.SaveChangesAsync();
        var repository = new PaymentPreferenceRepository(context);

        await repository.SaveAsync(new PaymentPreferenceDb
        {
            AccountId = accountId,
            PaymentType = 2,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "user@test.fr"
        });

        var saved = await context.PaymentPreferences.SingleAsync(candidate => candidate.AccountId == accountId);
        saved.PaymentType.Should().Be(2);
    }

    /// <summary>
    /// Verifies that saving an existing detached preference updates it.
    /// </summary>
    [Fact]
    public async Task SaveAsync_WhenPreferenceExists_UpdatesPreference()
    {
        var accountId = 6201104;
        await using var context = new MandateContext(CreateContextOptions());
        await context.Company.AddAsync(CreateCompany(accountId));
        var preference = new PaymentPreferenceDb
        {
            AccountId = accountId,
            PaymentType = 2,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "user@test.fr"
        };
        await context.PaymentPreferences.AddAsync(preference);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();
        var repository = new PaymentPreferenceRepository(context);

        await repository.SaveAsync(new PaymentPreferenceDb
        {
            Id = preference.Id,
            AccountId = accountId,
            PaymentType = null,
            CreatedAt = preference.CreatedAt,
            CreatedBy = preference.CreatedBy
        });

        var saved = await context.PaymentPreferences.SingleAsync(candidate => candidate.AccountId == accountId);
        saved.PaymentType.Should().BeNull();
    }

    /// <summary>
    /// Verifies that saving a null preference throws.
    /// </summary>
    [Fact]
    public async Task SaveAsync_WhenPreferenceIsNull_Throws()
    {
        await using var context = new MandateContext(CreateContextOptions());
        var repository = new PaymentPreferenceRepository(context);

        Func<Task> act = async () => await repository.SaveAsync(null!);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    /// <summary>
    /// Verifies that onboarding cleanup deletes only payment preferences and SEPA mandates for the requested account.
    /// </summary>
    [Fact]
    public async Task CleanupOnboardingDataAsync_WhenRowsExist_DeletesOnlyRequestedAccountRows()
    {
        var accountId = 6201105;
        var otherAccountId = 6201106;
        await using var context = new MandateContext(CreateContextOptions());
        await context.Company.AddRangeAsync(CreateCompany(accountId), CreateCompany(otherAccountId));
        await context.PaymentPreferences.AddRangeAsync(
            CreatePaymentPreference(accountId),
            CreatePaymentPreference(otherAccountId));
        await context.SepaMandates.AddRangeAsync(
            CreateSepaMandate(accountId),
            CreateSepaMandate(otherAccountId));
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();
        var repository = new PaymentPreferenceRepository(context);

        var deletedRows = await repository.CleanupOnboardingDataAsync(accountId);

        deletedRows.Should().Be(2);
        context.PaymentPreferences.Should().ContainSingle(preference => preference.AccountId == otherAccountId);
        context.PaymentPreferences.Should().NotContain(preference => preference.AccountId == accountId);
        context.SepaMandates.Should().ContainSingle(mandate => mandate.AccountId == otherAccountId);
        context.SepaMandates.Should().NotContain(mandate => mandate.AccountId == accountId);
    }

    /// <summary>
    /// Verifies that onboarding cleanup is idempotent when no rows exist for the account.
    /// </summary>
    [Fact]
    public async Task CleanupOnboardingDataAsync_WhenNoRowsExist_ReturnsZero()
    {
        await using var context = new MandateContext(CreateContextOptions());
        var repository = new PaymentPreferenceRepository(context);

        var deletedRows = await repository.CleanupOnboardingDataAsync(6201199);

        deletedRows.Should().Be(0);
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

    /// <summary>
    /// Creates a payment preference row for tests.
    /// </summary>
    /// <param name="accountId">The account identifier.</param>
    /// <returns>The payment preference row.</returns>
    private static PaymentPreferenceDb CreatePaymentPreference(int accountId)
    {
        return new PaymentPreferenceDb
        {
            AccountId = accountId,
            PaymentType = 2,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "user@test.fr"
        };
    }

    /// <summary>
    /// Creates a SEPA mandate row for tests.
    /// </summary>
    /// <param name="accountId">The account identifier.</param>
    /// <returns>The SEPA mandate row.</returns>
    private static SepaMandateDb CreateSepaMandate(int accountId)
    {
        return new SepaMandateDb
        {
            AccountId = accountId,
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
        };
    }
}
