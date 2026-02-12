// <copyright file="FontCMapTest.cs" company="Pulse">
// Copyright (c) Pulse. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Application.Tests
{
    using PdfSharp.Pdf;
    using PdfSharp.Pdf.Advanced;

    public class FontCMapTest
    {
        private const string BfCharCMap = @"
/CIDInit /ProcSet findresource begin
12 dict begin
begincmap
/CIDSystemInfo << /Registry (Adobe) /Ordering (UCS) /Supplement 0 >> def
/CMapName /Adobe-Identity-UCS def
/CMapType 2 def
1 begincodespacerange
<00> <FF>
endcodespacerange
5 beginbfchar
<41> <0041>
<42> <0042>
<43> <0043>
<7B> <007B>
<7D> <007D>
endbfchar
endcmap
CMapName currentdict /CMap defineresource pop
end
end
";

        private const string BfRangeCMap = @"
/CIDInit /ProcSet findresource begin
12 dict begin
begincmap
/CIDSystemInfo << /Registry (Adobe) /Ordering (UCS) /Supplement 0 >> def
/CMapName /Adobe-Identity-UCS def
/CMapType 2 def
1 begincodespacerange
<00> <FF>
endcodespacerange
1 beginbfrange
<41> <5A> <0041>
endbfrange
endcmap
CMapName currentdict /CMap defineresource pop
end
end
";

        private const string TwoByteCMap = @"
/CIDInit /ProcSet findresource begin
12 dict begin
begincmap
/CIDSystemInfo << /Registry (Adobe) /Ordering (UCS) /Supplement 0 >> def
/CMapName /Adobe-Identity-UCS def
/CMapType 2 def
1 begincodespacerange
<0000> <FFFF>
endcodespacerange
3 beginbfchar
<0041> <0041>
<0042> <0042>
<0043> <0043>
endbfchar
endcmap
CMapName currentdict /CMap defineresource pop
end
end
";

        [Fact]
        public void ParsePageFonts_WithBfCharCMap_ReturnsMaps()
        {
            var (doc, page) = CreatePageWithCMapFont("/F1", BfCharCMap);

            var maps = FontCMap.ParsePageFonts(page);

            maps.Should().ContainKey("/F1");
        }

        [Fact]
        public void ParsePageFonts_WithBfRangeCMap_ReturnsMaps()
        {
            var (doc, page) = CreatePageWithCMapFont("/F1", BfRangeCMap);

            var maps = FontCMap.ParsePageFonts(page);

            maps.Should().ContainKey("/F1");
        }

        [Fact]
        public void ParsePageFonts_EmptyPage_ReturnsEmpty()
        {
            var doc = new PdfDocument();
            var page = doc.AddPage();

            var maps = FontCMap.ParsePageFonts(page);

            maps.Should().BeEmpty();
        }

        [Fact]
        public void ParsePageFonts_FontWithoutToUnicode_ExcludesFromResult()
        {
            var doc = new PdfDocument();
            var page = doc.AddPage();

            var resources = new PdfDictionary(doc);
            page.Elements.SetObject("/Resources", resources);
            var fonts = new PdfDictionary(doc);
            resources.Elements.SetObject("/Font", fonts);

            var fontDict = new PdfDictionary(doc);
            doc.Internals.AddObject(fontDict);
            fontDict.Elements.SetName("/Type", "/Font");
            fontDict.Elements.SetName("/Subtype", "/Type1");
            fontDict.Elements.SetName("/BaseFont", "/Helvetica");
            fonts.Elements.SetReference("/F1", fontDict);

            var maps = FontCMap.ParsePageFonts(page);

            maps.Should().NotContainKey("/F1");
        }

        [Fact]
        public void ParsePageFonts_PageWithResourcesButNoFonts_ReturnsEmpty()
        {
            var doc = new PdfDocument();
            var page = doc.AddPage();

            var resources = new PdfDictionary(doc);
            page.Elements.SetObject("/Resources", resources);

            var maps = FontCMap.ParsePageFonts(page);

            maps.Should().BeEmpty();
        }

        [Fact]
        public void Decode_SingleByte_BfChar_ReturnsUnicode()
        {
            var (doc, page) = CreatePageWithCMapFont("/F1", BfCharCMap);
            var maps = FontCMap.ParsePageFonts(page);
            var map = maps["/F1"];

            var decoded = map.Decode("\x41\x42\x43");

            decoded.Should().Be("ABC");
        }

        [Fact]
        public void Decode_SingleByte_UnknownGlyph_ReturnsOriginalChar()
        {
            var (doc, page) = CreatePageWithCMapFont("/F1", BfCharCMap);
            var maps = FontCMap.ParsePageFonts(page);
            var map = maps["/F1"];

            var decoded = map.Decode("\x30");

            decoded.Should().Be("0");
        }

        [Fact]
        public void Decode_TwoByte_ReturnsUnicode()
        {
            var (doc, page) = CreatePageWithCMapFont("/F1", TwoByteCMap);
            var maps = FontCMap.ParsePageFonts(page);
            var map = maps["/F1"];

            var decoded = map.Decode("\x00\x41\x00\x42");

            decoded.Should().Be("AB");
        }

        [Fact]
        public void Decode_TwoByte_UnknownGlyph_ReturnsQuestionMark()
        {
            var (doc, page) = CreatePageWithCMapFont("/F1", TwoByteCMap);
            var maps = FontCMap.ParsePageFonts(page);
            var map = maps["/F1"];

            var decoded = map.Decode("\x99\x99");

            decoded.Should().Be("?");
        }

        [Fact]
        public void Encode_SingleByte_ReturnsGlyphBytes()
        {
            var (doc, page) = CreatePageWithCMapFont("/F1", BfCharCMap);
            var maps = FontCMap.ParsePageFonts(page);
            var map = maps["/F1"];

            var encoded = map.Encode("ABC");

            encoded.Should().Be("\x41\x42\x43");
        }

        [Fact]
        public void Encode_TwoByte_ReturnsGlyphBytes()
        {
            var (doc, page) = CreatePageWithCMapFont("/F1", TwoByteCMap);
            var maps = FontCMap.ParsePageFonts(page);
            var map = maps["/F1"];

            var encoded = map.Encode("AB");

            encoded.Should().Be("\x00\x41\x00\x42");
        }

        [Fact]
        public void Encode_UnknownChar_SkipsIt()
        {
            var (doc, page) = CreatePageWithCMapFont("/F1", BfCharCMap);
            var maps = FontCMap.ParsePageFonts(page);
            var map = maps["/F1"];

            var encoded = map.Encode("AZB");

            encoded.Should().Be("\x41\x42");
        }

        [Fact]
        public void CanEncode_AllCharsInMap_ReturnsTrue()
        {
            var (doc, page) = CreatePageWithCMapFont("/F1", BfCharCMap);
            var maps = FontCMap.ParsePageFonts(page);
            var map = maps["/F1"];

            map.CanEncode("ABC").Should().BeTrue();
        }

        [Fact]
        public void CanEncode_SomeCharsNotInMap_ReturnsFalse()
        {
            var (doc, page) = CreatePageWithCMapFont("/F1", BfCharCMap);
            var maps = FontCMap.ParsePageFonts(page);
            var map = maps["/F1"];

            map.CanEncode("ABCZ").Should().BeFalse();
        }

        [Fact]
        public void Decode_AndEncode_RoundTrip_PreservesText()
        {
            var (doc, page) = CreatePageWithCMapFont("/F1", BfCharCMap);
            var maps = FontCMap.ParsePageFonts(page);
            var map = maps["/F1"];

            var original = "ABC";
            var encoded = map.Encode(original);
            var decoded = map.Decode(encoded);

            decoded.Should().Be(original);
        }

        [Fact]
        public void ParsePageFonts_BfRange_DecodesFullRange()
        {
            var (doc, page) = CreatePageWithCMapFont("/F1", BfRangeCMap);
            var maps = FontCMap.ParsePageFonts(page);
            var map = maps["/F1"];

            map.CanEncode("ABCXYZ").Should().BeTrue();
            map.Decode("\x41\x5A").Should().Be("AZ");
        }

        private static (PdfDocument doc, PdfPage page) CreatePageWithCMapFont(string fontName, string cmapContent)
        {
            var doc = new PdfDocument();
            var page = doc.AddPage();

            var resources = new PdfDictionary(doc);
            page.Elements.SetObject("/Resources", resources);
            var fonts = new PdfDictionary(doc);
            resources.Elements.SetObject("/Font", fonts);

            var toUnicodeDict = new PdfDictionary(doc);
            doc.Internals.AddObject(toUnicodeDict);
            toUnicodeDict.CreateStream(System.Text.Encoding.ASCII.GetBytes(cmapContent));

            var fontDict = new PdfDictionary(doc);
            doc.Internals.AddObject(fontDict);
            fontDict.Elements.SetName("/Type", "/Font");
            fontDict.Elements.SetName("/Subtype", "/TrueType");
            fontDict.Elements.SetName("/BaseFont", "/TestFont");
            fontDict.Elements.SetReference("/ToUnicode", toUnicodeDict);
            fonts.Elements.SetReference(fontName, fontDict);

            return (doc, page);
        }
    }
}
