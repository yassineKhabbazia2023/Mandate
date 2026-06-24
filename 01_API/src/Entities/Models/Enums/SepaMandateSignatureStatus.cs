// <copyright file="SepaMandateSignatureStatus.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate;

/// <summary>
/// Represents GetAccept signature statuses persisted for SEPA mandates.
/// </summary>
public enum SepaMandateSignatureStatus
{
    /// <summary>
    /// The signature request is in draft.
    /// </summary>
    Draft = 0,

    /// <summary>
    /// The signature request is being processed and is considered in progress.
    /// </summary>
    Processing = 1,

    /// <summary>
    /// The signature request is sealed.
    /// </summary>
    Sealed = 2,

    /// <summary>
    /// The signature request was sent.
    /// </summary>
    Sent = 3,

    /// <summary>
    /// The signature request was viewed.
    /// </summary>
    Viewed = 4,

    /// <summary>
    /// The signature request was reviewed.
    /// </summary>
    Reviewed = 5,

    /// <summary>
    /// The signature request was signed.
    /// </summary>
    Signed = 6,

    /// <summary>
    /// The signature request was rejected.
    /// </summary>
    Rejected = 7,

    /// <summary>
    /// The signature request was recalled.
    /// </summary>
    Recalled = 8
}
