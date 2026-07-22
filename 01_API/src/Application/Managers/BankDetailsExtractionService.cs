// <copyright file="BankDetailsExtractionService.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Application.Managers;

using KPMG.Pulse.Back.Accounting.Mandate.Application.Interfaces;
using KPMG.Pulse.Back.Accounting.Mandate.Application.Models;
using Microsoft.Extensions.Logging;

/// <summary>
/// Validates and extracts banking details from French IBAN and BIC values.
/// </summary>
/// <param name="logger">The logger.</param>
public sealed class BankDetailsExtractionService(
    ILogger<BankDetailsExtractionService> logger) : IBankDetailsExtractionService
{
    private const int FrenchIbanLength = 27;
    private const int ShortBicLength = 8;
    private const int LongBicLength = 11;

    /// <inheritdoc />
    public BankDetailsExtractionResult Extract(string iban, string bic)
    {
        logger.LogInformation("Starting bank details extraction");

        var validationErrors = new Dictionary<string, string[]>();
        var normalizedIban = ValidateAndNormalizeIban(iban, validationErrors);
        var normalizedBic = ValidateAndNormalizeBic(bic, validationErrors);

        if (normalizedIban is not null
            && normalizedBic is not null
            && !normalizedIban.AsSpan(0, 2).SequenceEqual(normalizedBic.AsSpan(4, 2)))
        {
            validationErrors[nameof(bic)] = ["The BIC country code must match the IBAN country code."];
        }

        if (validationErrors.Count > 0)
        {
            logger.LogWarning("Bank details extraction rejected invalid banking identifiers");
            return new BankDetailsExtractionResult(null, validationErrors);
        }

        var details = ExtractDetails(normalizedIban!, normalizedBic!);
        logger.LogInformation("Completed bank details extraction");
        return new BankDetailsExtractionResult(details, validationErrors);
    }

    /// <summary>
    /// Validates and normalizes a French IBAN.
    /// </summary>
    /// <param name="iban">The IBAN to validate.</param>
    /// <param name="validationErrors">The validation-error collection.</param>
    /// <returns>The normalized IBAN, or <c>null</c> when validation fails.</returns>
    private static string? ValidateAndNormalizeIban(
        string iban,
        IDictionary<string, string[]> validationErrors)
    {
        if (string.IsNullOrEmpty(iban))
        {
            validationErrors[nameof(iban)] = ["The IBAN is required."];
            return null;
        }

        if (iban.Any(char.IsWhiteSpace))
        {
            validationErrors[nameof(iban)] = ["The IBAN must not contain whitespace."];
            return null;
        }

        var normalizedIban = iban.ToUpperInvariant();
        if (normalizedIban.Length != FrenchIbanLength
            || !normalizedIban.StartsWith("FR", StringComparison.Ordinal)
            || !AreDigits(normalizedIban.AsSpan(2, 2))
            || !AreDigits(normalizedIban.AsSpan(4, 10))
            || !AreUppercaseLettersOrDigits(normalizedIban.AsSpan(14, 11))
            || !AreDigits(normalizedIban.AsSpan(25, 2))
            || !HasValidIbanChecksum(normalizedIban))
        {
            validationErrors[nameof(iban)] = ["A valid French IBAN is required."];
            return null;
        }

        return normalizedIban;
    }

    /// <summary>
    /// Validates and normalizes a BIC.
    /// </summary>
    /// <param name="bic">The BIC to validate.</param>
    /// <param name="validationErrors">The validation-error collection.</param>
    /// <returns>The normalized BIC, or <c>null</c> when validation fails.</returns>
    private static string? ValidateAndNormalizeBic(
        string bic,
        IDictionary<string, string[]> validationErrors)
    {
        if (string.IsNullOrEmpty(bic))
        {
            validationErrors[nameof(bic)] = ["The BIC is required."];
            return null;
        }

        if (bic.Any(char.IsWhiteSpace))
        {
            validationErrors[nameof(bic)] = ["The BIC must not contain whitespace."];
            return null;
        }

        var normalizedBic = bic.ToUpperInvariant();
        if ((normalizedBic.Length != ShortBicLength && normalizedBic.Length != LongBicLength)
            || !AreUppercaseLetters(normalizedBic.AsSpan(0, 6))
            || !AreUppercaseLettersOrDigits(normalizedBic.AsSpan(6, 2))
            || (normalizedBic.Length == LongBicLength
                && !AreUppercaseLettersOrDigits(normalizedBic.AsSpan(8, 3))))
        {
            validationErrors[nameof(bic)] = ["A valid 8 or 11 character BIC is required."];
            return null;
        }

        return normalizedBic;
    }

    /// <summary>
    /// Extracts response data from validated, normalized identifiers.
    /// </summary>
    /// <param name="iban">The normalized IBAN.</param>
    /// <param name="bic">The normalized BIC.</param>
    /// <returns>The extracted bank details.</returns>
    private static ExtractedBankDetails ExtractDetails(string iban, string bic)
    {
        var ibanDetails = new ExtractedIbanDetails(
            iban[..2],
            iban.Substring(2, 2),
            iban[4..]);
        var ribDetails = new ExtractedRibDetails(
            iban.Substring(4, 5),
            iban.Substring(9, 5),
            iban.Substring(14, 11),
            iban.Substring(25, 2));
        var bicDetails = new ExtractedBicDetails(
            bic[..4],
            bic.Substring(4, 2),
            bic.Substring(6, 2),
            bic.Length == LongBicLength ? bic[8..] : null);

        return new ExtractedBankDetails(ibanDetails, ribDetails, bicDetails, bicDetails.BankCode);
    }

    /// <summary>
    /// Validates an IBAN using the ISO 13616 modulo-97 checksum.
    /// </summary>
    /// <param name="iban">The normalized IBAN.</param>
    /// <returns><c>true</c> when the checksum is valid; otherwise, <c>false</c>.</returns>
    private static bool HasValidIbanChecksum(string iban)
    {
        var rearrangedIban = string.Concat(iban.AsSpan(4), iban.AsSpan(0, 4));
        var remainder = 0;

        foreach (var character in rearrangedIban)
        {
            if (char.IsDigit(character))
            {
                remainder = ((remainder * 10) + (character - '0')) % 97;
                continue;
            }

            var numericValue = character - 'A' + 10;
            remainder = ((remainder * 100) + numericValue) % 97;
        }

        return remainder == 1;
    }

    /// <summary>
    /// Determines whether every character is an ASCII digit.
    /// </summary>
    /// <param name="value">The characters to inspect.</param>
    /// <returns><c>true</c> when every character is a digit; otherwise, <c>false</c>.</returns>
    private static bool AreDigits(ReadOnlySpan<char> value)
    {
        return value.IndexOfAnyExceptInRange('0', '9') < 0;
    }

    /// <summary>
    /// Determines whether every character is an uppercase ASCII letter.
    /// </summary>
    /// <param name="value">The characters to inspect.</param>
    /// <returns><c>true</c> when every character is an uppercase letter; otherwise, <c>false</c>.</returns>
    private static bool AreUppercaseLetters(ReadOnlySpan<char> value)
    {
        return value.IndexOfAnyExceptInRange('A', 'Z') < 0;
    }

    /// <summary>
    /// Determines whether every character is an uppercase ASCII letter or digit.
    /// </summary>
    /// <param name="value">The characters to inspect.</param>
    /// <returns><c>true</c> when every character is allowed; otherwise, <c>false</c>.</returns>
    private static bool AreUppercaseLettersOrDigits(ReadOnlySpan<char> value)
    {
        foreach (var character in value)
        {
            if (!char.IsAsciiLetterUpper(character) && !char.IsAsciiDigit(character))
            {
                return false;
            }
        }

        return true;
    }
}
