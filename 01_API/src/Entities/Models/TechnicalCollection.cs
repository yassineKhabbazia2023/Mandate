// <copyright file="TechnicalCollection.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate
{
    using Newtonsoft.Json;

    public class TechnicalCollection
    {
        [JsonConstructor]
        public TechnicalCollection(
            Guid id,
            string folderId,
            string ribId,
            BankDetails bankDetails,
            string statusCode)
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

        public string StatusCode { get; }
    }
}
