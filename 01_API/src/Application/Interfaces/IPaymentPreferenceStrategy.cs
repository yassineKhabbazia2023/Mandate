// <copyright file="IPaymentPreferenceStrategy.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Application.Interfaces;

/// <summary>
/// Handles persistence behavior for one payment preference type.
/// </summary>
public interface IPaymentPreferenceStrategy
{
    /// <summary>
    /// Gets the payment preference type handled by this strategy.
    /// </summary>
    PaymentPreferenceType PaymentType { get; }

    /// <summary>
    /// Applies the payment preference to the provided stored preference.
    /// </summary>
    /// <param name="preference">The preference to update.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task ApplyAsync(PaymentPreference preference);
}
