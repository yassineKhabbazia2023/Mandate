// <copyright file="BankDetailsExtractionMapper.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.AspNetCore.Mappers;

using KPMG.Pulse.Back.Accounting.Mandate.Application.Models;
using KPMG.Pulse.Back.Accounting.Mandate.AspNetCore.Models;

/// <summary>
/// Maps extracted application banking details to the API response contract.
/// </summary>
public static class BankDetailsExtractionMapper
{
    /// <summary>
    /// Maps extracted banking details to the API response contract.
    /// </summary>
    /// <param name="details">The extracted banking details.</param>
    /// <returns>The API response.</returns>
    public static BankDetailsExtractionResponse ToResponse(ExtractedBankDetails details)
    {
        ArgumentNullException.ThrowIfNull(details);

        return new BankDetailsExtractionResponse
        {
            Iban = new ExtractedIbanResponse
            {
                CountryCode = details.Iban.CountryCode,
                CheckDigits = details.Iban.CheckDigits,
                BankAccountPart = details.Iban.BankAccountPart
            },
            Rib = new ExtractedRibResponse
            {
                BankCode = details.Rib.BankCode,
                BranchCode = details.Rib.BranchCode,
                AccountNumber = details.Rib.AccountNumber,
                RibKey = details.Rib.RibKey
            },
            Bic = new ExtractedBicResponse
            {
                BankCode = details.Bic.BankCode,
                CountryCode = details.Bic.CountryCode,
                LocationCode = details.Bic.LocationCode,
                BranchCode = details.Bic.BranchCode
            },
            Domiciliation = details.Domiciliation
        };
    }
}
