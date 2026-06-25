// <copyright file="GetAcceptDocumentStatusResponse.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Application.Models;

/// <summary>
/// Represents the GetAccept document status response used to synchronize SEPA signatures.
/// </summary>
/// <param name="Status">The raw GetAccept status field.</param>
/// <param name="SignedDocumentUrl">The signed document download URL when available.</param>
public sealed record GetAcceptDocumentStatusResponse(
    string? Status,
    string? SignedDocumentUrl);
