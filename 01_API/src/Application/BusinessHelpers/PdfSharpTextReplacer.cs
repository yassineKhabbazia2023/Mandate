// <copyright file="PdfSharpTextReplacer.cs" company="Pulse">
// Copyright (c) Pulse. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Application
{
    using PdfSharp.Pdf;
    using PdfSharp.Pdf.Content;
    using PdfSharp.Pdf.Content.Objects;
    using PdfSharp.Pdf.IO;

    internal sealed class PdfSharpTextReplacer : IPdfTextReplacer
    {
        public byte[] ReplaceText(byte[] pdfTemplate, Replacement[] replacements)
        {
            using var ms = new MemoryStream(pdfTemplate);
            using var doc = PdfReader.Open(ms, PdfDocumentOpenMode.Modify);

            foreach (var page in doc.Pages)
            {
                ReplaceTextInPage(page, doc, replacements);
            }

            using var msOut = new MemoryStream();
            doc.Save(msOut);
            return msOut.ToArray();
        }

        private static void ReplaceTextInPage(PdfPage page, PdfDocument doc, Replacement[] replacements)
        {
            var fontMaps = FontCMap.ParsePageFonts(page);
            var content = ContentReader.ReadContent(page);
            string? arialFontName = null;
            string? currentFont = null;
            double currentFontSize = 10;
            var newContent = new CSequence();

            foreach (var cObject in content)
            {
                if (cObject is not COperator op)
                {
                    newContent.Add(cObject);
                    continue;
                }

                if (op.OpCode.OpCodeName == OpCodeName.Tf)
                {
                    currentFont = op.Operands.OfType<CName>().FirstOrDefault()?.Name;
                    currentFontSize = GetFontSize(op);
                    newContent.Add(cObject);
                    continue;
                }

                if (op.OpCode.OpCodeName is OpCodeName.Tj or OpCodeName.TJ)
                {
                    fontMaps.TryGetValue(currentFont ?? "", out var map);
                    var fallbackText = TryReplaceText(op, map, replacements);
                    if (fallbackText != null)
                    {
                        arialFontName ??= ArialFontEmbedder.EnsureFont(page, doc);
                        EmitFontSwitchedText(newContent, fallbackText, arialFontName, currentFont ?? "", currentFontSize);
                        continue;
                    }
                }

                newContent.Add(cObject);
            }

            page.Contents.ReplaceContent(newContent);
        }

        private static string? TryReplaceText(COperator op, FontCMap? map, Replacement[] replacements)
        {
            if (map == null)
            {
                DirectReplace(op, replacements);
                return null;
            }

            var decoded = DecodeOperatorText(op, map);
            var replaced = ApplyReplacements(decoded, replacements, out bool matched);

            if (!matched)
            {
                return null;
            }

            if (map.CanEncode(replaced))
            {
                SetOperatorText(op, map.Encode(replaced));
                return null;
            }

            return replaced;
        }

        private static string DecodeOperatorText(COperator op, FontCMap map)
        {
            if (op.OpCode.OpCodeName == OpCodeName.Tj)
            {
                var raw = op.Operands.OfType<CString>().FirstOrDefault()?.Value ?? "";
                return map.Decode(raw);
            }

            var rawConcat = string.Concat(
                op.Operands.OfType<CArray>()
                    .SelectMany(a => a.OfType<CString>())
                    .Select(s => s.Value));
            return map.Decode(rawConcat);
        }

        private static string ApplyReplacements(string text, Replacement[] replacements, out bool matched)
        {
            matched = false;
            var result = text;
            foreach (var (placeholder, value) in replacements)
            {
                if (result.Contains(placeholder))
                {
                    result = result.Replace(placeholder, value);
                    matched = true;
                }
            }

            return result;
        }

        private static void SetOperatorText(COperator op, string encodedValue)
        {
            if (op.OpCode.OpCodeName == OpCodeName.Tj)
            {
                var cString = op.Operands.OfType<CString>().FirstOrDefault();
                if (cString != null)
                {
                    cString.Value = encodedValue;
                }
            }
            else
            {
                foreach (var operand in op.Operands)
                {
                    if (operand is CArray cArray)
                    {
                        cArray.Clear();
                        cArray.Add((CObject)new CString { CStringType = CStringType.String, Value = encodedValue });
                    }
                }
            }
        }

        private static void EmitFontSwitchedText(
            CSequence output, string text, string targetFont, string originalFont, double fontSize)
        {
            output.Add(CreateTfOperator(targetFont, fontSize));
            output.Add(CreateTjOperator(text));
            output.Add(CreateTfOperator(originalFont, fontSize));
        }

        private static void DirectReplace(COperator op, Replacement[] replacements)
        {
            if (op.OpCode.OpCodeName == OpCodeName.Tj)
            {
                var cString = op.Operands.OfType<CString>().FirstOrDefault();
                if (cString != null)
                {
                    cString.Value = ApplyReplacements(cString.Value, replacements, out _);
                }
            }
            else
            {
                foreach (var cArray in op.Operands.OfType<CArray>())
                {
                    var concat = string.Concat(cArray.OfType<CString>().Select(s => s.Value));
                    var replaced = ApplyReplacements(concat, replacements, out bool matched);
                    if (matched)
                    {
                        cArray.Clear();
                        cArray.Add((CObject)new CString { CStringType = CStringType.String, Value = replaced });
                    }
                }
            }
        }

        private static double GetFontSize(COperator tfOp)
        {
            foreach (var operand in tfOp.Operands)
            {
                if (operand is CReal r) return r.Value;
                if (operand is CInteger i) return i.Value;
            }

            return 10;
        }

        private static COperator CreateTfOperator(string fontName, double fontSize)
        {
            var op = OpCodes.OperatorFromName("Tf");
            op.Operands.Add((CObject)new CName { Name = fontName });
            op.Operands.Add((CObject)new CReal { Value = fontSize });
            return op;
        }

        private static COperator CreateTjOperator(string text)
        {
            var op = OpCodes.OperatorFromName("Tj");
            op.Operands.Add((CObject)new CString { CStringType = CStringType.String, Value = text });
            return op;
        }
    }
}
