// <copyright file="CompanyCollaboratorDb.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Sql
{
    public class CompanyCollaboratorDb
    {
        public int CompanyId { get; set; }

        public CompanyDb? Company { get; set; } = null!;

        public int CollaboratorId { get; set; }

        public CollaboratorDb? Collaborator { get; set; } = null!;
    }
}
