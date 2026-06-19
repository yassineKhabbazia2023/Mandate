// <copyright file="PaymentPreferenceDb.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Sql;

/// <summary>
/// Represents a prospect payment preference stored by account.
/// </summary>
public class PaymentPreferenceDb
{
    /// <summary>
    /// Gets or sets the payment preference identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the account identifier.
    /// </summary>
    public int AccountId { get; set; }

    /// <summary>
    /// Gets or sets the stored payment type value.
    /// </summary>
    public int? PaymentType { get; set; }

    /// <summary>
    /// Gets or sets the creation date.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the creator email.
    /// </summary>
    public string CreatedBy { get; set; } = null!;

    /// <summary>
    /// Gets or sets the linked account.
    /// </summary>
    public CompanyDb Account { get; set; } = null!;
}
