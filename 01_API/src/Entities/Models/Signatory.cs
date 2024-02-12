// <copyright file="Signatory.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate
{
    public class Signatory
    {
        public Signatory(string? title, string? firstName, string? lastName, string? email)
        {
            this.Title = title;
            this.FirstName = firstName;
            this.LastName = lastName;
            this.Email = email;
        }

        public string? Title { get; }

        public string? FirstName { get; }

        public string? LastName { get; }

        public string? Email { get; }
    }
}
