// <copyright file="SepaSynchronizationPaymentPreferenceReadStrategy.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Application.Strategies;

using KPMG.Pulse.Back.Accounting.Mandate.Application.Interfaces;
using KPMG.Pulse.Back.Accounting.Mandate.Application.Models;
using Microsoft.Extensions.Logging;

/// <summary>
/// Synchronizes pending SEPA signatures with GetAccept when reading payment preferences.
/// </summary>
public sealed class SepaSynchronizationPaymentPreferenceReadStrategy(
    ISepaMandateStore sepaMandateStore,
    IGetAcceptClient getAcceptClient,
    ILogger<SepaSynchronizationPaymentPreferenceReadStrategy> logger) : IPaymentPreferenceReadStrategy
{
    /// <inheritdoc />
    public bool Supports(PaymentPreferenceType? paymentType)
    {
        return paymentType == PaymentPreferenceType.MandateSepa;
    }

    /// <inheritdoc />
    public async Task<PaymentPreferenceResult> GetAsync(int accountId, PaymentPreference? preference)
    {
        var mandate = await sepaMandateStore.GetLatestByAccountIdAsync(accountId);
        if (mandate is null)
        {
            logger.LogWarning("SEPA payment preference for account {AccountId} has no SEPA mandate row", accountId);
            return new PaymentPreferenceResult(true, null);
        }

        logger.LogInformation(
            "Reading SEPA payment preference for account {AccountId} from mandate {SepaMandateId}. SignatureStatus: {SignatureStatus}, IsSentToAkuiteo: {IsSentToAkuiteo}, HasSignedMandateDocumentId: {HasSignedMandateDocumentId}",
            accountId,
            mandate.Id,
            mandate.SignatureStatus,
            mandate.IsSentToAkuiteo,
            !string.IsNullOrWhiteSpace(mandate.SignedMandateDocumentId));

        if (mandate.SignatureStatus == SepaMandateSignatureStatus.Signed
            && mandate.IsSentToAkuiteo)
        {
            logger.LogInformation(
                "SEPA mandate {SepaMandateId} for account {AccountId} is already signed and sent to Akuiteo",
                mandate.Id,
                accountId);
            return new PaymentPreferenceResult(true, PaymentPreferenceType.MandateSepa);
        }

        if (string.IsNullOrWhiteSpace(mandate.SignatureRequestId))
        {
            logger.LogWarning("SEPA mandate {SepaMandateId} for account {AccountId} has no GetAccept signature request id", mandate.Id, accountId);
            return new PaymentPreferenceResult(true, null);
        }

        var signatureStatus = mandate.SignatureStatus;
        GetAcceptDocumentStatusResponse? getAcceptStatus = null;

        if (mandate.SignatureStatus != SepaMandateSignatureStatus.Signed)
        {
            getAcceptStatus = await getAcceptClient.GetDocumentStatusAsync(mandate.SignatureRequestId);
            signatureStatus = MapGetAcceptStatus(getAcceptStatus.Status);
            await sepaMandateStore.UpdateSignatureStatusAsync(mandate.Id, signatureStatus);
            logger.LogInformation(
                "Refreshed GetAccept status for mandate {SepaMandateId} on account {AccountId}. RawStatus: {RawStatus}, MappedStatus: {MappedStatus}, HasDownloadUrl: {HasDownloadUrl}",
                mandate.Id,
                accountId,
                getAcceptStatus.Status,
                signatureStatus,
                !string.IsNullOrWhiteSpace(getAcceptStatus.SignedDocumentUrl));
        }

        if (signatureStatus != SepaMandateSignatureStatus.Signed)
        {
            logger.LogInformation(
                "SEPA mandate {SepaMandateId} for account {AccountId} is not signed yet after GetAccept refresh",
                mandate.Id,
                accountId);
            return new PaymentPreferenceResult(true, null);
        }

        if (mandate.IsSentToAkuiteo)
        {
            logger.LogInformation(
                "SEPA mandate {SepaMandateId} for account {AccountId} is signed and already sent to Akuiteo",
                mandate.Id,
                accountId);
            return new PaymentPreferenceResult(true, PaymentPreferenceType.MandateSepa);
        }

        if (getAcceptStatus is null)
        {
            getAcceptStatus = await getAcceptClient.GetDocumentStatusAsync(mandate.SignatureRequestId);
        }

        if (string.IsNullOrWhiteSpace(getAcceptStatus.SignedDocumentUrl))
        {
            throw new InvalidOperationException(
                $"GetAccept document {mandate.SignatureRequestId} is signed but did not include a download_url.");
        }

        var signedDocument = await getAcceptClient.DownloadSignedDocumentAsync(getAcceptStatus.SignedDocumentUrl);
        logger.LogInformation(
            "Downloaded signed mandate for mandate {SepaMandateId} on account {AccountId}. FileName: {FileName}, ContentType: {ContentType}, ContentLength: {ContentLength}",
            mandate.Id,
            accountId,
            signedDocument.FileName,
            signedDocument.ContentType,
            signedDocument.Content.Length);
        return new PaymentPreferenceResult(
            true,
            PaymentPreferenceType.MandateSepa,
            accountId,
            mandate.DocumentId,
            signedDocument.Content,
            signedDocument.ContentType,
            signedDocument.FileName,
            mandate.SignedMandateDocumentId,
            mandate.Iban,
            mandate.Bic);
    }

    /// <summary>
    /// Maps the GetAccept document response <c>status</c> field to the internal signature status.
    /// </summary>
    /// <param name="status">The raw GetAccept status.</param>
    /// <returns>The internal signature status.</returns>
    public static SepaMandateSignatureStatus MapGetAcceptStatus(string? status)
    {
        return status?.Trim().ToLowerInvariant() switch
        {
            "draft" => SepaMandateSignatureStatus.Draft,
            "processing" => SepaMandateSignatureStatus.Processing,
            "sealed" => SepaMandateSignatureStatus.Sealed,
            "sent" => SepaMandateSignatureStatus.Sent,
            "viewed" => SepaMandateSignatureStatus.Viewed,
            "reviewed" => SepaMandateSignatureStatus.Reviewed,
            "signed" => SepaMandateSignatureStatus.Signed,
            "rejected" => SepaMandateSignatureStatus.Rejected,
            "recalled" => SepaMandateSignatureStatus.Recalled,
            _ => SepaMandateSignatureStatus.Processing
        };
    }
}
