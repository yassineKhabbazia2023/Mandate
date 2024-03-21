// <copyright file="TechnicalCollectionSummary.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Client
{
    using Newtonsoft.Json;

    public class TechnicalCollectionSummary
    {
        [JsonConstructor]
        public TechnicalCollectionSummary(
            Guid id,
            string folderId,
            string ribId,
            BankDetails bankDetails,
            int statusCode)
        {
            this.Id = id;
            this.FolderId = folderId;
            this.RibId = ribId;
            this.BankDetails = bankDetails;
            this.StatusCode = statusCode;
        }

        public Guid Id { get; }

        public string FolderId { get; }

        public string RibId { get; }

        public BankDetails BankDetails { get; }

        public int StatusCode { get; }
    }
}
