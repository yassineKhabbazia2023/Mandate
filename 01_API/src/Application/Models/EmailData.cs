// <copyright file="EmailData.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Application
{
    public class EmailData : Dictionary<string, string>
    {
        private EmailData(Collection collection)
        {
            AccountNumber = collection.GetAccountNumber();
            BranchCode = collection.GetBranchCode();
            CheckDigits = collection.GetCheckDigits();
            CompanyName = collection.GetCompanyName();
            this["collaboratorEmail"] = collection.GetSignatoryEmail();
            this["siretNumber"] = collection.GetSiretNumber();
            this["accountHolder"] = collection.GetSignatoryFullName();
            this["bankName"] = collection.GetBankName();
            this["bankCode"] = collection.GetBankCode();
            this["accountNumber"] = AccountNumber;
            this["branchCode"] = BranchCode;
            this["checkDigits"] = CheckDigits;
            this["companyName"] = CompanyName;
        }

        public string BranchCode { get; }

        public string AccountNumber { get; }

        public string CheckDigits { get; }

        public string CompanyName { get; }

        public static EmailData FromCollection(Collection collection) =>
            new(collection);
    }
}