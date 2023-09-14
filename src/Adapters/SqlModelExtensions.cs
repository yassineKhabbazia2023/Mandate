// <copyright file="SqlModelExtensions.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Adapters
{
    public static class SqlModelExtensions
    {
        public static Bank ToModel(this Sql.RefBankDb source)
        {
            var bankagreement = new BankAgreement(source.IsJdcPartner, source.IsJdcScrapable, source.HasReleveAgreement);
            return new Bank(source.BankCode, source.BankName, source.BankGroup, bankagreement);
        }

        public static Client.BankDetail ToBankDetail(this Bank source)
        {
            var bankJdcDetail = new Client.BankJdcDetail(source.JdcAgreement.IsJdcPartner, source.JdcAgreement.IsJdcScrapable);
            return new Client.BankDetail(source.Code, source.Name, bankJdcDetail);
        }
    }
}
