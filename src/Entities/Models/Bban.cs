// <copyright file="Bban.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate
{
    public class Bban
    {
        public Bban(string? bbanServicesProviderId, string bankCode, string branchCode, string accountNumber, string checkDigits, Bank? bank)
        {
            this.BbanServicesProviderId = bbanServicesProviderId;
            this.BankCode = bankCode;
            this.BranchCode = branchCode;
            this.AccountNumber = accountNumber;
            this.CheckDigits = checkDigits;
            this.Bank = bank;
        }

        public string? BbanServicesProviderId { get; }

        public string BankCode { get; }

        public string BranchCode { get; }

        public string AccountNumber { get; }

        public string CheckDigits { get; }

        public Bank? Bank { get; }
    }
}
