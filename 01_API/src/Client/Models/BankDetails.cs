// <copyright file="BankDetails.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Client
{
    public class BankDetails
    {
        public BankDetails(string bankCode, string branchCode, string accountNumber, string checkDigits)
        {
            this.BankCode = bankCode;
            this.BranchCode = branchCode;
            this.AccountNumber = accountNumber;
            this.CheckDigits = checkDigits;
        }

        public string BankCode { get; }

        public string BranchCode { get; }

        public string AccountNumber { get; }

        public string CheckDigits { get; }
    }
}
