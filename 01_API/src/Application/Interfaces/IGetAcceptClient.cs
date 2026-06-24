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
}
