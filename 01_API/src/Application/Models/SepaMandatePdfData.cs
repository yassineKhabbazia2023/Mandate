// <copyright file="SepaMandatePdfData.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Application.Models;

/// <summary>
/// Carries bank information used to fill the SEPA mandate PDF template.
/// </summary>
/// <param name="AccountHolder">The account holder.</param>
/// <param name="AccountNumber">The account number.</param>
/// <param name="Address">The account holder address.</param>
/// <param name="AddressLine2">The complementary account holder address line.</param>
/// <param name="City">The account holder city.</param>
/// <param name="Country">The account holder country.</param>
/// <param name="PostalCode">The account holder postal code.</param>
/// <param name="Iban">The IBAN.</param>
/// <param name="Bic">The BIC.</param>
public sealed record SepaMandatePdfData(
    string AccountHolder,
    string AccountNumber,
    string Address,
    string? AddressLine2,
    string? City,
    string? Country,
    string? PostalCode,
    string Iban,
    string Bic);
