// <copyright file="PdfAcroFormFlattener.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Application.BusinessHelpers;

using System.Globalization;
using System.Text;
using PdfSharp.Pdf;

/// <summary>
/// Converts AcroForm widget appearances into static page content.
/// </summary>
internal static class PdfAcroFormFlattener
{
    private const string WidgetSubtype = "/Widget";
    private const string FlattenedWidgetPrefix = "/FlattenedWidget";

    /// <summary>
    /// Draws every available widget appearance on its page and removes the interactive form structure.
    /// </summary>
    /// <param name="document">The PDF document to finalize.</param>
    public static void Flatten(PdfDocument document)
    {
        var appearanceIndex = 0;
        foreach (var page in document.Pages)
        {
            FlattenPage(page, ref appearanceIndex);
        }

        document.Internals.Catalog.Elements.Remove("/AcroForm");
    }

    /// <summary>
    /// Draws widget appearances on one page and removes the corresponding annotations.
    /// </summary>
    /// <param name="page">The page to finalize.</param>
    /// <param name="appearanceIndex">The document-wide appearance resource index.</param>
    private static void FlattenPage(PdfPage page, ref int appearanceIndex)
    {
        var widgets = Enumerable.Range(0, page.Annotations.Count)
            .Select(annotationIndex => page.Annotations[annotationIndex])
            .Where(annotation => annotation.Elements.GetName("/Subtype") == WidgetSubtype)
            .ToArray();
        if (widgets.Length == 0)
        {
            return;
        }

        var flattenedContent = new StringBuilder();
        foreach (var widget in widgets)
        {
            var appearance = widget.Elements.GetDictionary("/AP")?.Elements.GetDictionary("/N");
            if (appearance?.Stream is not null)
            {
                var resourceName = $"{FlattenedWidgetPrefix}{appearanceIndex + 1}";
                if (TryAppendAppearanceDrawing(flattenedContent, resourceName, widget, appearance))
                {
                    appearanceIndex++;
                    AddAppearanceResource(page, resourceName, appearance);
                }
            }

            page.Annotations.Remove(widget);
        }

        if (flattenedContent.Length == 0)
        {
            return;
        }

        var content = page.Contents.AppendContent();
        content.CreateStream(Encoding.ASCII.GetBytes(flattenedContent.ToString()));
    }

    /// <summary>
    /// Adds an appearance as a page Form XObject resource.
    /// </summary>
    /// <param name="page">The page that will draw the appearance.</param>
    /// <param name="resourceName">The unique page resource name.</param>
    /// <param name="appearance">The normal widget appearance.</param>
    private static void AddAppearanceResource(PdfPage page, string resourceName, PdfDictionary appearance)
    {
        var resources = page.Resources;
        var xObjects = resources.Elements.GetDictionary("/XObject");
        if (xObjects is null)
        {
            xObjects = new PdfDictionary(page.Owner);
            resources.Elements.SetObject("/XObject", xObjects);
        }

        xObjects.Elements.SetReference(resourceName, appearance);
    }

    /// <summary>
    /// Tries to append drawing instructions that map an appearance bounding box to its annotation rectangle.
    /// </summary>
    /// <param name="content">The static page content under construction.</param>
    /// <param name="resourceName">The appearance resource name.</param>
    /// <param name="widget">The widget annotation.</param>
    /// <param name="appearance">The widget appearance.</param>
    /// <returns><see langword="true" /> when drawing instructions were appended; otherwise, <see langword="false" />.</returns>
    private static bool TryAppendAppearanceDrawing(
        StringBuilder content,
        string resourceName,
        PdfDictionary widget,
        PdfDictionary appearance)
    {
        var rectangle = widget.Elements.GetRectangle("/Rect");
        var boundingBox = appearance.Elements.GetRectangle("/BBox");
        if (rectangle.IsEmpty
            || boundingBox.IsEmpty
            || Math.Abs(boundingBox.Width) < double.Epsilon
            || Math.Abs(boundingBox.Height) < double.Epsilon)
        {
            return false;
        }

        var scaleX = rectangle.Width / boundingBox.Width;
        var scaleY = rectangle.Height / boundingBox.Height;
        var translateX = rectangle.X1 - (boundingBox.X1 * scaleX);
        var translateY = rectangle.Y1 - (boundingBox.Y1 * scaleY);

        content.AppendLine("q")
            .Append(Format(scaleX)).Append(" 0 0 ")
            .Append(Format(scaleY)).Append(' ')
            .Append(Format(translateX)).Append(' ')
            .Append(Format(translateY)).AppendLine(" cm")
            .Append(resourceName).AppendLine(" Do")
            .AppendLine("Q");
        return true;
    }

    /// <summary>
    /// Formats a PDF number using a culture-independent representation.
    /// </summary>
    /// <param name="value">The numeric value.</param>
    /// <returns>The PDF number representation.</returns>
    private static string Format(double value)
    {
        return value.ToString("0.################", CultureInfo.InvariantCulture);
    }
}
