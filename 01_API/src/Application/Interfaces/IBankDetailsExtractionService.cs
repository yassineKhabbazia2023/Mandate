// <copyright file="IBankDetailsExtractionService.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Application.Interfaces;

using KPMG.Pulse.Back.Accounting.Mandate.Application.Models;

/// <summary>
/// Extracts normalized banking details from an IBAN and a BIC.
/// </summary>
public interface IBankDetailsExtractionService
{
    /// <summary>
    /// Extracts banking details after validating the supplied identifiers.
    /// </summary>
    /// <param name="iban">The French IBAN to extract.</param>
    /// <param name="bic">The BIC associated with the IBAN.</param>
    /// <returns>The extraction result.</returns>
    BankDetailsExtractionResult Extract(string iban, string bic);
}
