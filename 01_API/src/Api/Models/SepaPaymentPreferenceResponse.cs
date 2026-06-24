// <copyright file="SepaPaymentPreferenceResponse.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.AspNetCore.Models;

/// <summary>
/// Represents the SEPA mandate signature response.
/// </summary>
public sealed class SepaPaymentPreferenceResponse
{
    /// <summary>
    /// Gets or sets the signature URL.
    /// </summary>
    public string SignatureUrl { get; set; } = string.Empty;
}
