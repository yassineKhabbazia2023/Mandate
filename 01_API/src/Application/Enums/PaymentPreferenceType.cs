// <copyright file="PaymentPreferenceType.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Application;

/// <summary>
/// Represents supported prospect payment preference types.
/// </summary>
public enum PaymentPreferenceType
{
    /// <summary>
    /// SEPA mandate payment preference.
    /// </summary>
    MandateSepa = 1,

    /// <summary>
    /// Alternative payment preference.
    /// </summary>
    Other = 2
}
