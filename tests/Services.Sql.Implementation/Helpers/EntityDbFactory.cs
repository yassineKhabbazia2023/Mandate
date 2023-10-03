// <copyright file="EntityDbFactory.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Sql.Implementation.Tests
{
    using System.Data;

    public static class EntityDbFactory
    {
        public static RefBankDb RefBankDb => new ()
        {
            BankCode = "12345",
            BankName = "bn",
            BankCommercialName = "bcn",
            BankCategory = "bca",
            BankGroup = "bg",
            IsJdcScrapable = true,
            IsJdcPartner = true,
            HasReleveAgreement = false,
            HasLiasseAgreement = false,
            AllowsDemat = false,
            JdcPartnership = (JdcPartnership)2,
        };

        public static RefBankDb FromRow(DataRow source)
        {
            return new RefBankDb()
            {
                BankCode = (string)source["BankCode"],
                BankName = (string)source["BankName"],
                BankCommercialName = (string)source["BankCommercialName"],
                BankCategory = (string)source["BankCategory"],
                BankGroup = (string)source["BankGroup"],
                IsJdcScrapable = (bool)source["IsJdcScrapable"],
                IsJdcPartner = (bool)source["IsJdcPartner"],
                HasReleveAgreement = (bool)source["HasReleveAgreement"],
                HasLiasseAgreement = (bool)source["HasLiasseAgreement"],
                AllowsDemat = (bool)source["AllowsDemat"],
                JdcPartnership = (JdcPartnership)source["JdcPartnership"],
            };
        }
    }
}
