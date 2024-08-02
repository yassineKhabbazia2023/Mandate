// <copyright file="SqlExtensions.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Function;

using System;
using KPMG.Pulse.Back.Accounting.Mandate.Sql;

public static class SqlExtensions
{
    public static CollaboratorDb ToSql(this Contact contact)
    {
        ArgumentNullException.ThrowIfNull(contact);

        return new CollaboratorDb()
        {
            Id = contact.Id,
            Email = contact.Email,
            FirstName = contact.FirstName,
            LastName = contact.LastName,
            IsActive = contact.IsActive,
        };
    }

    public static CompanyCollaboratorDb ToSql(this AccountContact accountContact)
    {
        ArgumentNullException.ThrowIfNull(accountContact);
        return new CompanyCollaboratorDb()
        {
            CompanyId = accountContact.AccountId,
            CollaboratorId = accountContact.ContactId,
        };
    }

    public static CompanyDb ToSql(this Account account)
    {
        ArgumentNullException.ThrowIfNull(account);
        return new CompanyDb()
        {
            Id = account.Id,
            Name = account.Name,
            SiretNumber = account.SiretNumber,
            ErpId = account.AccountNumber,
            IsActive = account.IsActive,
        };
    }

    public static Account ToModel(this CompanyDb account)
    {
        ArgumentNullException.ThrowIfNull(account);
        return new Account(account.Id, account.Name!, account.SiretNumber, account.ErpId ?? string.Empty, account.IsActive);
    }

    public static AccountContact ToModel(this CompanyCollaboratorDb companyCollaborator)
    {
        ArgumentNullException.ThrowIfNull(companyCollaborator);
        return new AccountContact(companyCollaborator.CompanyId, companyCollaborator.CollaboratorId);
    }

    public static Contact ToModel(this CollaboratorDb collaborator)
    {
        ArgumentNullException.ThrowIfNull(collaborator);
        return new Contact(collaborator.Id, collaborator.FirstName!, collaborator.LastName!, collaborator.Email, collaborator.IsActive);
    }
}
