// <copyright file="DefaultPaymentPreferenceReadStrategy.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Application.Strategies;

using KPMG.Pulse.Back.Accounting.Mandate.Application.Interfaces;

/// <summary>
/// Handles the default GET payment preference behavior.
/// </summary>
public sealed class DefaultPaymentPreferenceReadStrategy : IPaymentPreferenceReadStrategy
{
    /// <inheritdoc />
    public bool Supports(PaymentPreferenceType? paymentType)
    {
        return paymentType != PaymentPreferenceType.MandateSepa;
    }

    /// <inheritdoc />
    public Task<PaymentPreferenceResult> GetAsync(int accountId, PaymentPreference? preference)
    {
        return Task.FromResult(new PaymentPreferenceResult(
            true,
            MapPaymentType(preference?.PaymentType)));
    }

    /// <summary>
    /// Maps the stored payment type to the application enum.
    /// </summary>
    /// <param name="paymentType">The stored payment type.</param>
    /// <returns>The application payment type.</returns>
    private static PaymentPreferenceType? MapPaymentType(int? paymentType)
    {
        return paymentType switch
        {
            (int)PaymentPreferenceType.MandateSepa => PaymentPreferenceType.MandateSepa,
            (int)PaymentPreferenceType.Other => PaymentPreferenceType.Other,
            _ => null
        };
    }
}
