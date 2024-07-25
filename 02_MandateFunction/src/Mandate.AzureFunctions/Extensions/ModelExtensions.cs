// <copyright file="ModelExtensions.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Function;
using global::Pulse.Back.Events.IntegrationEvents.EventsData;
using KPMG.Pulse.Back.Accounting.Mandate.Client;

public static class ModelExtensions
{
    public static Contact ToModel(this ContactStateEventData contactStateEventData)
    {
        return new Contact(
            contactStateEventData.ContactId,
            contactStateEventData.FirstName,
            contactStateEventData.LastName,
            contactStateEventData.Email,
            contactStateEventData.IsActive);
    }

    public static AccountContact ToModel(this RoleCreatedEventData roleCreatedEventData)
    {
        ArgumentNullException.ThrowIfNull(roleCreatedEventData);
        return new AccountContact(
            roleCreatedEventData.AccountId,
            roleCreatedEventData.ContactId);
    }

    public static AccountContact ToModel(this RoleDeletedEventData roleDeletedEventData)
    {
        ArgumentNullException.ThrowIfNull(roleDeletedEventData);

        return new AccountContact(
            roleDeletedEventData.AccountId,
            roleDeletedEventData.ContactId);
    }

    public static Account ToModel(this AccountStateEventData accountStateEventData)
    {
        ArgumentNullException.ThrowIfNull(accountStateEventData);

        string? cleanedSiretNumber = accountStateEventData.SiretNumber?.Trim().Replace(" ", string.Empty);

        if (cleanedSiretNumber?.Length > 14)
        {
            throw new ArgumentException("Siret number cannot be more than 14 characters.");
        }

        return new Account(
            accountStateEventData.AccountId,
            accountStateEventData.LegalName,
            cleanedSiretNumber,
            accountStateEventData.AccountNumber,
            true);
    }
}
