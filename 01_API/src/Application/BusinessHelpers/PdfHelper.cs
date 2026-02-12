// <copyright file="PdfHelper.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Application
{
    public class PdfHelper : IPdfHelper
    {
        private readonly IDatabaseService databaseService;
        private readonly IPdfTextReplacer pdfTextReplacer;

        public PdfHelper(IDatabaseService databaseService, IPdfTextReplacer pdfTextReplacer)
        {
            this.databaseService = databaseService;
            this.pdfTextReplacer = pdfTextReplacer;
        }

        public async Task<byte[]> GeneratePdfFromTemplateAsync(Collection source)
        {
            ArgumentNullException.ThrowIfNull(source);
            ArgumentNullException.ThrowIfNull(source.Bban);

            var template = await this.databaseService.GetPdfTemplateByBankCodeAsync(source.Bban.BankCode);
            var replacements = BuildReplacements(source);
            return this.pdfTextReplacer.ReplaceText(template, replacements);
        }

        private static Replacement[] BuildReplacements(Collection source)
        {
            return
            [
                new Replacement("{bankCode}", source.Bban!.BankCode),
                new Replacement("{branchCode}", source.Bban.BranchCode),
                new Replacement("{accountNumber}", source.Bban.AccountNumber),
                new Replacement("{checkDigits}", source.Bban.CheckDigits),
                new Replacement("{bankName}", source.Bban.Bank?.Name ?? string.Empty),
                new Replacement("{companyName}", source.Company?.Name ?? string.Empty),
                new Replacement("{siren}", source.Company?.SiretNumber ?? string.Empty),
                new Replacement("{companyAddressStreet}", source.Company?.Address?.Street ?? string.Empty),
                new Replacement("{companyAddressZipCode}", source.Company?.Address?.ZipCode ?? string.Empty),
                new Replacement("{companyAddressCountry}", source.Company?.Address?.Country ?? string.Empty),
                new Replacement("{companyAddressComplements}", source.Company?.Address?.Complements ?? string.Empty),
                new Replacement("{signatory}", source.Company?.Signatory?.FullName ?? string.Empty),
                new Replacement("{companyAddress}", source.Company?.Address?.FullAddress ?? string.Empty),
            ];
        }
    }
}
