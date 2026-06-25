// <copyright file="ISepaMandateRepository.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Sql;

/// <summary>
/// Provides persistence operations for SEPA mandates.
/// </summary>
public interface ISepaMandateRepository
{
    /// <summary>
    /// Gets the latest SEPA mandate row for an account.
    /// </summary>
    /// <param name="accountId">The account identifier.</param>
    /// <returns>The latest SEPA mandate row, or <c>null</c> when none exists.</returns>
    Task<SepaMandateDb?> GetLatestByAccountIdAsync(int accountId);

    /// <summary>
    /// Saves a SEPA mandate and its payment preference in a single transaction.
    /// </summary>
    /// <param name="sepaMandate">The SEPA mandate row.</param>
    /// <param name="paymentPreference">The payment preference row.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SaveWithPaymentPreferenceAsync(
        SepaMandateDb sepaMandate,
        PaymentPreferenceDb paymentPreference);

    /// <summary>
    /// Updates the signature status for a SEPA mandate.
    /// </summary>
    /// <param name="sepaMandateId">The SEPA mandate identifier.</param>
    /// <param name="signatureStatus">The signature status.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UpdateSignatureStatusAsync(int sepaMandateId, int signatureStatus);

    /// <summary>
    /// Marks the latest SEPA mandate for an account as sent to Akuiteo.
    /// </summary>
    /// <param name="accountId">The account identifier.</param>
    /// <param name="sentAt">The sent date.</param>
    /// <returns><c>true</c> when a row was updated; otherwise, <c>false</c>.</returns>
    Task<bool> MarkSentToAkuiteoAsync(int accountId, DateTime sentAt);

    /// <summary>
    /// Saves the uploaded signed mandate document identifier on the latest SEPA mandate for an account.
    /// </summary>
    /// <param name="accountId">The account identifier.</param>
    /// <param name="signedMandateDocumentId">The uploaded signed mandate Prospect document identifier.</param>
    /// <returns><c>true</c> when a row was updated; otherwise, <c>false</c>.</returns>
    Task<bool> SaveSignedMandateDocumentIdAsync(int accountId, string signedMandateDocumentId);
}
