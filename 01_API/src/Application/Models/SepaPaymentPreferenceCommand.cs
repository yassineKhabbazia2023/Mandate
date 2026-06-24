// <copyright file="SepaPaymentPreferenceCommand.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Application.Models;

/// <summary>
/// Carries the data required to generate a SEPA mandate and send it for signature.
/// </summary>
/// <param name="DocumentId">The uploaded RIB document identifier.</param>
/// <param name="AccountHolder">The account holder.</param>
/// <param name="Address">The account holder address.</param>
/// <param name="AddressLine2">The complementary account holder address line.</param>
/// <param name="City">The account holder city.</param>
/// <param name="Country">The account holder country.</param>
/// <param name="PostalCode">The account holder postal code.</param>
/// <param name="Iban">The IBAN.</param>
/// <param name="Bic">The BIC.</param>
/// <param name="Recipient">The signature recipient.</param>
public sealed record SepaPaymentPreferenceCommand(
    int DocumentId,
    string AccountHolder,
    string Address,
    string? AddressLine2,
    string? City,
    string? Country,
    string? PostalCode,
    string Iban,
    string Bic,
    SepaRecipient Recipient);
