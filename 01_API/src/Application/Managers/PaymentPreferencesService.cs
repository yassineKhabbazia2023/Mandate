// <copyright file="PaymentPreferencesService.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Application.Managers;

using KPMG.Pulse.Back.Accounting.Mandate.Application.Interfaces;
using KPMG.Pulse.Back.Accounting.Mandate.Application.Models;
using Microsoft.Extensions.Logging;

/// <inheritdoc />
/// <param name="paymentPreferenceStore">The payment preference store.</param>
/// <param name="paymentPreferenceCleanupStore">The payment preference cleanup store.</param>
/// <param name="sepaMandateStore">The SEPA mandate store.</param>
/// <param name="sepaMandatePdfGenerator">The SEPA mandate PDF generator.</param>
/// <param name="getAcceptClient">The GetAccept client.</param>
/// <param name="strategies">The payment preference write strategies.</param>
/// <param name="readStrategies">The payment preference read strategies.</param>
/// <param name="logger">The logger.</param>
public sealed class PaymentPreferencesService(
    IPaymentPreferenceStore paymentPreferenceStore,
    IPaymentPreferenceCleanupStore paymentPreferenceCleanupStore,
    ISepaMandateStore sepaMandateStore,
    ISepaMandatePdfGenerator sepaMandatePdfGenerator,
    IGetAcceptClient getAcceptClient,
    IEnumerable<IPaymentPreferenceStrategy> strategies,
    IEnumerable<IPaymentPreferenceReadStrategy> readStrategies,
    ILogger<PaymentPreferencesService> logger) : IPaymentPreferencesService
{
    /// <inheritdoc />
    public async Task<PaymentPreferenceResult> GetAsync(int accountId)
    {
        if (!await paymentPreferenceStore.AccountExistsAsync(accountId))
        {
            logger.LogWarning("Payment preference requested for unknown account {AccountId}", accountId);
            return new PaymentPreferenceResult(false, null);
        }

        var preference = await paymentPreferenceStore.GetByAccountIdAsync(accountId);
        var paymentType = MapPaymentType(preference?.PaymentType);
        var readStrategy = ResolveReadStrategy(paymentType);
        return await readStrategy.GetAsync(accountId, preference);
    }

    /// <inheritdoc />
    public async Task<bool> MarkSentToAkuiteoAsync(int accountId)
    {
        if (!await paymentPreferenceStore.AccountExistsAsync(accountId))
        {
            logger.LogWarning("Mark sent to Akuiteo requested for unknown account {AccountId}", accountId);
            return false;
        }

        var marked = await sepaMandateStore.MarkSentToAkuiteoAsync(accountId, DateTime.UtcNow);
        if (marked)
        {
            logger.LogInformation("Marked SEPA mandate as sent to Akuiteo for account {AccountId}", accountId);
        }

        return marked;
    }

    /// <inheritdoc />
    public async Task<bool> SaveSignedMandateDocumentIdAsync(int accountId, string signedMandateDocumentId)
    {
        if (string.IsNullOrWhiteSpace(signedMandateDocumentId))
        {
            logger.LogWarning("Signed mandate document identifier save requested without document identifier for account {AccountId}", accountId);
            return false;
        }

        if (!await paymentPreferenceStore.AccountExistsAsync(accountId))
        {
            logger.LogWarning("Signed mandate document identifier save requested for unknown account {AccountId}", accountId);
            return false;
        }

        var saved = await sepaMandateStore.SaveSignedMandateDocumentIdAsync(accountId, signedMandateDocumentId);
        if (saved)
        {
            logger.LogInformation(
                "Saved signed mandate document identifier {SignedMandateDocumentId} for account {AccountId}",
                signedMandateDocumentId,
                accountId);
        }

        return saved;
    }

    /// <inheritdoc />
    public async Task<string?> GetSignedMandateDocumentIdAsync(int accountId)
    {
        if (!await paymentPreferenceStore.AccountExistsAsync(accountId))
        {
            logger.LogWarning("Signed mandate document identifier requested for unknown account {AccountId}", accountId);
            return null;
        }

        var mandate = await sepaMandateStore.GetLatestByAccountIdAsync(accountId);
        if (mandate is null)
        {
            logger.LogWarning("Signed mandate document identifier requested for account {AccountId} without SEPA mandate", accountId);
            return null;
        }

        if (mandate.SignatureStatus != SepaMandateSignatureStatus.Signed)
        {
            logger.LogWarning(
                "Signed mandate document identifier requested for account {AccountId} while mandate {SepaMandateId} is not signed",
                accountId,
                mandate.Id);
            return null;
        }

        return string.IsNullOrWhiteSpace(mandate.SignedMandateDocumentId)
            ? null
            : mandate.SignedMandateDocumentId;
    }

    /// <inheritdoc />
    public async Task<bool> SetOtherAsync(int accountId, string createdBy)
    {
        if (!await paymentPreferenceStore.AccountExistsAsync(accountId))
        {
            logger.LogWarning("Payment preference OTHER requested for unknown account {AccountId}", accountId);
            return false;
        }

        var paymentPrefrenceStrategy = ResolveStrategy(PaymentPreferenceType.Other);
        var preference = await paymentPreferenceStore.GetByAccountIdAsync(accountId)
            ?? new PaymentPreference(0, accountId, null, DateTime.UtcNow, createdBy);

        await paymentPrefrenceStrategy.ApplyAsync(preference);
        await paymentPreferenceStore.SaveAsync(preference);
        logger.LogInformation("Saved payment preference OTHER for account {AccountId}", accountId);
        return true;
    }

    /// <inheritdoc />
    public async Task<SepaPaymentPreferenceResult> SetSepaAsync(
        int accountId,
        SepaPaymentPreferenceCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);

        if (!await paymentPreferenceStore.AccountExistsAsync(accountId))
        {
            logger.LogWarning("Payment preference SEPA requested for unknown account {AccountId}", accountId);
            return new SepaPaymentPreferenceResult(false, null);
        }

        var accountNumber = await paymentPreferenceStore.GetAccountNumberAsync(accountId) ?? string.Empty;
        var mandatePdf = await sepaMandatePdfGenerator.GenerateAsync(new SepaMandatePdfData(
            command.AccountHolder,
            accountNumber,
            command.Address,
            command.AddressLine2,
            command.City,
            command.Country,
            command.PostalCode,
            command.Iban,
            command.Bic));

        var signature = await getAcceptClient.SendMandateForSignatureAsync(
            new GetAcceptMandateSignatureRequest(
                mandatePdf,
                BuildMandateFileName(accountId),
                command.Recipient));

        var paymentPrefrenceStrategy = ResolveStrategy(PaymentPreferenceType.MandateSepa);
        var preference = await paymentPreferenceStore.GetByAccountIdAsync(accountId)
            ?? new PaymentPreference(0, accountId, null, DateTime.UtcNow, command.Recipient.Email);

        await paymentPrefrenceStrategy.ApplyAsync(preference);

        var sepaMandate = new SepaMandate(
            id: 0,
            accountId: accountId,
            documentId: command.DocumentId,
            accountHolder: command.AccountHolder,
            iban: command.Iban,
            bic: command.Bic,
            address: command.Address,
            signatureRequestId: signature.SignatureRequestId,
            signatureUrl: signature.SignatureUrl,
            signatureStatus: SepaMandateSignatureStatus.Processing,
            isSentToAkuiteo: false,
            sentToAkuiteoAt: null,
            createdAt: DateTime.UtcNow,
            createdBy: command.Recipient.Email);

        await sepaMandateStore.SaveWithPaymentPreferenceAsync(sepaMandate, preference);
        logger.LogInformation(
            "Saved SEPA payment preference for account {AccountId} with signature request {SignatureRequestId}",
            accountId,
            signature.SignatureRequestId);
        return new SepaPaymentPreferenceResult(true, signature.SignatureUrl);
    }

    /// <inheritdoc />
    public async Task<bool> ResetAsync(int accountId)
    {
        if (!await paymentPreferenceStore.AccountExistsAsync(accountId))
        {
            logger.LogWarning("Payment preference reset requested for unknown account {AccountId}", accountId);
            return false;
        }

        var preference = await paymentPreferenceStore.GetByAccountIdAsync(accountId);
        if (preference is null)
        {
            logger.LogWarning("Payment preference reset requested for account {AccountId} without preference row", accountId);
            return false;
        }

        preference.ResetPaymentType();
        await paymentPreferenceStore.SaveAsync(preference);
        logger.LogInformation("Reset payment preference for account {AccountId}", accountId);
        return true;
    }

    /// <inheritdoc />
    public async Task<bool> CleanupAsync(int accountId)
    {
        logger.LogInformation("Starting Mandate onboarding cleanup for account {AccountId}", accountId);

        if (!await paymentPreferenceStore.AccountExistsAsync(accountId))
        {
            logger.LogWarning("Mandate onboarding cleanup requested for unknown account {AccountId}", accountId);
            return false;
        }

        var deletedRows = await paymentPreferenceCleanupStore.CleanupAsync(accountId);
        logger.LogInformation(
            "Completed Mandate onboarding cleanup for account {AccountId}. DeletedRows: {DeletedRows}",
            accountId,
            deletedRows);
        return true;
    }

    private IPaymentPreferenceStrategy ResolveStrategy(PaymentPreferenceType paymentType)
    {
        return strategies.Single(strategy => strategy.PaymentType == paymentType);
    }

    private IPaymentPreferenceReadStrategy ResolveReadStrategy(PaymentPreferenceType? paymentType)
    {
        return readStrategies.Single(strategy => strategy.Supports(paymentType));
    }

    private static PaymentPreferenceType? MapPaymentType(int? paymentType)
    {
        return paymentType switch
        {
            (int)PaymentPreferenceType.MandateSepa => PaymentPreferenceType.MandateSepa,
            (int)PaymentPreferenceType.Other => PaymentPreferenceType.Other,
            _ => null
        };
    }

    private static string BuildMandateFileName(int accountId)
    {
        return $"mandate-sepa-{accountId}.pdf";
    }
}
