// <copyright file="BankDetailsExtractionResult.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Application.Models;

/// <summary>
/// Represents either extracted bank details or validation errors.
/// </summary>
/// <param name="BankDetails">The extracted bank details when validation succeeds.</param>
/// <param name="ValidationErrors">Validation errors keyed by request field.</param>
public sealed record BankDetailsExtractionResult(
    ExtractedBankDetails? BankDetails,
    IReadOnlyDictionary<string, string[]> ValidationErrors)
{
    /// <summary>
    /// Gets a value indicating whether extraction succeeded.
    /// </summary>
    public bool IsSuccess => BankDetails is not null && ValidationErrors.Count == 0;
}
