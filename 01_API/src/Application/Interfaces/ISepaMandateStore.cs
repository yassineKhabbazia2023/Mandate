// <copyright file="ISepaMandateStore.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Application.Interfaces;

/// <summary>
/// Provides SEPA mandate persistence operations required by the onboarding payment preference flow.
/// </summary>
public interface ISepaMandateStore
{
    /// <summary>
    /// Gets the latest SEPA mandate for an account.
    /// </summary>
    /// <param name="accountId">The account identifier.</param>
    /// <returns>The latest SEPA mandate, or <c>null</c> when none exists.</returns>
    Task<SepaMandate?> GetLatestByAccountIdAsync(int accountId);

    /// <summary>
    /// Saves a SEPA mandate and the associated payment preference in the same persistence operation.
    /// </summary>
    /// <param name="sepaMandate">The SEPA mandate to save.</param>
    /// <param name="paymentPreference">The payment preference to save with the mandate.</param>
    /// <returns>A task that represents the asynchronous save operation.</returns>
    Task SaveWithPaymentPreferenceAsync(SepaMandate sepaMandate, PaymentPreference paymentPreference);

    /// <summary>
    /// Updates the synchronized signature status for a SEPA mandate.
    /// </summary>
    /// <param name="sepaMandateId">The SEPA mandate identifier.</param>
    /// <param name="signatureStatus">The synchronized signature status.</param>
    /// <returns>A task that represents the asynchronous update operation.</returns>
    Task UpdateSignatureStatusAsync(int sepaMandateId, SepaMandateSignatureStatus signatureStatus);

    /// <summary>
    /// Marks the latest SEPA mandate for an account as sent to Akuiteo.
    /// </summary>
    /// <param name="accountId">The account identifier.</param>
    /// <param name="sentAt">The date when the mandate was sent to Akuiteo.</param>
    /// <returns><c>true</c> when a mandate was updated; otherwise, <c>false</c>.</returns>
    Task<bool> MarkSentToAkuiteoAsync(int accountId, DateTime sentAt);

    /// <summary>
    /// Saves the uploaded signed mandate document identifier on the latest SEPA mandate for an account.
    /// </summary>
    /// <param name="accountId">The account identifier.</param>
    /// <param name="signedMandateDocumentId">The uploaded signed mandate Prospect document identifier.</param>
    /// <returns><c>true</c> when a mandate was updated; otherwise, <c>false</c>.</returns>
    Task<bool> SaveSignedMandateDocumentIdAsync(int accountId, string signedMandateDocumentId);
}
