// <copyright file="AsposeHelper.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Application
{
    using Aspose.Pdf.Text;
    using Microsoft.Extensions.Logging;

    public record Replacement(string WildCard, string ValueToWrite);

    public class AsposeHelper : IAsposeHelper
    {
        private readonly IDatabaseService databaseService;

        public AsposeHelper(IDatabaseService databaseService)
        {
            this.databaseService = databaseService;
            new Aspose.Pdf.License().SetLicense("Aspose.Total.lic");
        }

        public async Task<byte[]> GeneratePdfFromTemplateAsync(Collection source)
        {
            if (source == null || source.Bban == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var bankCode = source.Bban.BankCode;
            var template = await this.databaseService.GetPdfTemplateByBankCodeAsync(bankCode);
            await using var ms = new MemoryStream(template);
            using var pdfDocument = new Aspose.Pdf.Document(ms);

            var replacements = new[] {
                new Replacement("{bankCode}", source.Bban.BankCode),
                new Replacement("{branchCode}", source.Bban.BranchCode),
                new Replacement("{accountNumber}", source.Bban.AccountNumber),
                new Replacement("{checkDigits}", source.Bban.CheckDigits),
                new Replacement("{bankName}", source.Bban.Bank?.Name ?? string.Empty),
                new Replacement("{companyName}", source.Company?.Name ?? string.Empty),
                new Replacement("{siren}", source.Company?.SiretNumber ?? string.Empty),
                new Replacement("{companyAddressStreet}", source.Company?.Address?.Street ?? string.Empty),
                new Replacement("{companyAddressZipCode}", source.Company?.Address?.ZipCode ?? string.Empty),
                new Replacement("{companyAddressCountry}", source.Company?.Address ?.Country ?? string.Empty),
                new Replacement("{companyAddressComplements}", source.Company?.Address?.Complements ?? string.Empty),
                new Replacement("{signatory}", source.Company?.Signatory?.FullName ?? string.Empty),
                new Replacement("{companyAddress}", source.Company?.Address?.FullAddress ?? string.Empty)
            };

            ReplaceInDocument(pdfDocument, replacements);
            await using var msOut = new MemoryStream();
            pdfDocument.Save(msOut);
            return msOut.ToArray();
        }

        private static void ReplaceInDocument(Aspose.Pdf.Document pdfDocument, Replacement[] replacements)
        {
            FontRepository.Sources.Add(new FolderFontSource(AppDomain.CurrentDomain.BaseDirectory));

            foreach (var (wildCard, valueToWrite) in replacements)
            {
                var tfa = new TextFragmentAbsorber(wildCard);
                pdfDocument.Pages.Accept(tfa);
                foreach (TextFragment tf in tfa.TextFragments)
                {
                    tf.TextState.Font = FontRepository.FindFont("Arial");
                    tf.Text = tf.Text.Replace(wildCard, valueToWrite);
                }
            }
        }
    }
}
