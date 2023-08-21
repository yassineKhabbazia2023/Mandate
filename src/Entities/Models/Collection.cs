// <copyright file="Collection.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate
{
    public class Collection
    {
        public Collection(Guid id, Company company, Bban bban, DateTime? creationDate, CollectionStatus status)
        {
            this.Id = id;
            this.Company = company;
            this.Bban = bban;
            this.CreationDate = creationDate;
            this.Status = status;
        }

        public Guid Id { get; }

        public Company Company { get; }

        public Bban Bban { get; }

        public DateTime? CreationDate { get; }

        public CollectionStatus Status { get; }
    }
}
