// <copyright file="SepaRecipient.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Application.Models;

/// <summary>
/// Represents the recipient that must sign a SEPA mandate.
/// </summary>
/// <param name="Email">The recipient email.</param>
/// <param name="FirstName">The recipient first name.</param>
/// <param name="LastName">The recipient last name.</param>
public sealed record SepaRecipient(
    string Email,
    string FirstName,
    string LastName);
