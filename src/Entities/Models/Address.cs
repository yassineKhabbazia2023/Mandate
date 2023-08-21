// <copyright file="Address.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate
{
    public class Address
    {
        public Address(string? street, string? complements, string? zipCode, string? city, string? country)
        {
            this.Street = street;
            this.Complements = complements;
            this.ZipCode = zipCode;
            this.City = city;
            this.Country = country;
        }

        public string? Street { get; }

        public string? Complements { get; }

        public string? ZipCode { get; }

        public string? City { get; }

        public string? Country { get; }
    }
}
