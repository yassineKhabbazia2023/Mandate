// <copyright file="Address.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Client
{
    using Newtonsoft.Json;

    public class Address
    {
        [JsonConstructor]
        public Address(string? street, string? addressComplement, string? zipCode, string? city, string? country)
        {
            this.Street = street;
            this.AddressComplement = addressComplement;
            this.ZipCode = zipCode;
            this.City = city;
            this.Country = country;
        }

        public string? Street { get; }

        public string? AddressComplement { get; }

        public string? ZipCode { get; }

        public string? City { get; }

        public string? Country { get; }
    }
}
