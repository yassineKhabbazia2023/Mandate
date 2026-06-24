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
}
