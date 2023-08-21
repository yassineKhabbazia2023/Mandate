// <copyright file="Signatory.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate
{
    public class Signatory
    {
        public Signatory(string? title, string? firstName, string? lastName, Address? address)
        {
            this.Title = title;
            this.FirstName = firstName;
            this.LastName = lastName;
            this.Address = address;
        }

        public string? Title { get; }

        public string? FirstName { get; }

        public string? LastName { get; }

        public Address? Address { get; }
    }
}
