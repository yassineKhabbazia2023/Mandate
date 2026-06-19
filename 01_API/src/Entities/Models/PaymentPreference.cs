// <copyright file="PaymentPreference.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate;

/// <summary>
/// Represents an account payment preference.
/// </summary>
public sealed class PaymentPreference
{
    private int? paymentType;

    /// <summary>
    /// Initializes a new instance of the <see cref="PaymentPreference"/> class.
    /// </summary>
    /// <param name="id">The payment preference identifier.</param>
    /// <param name="accountId">The account identifier.</param>
    /// <param name="paymentType">The selected payment type.</param>
    /// <param name="createdAt">The creation date.</param>
    /// <param name="createdBy">The creator email.</param>
    public PaymentPreference(int id, int accountId, int? paymentType, DateTime createdAt, string createdBy)
    {
        this.Id = id;
        this.AccountId = accountId;
        this.paymentType = paymentType;
        this.CreatedAt = createdAt;
        this.CreatedBy = createdBy;
    }

    /// <summary>
    /// Gets the payment preference identifier.
    /// </summary>
    public int Id { get; }

    /// <summary>
    /// Gets the account identifier.
    /// </summary>
    public int AccountId { get; }

    /// <summary>
    /// Gets the payment type.
    /// </summary>
    public int? PaymentType => this.paymentType;

    /// <summary>
    /// Gets the creation date.
    /// </summary>
    public DateTime CreatedAt { get; }

    /// <summary>
    /// Gets the creator email.
    /// </summary>
    public string CreatedBy { get; }

    /// <summary>
    /// Sets the selected payment type.
    /// </summary>
    /// <param name="paymentType">The selected payment type.</param>
    public void SetPaymentType(int paymentType)
    {
        this.paymentType = paymentType;
    }

    /// <summary>
    /// Clears the selected payment type.
    /// </summary>
    public void ResetPaymentType()
    {
        this.paymentType = null;
    }
}
