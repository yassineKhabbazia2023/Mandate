// <copyright file="PaymentPreferenceRepository.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Sql.Implementation;

using KPMG.Pulse.Back.Accounting.Mandate.Sql;
using Microsoft.EntityFrameworkCore;

/// <inheritdoc />
public class PaymentPreferenceRepository(MandateContext context) : IPaymentPreferenceRepository
{
    /// <inheritdoc />
    public async Task<bool> AccountExistsAsync(int accountId)
    {
        return await context.Company
            .AsNoTracking()
            .AnyAsync(account => account.Id == accountId);
    }

    /// <inheritdoc />
    public async Task<string?> GetAccountNumberAsync(int accountId)
    {
        return await context.Company
            .AsNoTracking()
            .Where(account => account.Id == accountId)
            .Select(account => account.ErpId)
            .FirstOrDefaultAsync();
    }

    /// <inheritdoc />
    public async Task<PaymentPreferenceDb?> GetByAccountIdAsync(int accountId)
    {
        return await context.PaymentPreferences
            .AsNoTracking()
            .Where(preference => preference.AccountId == accountId)
            .OrderByDescending(preference => preference.Id)
            .FirstOrDefaultAsync();
    }

    /// <inheritdoc />
    public async Task SaveAsync(PaymentPreferenceDb preference)
    {
        ArgumentNullException.ThrowIfNull(preference);

        if (preference.Id == 0)
        {
            await context.PaymentPreferences.AddAsync(preference);
        }
        else
        {
            context.PaymentPreferences.Update(preference);
        }

        await context.SaveChangesAsync();
    }
}
