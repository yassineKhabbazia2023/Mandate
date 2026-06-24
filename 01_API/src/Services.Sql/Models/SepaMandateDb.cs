// <copyright file="SepaMandateDb.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Sql;

/// <summary>
/// Represents a SEPA mandate database row.
/// </summary>
public class SepaMandateDb
{
    /// <summary>
    /// Gets or sets the SEPA mandate identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the account identifier.
    /// </summary>
    public int AccountId { get; set; }

    /// <summary>
    /// Gets or sets the account holder.
    /// </summary>
    public string AccountHolder { get; set; } = null!;

    /// <summary>
    /// Gets or sets the account holder address.
    /// </summary>
    public string Address { get; set; } = null!;

    /// <summary>
    /// Gets or sets the IBAN.
    /// </summary>
    public string Iban { get; set; } = null!;

    /// <summary>
    /// Gets or sets the BIC.
    /// </summary>
    public string Bic { get; set; } = null!;

    /// <summary>
    /// Gets or sets the uploaded RIB document identifier.
    /// </summary>
    public int RibDocumentId { get; set; }

    /// <summary>
    /// Gets or sets the GetAccept signature request identifier.
    /// </summary>
    public string? SignatureRequestId { get; set; }

    /// <summary>
    /// Gets or sets the GetAccept signature URL.
    /// </summary>
    public string? SignatureUrl { get; set; }

    /// <summary>
    /// Gets or sets the signature status.
    /// </summary>
    public int SignatureStatus { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the mandate was sent to Akuiteo.
    /// </summary>
    public bool IsSentToAkuiteo { get; set; }

    /// <summary>
    /// Gets or sets the date when the mandate was sent to Akuiteo.
    /// </summary>
    public DateTime? SentToAkuiteoAt { get; set; }

    /// <summary>
    /// Gets or sets the creation date.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the creator.
    /// </summary>
    public string CreatedBy { get; set; } = null!;

    /// <summary>
    /// Gets or sets the linked account.
    /// </summary>
    public CompanyDb Account { get; set; } = null!;
}
