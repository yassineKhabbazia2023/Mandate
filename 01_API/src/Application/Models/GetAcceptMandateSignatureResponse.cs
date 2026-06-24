// <copyright file="GetAcceptMandateSignatureResponse.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Application.Models;

/// <summary>
/// Represents the GetAccept signature request metadata returned to Mandat.
/// </summary>
/// <param name="SignatureRequestId">The GetAccept document identifier.</param>
/// <param name="SignatureUrl">The recipient signature URL.</param>
public sealed record GetAcceptMandateSignatureResponse(
    string SignatureRequestId,
    string SignatureUrl);
