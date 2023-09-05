// <copyright file="MandateCollection.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Client
{
    using Newtonsoft.Json;

    public class MandateCollection
    {
        [JsonConstructor]
        public MandateCollection(
            Guid id,
            string erpId,
            string companyName,
            string bankName,
            string accountNumber,
            DateTime creationDate,
            DateTime modificationDate,
            int statusCode,
            string statusName)
        {
            this.Id = id;
            this.ErpId = erpId;
            this.CompanyName = companyName;
            this.BankName = bankName;
            this.AccountNumber = accountNumber;
            this.CreationDate = creationDate;
            this.ModificationDate = modificationDate;
            this.StatusCode = statusCode;
            this.StatusName = statusName;
        }

        public Guid Id { get; }

        public string ErpId { get; }

        public string CompanyName { get; }

        public string BankName { get; }

        public string AccountNumber { get; }

        public DateTime CreationDate { get; }

        public DateTime ModificationDate { get; }

        public int StatusCode { get; }

        public string StatusName { get; }
    }
}
