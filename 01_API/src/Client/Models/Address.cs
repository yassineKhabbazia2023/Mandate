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

        [JsonProperty("street")]
        public string? Street { get; }

        [JsonProperty("addressComplement")]
        public string? AddressComplement { get; }

        [JsonProperty("zipCode")]
        public string? ZipCode { get; }

        [JsonProperty("city")]
        public string? City { get; }

        [JsonProperty("country")]
        public string? Country { get; }
    }
}
