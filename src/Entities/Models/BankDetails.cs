// <copyright file="BankDetails.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate
{
    public class BankDetails
    {
        public BankDetails(string? bankName, string? bankCode, string? branchCode, string? accountNumber, string? checkDigits)
        {
            this.BankName = bankName;
            this.BankCode = bankCode;
            this.BranchCode = branchCode;
            this.AccountNumber = accountNumber;
            this.CheckDigits = checkDigits;
        }

        public string? BankName { get; }

        public string? BankCode { get; }

        public string? BranchCode { get; }

        public string? AccountNumber { get; }

        public string? CheckDigits { get; }
    }
}
