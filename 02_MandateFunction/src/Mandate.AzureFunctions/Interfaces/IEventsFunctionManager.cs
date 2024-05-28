// <copyright file="IEventsFunctionManager.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Function;

using KPMG.Pulse.Back.Accounting.Mandate.Client;

public interface IEventsFunctionManager
{
    Task CreateContactByEventAsync(Contact contact);

    Task UpdateContactByEventAsync(Contact contact);

    Task DeleteContactByEventAsync(int contactId);

    Task CreateRoleByEventAsync(AccountContact accountContact);

    Task DeleteRoleByEventAsync(AccountContact accountContact);

    Task CreateAccountByEventAsync(Account account);

    Task DeleteAccountByEventAsync(int accountId);

    Task UpdateAccountByEventAsync(Account account);
}
