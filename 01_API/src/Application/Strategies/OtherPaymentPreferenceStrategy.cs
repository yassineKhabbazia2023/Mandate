// <copyright file="OtherPaymentPreferenceStrategy.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Application.Strategies;

using KPMG.Pulse.Back.Accounting.Mandate.Application.Interfaces;

/// <summary>
/// Applies the OTHER payment preference.
/// </summary>
public sealed class OtherPaymentPreferenceStrategy : IPaymentPreferenceStrategy
{
    /// <inheritdoc />
    public PaymentPreferenceType PaymentType => PaymentPreferenceType.Other;

    /// <inheritdoc />
    public Task ApplyAsync(PaymentPreference preference)
    {
        ArgumentNullException.ThrowIfNull(preference);

        preference.SetPaymentType((int)PaymentPreferenceType.Other);
        return Task.CompletedTask;
    }
}
