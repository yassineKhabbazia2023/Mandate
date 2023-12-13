// <copyright file="Company.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate
{
    public class Company
    {
        public Company(Guid id, string? name, string siretNumber, string? erpId, string? bankServicesProviderId, Signatory? signatory, Address? address)
        {
            this.Id = id;
            this.Name = name;
            this.SiretNumber = siretNumber;
            this.ErpId = erpId;
            this.BankServicesProviderId = bankServicesProviderId;
            this.Signatory = signatory;
            this.Address = address;
        }

        public Guid Id { get; }

        public string? Name { get; }

        public string SiretNumber { get; }

        public string? ErpId { get; }

        // folderId
        public string? BankServicesProviderId { get; }

        public Signatory? Signatory { get; }

        public Address? Address { get; }
    }
}
