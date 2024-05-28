// <copyright file="MandateLogDb.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Sql
{
    public class MandateLogDb
    {
        public Guid Id { get; set; }

        public string BankCode { get; set; } = null!;

        public string BranchCode { get; set; } = null!;

        public string AccountNumber { get; set; } = null!;

        public string CheckDigits { get; set; } = null!;

        public string ErpId { get; set; } = null!;

        public string SiretNumber { get; set; } = null!;

        public string JdcDossierId { get; set; } = null!;

        public string JdcReleveId { get; set; } = null!;

        public string JdcRibId { get; set; } = null!;

        public DateTime CreationDate { get; set; } = DateTime.Now;

        public ExceptionType ExceptionType { get; set; }

        public string? ExceptionMessage { get; set; } = null!;

        public string? InnerExceptionMessage { get; set; } = null!;
    }
}