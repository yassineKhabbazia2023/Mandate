// <copyright file="IPaymentPreferenceStore.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Application.Interfaces;

/// <summary>
/// Provides payment preference persistence operations required by the onboarding payment preference flow.
/// </summary>
public interface IPaymentPreferenceStore
{
    /// <summary>
    /// Checks whether the account exists.
    /// </summary>
    /// <param name="accountId">The account identifier.</param>
    /// <returns><c>true</c> when the account exists; otherwise, <c>false</c>.</returns>
    Task<bool> AccountExistsAsync(int accountId);

    /// <summary>
    /// Gets the account number used by mandate documents.
    /// </summary>
    /// <param name="accountId">The account identifier.</param>
    /// <returns>The account number, or <c>null</c> when it is not configured.</returns>
    Task<string?> GetAccountNumberAsync(int accountId);

    /// <summary>
    /// Gets the payment preference for an account.
    /// </summary>
    /// <param name="accountId">The account identifier.</param>
    /// <returns>The payment preference, or <c>null</c> when no preference exists.</returns>
    Task<PaymentPreference?> GetByAccountIdAsync(int accountId);

    /// <summary>
    /// Saves a payment preference.
    /// </summary>
    /// <param name="paymentPreference">The payment preference to save.</param>
    /// <returns>A task that represents the asynchronous save operation.</returns>
    Task SaveAsync(PaymentPreference paymentPreference);
}
