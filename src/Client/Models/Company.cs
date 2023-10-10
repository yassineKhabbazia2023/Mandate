// <copyright file="Company.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Client
{
    using Newtonsoft.Json;

    public class Company
    {
        [JsonConstructor]
        public Company(Guid id, string? name, string siretNumber, string? erpId, Signatory? signatory, Address? address)
        {
            this.Id = id;
            this.Name = name;
            this.SiretNumber = siretNumber;
            this.ErpId = erpId;
            this.Signatory = signatory;
            this.Address = address;
        }

        public Guid Id { get; }

        public string? Name { get; }

        public string SiretNumber { get; }

        public string? ErpId { get; }

        public Signatory? Signatory { get; }

        public Address? Address { get; }
    }
}
