// <copyright file="JeDeclareFolderDb.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Sql
{
    public class JeDeclareFolderDb
    {
        public Guid Id { get; set; }

        public int CompanyId { get; set; }

        public CompanyDb? Company { get; set; } = null!;

        public string? JdcDossierId { get; set; } = null!;
    }
}
