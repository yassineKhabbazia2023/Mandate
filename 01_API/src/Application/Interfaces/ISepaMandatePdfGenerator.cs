// <copyright file="ISepaMandatePdfGenerator.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Application.Interfaces;

using KPMG.Pulse.Back.Accounting.Mandate.Application.Models;

/// <summary>
/// Generates SEPA mandate PDF files.
/// </summary>
public interface ISepaMandatePdfGenerator
{
    /// <summary>
    /// Generates a SEPA mandate PDF from the configured template and banking data.
    /// </summary>
    /// <param name="data">The SEPA mandate PDF data.</param>
    /// <returns>The generated PDF content.</returns>
    Task<byte[]> GenerateAsync(SepaMandatePdfData data);
}
