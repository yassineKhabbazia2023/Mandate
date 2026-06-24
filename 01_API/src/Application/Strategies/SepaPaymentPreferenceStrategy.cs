// <copyright file="SepaPaymentPreferenceStrategy.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Application.Strategies;

using KPMG.Pulse.Back.Accounting.Mandate.Application.Interfaces;

/// <summary>
/// Applies the MANDATE_SEPA payment preference.
/// </summary>
public sealed class SepaPaymentPreferenceStrategy : IPaymentPreferenceStrategy
{
    /// <inheritdoc />
    public PaymentPreferenceType PaymentType => PaymentPreferenceType.MandateSepa;

    /// <inheritdoc />
    public Task ApplyAsync(PaymentPreference preference)
    {
        ArgumentNullException.ThrowIfNull(preference);

        preference.SetPaymentType((int)PaymentPreferenceType.MandateSepa);
        return Task.CompletedTask;
    }
}
