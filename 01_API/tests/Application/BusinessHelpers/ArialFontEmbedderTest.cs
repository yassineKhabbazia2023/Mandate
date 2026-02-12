// <copyright file="ArialFontEmbedderTest.cs" company="Pulse">
// Copyright (c) Pulse. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Application.Tests
{
    using PdfSharp.Pdf;

    public class ArialFontEmbedderTest
    {
        [Fact]
        public void EnsureFont_FreshPage_CreatesAllFontResources()
        {
            var doc = new PdfDocument();
            var page = doc.AddPage();

            var result = ArialFontEmbedder.EnsureFont(page, doc);

            result.Should().Be("/Arial");

            var resources = page.Elements.GetDictionary("/Resources");
            resources.Should().NotBeNull();

            var fonts = resources!.Elements.GetDictionary("/Font");
            fonts.Should().NotBeNull();
            fonts!.Elements.ContainsKey("/Arial").Should().BeTrue();
        }

        [Fact]
        public void EnsureFont_CalledTwice_ReturnsSameKeyWithoutDuplication()
        {
            var doc = new PdfDocument();
            var page = doc.AddPage();

            var result1 = ArialFontEmbedder.EnsureFont(page, doc);
            var result2 = ArialFontEmbedder.EnsureFont(page, doc);

            result1.Should().Be("/Arial");
            result2.Should().Be("/Arial");
        }

        [Fact]
        public void EnsureFont_PageWithExistingResourcesNoFonts_CreatesFontDict()
        {
            var doc = new PdfDocument();
            var page = doc.AddPage();

            var resources = new PdfDictionary(doc);
            page.Elements.SetObject("/Resources", resources);

            var result = ArialFontEmbedder.EnsureFont(page, doc);

            result.Should().Be("/Arial");
            var fonts = resources.Elements.GetDictionary("/Font");
            fonts.Should().NotBeNull();
            fonts!.Elements.ContainsKey("/Arial").Should().BeTrue();
        }

        [Fact]
        public void EnsureFont_PageWithExistingFontsDict_AddsFontEntry()
        {
            var doc = new PdfDocument();
            var page = doc.AddPage();

            var resources = new PdfDictionary(doc);
            page.Elements.SetObject("/Resources", resources);
            var fonts = new PdfDictionary(doc);
            resources.Elements.SetObject("/Font", fonts);

            var result = ArialFontEmbedder.EnsureFont(page, doc);

            result.Should().Be("/Arial");
            fonts.Elements.ContainsKey("/Arial").Should().BeTrue();
        }
    }
}
