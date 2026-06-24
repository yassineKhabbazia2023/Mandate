// <copyright file="PaymentPreferencesSqlAdapter.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Adapters;

using KPMG.Pulse.Back.Accounting.Mandate.Application.Interfaces;
using KPMG.Pulse.Back.Accounting.Mandate.Sql;

/// <summary>
/// SQL adapter for onboarding payment preference persistence.
/// </summary>
/// <param name="paymentPreferenceRepository">The payment preference repository.</param>
/// <param name="sepaMandateRepository">The SEPA mandate repository.</param>
public sealed class PaymentPreferencesSqlAdapter(
    IPaymentPreferenceRepository paymentPreferenceRepository,
    ISepaMandateRepository sepaMandateRepository) : IPaymentPreferenceStore, ISepaMandateStore
{
    /// <inheritdoc />
    public async Task<bool> AccountExistsAsync(int accountId)
    {
        return await paymentPreferenceRepository.AccountExistsAsync(accountId);
    }

    /// <inheritdoc />
    public async Task<string?> GetAccountNumberAsync(int accountId)
    {
        return await paymentPreferenceRepository.GetAccountNumberAsync(accountId);
    }

    /// <inheritdoc />
    public async Task<PaymentPreference?> GetByAccountIdAsync(int accountId)
    {
        var preference = await paymentPreferenceRepository.GetByAccountIdAsync(accountId);
        return preference is null ? null : MapPaymentPreference(preference);
    }

    /// <inheritdoc />
    public async Task SaveAsync(PaymentPreference paymentPreference)
    {
        ArgumentNullException.ThrowIfNull(paymentPreference);

        await paymentPreferenceRepository.SaveAsync(MapPaymentPreference(paymentPreference));
    }

    /// <inheritdoc />
    public async Task SaveWithPaymentPreferenceAsync(
        SepaMandate sepaMandate,
        PaymentPreference paymentPreference)
    {
        ArgumentNullException.ThrowIfNull(sepaMandate);
        ArgumentNullException.ThrowIfNull(paymentPreference);

        await sepaMandateRepository.SaveWithPaymentPreferenceAsync(
            MapSepaMandate(sepaMandate),
            MapPaymentPreference(paymentPreference));
    }

    /// <summary>
    /// Maps a database payment preference to the domain model.
    /// </summary>
    /// <param name="preference">The database payment preference.</param>
    /// <returns>The domain payment preference.</returns>
    private static PaymentPreference MapPaymentPreference(PaymentPreferenceDb preference)
    {
        return new PaymentPreference(
            preference.Id,
            preference.AccountId,
            preference.PaymentType,
            preference.CreatedAt,
            preference.CreatedBy);
    }

    /// <summary>
    /// Maps a domain payment preference to the database model.
    /// </summary>
    /// <param name="paymentPreference">The domain payment preference.</param>
    /// <returns>The database payment preference.</returns>
    private static PaymentPreferenceDb MapPaymentPreference(PaymentPreference paymentPreference)
    {
        return new PaymentPreferenceDb
        {
            Id = paymentPreference.Id,
            AccountId = paymentPreference.AccountId,
            PaymentType = paymentPreference.PaymentType,
            CreatedAt = paymentPreference.CreatedAt,
            CreatedBy = paymentPreference.CreatedBy
        };
    }

    /// <summary>
    /// Maps a domain SEPA mandate to the database model.
    /// </summary>
    /// <param name="sepaMandate">The domain SEPA mandate.</param>
    /// <returns>The database SEPA mandate.</returns>
    private static SepaMandateDb MapSepaMandate(SepaMandate sepaMandate)
    {
        return new SepaMandateDb
        {
            Id = sepaMandate.Id,
            AccountId = sepaMandate.AccountId,
            RibDocumentId = sepaMandate.DocumentId,
            AccountHolder = sepaMandate.AccountHolder,
            Iban = sepaMandate.Iban,
            Bic = sepaMandate.Bic,
            Address = sepaMandate.Address,
            SignatureRequestId = sepaMandate.SignatureRequestId,
            SignatureUrl = sepaMandate.SignatureUrl,
            SignatureStatus = (int)sepaMandate.SignatureStatus,
            IsSentToAkuiteo = sepaMandate.IsSentToAkuiteo,
            SentToAkuiteoAt = sepaMandate.SentToAkuiteoAt,
            CreatedAt = sepaMandate.CreatedAt,
            CreatedBy = sepaMandate.CreatedBy
        };
    }
}
