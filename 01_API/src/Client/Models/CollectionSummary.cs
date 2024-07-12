// <copyright file="CollectionSummary.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Client
{
    using Newtonsoft.Json;

    public class CollectionSummary
    {
        [JsonConstructor]
        public CollectionSummary(
            Guid id,
            string erpId,
            string companyName,
            CollectionBankInfo collectionBankInfo,
            DateTime creationDate,
            DateTime modificationDate,
            int statusCode)
        {
            this.Id = id;
            this.ErpId = erpId;
            this.CompanyName = companyName;
            this.BankName = collectionBankInfo!.BankName!;
            this.AccountNumber = collectionBankInfo!.AccountNumber!;
            this.JdcPartnership = collectionBankInfo!.JdcPartnership;
            this.CreationDate = creationDate;
            this.ModificationDate = modificationDate;
            this.StatusCode = statusCode;
        }

        [JsonProperty("id")]
        public Guid Id { get; }

        [JsonProperty("erpId")]
        public string ErpId { get; }

        [JsonProperty("companyName")]
        public string CompanyName { get; }

        [JsonProperty("bankName")]
        public string BankName { get; }

        [JsonProperty("accountNumber")]
        public string AccountNumber { get; }

        [JsonProperty("jdcPartnership")]
        public int JdcPartnership { get; }

        [JsonProperty("creationDate")]
        public DateTime CreationDate { get; }

        [JsonProperty("modificationDate")]
        public DateTime ModificationDate { get; }

        [JsonProperty("statusCode")]
        public int StatusCode { get; }
    }
}
