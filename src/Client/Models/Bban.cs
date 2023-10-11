// <copyright file="Bban.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Client
{
    using Newtonsoft.Json;

    public class Bban
    {
        [JsonConstructor]
        public Bban(string bankCode, string branchCode, string accountNumber, string checkDigits)
        {
            this.BankCode = bankCode;
            this.BranchCode = branchCode;
            this.AccountNumber = accountNumber;
            this.CheckDigits = checkDigits;
        }

        [JsonProperty("bankCode")]
        public string BankCode { get; }

        [JsonProperty("branchCode")]
        public string BranchCode { get; }

        [JsonProperty("accountNumber")]
        public string AccountNumber { get; }

        [JsonProperty("checkDigits")]
        public string CheckDigits { get; }
    }
}
