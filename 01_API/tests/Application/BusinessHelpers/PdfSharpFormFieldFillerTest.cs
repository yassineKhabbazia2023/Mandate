// <copyright file="PdfSharpFormFieldFillerTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Application.Tests.BusinessHelpers;

using System.Text;
using KPMG.Pulse.Back.Accounting.Mandate.Application.BusinessHelpers;
using PdfSharp.Pdf;
using PdfSharp.Pdf.AcroForms;
using PdfSharp.Pdf.Content;
using PdfSharp.Pdf.Content.Objects;
using PdfSharp.Pdf.IO;

/// <summary>
/// Unit tests for <see cref="PdfSharpFormFieldFiller"/>.
/// </summary>
public sealed class PdfSharpFormFieldFillerTest
{
    /// <summary>
    /// Verifies that field values are replaced and finalized without changing template-defined appearance formatting.
    /// </summary>
    [Fact]
    public async Task FillFields_WhenTemplateContainsAcroFormFields_ReplacesValuesAndRendersWidgets()
    {
        var template = await new SepaMandateTemplateProvider().GetTemplateAsync();
        var filler = new PdfSharpFormFieldFiller(new PdfSharpTextReplacer());
        const string eventDate = "05/08/2026";
        Replacement[] replacements =
        [
            new Replacement("{BIC}", "AGRIFRPP"),
            new Replacement("{RAISON_SOCIALE}", "Jean Dupont"),
            new Replacement("{ADRESSE}", "10 rue de Paris"),
            new Replacement("{ADRESSE2}", "Batiment A"),
            new Replacement("{CP}", "75008"),
            new Replacement("{VILLE}", "Paris"),
            new Replacement("{PAYS}", "France"),
            new Replacement("{DATE1}", eventDate)
        ];
        string expectedBicAppearance;
        string[] expectedInformationAppearances;
        string[] templateEventDateAppearances;
        string[] expectedEventDateAppearances;
        using (var templateStream = new MemoryStream(template))
        using (var templateDocument = PdfReader.Open(templateStream, PdfDocumentOpenMode.Modify))
        {
            var templateBicField = templateDocument.AcroForm.Fields["BIC Bank Identifier Code"] as PdfTextField
                ?? throw new InvalidOperationException("The template BIC field was not found.");
            var templateInformationField = templateDocument.AcroForm.Fields["Informations"] as PdfTextField
                ?? throw new InvalidOperationException("The template information field was not found.");
            var templateEventDateField = templateDocument.AcroForm.Fields["Date7_af_date"] as PdfTextField
                ?? throw new InvalidOperationException("The template event date field was not found.");
            expectedBicAppearance = ApplyReplacements(GetNormalAppearanceContent(templateBicField), replacements);
            expectedInformationAppearances = Enumerable.Range(0, templateInformationField.Fields.Count)
                .Select(widgetIndex => ApplyReplacements(
                    GetNormalAppearanceContent((PdfDictionary)templateInformationField.Fields[widgetIndex]),
                    replacements))
                .ToArray();
            templateEventDateAppearances = Enumerable.Range(0, templateEventDateField.Fields.Count)
                .Select(widgetIndex => GetNormalAppearanceContent(
                    (PdfDictionary)templateEventDateField.Fields[widgetIndex]))
                .ToArray();
            expectedEventDateAppearances = templateEventDateAppearances
                .Select(appearance => ApplyReplacements(appearance, replacements))
                .ToArray();
        }

        expectedEventDateAppearances
            .Zip(templateEventDateAppearances)
            .Should()
            .OnlyContain(pair => !string.Equals(pair.First, pair.Second, StringComparison.Ordinal));

        var result = filler.FillFields(template, replacements);

        using var stream = new MemoryStream(result);
        using var document = PdfReader.Open(stream, PdfDocumentOpenMode.Modify);
        var flattenedAppearances = GetFlattenedAppearanceContents(document);

        document.Internals.Catalog.Elements.GetDictionary("/AcroForm").Should().BeNull();
        GetWidgetCount(document).Should().Be(0);
        GetFlattenedAppearanceDrawingCount(document).Should().Be(flattenedAppearances.Length);
        flattenedAppearances.Should().Contain(expectedBicAppearance);
        flattenedAppearances.Should().Contain(expectedInformationAppearances);
        flattenedAppearances.Should().Contain(expectedEventDateAppearances);
    }

    /// <summary>
    /// Verifies that fields without matching placeholders retain their template-defined appearance after finalization.
    /// </summary>
    [Fact]
    public async Task FillFields_WhenNoPlaceholderMatches_PreservesFieldValue()
    {
        var template = await new SepaMandateTemplateProvider().GetTemplateAsync();
        var filler = new PdfSharpFormFieldFiller(new PdfSharpTextReplacer());
        string expectedBicAppearance;
        using (var templateStream = new MemoryStream(template))
        using (var templateDocument = PdfReader.Open(templateStream, PdfDocumentOpenMode.Modify))
        {
            var templateBicField = templateDocument.AcroForm.Fields["BIC Bank Identifier Code"] as PdfTextField
                ?? throw new InvalidOperationException("The template BIC field was not found.");
            expectedBicAppearance = GetNormalAppearanceContent(templateBicField);
        }

        var result = filler.FillFields(template, [new Replacement("{UNKNOWN}", "value")]);

        using var stream = new MemoryStream(result);
        using var document = PdfReader.Open(stream, PdfDocumentOpenMode.Modify);
        document.Internals.Catalog.Elements.GetDictionary("/AcroForm").Should().BeNull();
        GetFlattenedAppearanceContents(document).Should().Contain(expectedBicAppearance);
    }

