// <copyright file="CompanyCollaboratorDb.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Sql
{
    public class CompanyCollaboratorDb
    {
        public Guid CompanyId { get; set; }

        public CompanyDb? Company { get; set; } = null!;

        public Guid CollaboratorId { get; set; }

        public CollaboratorDb? Collaborator { get; set; } = null!;
    }
}
