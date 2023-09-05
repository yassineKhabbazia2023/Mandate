// <copyright file="BankAgreement.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate
{
    public class BankAgreement
    {
        public BankAgreement(bool isJdcPartner, bool isJdcScrapable, bool? hasJdcReleve)
        {
            this.IsJdcPartner = isJdcPartner;
            this.IsJdcScrapable = isJdcScrapable;
            this.HasJdcReleve = hasJdcReleve;
        }

        public bool IsJdcPartner { get; }

        public bool IsJdcScrapable { get; }

        public bool? HasJdcReleve { get; }
    }
}
