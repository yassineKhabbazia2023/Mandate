// <copyright file="SepaMandatePdfGenerator.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Application.BusinessHelpers;

using System.Globalization;
using KPMG.Pulse.Back.Accounting.Mandate.Application.Interfaces;
using KPMG.Pulse.Back.Accounting.Mandate.Application.Models;

/// <summary>
/// Generates SEPA mandate PDF files from the existing PDF template replacement infrastructure.
/// </summary>
public sealed class SepaMandatePdfGenerator(
    ISepaMandateTemplateProvider templateProvider,
    IPdfFormFieldFiller pdfFormFieldFiller) : ISepaMandatePdfGenerator
{
    /// <inheritdoc />
    public async Task<byte[]> GenerateAsync(SepaMandatePdfData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        var bankDetails = FrenchIbanBankDetails.Parse(data.Iban);
        var template = await templateProvider.GetTemplateAsync().ConfigureAwait(false);

        return pdfFormFieldFiller.FillFields(template, BuildReplacements(data, bankDetails));
    }

    private static Replacement[] BuildReplacements(
        SepaMandatePdfData data,
        FrenchIbanBankDetails bankDetails)
    {
        return
        [
            new Replacement("{ACCOUNTNUMBER}", data.AccountNumber),
            new Replacement("{RAISON_SOCIALE}", data.AccountHolder.ToUpperInvariant()),
            new Replacement("{ADRESSE}", data.Address.ToUpperInvariant()),
            new Replacement("{ADRESSE2}", data.AddressLine2 ?? string.Empty),
            new Replacement("{CP}", (data.PostalCode ?? string.Empty).ToUpperInvariant()),
            new Replacement("{VILLE}", (data.City ?? string.Empty).ToUpperInvariant()),
            new Replacement("{PAYS}", (data.Country ?? string.Empty).ToUpperInvariant()),
            new Replacement("{DATE1}", DateTime.Today.ToString("dd/MM/yyyy", CultureInfo.GetCultureInfo("fr-FR"))),
            new Replacement("{CODE_BANK}", bankDetails.BankCode),
            new Replacement("{COMPTE}", data.Iban),
            new Replacement("{BIC}", data.Bic),
            new Replacement("{CLRB}", bankDetails.CheckDigits)
        ];
    }

    private sealed record FrenchIbanBankDetails(
        string BankCode,
        string BranchCode,
        string AccountNumber,
        string CheckDigits)
    {
        /// <summary>
        /// Parses a French IBAN into RIB components used by existing mandate templates.
        /// </summary>
        /// <param name="iban">The IBAN.</param>
        /// <returns>The extracted bank details.</returns>
        public static FrenchIbanBankDetails Parse(string iban)
        {
            var normalizedIban = new string(
                iban.Where(candidate => !char.IsWhiteSpace(candidate)).ToArray())
                .ToUpperInvariant();

            if (!normalizedIban.StartsWith("FR", StringComparison.Ordinal)
                || normalizedIban.Length < 27)
            {
                throw new ArgumentException("A valid French IBAN is required.", nameof(iban));
            }

            return new FrenchIbanBankDetails(
                normalizedIban.Substring(4, 5),
                normalizedIban.Substring(9, 5),
                normalizedIban.Substring(14, 11),
                normalizedIban.Substring(25, 2));
        }
    }
}
