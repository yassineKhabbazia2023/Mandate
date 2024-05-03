// <copyright file="EmailData.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Application
{
    public class EmailData
    {
        public EmailData(SignatoryDetails signatoryDetails, Bban bban, string? ibs, string? companyName, string userEmail)
        {
            this.CollaboratorEmail = signatoryDetails?.CollaboratorEmail;
            this.SignatoryName = signatoryDetails?.SignatoryName;
            this.SiretNumber = signatoryDetails?.SiretNumber;
            this.BankName = bban?.Bank?.Name;
            this.BankCode = bban?.BankCode;
            this.BranchCode = bban?.BranchCode;
            this.AccountNumber = bban?.AccountNumber;
            this.CheckDigits = bban?.CheckDigits;
            this.Ibs = ibs;
            this.CompanyName = companyName;
            this.UserEmail = userEmail;
        }

        public string? CollaboratorEmail { get; }

        public string? SignatoryName { get; }

        public string? SiretNumber { get; }

        public string? BankName { get; }

        public string? BankCode { get; }

        public string? BranchCode { get; }

        public string? AccountNumber { get; }

        public string? CheckDigits { get; }

        public string? Ibs { get; set; }

        public string? CompanyName { get; }

        public string UserEmail { get; set; }
    }
}
