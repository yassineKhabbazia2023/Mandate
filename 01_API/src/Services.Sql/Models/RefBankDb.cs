// <copyright file="RefBankDb.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Sql
{
    public class RefBankDb
    {
        public string BankCode { get; set; } = string.Empty;

        public string? BankName { get; set; } = null!;

        public string? BankCommercialName { get; set; } = null!;

        public string? BankCategory { get; set; } = null!;

        public string? BankGroup { get; set; } = null!;

        public bool IsJdcScrapable { get; set; } = false;

        public bool IsJdcPartner { get; set; } = false;

        public bool? HasReleveAgreement { get; set; } = null!;

        public bool? HasLiasseAgreement { get; set; } = null!;

        public bool? AllowsDemat { get; set; } = null!;

        public JdcPartnership JdcPartnership { get; set; } = 0;

        public string? EbicsCardId { get; set; } = null!;
    }
}
