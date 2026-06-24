// <copyright file="SepaMandateTemplateProvider.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Application.BusinessHelpers;

using KPMG.Pulse.Back.Accounting.Mandate.Application.Interfaces;

/// <summary>
/// Loads the SEPA mandate PDF template from the application output directory.
/// </summary>
public sealed class SepaMandateTemplateProvider : ISepaMandateTemplateProvider
{
    private const string TemplateDirectoryName = "Templates";
    private const string TemplateFileName = "RYDGE CONSEIL - Mandat de prelevement.pdf";

    /// <inheritdoc />
    public async Task<byte[]> GetTemplateAsync()
    {
        var templatePath = Path.Combine(AppContext.BaseDirectory, TemplateDirectoryName, TemplateFileName);
        if (!File.Exists(templatePath))
        {
            throw new FileNotFoundException("The SEPA mandate PDF template could not be found.", templatePath);
        }

        return await File.ReadAllBytesAsync(templatePath).ConfigureAwait(false);
    }
}
