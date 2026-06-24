// <copyright file="IPdfFormFieldFiller.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Application.Interfaces;

/// <summary>
/// Fills AcroForm fields in a PDF document.
/// </summary>
public interface IPdfFormFieldFiller
{
    /// <summary>
    /// Fills matching PDF form fields with the provided values.
    /// </summary>
    /// <param name="pdfTemplate">The PDF template content.</param>
    /// <param name="replacements">The form field replacements.</param>
    /// <returns>The filled PDF content.</returns>
    byte[] FillFields(byte[] pdfTemplate, Replacement[] replacements);
}
