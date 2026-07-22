// <copyright file="ExtractedRibResponse.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.AspNetCore.Models;

using Newtonsoft.Json;

/// <summary>
/// Represents extracted French RIB information.
/// </summary>
public sealed class ExtractedRibResponse
{
    /// <summary>
    /// Gets or sets the bank code.
    /// </summary>
    [JsonProperty("bankCode")]
    public string BankCode { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the branch code.
    /// </summary>
    [JsonProperty("branchCode")]
    public string BranchCode { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the bank account number.
    /// </summary>
    [JsonProperty("accountNumber")]
    public string AccountNumber { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the RIB key.
    /// </summary>
    [JsonProperty("ribKey")]
    public string RibKey { get; set; } = string.Empty;
}
