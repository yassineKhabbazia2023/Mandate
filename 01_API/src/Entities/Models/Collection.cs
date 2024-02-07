// <copyright file="Collection.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate
{
    public class Collection
    {
        public Collection(Guid id, string? collectionServicesProviderId, Company? company, Bban? bban, DateTime creationDate, DateTime modificationDate, Status status)
        {
            this.Id = id;
            this.CollectionServicesProviderId = collectionServicesProviderId;
            this.Company = company;
            this.Bban = bban;
            this.CreationDate = creationDate;
            this.Status = status;
            this.ModificationDate = modificationDate;
        }

        public Guid Id { get; }

        // releveId
        public string? CollectionServicesProviderId { get; }

        public Company? Company { get; }

        public Bban? Bban { get; }

        public DateTime CreationDate { get; }

        public DateTime ModificationDate { get; }

        public Status Status { get; }

        public string GetSignatoryEmail() => this.Company?.Signatory?.Email ?? string.Empty;

        public string GetErpId() => this.Company?.ErpId ?? string.Empty;

        public string GetSiretNumber() => this.Company?.SiretNumber ?? string.Empty;

        public string GetSignatoryFullName() => $"{this.Company?.Signatory?.LastName ?? string.Empty} {this.Company?.Signatory?.FirstName ?? string.Empty}";

        public string GetBankName() => this.Bban?.Bank?.Name ?? string.Empty;

        public string GetBankCode() => this.Bban?.BankCode ?? string.Empty;

        public string GetBranchCode() => this.Bban?.BranchCode ?? string.Empty;

        public string GetAccountNumber() => this.Bban?.AccountNumber ?? string.Empty;

        public string GetCheckDigits() => this.Bban?.CheckDigits ?? string.Empty;
    }
}
