// <copyright file="CollectionDb.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Sql
{
    public class CollectionDb
    {
        public Guid Id { get; set; }

        public int CompanyId { get; set; }

        public CompanyDb? Company { get; set; } = null!;

        public PersonalDb? Personal { get; set; } = null!;

        public JeDeclareCollectionDb? JeDeclareCollection { get; set; } = null!;

        public List<StatusDb> Statuses { get; set; } = null!;

        public string BankCode { get; set; } = null!;

        public RefBankDb? Bank { get; set; } = null!;

        public string BranchCode { get; set; } = null!;

        public string AccountNumber { get; set; } = null!;

        public string CheckDigits { get; set; } = null!;

        public int? LinkType { get; set; } = null!;

        public string? RejectReason { get; set; } = null!;
    }
}
