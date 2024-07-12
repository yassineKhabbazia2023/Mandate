// <copyright file="BankJdcDetail.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Client
{
    using Newtonsoft.Json;

    public class BankJdcDetail
    {
        [JsonConstructor]
        public BankJdcDetail(string jdcPartnership)
        {
            this.JdcPartnership = jdcPartnership;
        }

        [JsonProperty("jdcPartnership")]
        public string JdcPartnership { get; }
    }
}
