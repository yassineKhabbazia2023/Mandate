// <copyright file="BankJdcDetail.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Client
{
    using Newtonsoft.Json;

    public class BankJdcDetail
    {
        [JsonConstructor]
        public BankJdcDetail(bool isPartner, bool isScrapable)
        {
            this.IsPartner = isPartner;
            this.IsScrapable = isScrapable;
        }

        [JsonProperty("isPartner")]
        public bool IsPartner { get; }

        [JsonProperty("isScrapable")]
        public bool IsScrapable { get; }
    }
}
