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
    /// Saves a SEPA mandate and the associated payment preference in the same persistence operation.
    /// </summary>
    /// <param name="sepaMandate">The SEPA mandate to save.</param>
    /// <param name="paymentPreference">The payment preference to save with the mandate.</param>
    /// <returns>A task that represents the asynchronous save operation.</returns>
    Task SaveWithPaymentPreferenceAsync(SepaMandate sepaMandate, PaymentPreference paymentPreference);
}
