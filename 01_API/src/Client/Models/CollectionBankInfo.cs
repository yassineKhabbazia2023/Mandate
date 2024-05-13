// <copyright file="CollectionBankInfo.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Client;

using Newtonsoft.Json;

public class CollectionBankInfo
{
    [JsonConstructor]
    public CollectionBankInfo(string bankName, string accountNumber, int jdcPartnership)
    {
        this.BankName = bankName;
        this.AccountNumber = accountNumber;
        this.JdcPartnership = jdcPartnership;
    }

    public string BankName { get; }

    public string AccountNumber { get; }

    public int JdcPartnership { get; }
}
