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

        public string? CollectionServicesProviderId { get; }

        public Company? Company { get; }

        public Bban? Bban { get; }

        public DateTime CreationDate { get; }

        public DateTime ModificationDate { get; }

        public Status Status { get; }
    }
}
