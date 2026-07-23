// <copyright file="PaymentPreferenceResult.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Application;

/// <summary>
/// Represents the result of reading a payment preference.
/// </summary>
/// <param name="AccountFound">A value indicating whether the account exists.</param>
/// <param name="PaymentType">The selected payment type, or null when not selected yet.</param>
/// <param name="AccountId">The account identifier when the result carries a signed mandate to upload.</param>
/// <param name="RibDocumentId">The Prospect RIB document identifier to upload with the signed mandate.</param>
/// <param name="SignedMandatePdf">The signed mandate PDF bytes when the signature was synchronized as signed.</param>
/// <param name="SignedMandateContentType">The signed mandate PDF content type.</param>
/// <param name="SignedMandateFileName">The signed mandate PDF file name.</param>
/// <param name="SignedMandateDocumentId">The Prospect document identifier when the signed mandate was already persisted.</param>
/// <param name="Iban">The persisted IBAN when a signed mandate is ready for Akuiteo finalization.</param>
/// <param name="Bic">The persisted BIC when a signed mandate is ready for Akuiteo finalization.</param>
public sealed record PaymentPreferenceResult(
    bool AccountFound,
    PaymentPreferenceType? PaymentType,
    int? AccountId = null,
    int? RibDocumentId = null,
    byte[]? SignedMandatePdf = null,
    string? SignedMandateContentType = null,
    string? SignedMandateFileName = null,
    string? SignedMandateDocumentId = null,
    string? Iban = null,
    string? Bic = null);
