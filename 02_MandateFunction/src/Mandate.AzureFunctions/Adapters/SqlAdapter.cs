// <copyright file="SqlAdapter.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Function;

using System;
using System.Threading.Tasks;
using KPMG.Pulse.Back.Accounting.Mandate.Sql;

public class SqlAdapter : ISqlAdapter
{
    private readonly IMandateRepository mandateRepository;

    public SqlAdapter(IMandateRepository mandateRepository)
    {
        this.mandateRepository = mandateRepository;
    }

    public async Task CreateContactByEventAsync(Contact contact)
    {
        ArgumentNullException.ThrowIfNull(contact);
        var dbContact = contact.ToSql();
        dbContact.IsActive = true;
        await this.mandateRepository.CreateContactByEventAsync(dbContact);
    }

    public async Task UpdateContactByEventAsync(Contact contact)
    {
        ArgumentNullException.ThrowIfNull(contact);
        await this.mandateRepository.UpdateContactByEventAsync(contact.ToSql());
    }

    public async Task DeleteContactByEventAsync(Contact contact)
    {
        ArgumentNullException.ThrowIfNull(contact);
        var dbContact = contact.ToSql();
        dbContact.IsActive = false;
        await this.mandateRepository.UpdateContactByEventAsync(dbContact);
    }

    public async Task<Contact?> GetActiveContactByIdAsync(int contactId)
    {
        var dbContact = await this.mandateRepository.GetActiveContactByIdAsync(contactId);
        return dbContact?.ToModel();
    }

    public async Task<Account?> GetActiveAccountByIdAsync(int accountId)
    {
        CompanyDb? accountDb = await this.mandateRepository.GetActiveAccountByIdAsync(accountId);
        return accountDb?.ToModel();
    }

    public async Task CreateRoleByEventAsync(AccountContact accountContact)
    {
        ArgumentNullException.ThrowIfNull(accountContact);
        await this.mandateRepository.CreateRoleAsync(accountContact.ToSql());
    }

    public async Task DeleteRoleByEventAsync(AccountContact accountContact)
    {
        ArgumentNullException.ThrowIfNull(accountContact);
        await this.mandateRepository.DeleteRoleAsync(accountContact.ToSql());
    }

    public async Task<AccountContact?> GetAccountContactByAccountIdAndContactIdAsync(int accountId, int contactId)
    {
        var accountContactDB = await this.mandateRepository.GetAccountContactByAccountIdAndContactIdAsync(accountId, contactId);
        return accountContactDB?.ToModel();
    }

    public async Task CreateAccountByEventAsync(Account account)
    {
        ArgumentNullException.ThrowIfNull(account);
        var dbAccount = account.ToSql();
        dbAccount.IsActive = true;
        await this.mandateRepository.CreateCompanyAsync(dbAccount);
    }

    public async Task DeleteAccountByEventAsync(Account account)
    {
        var dbAccount = account.ToSql();
        dbAccount.IsActive = false;
        await this.mandateRepository.UpdateCompanyAsync(dbAccount);
    }

    public async Task UpdateAccountByEventAsync(Account account)
    {
        await this.mandateRepository.UpdateCompanyAsync(account.ToSql());
    }

    public async Task<Contact?> GetContactByIdAsync(int contactId)
    {
        var dbContact = await this.mandateRepository.GetContactByIdAsync(contactId);
        return dbContact?.ToModel();
    }

    public async Task<Account?> GetAccountByIdAsync(int accountId)
    {
        CompanyDb? accountDb = await this.mandateRepository.GetAccountByIdAsync(accountId);
        return accountDb?.ToModel();
    }
}
