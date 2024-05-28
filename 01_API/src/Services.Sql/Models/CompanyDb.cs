// <copyright file="CompanyDb.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Sql
{
    public class CompanyDb
    {
        public int Id { get; set; }

        public PersonalDb? Personal { get; set; } = null!;

        public JeDeclareFolderDb? JeDeclareFolder { get; set; } = null!;

        public List<CollectionDb> Collections { get; set; } = null!;

        public string? Name { get; set; }

        public string SiretNumber { get; set; } = null!;

        public string? ErpId { get; set; }

        public List<CompanyCollaboratorDb> CompanyCollaborators { get; set; } = null!;

        public bool IsActive { get; set; }
    }
}
