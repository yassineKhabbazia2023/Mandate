// <copyright file="BankAgreement.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate
{
    public class BankAgreement
    {
        public BankAgreement(JdcPartnership jdcPartnership)
        {
            this.JdcPartnership = jdcPartnership;
        }

        public JdcPartnership JdcPartnership { get; }
    }
}
