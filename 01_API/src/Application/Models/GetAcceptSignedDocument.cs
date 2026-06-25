// <copyright file="GetAcceptSignedDocument.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Application.Models;

/// <summary>
/// Represents signed document content downloaded from GetAccept.
/// </summary>
/// <param name="Content">The signed document bytes.</param>
/// <param name="ContentType">The signed document content type.</param>
/// <param name="FileName">The signed document file name.</param>
public sealed record GetAcceptSignedDocument(
    byte[] Content,
    string ContentType,
    string FileName);
