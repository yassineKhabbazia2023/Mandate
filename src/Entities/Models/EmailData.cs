// <copyright file="EmailData.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate
{
    public class EmailData
    {
        public EmailData(SignatoryDetails signatoryDetails, BankDetails bankDetails, string? ibs)
        {
            this.CollaboratorEmail = signatoryDetails.CollaboratorEmail;
            this.SignatoryName = signatoryDetails.SignatoryName;
            this.SiretNumber = signatoryDetails.SiretNumber;
            this.BankName = bankDetails.BankName;
            this.BankCode = bankDetails.BankCode;
            this.BranchCode = bankDetails.BranchCode;
            this.AccountNumber = bankDetails.AccountNumber;
            this.CheckDigits = bankDetails.CheckDigits;
            this.Ibs = ibs;
        }

        public string? CollaboratorEmail { get; }

        public string? SignatoryName { get; }

        public string? SiretNumber { get; }

        public string? BankName { get; }

        public string? BankCode { get; }

        public string? BranchCode { get; }

        public string? AccountNumber { get; }

        public string? CheckDigits { get; }

        public string? Ibs { get; }
    }
}
