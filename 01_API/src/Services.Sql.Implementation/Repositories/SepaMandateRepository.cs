// <copyright file="SepaMandateRepository.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Sql.Implementation;

using KPMG.Pulse.Back.Accounting.Mandate.Sql;
using Microsoft.EntityFrameworkCore;

/// <inheritdoc />
public sealed class SepaMandateRepository(MandateContext context) : ISepaMandateRepository
{
    /// <inheritdoc />
    public async Task<SepaMandateDb?> GetLatestByAccountIdAsync(int accountId)
    {
        return await context.SepaMandates
            .AsNoTracking()
            .Where(mandate => mandate.AccountId == accountId)
            .OrderByDescending(mandate => mandate.Id)
            .FirstOrDefaultAsync();
    }

    /// <inheritdoc />
    public async Task SaveWithPaymentPreferenceAsync(
        SepaMandateDb sepaMandate,
        PaymentPreferenceDb paymentPreference)
    {
        ArgumentNullException.ThrowIfNull(sepaMandate);
        ArgumentNullException.ThrowIfNull(paymentPreference);

        var strategy = context.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await context.Database.BeginTransactionAsync();

            if (paymentPreference.Id == 0)
            {
                await context.PaymentPreferences.AddAsync(paymentPreference);
            }
            else
            {
                context.PaymentPreferences.Update(paymentPreference);
            }

            await context.SepaMandates.AddAsync(sepaMandate);
            await context.SaveChangesAsync();
            await transaction.CommitAsync();
        });
    }

    /// <inheritdoc />
    public async Task UpdateSignatureStatusAsync(int sepaMandateId, int signatureStatus)
    {
        await context.SepaMandates
            .Where(mandate => mandate.Id == sepaMandateId)
            .ExecuteUpdateAsync(setters => setters.SetProperty(
                mandate => mandate.SignatureStatus,
                signatureStatus));
    }

    /// <inheritdoc />
    public async Task<bool> MarkSentToAkuiteoAsync(int accountId, DateTime sentAt)
    {
        var mandateId = await context.SepaMandates
            .AsNoTracking()
            .Where(mandate => mandate.AccountId == accountId)
            .OrderByDescending(mandate => mandate.Id)
            .Select(mandate => (int?)mandate.Id)
            .FirstOrDefaultAsync();

        if (!mandateId.HasValue)
        {
            return false;
        }

        var updatedRows = await context.SepaMandates
            .Where(mandate => mandate.Id == mandateId.Value)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(mandate => mandate.IsSentToAkuiteo, true)
                .SetProperty(mandate => mandate.SentToAkuiteoAt, sentAt));

        return updatedRows > 0;
    }

    /// <inheritdoc />
    public async Task<bool> SaveSignedMandateDocumentIdAsync(int accountId, string signedMandateDocumentId)
    {
        var mandateId = await context.SepaMandates
            .AsNoTracking()
            .Where(mandate => mandate.AccountId == accountId)
            .OrderByDescending(mandate => mandate.Id)
            .Select(mandate => (int?)mandate.Id)
            .FirstOrDefaultAsync();

        if (!mandateId.HasValue)
        {
            return false;
        }

        var updatedRows = await context.SepaMandates
            .Where(mandate => mandate.Id == mandateId.Value)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(mandate => mandate.SignedMandateDocumentId, signedMandateDocumentId));

        return updatedRows > 0;
    }
}
