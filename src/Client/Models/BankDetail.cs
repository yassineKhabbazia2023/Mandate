// <copyright file="BankDetail.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Client
{
    using Newtonsoft.Json;

    public class BankDetail
    {
        [JsonConstructor]
        public BankDetail(string bankCode, string bankName, BankJdcDetail jdcDetail)
        {
            this.BankCode = bankCode;
            this.BankName = bankName;
            this.JdcDetail = jdcDetail;
        }

        [JsonProperty("code")]
        public string BankCode { get; }

        [JsonProperty("name")]
        public string BankName { get; }

        [JsonProperty("jeDeclare")]
        public BankJdcDetail JdcDetail { get; }
    }
}
