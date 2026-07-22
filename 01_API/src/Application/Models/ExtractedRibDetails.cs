// <copyright file="ExtractedRibDetails.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Application.Models;

/// <summary>
/// Represents French RIB details extracted from an IBAN.
/// </summary>
/// <param name="BankCode">The bank code.</param>
/// <param name="BranchCode">The branch code.</param>
/// <param name="AccountNumber">The bank account number.</param>
/// <param name="RibKey">The RIB key.</param>
public sealed record ExtractedRibDetails(
    string BankCode,
    string BranchCode,
    string AccountNumber,
    string RibKey);
