// <copyright file="FormioBban.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Formio.Client
{
    using Newtonsoft.Json;

    public class FormioBban
    {
        public FormioBban(string bankCode, string bankSortCode, string bankAccountNumber, string bankCheckNumber)
        {
            this.BankCode = bankCode;
            this.BankSortCode = bankSortCode;
            this.BankAccountNumber = bankAccountNumber;
            this.BankCheckNumber = bankCheckNumber;
        }

        [JsonProperty("bankCode")]
        public string BankCode { get; }

        [JsonProperty("bankSortCode")]
        public string BankSortCode { get; }

        [JsonProperty("bankAccountNumber")]
        public string BankAccountNumber { get; }

        [JsonProperty("bankCheckNumber")]
        public string BankCheckNumber { get; }
    }
}
