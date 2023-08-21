// <copyright file="CompanyDb.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Sql
{
    public class CompanyDb
    {
        public Guid Id { get; set; }

        public CompanyPersonalDb? CompanyPersonal { get; set; } = null!;

        public JeDeclareFolderDb? JeDeclareFolder { get; set; } = null!;

        public List<CollectionDb> Collections { get; set; } = null!;

        public string? Name { get; set; }

        public string SiretNumber { get; set; } = null!;

        public string? ErpId { get; set; }

        public string? BankServicesProviderId { get; set; }
    }
}
