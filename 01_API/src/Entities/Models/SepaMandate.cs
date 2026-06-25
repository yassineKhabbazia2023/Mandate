// <copyright file="SepaMandate.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate;

/// <summary>
/// Represents a SEPA mandate signature request persisted for an account.
/// </summary>
public sealed class SepaMandate
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SepaMandate"/> class.
    /// </summary>
    /// <param name="id">The SEPA mandate identifier.</param>
    /// <param name="accountId">The account identifier.</param>
    /// <param name="documentId">The uploaded RIB document identifier.</param>
    /// <param name="accountHolder">The account holder.</param>
    /// <param name="iban">The IBAN.</param>
    /// <param name="bic">The BIC.</param>
    /// <param name="address">The account holder address.</param>
    /// <param name="signatureRequestId">The GetAccept signature request identifier.</param>
    /// <param name="signatureUrl">The GetAccept signature URL.</param>
    /// <param name="signatureStatus">The signature status.</param>
    /// <param name="isSentToAkuiteo">A value indicating whether the mandate was sent to Akuiteo.</param>
    /// <param name="sentToAkuiteoAt">The date when the mandate was sent to Akuiteo.</param>
    /// <param name="createdAt">The creation date.</param>
    /// <param name="createdBy">The creator.</param>
    /// <param name="signedMandateDocumentId">The uploaded signed mandate Prospect document identifier.</param>
    public SepaMandate(
        int id,
        int accountId,
        int documentId,
        string accountHolder,
        string iban,
        string bic,
        string address,
        string? signatureRequestId,
        string? signatureUrl,
        SepaMandateSignatureStatus signatureStatus,
        bool isSentToAkuiteo,
        DateTime? sentToAkuiteoAt,
        DateTime createdAt,
        string createdBy,
        string? signedMandateDocumentId = null)
    {
        this.Id = id;
        this.AccountId = accountId;
        this.DocumentId = documentId;
        this.AccountHolder = accountHolder;
        this.Iban = iban;
        this.Bic = bic;
        this.Address = address;
        this.SignatureRequestId = signatureRequestId;
        this.SignatureUrl = signatureUrl;
        this.SignatureStatus = signatureStatus;
        this.IsSentToAkuiteo = isSentToAkuiteo;
        this.SentToAkuiteoAt = sentToAkuiteoAt;
        this.CreatedAt = createdAt;
        this.CreatedBy = createdBy;
        this.SignedMandateDocumentId = signedMandateDocumentId;
    }

    /// <summary>
    /// Gets the SEPA mandate identifier.
    /// </summary>
    public int Id { get; }

    /// <summary>
    /// Gets the account identifier.
    /// </summary>
    public int AccountId { get; }

    /// <summary>
    /// Gets the uploaded RIB document identifier.
    /// </summary>
    public int DocumentId { get; }

    /// <summary>
    /// Gets the account holder.
    /// </summary>
    public string AccountHolder { get; }

    /// <summary>
    /// Gets the IBAN.
    /// </summary>
    public string Iban { get; }

    /// <summary>
    /// Gets the BIC.
    /// </summary>
    public string Bic { get; }

    /// <summary>
    /// Gets the account holder address.
    /// </summary>
    public string Address { get; }

    /// <summary>
    /// Gets the GetAccept signature request identifier.
    /// </summary>
    public string? SignatureRequestId { get; }

    /// <summary>
    /// Gets the GetAccept signature URL.
    /// </summary>
    public string? SignatureUrl { get; }

    /// <summary>
    /// Gets the signature status.
    /// </summary>
    public SepaMandateSignatureStatus SignatureStatus { get; }

    /// <summary>
    /// Gets a value indicating whether the mandate was sent to Akuiteo.
    /// </summary>
    public bool IsSentToAkuiteo { get; }

    /// <summary>
    /// Gets the date when the mandate was sent to Akuiteo.
    /// </summary>
    public DateTime? SentToAkuiteoAt { get; }

    /// <summary>
    /// Gets the uploaded signed mandate Prospect document identifier.
    /// </summary>
    public string? SignedMandateDocumentId { get; }

    /// <summary>
    /// Gets the creation date.
    /// </summary>
    public DateTime CreatedAt { get; }

    /// <summary>
    /// Gets the creator.
    /// </summary>
    public string CreatedBy { get; }
}
