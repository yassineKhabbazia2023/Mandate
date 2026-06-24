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
}
