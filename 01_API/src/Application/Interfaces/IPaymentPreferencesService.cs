// <copyright file="IPaymentPreferencesService.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Application.Interfaces;

using KPMG.Pulse.Back.Accounting.Mandate.Application.Models;

/// <summary>
/// Orchestrates prospect payment preference operations.
/// </summary>
public interface IPaymentPreferencesService
{
    /// <summary>
    /// Gets the current payment preference for an account.
    /// </summary>
    /// <param name="accountId">The account identifier.</param>
    /// <returns>The payment preference result.</returns>
    Task<PaymentPreferenceResult> GetAsync(int accountId);

    /// <summary>
    /// Sets the account payment preference to OTHER.
    /// </summary>
    /// <param name="accountId">The account identifier.</param>
    /// <param name="createdBy">The creator email.</param>
    /// <returns>True when the account exists and the preference was saved; otherwise false.</returns>
    Task<bool> SetOtherAsync(int accountId, string createdBy);

    /// <summary>
    /// Generates a SEPA mandate, sends it for signature, and sets the account payment preference to MANDATE_SEPA.
    /// </summary>
    /// <param name="accountId">The account identifier.</param>
    /// <param name="command">The SEPA mandate command.</param>
    /// <returns>The SEPA payment preference result.</returns>
    Task<SepaPaymentPreferenceResult> SetSepaAsync(
        int accountId,
        SepaPaymentPreferenceCommand command);

    /// <summary>
    /// Resets the account payment preference to an unselected state.
    /// </summary>
    /// <param name="accountId">The account identifier.</param>
    /// <returns>True when the account and payment preference exist and the preference was reset; otherwise false.</returns>
    Task<bool> ResetAsync(int accountId);
}
