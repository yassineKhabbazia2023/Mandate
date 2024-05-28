// <copyright file="AccountContact.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Function;
public class AccountContact
{
    public AccountContact(int accountId, int contactId)
    {
        this.AccountId = accountId;
        this.ContactId = contactId;
    }

    public int AccountId { get; }

    public int ContactId { get; }
}
