// <copyright file="Account.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Function;

public class Account
{
    public Account(int id, string name, string siretNumber, string accountNumber, bool isActive)
    {
        this.Id = id;
        this.Name = name;
        this.SiretNumber = siretNumber;
        this.AccountNumber = accountNumber;
        this.IsActive = isActive;
    }

    public int Id { get; }

    public string Name { get; }

    public string SiretNumber { get; }

    public string AccountNumber { get; }

    public bool IsActive { get; }
}
