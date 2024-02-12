// <copyright file="AsposeHelper.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Application
{
    using System.Text;
    using Aspose.Pdf.Facades;
    using Aspose.Pdf.Text;
    using Microsoft.Extensions.Logging;

    public class AsposeHelper : IAsposeHelper
    {
        private readonly ILogger<AsposeHelper> logger;
        private readonly IDatabaseService databaseService;

        public AsposeHelper(ILogger<AsposeHelper> logger, IDatabaseService databaseService)
        {
            this.logger = logger;
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
            var template = await this.databaseService.GetPdfTemplateByBankCodeAsync(bankCode).ConfigureAwait(false);
            using var ms = new MemoryStream(template);
            using var pdfDocument = new Aspose.Pdf.Document(ms);

            ReplaceInDocument(pdfDocument, "{bankCode}", source.Bban.BankCode);
            ReplaceInDocument(pdfDocument, "{branchCode}", source.Bban.BranchCode);
            ReplaceInDocument(pdfDocument, "{accountNumber}", source.Bban.AccountNumber);
            ReplaceInDocument(pdfDocument, "{checkDigits}", source.Bban.CheckDigits);
            if (source.Bban.Bank != null)
            {
                ReplaceInDocument(pdfDocument, "{bankName}", source.Bban.Bank.Name ?? string.Empty);
            }

            if (source.Company != null)
            {
                ReplaceInDocument(pdfDocument, "{companyName}", source.Company.Name ?? string.Empty);
                ReplaceInDocument(pdfDocument, "{siren}", source.Company.SiretNumber ?? string.Empty);
                if (source.Company.Signatory != null)
                {
                    var sbSignatory = new StringBuilder();
                    sbSignatory.Append(source.Company.Signatory.Title ?? string.Empty).Append(' ');
                    sbSignatory.Append(source.Company.Signatory.FirstName ?? string.Empty).Append(' ');
                    sbSignatory.Append(source.Company.Signatory.LastName ?? string.Empty);
                    ReplaceInDocument(pdfDocument, "{signatory}", sbSignatory.ToString());
                }

                if (source.Company.Address != null)
                {
                    var sbAddress = new StringBuilder();
                    sbAddress.Append(source.Company.Address.Street ?? string.Empty);
                    if (!string.IsNullOrWhiteSpace(source.Company.Address.Complements))
                    {
                        sbAddress.Append("  -  ").Append(source.Company.Address.Complements ?? string.Empty);
                    }

                    sbAddress.Append("  -  ");
                    sbAddress.Append(source.Company.Address.ZipCode ?? string.Empty).Append(' ');
                    sbAddress.Append(source.Company.Address.City ?? string.Empty).Append("  -  ");
                    sbAddress.Append(source.Company.Address.Country ?? string.Empty);
                    ReplaceInDocument(pdfDocument, "{companyAddress}", sbAddress.ToString());
                }
            }

            using var msOut = new MemoryStream();
            pdfDocument.Save(msOut);
            return msOut.ToArray();
        }

        public byte[] DeleteFirstPageFromPdf(MemoryStream sourcePdf)
        {
            const string origin = $"{nameof(AsposeHelper)}::{nameof(this.DeleteFirstPageFromPdf)}";

            this.logger.LogInformation("{origin} - Instanciating a new PdfFileEditor to remove the first page for the PDF mandat provided by JeDeclare...", origin);

            var pdfEditor = new PdfFileEditor();

            using var streamOut = new MemoryStream();
            int[] pagesToDelete = new int[] { 1 };

            // Delete pages
            pdfEditor.Delete(sourcePdf, pagesToDelete, streamOut);

            return streamOut.ToArray();
        }

        private static void ReplaceInDocument(Aspose.Pdf.Document pdfDocument, string oldValue, string newValue)
        {
            var tfa = new TextFragmentAbsorber(oldValue);
            pdfDocument.Pages.Accept(tfa);
            var tfc = tfa.TextFragments;
            foreach (TextFragment tf in tfc)
            {
                tf.Text = tf.Text.Replace(oldValue, newValue);
            }
        }
    }
}
