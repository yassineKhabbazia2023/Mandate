// <copyright file="ExtractedBankDetails.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Application.Models;

/// <summary>
/// Represents all banking details extracted from an IBAN and a BIC.
/// </summary>
/// <param name="Iban">The extracted IBAN details.</param>
/// <param name="Rib">The extracted RIB details.</param>
/// <param name="Bic">The extracted BIC details.</param>
/// <param name="Domiciliation">The four-character BIC bank code used as the domiciliation.</param>
public sealed record ExtractedBankDetails(
    ExtractedIbanDetails Iban,
    ExtractedRibDetails Rib,
    ExtractedBicDetails Bic,
    string Domiciliation);
