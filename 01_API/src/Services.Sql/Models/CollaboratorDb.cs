// <copyright file="CollaboratorDb.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Sql
{
    public class CollaboratorDb
    {
        public int Id { get; set; }

        public string Email { get; set; } = null!;

        public string? FirstName { get; set; } = null!;

        public string? LastName { get; set; } = null!;

        public bool IsActive { get; set; }

        public List<CompanyCollaboratorDb> CompanyCollaborators { get; set; } = null!;

        public List<CollectionDb> Collections { get; set; } = null!;
    }
}
