// <copyright file="BankDetailsExtractionResponse.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.AspNetCore.Models;

using Newtonsoft.Json;

/// <summary>
/// Represents banking information extracted from an IBAN and a BIC.
/// </summary>
public sealed class BankDetailsExtractionResponse
{
    /// <summary>
    /// Gets or sets the extracted IBAN information.
    /// </summary>
    [JsonProperty("iban")]
    public ExtractedIbanResponse Iban { get; set; } = null!;

    /// <summary>
    /// Gets or sets the extracted RIB information.
    /// </summary>
    [JsonProperty("rib")]
    public ExtractedRibResponse Rib { get; set; } = null!;

    /// <summary>
    /// Gets or sets the extracted BIC information.
    /// </summary>
    [JsonProperty("bic")]
    public ExtractedBicResponse Bic { get; set; } = null!;

    /// <summary>
    /// Gets or sets the four-character BIC bank code used as the domiciliation.
    /// </summary>
    [JsonProperty("domiciliation")]
    public string Domiciliation { get; set; } = string.Empty;
}
