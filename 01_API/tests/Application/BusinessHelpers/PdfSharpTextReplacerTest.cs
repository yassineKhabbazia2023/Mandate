// <copyright file="PdfSharpTextReplacerTest.cs" company="Pulse">
// Copyright (c) Pulse. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Application.Tests
{
    using PdfSharp.Drawing;
    using PdfSharp.Pdf;
    using PdfSharp.Pdf.Content;
    using PdfSharp.Pdf.Content.Objects;
    using PdfSharp.Pdf.IO;
    using System.Text;

    public class PdfSharpTextReplacerTest
    {
        [Fact]
        public void ReplaceText_WithSimplePlaceholders_ReplacesAll()
        {
            var templateBytes = CreateTextPlaceholderTemplate();
            var replacer = new PdfSharpTextReplacer();

            var replacements = new Replacement[]
            {
                new("{bankCode}", "12345"),
                new("{branchCode}", "67890"),
                new("{accountNumber}", "12345678901"),
                new("{checkDigits}", "55"),
                new("{bankName}", "Test Bank"),
                new("{companyName}", "Test Company"),
                new("{siren}", "12345678901234"),
                new("{companyAddressStreet}", "10 rue de la Paix"),
                new("{companyAddressZipCode}", "75002"),
                new("{companyAddressCountry}", "France"),
                new("{companyAddressComplements}", "Bat A"),
                new("{signatory}", "Jean Dupont"),
                new("{companyAddress}", "10 rue de la Paix - Bat A - 75002 - Paris - France"),
            };

            var result = replacer.ReplaceText(templateBytes, replacements);

            result.Should().NotBeNull();
            result.Length.Should().BeGreaterThan(0);

            using var ms = new MemoryStream(result);
            using var pdfDoc = PdfReader.Open(ms, PdfDocumentOpenMode.Modify);
            var page = pdfDoc.Pages[0];
            var content = ContentReader.ReadContent(page);
            var allText = ExtractAllText(content);

            allText.Should().Contain("12345");
            allText.Should().Contain("67890");
            allText.Should().Contain("12345678901");
            allText.Should().Contain("55");
            allText.Should().Contain("Test Bank");
            allText.Should().Contain("Test Company");
            allText.Should().Contain("12345678901234");
            allText.Should().Contain("10 rue de la Paix");
            allText.Should().Contain("75002");
            allText.Should().Contain("France");
            allText.Should().Contain("Bat A");
            allText.Should().Contain("Jean Dupont");
            allText.Should().Contain("10 rue de la Paix - Bat A - 75002 - Paris - France");

            allText.Should().NotContain("{bankCode}");
            allText.Should().NotContain("{branchCode}");
            allText.Should().NotContain("{accountNumber}");
            allText.Should().NotContain("{checkDigits}");
            allText.Should().NotContain("{bankName}");
            allText.Should().NotContain("{companyName}");
            allText.Should().NotContain("{siren}");
            allText.Should().NotContain("{companyAddressStreet}");
            allText.Should().NotContain("{companyAddressZipCode}");
            allText.Should().NotContain("{companyAddressCountry}");
            allText.Should().NotContain("{companyAddressComplements}");
            allText.Should().NotContain("{signatory}");
            allText.Should().NotContain("{companyAddress}");
        }

        [Fact]
        public void ReplaceText_CMapFont_Tj_CanEncode_ReplacesViaMap()
        {
            var templateBytes = CreateCMapFontPdf("{bankCode}");
            var replacer = new PdfSharpTextReplacer();

            var result = replacer.ReplaceText(templateBytes, [new("{bankCode}", "12345")]);

            result.Should().NotBeNull();

            using var ms = new MemoryStream(result);
            using var pdfDoc = PdfReader.Open(ms, PdfDocumentOpenMode.Modify);
            var allText = ExtractAllText(ContentReader.ReadContent(pdfDoc.Pages[0]));

            allText.Should().Contain("12345");
            allText.Should().NotContain("{bankCode}");
        }

        [Fact]
        public void ReplaceText_CMapFont_TjArray_CanEncode_ReplacesViaMap()
        {
            var templateBytes = CreateCMapFontPdf("{bankCode}", useTjArray: true);
            var replacer = new PdfSharpTextReplacer();

            var result = replacer.ReplaceText(templateBytes, [new("{bankCode}", "12345")]);

            result.Should().NotBeNull();

            using var ms = new MemoryStream(result);
            using var pdfDoc = PdfReader.Open(ms, PdfDocumentOpenMode.Modify);
            var allText = ExtractAllText(ContentReader.ReadContent(pdfDoc.Pages[0]));

            allText.Should().Contain("12345");
            allText.Should().NotContain("{bankCode}");
        }

        [Fact]
        public void ReplaceText_CMapFont_CannotEncode_UsesFontFallback()
        {
            var templateBytes = CreateCMapFontPdf("{bankCode}", lettersOnly: true);
            var replacer = new PdfSharpTextReplacer();

            var result = replacer.ReplaceText(templateBytes, [new("{bankCode}", "12345")]);

            result.Should().NotBeNull();
            result.Length.Should().BeGreaterThan(0);
        }

        [Fact]
        public void ReplaceText_CMapFont_NoMatch_PreservesContent()
        {
            var templateBytes = CreateCMapFontPdf("plain text");
            var replacer = new PdfSharpTextReplacer();

            var result = replacer.ReplaceText(templateBytes, [new("{bankCode}", "12345")]);

            result.Should().NotBeNull();
            result.Length.Should().BeGreaterThan(0);
        }

        [Fact]
        public void ReplaceText_NoMatchingPlaceholders_ReturnsValidPdf()
        {
            var templateBytes = CreateTextPlaceholderTemplate();
            var replacer = new PdfSharpTextReplacer();

            var result = replacer.ReplaceText(templateBytes, [new("{nonExistent}", "value")]);

            result.Should().NotBeNull();
            result.Length.Should().BeGreaterThan(0);
        }

        [Fact]
        public void ReplaceText_EmptyReplacements_ReturnsValidPdf()
        {
            var templateBytes = CreateTextPlaceholderTemplate();
            var replacer = new PdfSharpTextReplacer();

            var result = replacer.ReplaceText(templateBytes, Array.Empty<Replacement>());

            result.Should().NotBeNull();
            result.Length.Should().BeGreaterThan(0);
        }

        [Fact]
        public void ReplaceText_MultiplePages_ReplacesAcrossPages()
        {
            var templateBytes = CreateMultiPageTemplate();
            var replacer = new PdfSharpTextReplacer();

            var result = replacer.ReplaceText(templateBytes, [new("{bankCode}", "12345")]);

            result.Should().NotBeNull();
            using var ms = new MemoryStream(result);
            using var pdfDoc = PdfReader.Open(ms, PdfDocumentOpenMode.Modify);
            pdfDoc.Pages.Count.Should().Be(2);
        }

        [Fact]
        public void ReplaceText_FontWithoutCMap_Tj_UsesDirectReplace()
        {
            var templateBytes = CreateType1FontPdf("{bankCode}");
            var replacer = new PdfSharpTextReplacer();

            var result = replacer.ReplaceText(templateBytes, [new("{bankCode}", "12345")]);

            var allText = ExtractTextFromPdf(result);
            allText.Should().Contain("12345");
            allText.Should().NotContain("{bankCode}");
        }

        [Fact]
        public void ReplaceText_FontWithoutCMap_TjArray_UsesDirectReplace()
        {
            var templateBytes = CreateType1FontPdf("{bankCode}", useTjArray: true);
            var replacer = new PdfSharpTextReplacer();

            var result = replacer.ReplaceText(templateBytes, [new("{bankCode}", "12345")]);

            var allText = ExtractTextFromPdf(result);
            allText.Should().Contain("12345");
            allText.Should().NotContain("{bankCode}");
        }

        [Fact]
        public void ReplaceText_FontWithoutCMap_NoMatch_PreservesText()
        {
            var templateBytes = CreateType1FontPdf("plain text no placeholders");
            var replacer = new PdfSharpTextReplacer();

            var result = replacer.ReplaceText(templateBytes, [new("{bankCode}", "12345")]);

            var allText = ExtractTextFromPdf(result);
            allText.Should().Contain("plain text no placeholders");
        }

        #region Helpers

        private static string ExtractTextFromPdf(byte[] pdfBytes)
        {
            using var ms = new MemoryStream(pdfBytes);
            using var doc = PdfReader.Open(ms, PdfDocumentOpenMode.Modify);
            return ExtractAllText(ContentReader.ReadContent(doc.Pages[0]));
        }

        private static string ExtractAllText(CSequence sequence)
        {
            var sb = new StringBuilder();
            foreach (var cObject in sequence)
            {
                if (cObject is COperator op &&
                    op.OpCode.OpCodeName is OpCodeName.Tj or OpCodeName.TJ)
                {
                    foreach (var operand in op.Operands)
                    {
                        if (operand is CString cString)
                        {
                            sb.Append(cString.Value);
                        }
                        else if (operand is CArray cArray)
                        {
                            foreach (var item in cArray)
                            {
                                if (item is CString arrayString)
                                {
                                    sb.Append(arrayString.Value);
                                }
                            }
                        }
                    }
                }
            }

            return sb.ToString();
        }

        private static byte[] CreateCMapFontPdf(string text, bool useTjArray = false, bool lettersOnly = false)
        {
            var doc = new PdfDocument();
            var page = doc.AddPage();

            SetupCMapFont(doc, page, lettersOnly);
            WriteTextContent(doc, page, text, useTjArray);

            using var ms = new MemoryStream();
            doc.Save(ms);
            return ms.ToArray();
        }

        private static byte[] CreateType1FontPdf(string text, bool useTjArray = false)
        {
            var doc = new PdfDocument();
            var page = doc.AddPage();

            SetupType1Font(doc, page);
            WriteTextContent(doc, page, text, useTjArray);

            using var ms = new MemoryStream();
            doc.Save(ms);
            return ms.ToArray();
        }

        private static void SetupCMapFont(PdfDocument doc, PdfPage page, bool lettersOnly)
        {
            var resources = new PdfDictionary(doc);
            page.Elements.SetObject("/Resources", resources);
            var fonts = new PdfDictionary(doc);
            resources.Elements.SetObject("/Font", fonts);

            var ranges = lettersOnly
                ? "3 beginbfrange\n<41> <5A> <0041>\n<61> <7A> <0061>\n<7B> <7D> <007B>\nendbfrange"
                : "1 beginbfrange\n<20> <7E> <0020>\nendbfrange";

            var cmapBytes = Encoding.ASCII.GetBytes(
                "begincmap\n1 begincodespacerange\n<00> <FF>\nendcodespacerange\n"
                + ranges
                + "\nendcmap\n");

            var toUnicode = new PdfDictionary(doc);
            doc.Internals.AddObject(toUnicode);
            toUnicode.CreateStream(cmapBytes);

            var fontDict = new PdfDictionary(doc);
            doc.Internals.AddObject(fontDict);
            fontDict.Elements.SetName("/Type", "/Font");
            fontDict.Elements.SetName("/Subtype", "/TrueType");
            fontDict.Elements.SetName("/BaseFont", "/TestFont");
            fontDict.Elements.SetReference("/ToUnicode", toUnicode);
            fonts.Elements.SetReference("/F1", fontDict);
        }

        private static void SetupType1Font(PdfDocument doc, PdfPage page)
        {
            var resources = new PdfDictionary(doc);
            page.Elements.SetObject("/Resources", resources);
            var fonts = new PdfDictionary(doc);
            resources.Elements.SetObject("/Font", fonts);

            var fontDict = new PdfDictionary(doc);
            doc.Internals.AddObject(fontDict);
            fontDict.Elements.SetName("/Type", "/Font");
            fontDict.Elements.SetName("/Subtype", "/Type1");
            fontDict.Elements.SetName("/BaseFont", "/Helvetica");
            fontDict.Elements.SetName("/Encoding", "/WinAnsiEncoding");
            fonts.Elements.SetReference("/F1", fontDict);
        }

        private static void WriteTextContent(PdfDocument doc, PdfPage page, string text, bool useTjArray)
        {
            var content = new CSequence();
            content.Add(new CComment { Text = "test" });
            content.Add(OpCodes.OperatorFromName("BT"));

            var tfOp = OpCodes.OperatorFromName("Tf");
            tfOp.Operands.Add((CObject)new CName { Name = "/F1" });
            tfOp.Operands.Add((CObject)new CReal { Value = 12 });
            content.Add(tfOp);

            if (useTjArray)
            {
                var mid = text.Length / 2;
                var array = new CArray();
                array.Add((CObject)new CString { CStringType = CStringType.String, Value = text[..mid] });
                array.Add((CObject)new CInteger { Value = -20 });
                array.Add((CObject)new CString { CStringType = CStringType.String, Value = text[mid..] });

                var tjOp = OpCodes.OperatorFromName("TJ");
                tjOp.Operands.Add((CObject)array);
                content.Add(tjOp);
            }
            else
            {
                var tjOp = OpCodes.OperatorFromName("Tj");
                tjOp.Operands.Add((CObject)new CString { CStringType = CStringType.String, Value = text });
                content.Add(tjOp);
            }

            content.Add(OpCodes.OperatorFromName("ET"));
            page.Contents.ReplaceContent(content);
        }

        private static byte[] CreateTextPlaceholderTemplate()
        {
            var doc = new PdfDocument();
            var page = doc.AddPage();

            using var gfx = XGraphics.FromPdfPage(page);
            var font = new XFont("Arial", 10);

            var placeholders = new[]
            {
                "{bankCode}", "{branchCode}", "{accountNumber}", "{checkDigits}",
                "{bankName}", "{companyName}", "{siren}",
                "{companyAddressStreet}", "{companyAddressZipCode}",
                "{companyAddressCountry}", "{companyAddressComplements}",
                "{signatory}", "{companyAddress}",
            };

            double y = 50;
            foreach (var placeholder in placeholders)
            {
                gfx.DrawString(placeholder, font, XBrushes.Black, new XPoint(50, y));
                y += 20;
            }

            using var ms = new MemoryStream();
            doc.Save(ms);
            return ms.ToArray();
        }

        private static byte[] CreateMultiPageTemplate()
        {
            var doc = new PdfDocument();

            for (int p = 0; p < 2; p++)
            {
                var page = doc.AddPage();
                using var gfx = XGraphics.FromPdfPage(page);
                var font = new XFont("Arial", 10);
                gfx.DrawString("{bankCode}", font, XBrushes.Black, new XPoint(50, 50));
            }

            using var ms = new MemoryStream();
            doc.Save(ms);
            return ms.ToArray();
        }

        #endregion
    }
}
