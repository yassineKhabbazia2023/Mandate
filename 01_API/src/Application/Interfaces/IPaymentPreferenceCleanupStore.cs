// <copyright file="IPaymentPreferenceCleanupStore.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Application.Interfaces;

/// <summary>
/// Provides cleanup persistence operations for onboarding payment preferences.
/// </summary>
public interface IPaymentPreferenceCleanupStore
{
    /// <summary>
    /// Deletes onboarding payment preferences and SEPA mandates for an account.
    /// </summary>
    /// <param name="accountId">The account identifier.</param>
    /// <returns>The number of deleted rows.</returns>
    Task<int> CleanupAsync(int accountId);
}
