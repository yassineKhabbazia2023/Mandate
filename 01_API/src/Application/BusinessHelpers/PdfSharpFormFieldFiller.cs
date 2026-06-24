// <copyright file="PdfSharpFormFieldFiller.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Application.BusinessHelpers;

using KPMG.Pulse.Back.Accounting.Mandate.Application;
using KPMG.Pulse.Back.Accounting.Mandate.Application.Interfaces;

/// <summary>
/// Fills SEPA PDF template placeholders using the existing PDF text replacement infrastructure.
/// </summary>
/// <param name="pdfTextReplacer">The PDF text replacer.</param>
internal sealed class PdfSharpFormFieldFiller(IPdfTextReplacer pdfTextReplacer) : IPdfFormFieldFiller
{
    /// <inheritdoc />
    public byte[] FillFields(byte[] pdfTemplate, Replacement[] replacements)
    {
        return pdfTextReplacer.ReplaceText(pdfTemplate, replacements);
    }
}
