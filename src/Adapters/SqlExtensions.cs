// <copyright file="SqlExtensions.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Adapters
{
    public static class SqlExtensions
    {
        public static Bank ToModel(this Sql.RefBankDb source)
        {
            var bankagreement = new BankAgreement(source.IsJdcPartner, source.IsJdcScrapable, source.HasReleveAgreement);
            return new Bank(source.BankCode, source.BankName, source.BankGroup, bankagreement);
        }

        public static Collection ToModel(this Sql.CollectionDb source)
        {
            Company company = new Company(
                source.Company!.Id,
                source.Company!.Name,
                source.Company.SiretNumber,
                source.Company.ErpId,
                source.Company.BankServicesProviderId,
                default,
                default);

            Bank? bank = new Bank(source.Bank!.BankCode, source.Bank!.BankName, source.Bank!.BankGroup, null!);

            Bban? bban = new Bban(source.BankCode!, source.BranchCode!, source.AccountNumber!, source.CheckDigits!, bank);

            var currentStatus = source.Statuses?.SingleOrDefault(i => i.IsCurrent);
            var creationStatus = source.Statuses?.SingleOrDefault(i => i.StatusCode == -1);

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

        public static Sql.CollectionQuery ToSql(this CollectionQueryDto source)
        {
            return new Sql.CollectionQuery
            {
                SearchTerm = source.SearchTerm,
                CreationDateStart = source.CreationDateStart,
                CreationDateEnd = source.CreationDateEnd,
                ModificationDateStart = source.ModificationDateStart,
                ModificationDateEnd = source.ModificationDateEnd,
                StatusCodes = source.StatusCodes,
                Limit = source.Limit,
                Skip = source.Skip,
                SortOrder = (Sql.SortOrder)source.SortOrder,
                SortCriteria = (Sql.CollectionSortCriteria)source.SortCriteria,
                CollaboratorId = source.CollaboratorId,
            };
        }
    }
}