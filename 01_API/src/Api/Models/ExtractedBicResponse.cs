// <copyright file="ExtractedBicResponse.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.AspNetCore.Models;

using Newtonsoft.Json;

/// <summary>
/// Represents extracted BIC information.
/// </summary>
public sealed class ExtractedBicResponse
{
    /// <summary>
    /// Gets or sets the bank code.
    /// </summary>
    [JsonProperty("bankCode")]
    public string BankCode { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the ISO country code.
    /// </summary>
    [JsonProperty("countryCode")]
    public string CountryCode { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the location code.
    /// </summary>
    [JsonProperty("locationCode")]
    public string LocationCode { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the optional branch code.
    /// </summary>
    [JsonProperty("branchCode")]
    public string? BranchCode { get; set; }
}
