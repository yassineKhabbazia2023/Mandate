// <copyright file="GetAcceptMandateSignatureRequest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Application.Models;

/// <summary>
/// Carries a generated mandate document to send for signature through GetAccept.
/// </summary>
/// <param name="FileContent">The generated PDF content.</param>
/// <param name="FileName">The generated PDF file name.</param>
/// <param name="Recipient">The signature recipient.</param>
public sealed record GetAcceptMandateSignatureRequest(
    byte[] FileContent,
    string FileName,
    SepaRecipient Recipient);
