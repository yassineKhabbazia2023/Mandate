// <copyright file="ISqlAdapter.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Function;

using KPMG.Pulse.Back.Accounting.Mandate.Client;

public interface ISqlAdapter
{
    Task CreateContactByEventAsync(Contact contact);

    Task UpdateContactByEventAsync(Contact contact);

    Task DeleteContactByEventAsync(Contact contact);

    Task<Contact?> GetActiveContactByIdAsync(int contactId);

    Task<Account?> GetActiveAccountByIdAsync(int accountId);

    Task<Contact?> GetContactByIdAsync(int contactId);

    Task<Account?> GetAccountByIdAsync(int accountId);

    Task CreateRoleByEventAsync(AccountContact accountContact);

    Task DeleteRoleByEventAsync(AccountContact accountContact);

    Task<AccountContact?> GetAccountContactByAccountIdAndContactIdAsync(int accountId, int contactId);

    Task CreateAccountByEventAsync(Account account);

    Task DeleteAccountByEventAsync(Account account);

    Task UpdateAccountByEventAsync(Account account);
}
