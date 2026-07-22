// <copyright file="ExtractedIbanResponse.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.AspNetCore.Models;

using Newtonsoft.Json;

/// <summary>
/// Represents extracted IBAN information.
/// </summary>
public sealed class ExtractedIbanResponse
{
    /// <summary>
    /// Gets or sets the ISO country code.
    /// </summary>
    [JsonProperty("countryCode")]
    public string CountryCode { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the IBAN check digits.
    /// </summary>
    [JsonProperty("checkDigits")]
    public string CheckDigits { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the bank-account portion of the IBAN.
    /// </summary>
    [JsonProperty("bankAccountPart")]
    public string BankAccountPart { get; set; } = string.Empty;
}
