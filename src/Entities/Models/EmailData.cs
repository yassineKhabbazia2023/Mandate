// <copyright file="EmailData.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate
{
    public class EmailData
    {
        public string? CollaboratorEmail { get; }

        public string? Ibs { get; }

        public string? SiretNumber { get; }

        public string? SignatoryName { get; }

        public string? BankName { get; }

        public string? BankCode { get; }

        public string? BranchCode { get; }

        public string? AccountNumber { get; }

        public string? CheckDigits { get; }
    }
}
