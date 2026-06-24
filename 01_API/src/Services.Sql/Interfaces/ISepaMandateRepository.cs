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
    /// Saves a SEPA mandate and its payment preference in a single transaction.
    /// </summary>
    /// <param name="sepaMandate">The SEPA mandate row.</param>
    /// <param name="paymentPreference">The payment preference row.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SaveWithPaymentPreferenceAsync(
        SepaMandateDb sepaMandate,
        PaymentPreferenceDb paymentPreference);
}
