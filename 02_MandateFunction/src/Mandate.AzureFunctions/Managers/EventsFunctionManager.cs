// <copyright file="EventsFunctionManager.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Function;

using KPMG.Pulse.Back.Accounting.Mandate.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using System.Data.Common;
using System.Security.Principal;

public class EventsFunctionManager : IEventsFunctionManager
{
    private readonly ILogger<EventsFunctionManager> logger;
    private readonly ISqlAdapter sqlAdapter;

    public EventsFunctionManager(ILogger<EventsFunctionManager> logger, ISqlAdapter sqlAdapter)
    {
        this.logger = logger;
        this.sqlAdapter = sqlAdapter;
    }

    public async Task CreateAccountByEventAsync(Account account)
    {
        var dbCompany = await this.sqlAdapter.GetAccountByIdAsync(account.Id);
        if (dbCompany != null)
        {
            this.logger.LogError("Account with ID {Id} already exists in database. Start updating...", account.Id);
            await this.UpdateAccountByEventAsync(account);
            return;
        }

        await this.sqlAdapter.CreateAccountByEventAsync(account);
        this.logger.LogInformation("Account with ID {Id} successfully created in database.", account.Id);
    }

    public async Task CreateContactByEventAsync(Contact contact)
    {
        var dbCollab = await this.sqlAdapter.GetContactByIdAsync(contact.Id);
        if (dbCollab != null)
        {
            this.logger.LogError("Contact with ID {Id} already exists in database. Start updating...", contact.Id);
            await this.UpdateContactByEventAsync(contact);
            return;
        }

        await this.sqlAdapter.CreateContactByEventAsync(contact);
        this.logger.LogInformation("Contact with ID {Id} successfully created in database.", contact.Id);
    }

    public async Task CreateRoleByEventAsync(AccountContact accountContact)
    {
        var dbCompany = await this.sqlAdapter.GetAccountByIdAsync(accountContact.AccountId);
        if (dbCompany == null)
        {
            this.logger.LogError("Company with ID {CompanyId} does not exists in database.", accountContact.AccountId);
            return;
        }

        var dbCollab = await this.sqlAdapter.GetContactByIdAsync(accountContact.ContactId);
        if (dbCollab == null)
        {
            this.logger.LogError("Contact with ID {Id} does not exists in database.", accountContact.ContactId);
            return;
        }

        var dbAccountContact = await this.sqlAdapter.GetAccountContactByAccountIdAndContactIdAsync(accountContact.AccountId, accountContact.ContactId);
        if (dbAccountContact != null)
        {
            this.logger.LogError("Relation with AccountID({AccountId}) and ContactId({ContactId}) already exists in database.", accountContact.AccountId, accountContact.ContactId);
            return;
        }

        await this.sqlAdapter.CreateRoleByEventAsync(accountContact);
        this.logger.LogInformation("Relation with AccountID({AccountId}) and ContactId({ContactId}) successfully created in database.", accountContact.AccountId, accountContact.ContactId);
    }

    public async Task DeleteAccountByEventAsync(int accountId)
    {
        var dbCompany = await this.sqlAdapter.GetAccountByIdAsync(accountId);
        if (dbCompany == null)
        {
            this.logger.LogError("Company with ID {CompanyId} does not exists in database.", accountId);
            return;
        }

        await this.sqlAdapter.DeleteAccountByEventAsync(dbCompany);
        this.logger.LogInformation("Account with ID {Id} successfully removed from database.", accountId);

    }

    public async Task DeleteRoleByEventAsync(AccountContact accountContact)
    {   
        await this.sqlAdapter.DeleteRoleByEventAsync(accountContact);
        this.logger.LogInformation("Relation with AccountID({AccountId}) and ContactId({ContactId}) successfully removed from database.", accountContact.AccountId, accountContact.ContactId);
    }

    public async Task DeleteContactByEventAsync(int contactId)
    {
        var dbCollab = await this.sqlAdapter.GetContactByIdAsync(contactId);
        if (dbCollab == null)
        {
            this.logger.LogError("Contact with ID {Id} does not exists in database.", contactId);
            return;
        }

        await this.sqlAdapter.DeleteContactByEventAsync(dbCollab!);
        this.logger.LogInformation("Contact with ID {Id} successfully removed from database.", contactId);
    }

    public async Task UpdateAccountByEventAsync(Account account)
    {
        var dbCompany = await this.sqlAdapter.GetAccountByIdAsync(account.Id);
        if (dbCompany == null)
        {
            this.logger.LogError("Company with ID {CompanyId} does not exists in database. Start creating new entry...", account.Id);
            await this.CreateAccountByEventAsync(account);
            return;
        }

        await this.sqlAdapter.UpdateAccountByEventAsync(account);
        this.logger.LogInformation("Account with ID {Id} successfully updated in database.", account.Id);

    }

    public async Task UpdateContactByEventAsync(Contact contact)
    {
        var dbCollab = await this.sqlAdapter.GetContactByIdAsync(contact.Id);
        if (dbCollab == null)
        {
            this.logger.LogError("Contact with ID {Id} does not exists in database. Start creating new entry...", contact.Id);
            await this.CreateContactByEventAsync(contact);
            return;
        }

        await this.sqlAdapter.UpdateContactByEventAsync(contact);
        this.logger.LogInformation("Contact with ID {Id} successfully updated in database.", contact.Id);
    }
}
