// <copyright file="ISepaMandateTemplateProvider.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Application.Interfaces;

/// <summary>
/// Provides the SEPA mandate PDF template used by mandate generation.
/// </summary>
public interface ISepaMandateTemplateProvider
{
    /// <summary>
    /// Gets the SEPA mandate PDF template content.
    /// </summary>
    /// <returns>The template PDF bytes.</returns>
    Task<byte[]> GetTemplateAsync();
}
