// <copyright file="Collaborator.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate
{
    public class Collaborator
    {
        public Collaborator(int id, string email, string? firstName, string? lastName)
        {
            this.Id = id;
            this.Email = email;
            this.FirstName = firstName;
            this.LastName = lastName;
        }

        public int Id { get; }

        public string Email { get; }

        public string? FirstName { get; }

        public string? LastName { get; }
    }
}
