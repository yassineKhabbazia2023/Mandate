// <copyright file="PaymentPreferenceResponse.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.AspNetCore.Models;

/// <summary>
/// Represents the current payment preference response.
/// </summary>
public sealed class PaymentPreferenceResponse
{
    /// <summary>
    /// Gets or sets the selected payment type.
    /// </summary>
    public string? PaymentType { get; set; }
}
