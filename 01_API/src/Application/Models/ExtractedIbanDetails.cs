// <copyright file="ExtractedIbanDetails.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Application.Models;

/// <summary>
/// Represents details extracted from an IBAN.
/// </summary>
/// <param name="CountryCode">The ISO country code.</param>
/// <param name="CheckDigits">The IBAN check digits.</param>
/// <param name="BankAccountPart">The bank-account portion of the IBAN.</param>
public sealed record ExtractedIbanDetails(
    string CountryCode,
    string CheckDigits,
    string BankAccountPart);
