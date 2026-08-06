// <copyright file="FontCMap.cs" company="Pulse">
// Copyright (c) Pulse. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Application
{
    using System.Text;
    using System.Text.RegularExpressions;
    using PdfSharp.Pdf;
    using PdfSharp.Pdf.Advanced;

    internal sealed partial class FontCMap
    {
        private readonly Dictionary<int, char> glyphToUnicode;
        private readonly Dictionary<char, int> unicodeToGlyph;
        private readonly int bytesPerGlyph;

        private FontCMap(Dictionary<int, char> glyphToUnicode, Dictionary<char, int> unicodeToGlyph, int bytesPerGlyph)
        {
            this.glyphToUnicode = glyphToUnicode;
            this.unicodeToGlyph = unicodeToGlyph;
            this.bytesPerGlyph = bytesPerGlyph;
        }

        public static Dictionary<string, FontCMap> ParsePageFonts(PdfPage page)
        {
            return ParseFonts(page);
        }

        /// <summary>
        /// Parses the fonts declared by a PDF page or form appearance resource dictionary.
        /// </summary>
        /// <param name="resourceOwner">The PDF dictionary that owns the resources.</param>
        /// <returns>The character maps indexed by PDF font resource name.</returns>
        public static Dictionary<string, FontCMap> ParseFonts(PdfDictionary resourceOwner)
        {
            var result = new Dictionary<string, FontCMap>();
            var resources = resourceOwner.Elements.GetDictionary("/Resources");
            var fonts = resources?.Elements.GetDictionary("/Font");
            if (fonts == null)
            {
                return result;
            }

            foreach (var key in fonts.Elements.Keys)
            {
                var fontDict = ResolveDict(fonts.Elements[key]);
                if (fontDict == null)
                {
                    continue;
                }

                var cmap = ParseFromDictionary(fontDict);
                if (cmap != null)
                {
                    result[key] = cmap;
                }
            }

            return result;
        }

        public bool CanEncode(string unicodeText)
        {
            return unicodeText.All(c => this.unicodeToGlyph.ContainsKey(c));
        }

        public string Decode(string rawValue)
        {
            var sb = new StringBuilder();
            if (this.bytesPerGlyph == 2)
            {
                for (int i = 0; i + 1 < rawValue.Length; i += 2)
                {
                    int glyphId = ((byte)rawValue[i] << 8) | (byte)rawValue[i + 1];
                    sb.Append(this.glyphToUnicode.TryGetValue(glyphId, out var c) ? c : '?');
                }
            }
            else
            {
                foreach (char ch in rawValue)
                {
                    int glyphId = (byte)ch;
                    sb.Append(this.glyphToUnicode.TryGetValue(glyphId, out var c) ? c : ch);
                }
            }

            return sb.ToString();
        }

        public string Encode(string unicodeText)
        {
            var sb = new StringBuilder();
            foreach (var c in unicodeText)
            {
                if (this.unicodeToGlyph.TryGetValue(c, out var glyphId))
                {
                    if (this.bytesPerGlyph == 2)
                    {
                        sb.Append((char)(glyphId >> 8));
                        sb.Append((char)(glyphId & 0xFF));
                    }
                    else
                    {
                        sb.Append((char)glyphId);
                    }
                }
            }

            return sb.ToString();
        }

        [GeneratedRegex(@"begincodespacerange\s*<([0-9A-Fa-f]+)>")]
        private static partial Regex CodespaceRangePattern();

        [GeneratedRegex(@"beginbfchar\s*(.*?)\s*endbfchar", RegexOptions.Singleline)]
        private static partial Regex BfCharSectionPattern();

        [GeneratedRegex(@"<([0-9A-Fa-f]+)>\s*<([0-9A-Fa-f]+)>")]
        private static partial Regex HexPairPattern();

        [GeneratedRegex(@"beginbfrange\s*(.*?)\s*endbfrange", RegexOptions.Singleline)]
        private static partial Regex BfRangeSectionPattern();

        [GeneratedRegex(@"<([0-9A-Fa-f]+)>\s*<([0-9A-Fa-f]+)>\s*<([0-9A-Fa-f]+)>")]
        private static partial Regex HexTripletPattern();

        private static FontCMap? ParseFromDictionary(PdfDictionary fontDict)
        {
            var toUnicodeDict = ResolveDict(fontDict.Elements.GetObject("/ToUnicode"));
            if (toUnicodeDict?.Stream == null)
            {
                return null;
            }

            var cmapBytes = toUnicodeDict.Stream.UnfilteredValue;
            if (cmapBytes == null || cmapBytes.Length == 0)
            {
                return null;
            }

            var cmapText = Encoding.ASCII.GetString(cmapBytes);
            var glyphToUnicode = new Dictionary<int, char>();

            int bytesPerGlyph = 1;
            var codespaceMatch = CodespaceRangePattern().Match(cmapText);
            if (codespaceMatch.Success)
            {
                bytesPerGlyph = codespaceMatch.Groups[1].Value.Length / 2;
            }

            ParseBfCharSections(cmapText, glyphToUnicode);
            ParseBfRangeSections(cmapText, glyphToUnicode);

            if (glyphToUnicode.Count == 0)
            {
                return null;
            }

            var unicodeToGlyph = new Dictionary<char, int>();
            foreach (var (glyphId, unicode) in glyphToUnicode)
            {
                unicodeToGlyph.TryAdd(unicode, glyphId);
            }

            return new FontCMap(glyphToUnicode, unicodeToGlyph, bytesPerGlyph);
        }

        private static void ParseBfCharSections(string cmapText, Dictionary<int, char> glyphToUnicode)
        {
            var entries = BfCharSectionPattern().Matches(cmapText)
                .SelectMany(section => HexPairPattern().Matches(section.Groups[1].Value)
                    .Select(entry => entry.Groups));

            foreach (var groups in entries)
            {
                var glyphId = Convert.ToInt32(groups[1].Value, 16);
                var unicode = Convert.ToInt32(groups[2].Value, 16);
                if (unicode <= 0xFFFF)
                {
                    glyphToUnicode[glyphId] = (char)unicode;
                }
            }
        }

        private static void ParseBfRangeSections(string cmapText, Dictionary<int, char> glyphToUnicode)
        {
            var entries = BfRangeSectionPattern().Matches(cmapText)
                .SelectMany(section => HexTripletPattern().Matches(section.Groups[1].Value)
                    .Select(entry => entry.Groups));

            foreach (var groups in entries)
            {
                var startGlyph = Convert.ToInt32(groups[1].Value, 16);
                var endGlyph = Convert.ToInt32(groups[2].Value, 16);
                var startUnicode = Convert.ToInt32(groups[3].Value, 16);

                for (int g = startGlyph; g <= endGlyph; g++)
                {
                    var unicode = startUnicode + (g - startGlyph);
                    if (unicode <= 0xFFFF)
                    {
                        glyphToUnicode[g] = (char)unicode;
                    }
                }
            }
        }

        private static PdfDictionary? ResolveDict(PdfItem? item) => item switch
        {
            PdfDictionary dict => dict,
            PdfReference pdfRef => pdfRef.Value as PdfDictionary,
            _ => null,
        };
    }
}
