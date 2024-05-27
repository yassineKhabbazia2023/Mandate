// <copyright file="CompanyCollaborator.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Client;

public class CompanyCollaborator
{
    public CompanyCollaborator(int accountId, int contactId)
    {
        this.AccountId = accountId;
        this.ContactId = contactId;
    }

    public int AccountId { get; }

    public int ContactId { get; }
}
