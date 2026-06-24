// <copyright file="IPaymentPreferenceRepository.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Sql;

/// <summary>
/// Provides persistence operations for prospect payment preferences.
/// </summary>
public interface IPaymentPreferenceRepository
{
    /// <summary>
    /// Gets whether the account exists.
    /// </summary>
    /// <param name="accountId">The account identifier.</param>
    /// <returns>True when the account exists; otherwise false.</returns>
    Task<bool> AccountExistsAsync(int accountId);

    /// <summary>
    /// Gets the account number for the account.
    /// </summary>
    /// <param name="accountId">The account identifier.</param>
    /// <returns>The account number, or null when it is not configured.</returns>
    Task<string?> GetAccountNumberAsync(int accountId);

    /// <summary>
    /// Gets the latest payment preference for an account.
    /// </summary>
    /// <param name="accountId">The account identifier.</param>
    /// <returns>The latest preference, or null when none exists.</returns>
    Task<PaymentPreferenceDb?> GetByAccountIdAsync(int accountId);

    /// <summary>
    /// Saves the payment preference.
    /// </summary>
    /// <param name="preference">The payment preference to save.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SaveAsync(PaymentPreferenceDb preference);
}
