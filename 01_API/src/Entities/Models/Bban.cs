// <copyright file="Bban.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate
{
    public class Bban
    {
        private Bank? bank;

        public Bban(string bankCode, string branchCode, string accountNumber, string checkDigits, string? bbanServicesProviderId, Bank? bank)
        {
            this.BbanServicesProviderId = bbanServicesProviderId;
            this.BankCode = bankCode;
            this.BranchCode = branchCode;
            this.AccountNumber = accountNumber;
            this.CheckDigits = checkDigits;
            this.bank = bank;
        }

        public string BankCode { get; }

        public string BranchCode { get; }

        public string AccountNumber { get; }

        public string CheckDigits { get; }

        // ribId
        public string? BbanServicesProviderId { get; }

        public Bank? Bank
        {
            get
            {
                return this.bank;
            }
        }

        public void SetBank(Bank b)
        {
            this.bank = b;
        }
    }
}
