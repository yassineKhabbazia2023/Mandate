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

        public static Client.MandateCollection ToMandateDetail(this Collection source)
        {
            return new Client.MandateCollection(
                    id: source.Id,
                    erpId: source.Company?.ErpId!,
                    companyName: source.Company?.Name!,
                    bankName: source.Bban?.Bank?.Name!,
                    accountNumber: source.Bban?.AccountNumber!,
                    creationDate: source.CreationDate,
                    modificationDate: source.ModificationDate,
                    statusCode: (int)source.Status.StatusCode,
                    statusName: source.Status?.StatusName!);
        }

        public static Collection ToModel(this Sql.CollectionDb source)
        {
            Company company = new Company(
                source.Company!.Id,
                source.Company!.Name,
                source.Company.SiretNumber,
                source.Company.ErpId,
                source.Company.BankServicesProviderId,
                null);

            Bank? bank = new Bank(source.Bank!.BankCode, source.Bank!.BankName, source.Bank!.BankGroup, null!);

            Bban? bban = new Bban(source.BankCode!, source.BranchCode!, source.AccountNumber!, source.CheckDigits!, bank);

            var currentStatus = source.Statuses?.SingleOrDefault(i => i.IsCurrent);
            var creationStatus = source.Statuses?.SingleOrDefault(i => i.RefStatusCode?.PulseCode! == -1);

            Status status = new Status(
                (CollectionStatus)currentStatus?.StatusCode!,
                currentStatus.RefStatusCode?.StatusNameFr!);

            return new Collection(
                id: source.Id,
                company: company,
                bban: bban,
                creationDate: (creationStatus?.StatusDate!).Value,
                modificationDate: (currentStatus.StatusDate!).Value,
                status: status);
        }
    }
}