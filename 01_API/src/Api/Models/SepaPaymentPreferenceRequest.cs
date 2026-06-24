// <copyright file="SepaPaymentPreferenceRequest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.AspNetCore.Models;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// Represents a request to generate a SEPA mandate and send it for signature.
/// </summary>
public sealed class SepaPaymentPreferenceRequest
{
    /// <summary>
    /// Gets or sets the uploaded RIB document identifier.
    /// </summary>
    [Range(1, int.MaxValue)]
    public int DocumentId { get; set; }

    /// <summary>
    /// Gets or sets the account holder.
    /// </summary>
    [Required]
    public string AccountHolder { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the account holder address.
    /// </summary>
    [Required]
    public string Address { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the complementary account holder address line.
    /// </summary>
    public string? AddressLine2 { get; set; }

    /// <summary>
    /// Gets or sets the account holder city.
    /// </summary>
    public string? City { get; set; }

    /// <summary>
    /// Gets or sets the account holder country.
    /// </summary>
    public string? Country { get; set; }

    /// <summary>
    /// Gets or sets the account holder postal code.
    /// </summary>
    public string? PostalCode { get; set; }

    /// <summary>
    /// Gets or sets the IBAN.
    /// </summary>
    [Required]
    public string Iban { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the BIC.
    /// </summary>
    [Required]
    public string Bic { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the signature recipient email.
    /// </summary>
    [Required]
    public string RecipientEmail { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the signature recipient first name.
    /// </summary>
    [Required]
    public string RecipientFirstName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the signature recipient last name.
    /// </summary>
    [Required]
    public string RecipientLastName { get; set; } = string.Empty;
}
