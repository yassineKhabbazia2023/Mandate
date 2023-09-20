// <copyright file="AsposeHelper.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Application
{
    using System.Text;
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

            this.ReplaceInDocument(pdfDocument, "{bankCode}", source.Bban.BankCode);
            this.ReplaceInDocument(pdfDocument, "{branchCode}", source.Bban.BranchCode);
            this.ReplaceInDocument(pdfDocument, "{accountNumber}", source.Bban.AccountNumber);
            this.ReplaceInDocument(pdfDocument, "{checkDigits}", source.Bban.CheckDigits);
            if (source.Bban.Bank != null)
            {
                this.ReplaceInDocument(pdfDocument, "{bankName}", source.Bban.Bank.Name ?? string.Empty);
            }

            if (source.Company != null)
            {
                this.ReplaceInDocument(pdfDocument, "{companyName}", source.Company.Name ?? string.Empty);
                this.ReplaceInDocument(pdfDocument, "{siren}", source.Company.SiretNumber ?? string.Empty);
                if (source.Company.Signatory != null)
                {
                    var sbSignatory = new StringBuilder();
                    sbSignatory.Append(source.Company.Signatory.Title ?? string.Empty).Append(' ');
                    sbSignatory.Append(source.Company.Signatory.FirstName ?? string.Empty).Append(' ');
                    sbSignatory.Append(source.Company.Signatory.LastName ?? string.Empty);
                    this.ReplaceInDocument(pdfDocument, "{signatory}", sbSignatory.ToString());
                    if (source.Company.Signatory.Address != null)
                    {
                        var sbAddress = new StringBuilder();
                        sbAddress.Append(source.Company.Signatory.Address.Street ?? string.Empty);
                        if (!string.IsNullOrWhiteSpace(source.Company.Signatory.Address.Complements))
                        {
                            sbAddress.Append("  -  ").Append(source.Company.Signatory.Address.Complements ?? string.Empty);
                        }

                        sbAddress.Append("  -  ");
                        sbAddress.Append(source.Company.Signatory.Address.ZipCode ?? string.Empty).Append(' ');
                        sbAddress.Append(source.Company.Signatory.Address.City ?? string.Empty).Append("  -  ");
                        sbAddress.Append(source.Company.Signatory.Address.Country ?? string.Empty);
                        this.ReplaceInDocument(pdfDocument, "{companyAddress}", sbAddress.ToString());
                    }
                }
            }

            using var msOut = new MemoryStream();
            pdfDocument.Save(msOut);
            return msOut.ToArray();
        }

        public async Task<byte[]> DeleteFirstPageFromPdf(byte[] sourcePdf)
        {
            await Task.CompletedTask;
            throw new NotImplementedException();
        }

        private void ReplaceInDocument(Aspose.Pdf.Document pdfDocument, string oldValue, string newValue)
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
