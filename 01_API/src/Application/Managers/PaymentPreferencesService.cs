// <copyright file="PaymentPreferencesService.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Application.Managers;

using KPMG.Pulse.Back.Accounting.Mandate.Application.Interfaces;
using KPMG.Pulse.Back.Accounting.Mandate.Application.Models;
using Microsoft.Extensions.Logging;

/// <inheritdoc />
public sealed class PaymentPreferencesService(
    IPaymentPreferenceStore paymentPreferenceStore,
    ISepaMandateStore sepaMandateStore,
    ISepaMandatePdfGenerator sepaMandatePdfGenerator,
    IGetAcceptClient getAcceptClient,
    IEnumerable<IPaymentPreferenceStrategy> strategies,
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
        return new PaymentPreferenceResult(true, MapPaymentType(preference?.PaymentType));
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
        return true;
    }

    private IPaymentPreferenceStrategy ResolveStrategy(PaymentPreferenceType paymentType)
    {
        return strategies.Single(strategy => strategy.PaymentType == paymentType);
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
