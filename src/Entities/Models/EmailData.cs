// <copyright file="EmailData.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate
{
    public class EmailData
    {
        public EmailData(string? collaboratorEmail, string? ibs, string? siretNumber, string? signatoryName, string? bankName, string? bankCode, string? branchCode, string? accountNumber, string? checkDigits)
        {
            this.CollaboratorEmail = collaboratorEmail;
            this.Ibs = ibs;
            this.SiretNumber = siretNumber;
            this.SignatoryName = signatoryName;
            this.BankName = bankName;
            this.BankCode = bankCode;
            this.BranchCode = branchCode;
            this.AccountNumber = accountNumber;
            this.CheckDigits = checkDigits;
        }

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
