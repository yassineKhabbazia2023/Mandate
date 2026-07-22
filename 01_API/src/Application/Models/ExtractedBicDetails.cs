// <copyright file="ExtractedBicDetails.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Application.Models;

/// <summary>
/// Represents details extracted from a BIC.
/// </summary>
/// <param name="BankCode">The bank code.</param>
/// <param name="CountryCode">The ISO country code.</param>
/// <param name="LocationCode">The location code.</param>
/// <param name="BranchCode">The optional branch code.</param>
public sealed record ExtractedBicDetails(
    string BankCode,
    string CountryCode,
    string LocationCode,
    string? BranchCode);
