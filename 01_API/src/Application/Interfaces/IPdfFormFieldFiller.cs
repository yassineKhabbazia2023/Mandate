// <copyright file="IPdfFormFieldFiller.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Application.Interfaces;

/// <summary>
/// Fills AcroForm fields in a PDF document and converts their appearances to static page content.
/// </summary>
public interface IPdfFormFieldFiller
{
    /// <summary>
    /// Fills matching PDF form fields and finalizes their appearances so downstream services cannot reinterpret them.
    /// </summary>
    /// <param name="pdfTemplate">The PDF template content.</param>
    /// <param name="replacements">The form field replacements.</param>
    /// <returns>The filled and finalized PDF content.</returns>
    byte[] FillFields(byte[] pdfTemplate, Replacement[] replacements);
}
