// <copyright file="Signatory.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Client
{
    using Newtonsoft.Json;

    public class Signatory
    {
        [JsonConstructor]
        public Signatory(string? title, string? firstName, string? lastName, string? email, Address? address)
        {
            this.Title = title;
            this.FirstName = firstName;
            this.LastName = lastName;
            this.Email = email;
            this.Address = address;
        }

        public string? Title { get; }

        public string? FirstName { get; }

        public string? LastName { get; }

        public string? Email { get; }

        public Address? Address { get; }
    }
}
