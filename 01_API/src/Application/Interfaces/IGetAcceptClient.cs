// <copyright file="IGetAcceptClient.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Application.Interfaces;

using KPMG.Pulse.Back.Accounting.Mandate.Application.Models;

/// <summary>
/// Sends documents for signature through GetAccept.
/// </summary>
public interface IGetAcceptClient
{
    /// <summary>
    /// Sends a generated SEPA mandate for signature and returns recipient signature metadata.
    /// </summary>
    /// <param name="request">The signature request.</param>
    /// <returns>The signature response.</returns>
    Task<GetAcceptMandateSignatureResponse> SendMandateForSignatureAsync(
        GetAcceptMandateSignatureRequest request);

    /// <summary>
    /// Gets the current GetAccept document status through <c>GET /v1/documents/{signatureRequestId}</c>.
    /// </summary>
    /// <param name="signatureRequestId">The GetAccept document identifier.</param>
    /// <returns>The document status response.</returns>
    Task<GetAcceptDocumentStatusResponse> GetDocumentStatusAsync(string signatureRequestId);

    /// <summary>
    /// Downloads the signed mandate PDF from the GetAccept signed document URL.
    /// </summary>
    /// <param name="signedDocumentUrl">The signed document URL returned by GetAccept.</param>
    /// <returns>The signed document content.</returns>
    Task<GetAcceptSignedDocument> DownloadSignedDocumentAsync(string signedDocumentUrl);
}
