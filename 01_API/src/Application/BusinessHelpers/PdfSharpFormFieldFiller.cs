// <copyright file="PdfSharpFormFieldFiller.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Application.BusinessHelpers;

using KPMG.Pulse.Back.Accounting.Mandate.Application;
using KPMG.Pulse.Back.Accounting.Mandate.Application.Interfaces;
using PdfSharp.Pdf;
using PdfSharp.Pdf.AcroForms;
using PdfSharp.Pdf.Content;
using PdfSharp.Pdf.Content.Objects;
using PdfSharp.Pdf.IO;

/// <summary>
/// Fills SEPA PDF template placeholders using the existing PDF text replacement infrastructure.
/// </summary>
/// <param name="pdfTextReplacer">The PDF text replacer.</param>
internal sealed class PdfSharpFormFieldFiller(IPdfTextReplacer pdfTextReplacer) : IPdfFormFieldFiller
{
    /// <inheritdoc />
    public byte[] FillFields(byte[] pdfTemplate, Replacement[] replacements)
    {
        var pageContentReplaced = pdfTextReplacer.ReplaceText(pdfTemplate, replacements);

        using var input = new MemoryStream(pageContentReplaced);
        using var document = PdfReader.Open(input, PdfDocumentOpenMode.Modify);
        var acroFormDictionary = document.Internals.Catalog.Elements.GetDictionary("/AcroForm");

        if (acroFormDictionary is not null)
        {
            var acroForm = document.AcroForm;
            for (var fieldIndex = 0; fieldIndex < acroForm.Fields.Count; fieldIndex++)
            {
                FillField(acroForm.Fields[fieldIndex], replacements);
            }

            acroForm.Elements.SetBoolean(PdfAcroForm.Keys.NeedAppearances, false);
            PdfAcroFormFlattener.Flatten(document);
        }

        using var output = new MemoryStream();
        document.Save(output, false);
        return output.ToArray();
    }

    /// <summary>
    /// Applies replacements to a field and its descendants.
    /// </summary>
    /// <param name="field">The field to fill.</param>
    /// <param name="replacements">The placeholder replacements.</param>
    private static void FillField(PdfAcroField field, Replacement[] replacements)
    {
        if (field is PdfTextField textField)
        {
            FillTextField(textField, replacements);
            return;
        }

        for (var fieldIndex = 0; fieldIndex < field.Fields.Count; fieldIndex++)
        {
            FillField(field.Fields[fieldIndex], replacements);
        }
    }

    /// <summary>
    /// Replaces placeholders in a text field while preserving its existing widget appearances.
    /// </summary>
    /// <param name="textField">The text field to fill.</param>
    /// <param name="replacements">The placeholder replacements.</param>
    private static void FillTextField(PdfTextField textField, Replacement[] replacements)
    {
        var currentText = textField.Text ?? string.Empty;
        var replacedText = ApplyReplacements(currentText, replacements);
        if (string.Equals(currentText, replacedText, StringComparison.Ordinal))
        {
            return;
        }

        textField.Elements.SetString(PdfAcroField.Keys.DV, replacedText);
        textField.Elements.SetString(PdfAcroField.Keys.V, replacedText);

        if (textField.HasKids)
        {
            var widgets = textField.Elements.GetArray(PdfAcroField.Keys.Kids)
                ?? throw new InvalidOperationException("The PDF text field widgets could not be found.");
            for (var widgetIndex = 0; widgetIndex < widgets.Elements.Count; widgetIndex++)
            {
                var widgetDictionary = widgets.Elements.GetDictionary(widgetIndex)
                    ?? throw new InvalidOperationException("The PDF text field widget could not be loaded.");
                ReplaceWidgetAppearance(widgetDictionary, replacements);
            }

            return;
        }

        ReplaceWidgetAppearance(textField, replacements);
    }

    /// <summary>
    /// Replaces placeholders inside an existing normal appearance stream without changing its drawing instructions.
    /// </summary>
    /// <param name="widget">The widget whose normal appearance is updated.</param>
    /// <param name="replacements">The placeholder replacements.</param>
    private static void ReplaceWidgetAppearance(PdfDictionary widget, Replacement[] replacements)
    {
        var appearance = widget.Elements.GetDictionary("/AP")?.Elements.GetDictionary("/N")
            ?? throw new InvalidOperationException("The PDF text field widget has no normal appearance.");
        var content = ContentReader.ReadContent(appearance.Stream.UnfilteredValue);
        var fontMaps = FontCMap.ParseFonts(appearance);
        string? currentFont = null;

        foreach (var contentObject in content)
        {
            if (contentObject is not COperator contentOperator)
            {
                continue;
            }

            if (contentOperator.OpCode.OpCodeName == OpCodeName.Tf)
            {
                currentFont = contentOperator.Operands.OfType<CName>().FirstOrDefault()?.Name;
                continue;
            }

            if (contentOperator.OpCode.OpCodeName is not (OpCodeName.Tj or OpCodeName.TJ))
            {
                continue;
            }

            fontMaps.TryGetValue(currentFont ?? string.Empty, out var fontMap);
            ReplaceOperatorStrings(contentOperator, fontMap, replacements);
        }

        appearance.Elements.Remove("/Filter");
        appearance.Elements.Remove("/DecodeParms");
        appearance.Stream.Value = content.ToContent();
    }

    /// <summary>
    /// Replaces placeholders in each string operand while retaining the operator and spacing operands.
    /// </summary>
    /// <param name="contentOperator">The PDF text-showing operator.</param>
    /// <param name="fontMap">The optional character map for the active font.</param>
    /// <param name="replacements">The placeholder replacements.</param>
    private static void ReplaceOperatorStrings(
        COperator contentOperator,
        FontCMap? fontMap,
        Replacement[] replacements)
    {
        foreach (var contentString in GetOperatorStrings(contentOperator))
        {
            var decodedText = fontMap?.Decode(contentString.Value) ?? contentString.Value;
            var replacedText = ApplyReplacements(decodedText, replacements);
            if (string.Equals(decodedText, replacedText, StringComparison.Ordinal))
            {
                continue;
            }

            if (fontMap is not null && !fontMap.CanEncode(replacedText))
            {
                throw new InvalidOperationException("The PDF form field font cannot encode a replacement value.");
            }

            contentString.Value = fontMap?.Encode(replacedText) ?? replacedText;
        }
    }

    /// <summary>
    /// Gets every string operand from a PDF text-showing operator.
    /// </summary>
    /// <param name="contentOperator">The PDF text-showing operator.</param>
    /// <returns>The operator string operands.</returns>
    private static IEnumerable<CString> GetOperatorStrings(COperator contentOperator)
    {
        return contentOperator.OpCode.OpCodeName == OpCodeName.Tj
            ? contentOperator.Operands.OfType<CString>()
            : contentOperator.Operands
                .OfType<CArray>()
                .SelectMany(contentArray => contentArray.OfType<CString>());
    }

    /// <summary>
    /// Applies every matching placeholder replacement to a text value.
    /// </summary>
    /// <param name="text">The source text.</param>
    /// <param name="replacements">The placeholder replacements.</param>
    /// <returns>The text with matching placeholders replaced.</returns>
    private static string ApplyReplacements(string text, Replacement[] replacements)
    {
        var result = text;
        foreach (var replacement in replacements)
        {
            result = result.Replace(replacement.WildCard, replacement.Value, StringComparison.Ordinal);
        }

        return result;
    }
}
