// <copyright file="PaymentPreferenceResult.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Application;

/// <summary>
/// Represents the result of reading a payment preference.
/// </summary>
/// <param name="AccountFound">A value indicating whether the account exists.</param>
/// <param name="PaymentType">The selected payment type, or null when not selected yet.</param>
public sealed record PaymentPreferenceResult(bool AccountFound, PaymentPreferenceType? PaymentType);
