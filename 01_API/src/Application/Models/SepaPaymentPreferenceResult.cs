// <copyright file="SepaPaymentPreferenceResult.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Application.Models;

/// <summary>
/// Represents the result of a SEPA payment preference operation.
/// </summary>
/// <param name="AccountFound">A value indicating whether the account was found.</param>
/// <param name="SignatureUrl">The signature URL when the mandate was generated.</param>
public sealed record SepaPaymentPreferenceResult(
    bool AccountFound,
    string? SignatureUrl);
