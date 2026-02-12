// <copyright file="ArialFontEmbedder.cs" company="Pulse">
// Copyright (c) Pulse. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Application
{
    using PdfSharp.Drawing;
    using PdfSharp.Pdf;
    using PdfSharp.Pdf.Advanced;

    internal static class ArialFontEmbedder
    {
        private const string FontKey = "/Arial";
        private const string TtfFileName = "Arial.ttf";
        private const int Ascent = 905;
        private const int Descent = -212;
        private const int CapHeight = 718;
        private const int StemV = 80;

        internal static string EnsureFont(PdfPage page, PdfDocument doc)
        {
            var resources = page.Elements.GetDictionary("/Resources");
            if (resources == null)
            {
                resources = new PdfDictionary(doc);
                page.Elements.SetObject("/Resources", resources);
            }

            var fonts = resources.Elements.GetDictionary("/Font");
            if (fonts == null)
            {
                fonts = new PdfDictionary(doc);
                resources.Elements.SetObject("/Font", fonts);
            }

            if (fonts.Elements.ContainsKey(FontKey))
            {
                return FontKey;
            }

            var ttfPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, TtfFileName);
            var ttfBytes = File.ReadAllBytes(ttfPath);

            var fontFileStream = new PdfDictionary(doc);
            doc.Internals.AddObject(fontFileStream);
            fontFileStream.CreateStream(ttfBytes);
            fontFileStream.Elements.SetInteger("/Length1", ttfBytes.Length);

            var descriptor = new PdfDictionary(doc);
            doc.Internals.AddObject(descriptor);
            descriptor.Elements.SetName("/Type", "/FontDescriptor");
            descriptor.Elements.SetName("/FontName", "/ArialMT");
            descriptor.Elements.SetInteger("/Flags", 32);
            descriptor.Elements.SetReal("/ItalicAngle", 0);
            descriptor.Elements.SetInteger("/Ascent", Ascent);
            descriptor.Elements.SetInteger("/Descent", Descent);
            descriptor.Elements.SetInteger("/CapHeight", CapHeight);
            descriptor.Elements.SetInteger("/StemV", StemV);
            descriptor.Elements.SetRectangle("/FontBBox", new PdfRectangle(new XPoint(-665, -325), new XPoint(2000, 1006)));
            descriptor.Elements.SetReference("/FontFile2", fontFileStream);

            var fontDict = new PdfDictionary(doc);
            doc.Internals.AddObject(fontDict);
            fontDict.Elements.SetName("/Type", "/Font");
            fontDict.Elements.SetName("/Subtype", "/TrueType");
            fontDict.Elements.SetName("/BaseFont", "/ArialMT");
            fontDict.Elements.SetName("/Encoding", "/WinAnsiEncoding");
            fontDict.Elements.SetReference("/FontDescriptor", descriptor);
            fonts.Elements.SetReference(FontKey, fontDict);

            return FontKey;
        }
    }
}
