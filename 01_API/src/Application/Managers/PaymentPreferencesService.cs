// <copyright file="PaymentPreferencesService.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Application.Managers;

using KPMG.Pulse.Back.Accounting.Mandate.Application.Interfaces;
using Microsoft.Extensions.Logging;

/// <inheritdoc />
public sealed class PaymentPreferencesService(
    IDatabaseService databaseService,
    IEnumerable<IPaymentPreferenceStrategy> strategies,
    ILogger<PaymentPreferencesService> logger) : IPaymentPreferencesService
{
    /// <inheritdoc />
    public async Task<PaymentPreferenceResult> GetAsync(int accountId)
    {
        if (!await databaseService.AccountExistsAsync(accountId))
        {
            logger.LogWarning("Payment preference requested for unknown account {AccountId}", accountId);
            return new PaymentPreferenceResult(false, null);
        }

        var preference = await databaseService.GetPaymentPreferenceByAccountIdAsync(accountId);
        return new PaymentPreferenceResult(true, MapPaymentType(preference?.PaymentType));
    }

    /// <inheritdoc />
    public async Task<bool> SetOtherAsync(int accountId, string createdBy)
    {
        if (!await databaseService.AccountExistsAsync(accountId))
        {
            logger.LogWarning("Payment preference OTHER requested for unknown account {AccountId}", accountId);
            return false;
        }

        var strategy = ResolveStrategy(PaymentPreferenceType.Other);
        var preference = await databaseService.GetPaymentPreferenceByAccountIdAsync(accountId)
            ?? new PaymentPreference(0, accountId, null, DateTime.UtcNow, createdBy);

        await strategy.ApplyAsync(preference);
        await databaseService.SavePaymentPreferenceAsync(preference);
        return true;
    }

    /// <inheritdoc />
    public async Task<bool> ResetAsync(int accountId)
    {
        if (!await databaseService.AccountExistsAsync(accountId))
        {
            logger.LogWarning("Payment preference reset requested for unknown account {AccountId}", accountId);
            return false;
        }

        var preference = await databaseService.GetPaymentPreferenceByAccountIdAsync(accountId);
        if (preference is null)
        {
            logger.LogWarning("Payment preference reset requested for account {AccountId} without preference row", accountId);
            return false;
        }

        preference.ResetPaymentType();
        await databaseService.SavePaymentPreferenceAsync(preference);
        return true;
    }

    private IPaymentPreferenceStrategy ResolveStrategy(PaymentPreferenceType paymentType)
    {
        return strategies.Single(strategy => strategy.PaymentType == paymentType);
    }

    private static PaymentPreferenceType? MapPaymentType(int? paymentType)
    {
        return paymentType switch
        {
            (int)PaymentPreferenceType.MandateSepa => PaymentPreferenceType.MandateSepa,
            (int)PaymentPreferenceType.Other => PaymentPreferenceType.Other,
            _ => null
        };
    }
}
