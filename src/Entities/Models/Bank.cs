// <copyright file="Bank.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate
{
    public class Bank
    {
        public Bank(string code, string? name, string? group, string? ebicsCardId, BankAgreement jdcAgreement)
        {
            this.Code = code;
            this.Name = name;
            this.Group = group;
            this.EbicsCardId = ebicsCardId;
            this.JdcAgreement = jdcAgreement;
        }

        public string Code { get; }

        public string? Name { get; }

        public string? Group { get; }

        public string? EbicsCardId { get; }

        public BankAgreement JdcAgreement { get; }
    }
}
