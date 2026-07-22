// <copyright file="BankDetailsExtractionRequest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.AspNetCore.Models;

using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

/// <summary>
/// Represents banking identifiers to extract.
/// </summary>
public sealed class BankDetailsExtractionRequest
{
    /// <summary>
    /// Gets or sets the French IBAN.
    /// </summary>
    [Required]
    [JsonProperty("iban")]
    public string Iban { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the BIC associated with the IBAN.
    /// </summary>
    [Required]
    [JsonProperty("bic")]
    public string Bic { get; set; } = string.Empty;
}