    /// <summary>
    /// Verifies that a PDF without an AcroForm remains readable.
    /// </summary>
    [Fact]
    public void FillFields_WhenTemplateHasNoAcroForm_ReturnsReadablePdf()
    {
        var filler = new PdfSharpFormFieldFiller(new PdfSharpTextReplacer());
        var template = CreatePdfWithoutAcroForm();

        var result = filler.FillFields(template, [new Replacement("{BIC}", "AGRIFRPP")]);

        using var stream = new MemoryStream(result);
        using var document = PdfReader.Open(stream, PdfDocumentOpenMode.Import);
        document.PageCount.Should().Be(1);
    }

    /// <summary>
    /// Creates a one-page PDF without interactive form fields.
    /// </summary>
    /// <returns>The PDF bytes.</returns>
    private static byte[] CreatePdfWithoutAcroForm()
    {
        using var document = new PdfDocument();
        document.AddPage();
        using var stream = new MemoryStream();
        document.Save(stream, false);
        return stream.ToArray();
    }

    /// <summary>
    /// Reads the unfiltered normal appearance content of a form field widget.
    /// </summary>
    /// <param name="widget">The form field widget.</param>
    /// <returns>The PDF appearance drawing instructions.</returns>
    private static string GetNormalAppearanceContent(PdfDictionary widget)
    {
        var appearance = widget.Elements.GetDictionary("/AP")?.Elements.GetDictionary("/N");
        appearance.Should().NotBeNull();
        return GetAppearanceContent(appearance!);
    }

    /// <summary>
    /// Reads the unfiltered content of a form appearance.
    /// </summary>
    /// <param name="appearance">The form appearance.</param>
    /// <returns>The PDF appearance drawing instructions.</returns>
    private static string GetAppearanceContent(PdfDictionary appearance)
    {
        var content = PdfSharp.Pdf.Content.ContentReader.ReadContent(appearance.Stream.UnfilteredValue);
        return Encoding.Latin1.GetString(content.ToContent());
    }

    /// <summary>
    /// Applies placeholder replacements to an expected appearance without altering its rendering instructions.
    /// </summary>
    /// <param name="appearance">The template appearance content.</param>
    /// <param name="replacements">The placeholder replacements.</param>
    /// <returns>The expected filled appearance content.</returns>
    private static string ApplyReplacements(string appearance, Replacement[] replacements)
    {
        return replacements.Aggregate(
            appearance,
            (current, replacement) => current.Replace(
                replacement.WildCard,
                replacement.Value,
                StringComparison.Ordinal));
    }

    /// <summary>
    /// Reads every flattened widget appearance registered by the form finalizer.
    /// </summary>
    /// <param name="document">The finalized PDF document.</param>
    /// <returns>The appearance drawing instructions.</returns>
    private static string[] GetFlattenedAppearanceContents(PdfDocument document)
    {
        return document.Pages
            .Cast<PdfPage>()
            .SelectMany(page =>
            {
                var xObjects = page.Elements.GetDictionary("/Resources")?.Elements.GetDictionary("/XObject");
                return xObjects is null
                    ? []
                    : xObjects.Elements.Keys
                        .Where(key => key.StartsWith("/FlattenedWidget", StringComparison.Ordinal))
                        .Select(key => GetAppearanceContent(
                            xObjects.Elements.GetDictionary(key)
                            ?? throw new InvalidOperationException("The flattened appearance could not be loaded.")));
            })
            .ToArray();
    }

    /// <summary>
    /// Counts the interactive widget annotations remaining in a PDF document.
    /// </summary>
    /// <param name="document">The PDF document.</param>
    /// <returns>The number of widget annotations.</returns>
    private static int GetWidgetCount(PdfDocument document)
    {
        return document.Pages
            .Cast<PdfPage>()
            .Sum(page => Enumerable.Range(0, page.Annotations.Count)
                .Count(annotationIndex =>
                    page.Annotations[annotationIndex].Elements.GetName("/Subtype") == "/Widget"));
    }

    /// <summary>
    /// Counts page drawing operations that render finalized widget appearances.
    /// </summary>
    /// <param name="document">The finalized PDF document.</param>
    /// <returns>The number of finalized appearance drawing operations.</returns>
    private static int GetFlattenedAppearanceDrawingCount(PdfDocument document)
    {
        return document.Pages
            .Cast<PdfPage>()
            .SelectMany(ContentReader.ReadContent)
            .OfType<COperator>()
            .Count(contentOperator => contentOperator.OpCode.OpCodeName == OpCodeName.Do
                && contentOperator.Operands
                    .OfType<CName>()
                    .Any(name => name.Name.StartsWith("/FlattenedWidget", StringComparison.Ordinal)));
    }
}
