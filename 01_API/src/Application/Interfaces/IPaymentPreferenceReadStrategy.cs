// <copyright file="IPaymentPreferenceReadStrategy.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Application.Interfaces;

/// <summary>
/// Handles GET payment preference behavior for one read scenario.
/// </summary>
public interface IPaymentPreferenceReadStrategy
{
    /// <summary>
    /// Determines whether the strategy supports the provided payment preference type.
    /// </summary>
    /// <param name="paymentType">The persisted payment preference type.</param>
    /// <returns><c>true</c> when the strategy supports the payment type; otherwise, <c>false</c>.</returns>
    bool Supports(PaymentPreferenceType? paymentType);

    /// <summary>
    /// Gets the payment preference response for an existing account.
    /// </summary>
    /// <param name="accountId">The account identifier.</param>
    /// <param name="preference">The persisted payment preference.</param>
    /// <returns>The payment preference result.</returns>
    Task<PaymentPreferenceResult> GetAsync(int accountId, PaymentPreference? preference);
}
