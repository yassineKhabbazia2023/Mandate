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

    /// <summary>
    /// Gets or sets the account identifier when a signed mandate must be uploaded by Gateway.
    /// </summary>
    public int? AccountId { get; set; }

    /// <summary>
    /// Gets or sets the Prospect RIB document identifier when a signed mandate must be uploaded by Gateway.
    /// </summary>
    public int? RibDocumentId { get; set; }

    /// <summary>
    /// Gets or sets the base64-encoded signed mandate PDF for Gateway internal orchestration.
    /// </summary>
    public string? SignedMandatePdfBase64 { get; set; }

    /// <summary>
    /// Gets or sets the signed mandate PDF content type.
    /// </summary>
    public string? SignedMandateContentType { get; set; }

    /// <summary>
    /// Gets or sets the signed mandate PDF file name.
    /// </summary>
    public string? SignedMandateFileName { get; set; }

    /// <summary>
    /// Gets or sets the Prospect document identifier when the signed mandate was already persisted.
    /// </summary>
    public string? SignedMandateDocumentId { get; set; }
}
