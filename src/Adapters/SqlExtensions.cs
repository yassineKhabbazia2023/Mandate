// <copyright file="SqlExtensions.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Adapters
{
    using KPMG.Pulse.Back.Accounting.Mandate.Models.Enums;

    public static class SqlExtensions
    {
        public static Bank ToModel(this Sql.RefBankDb source)
        {
            var bankagreement = new BankAgreement((JdcPartnership)source.JdcPartnership);
            return new Bank(source.BankCode, source.BankName, source.BankGroup, source.EbicsCardId, bankagreement);
        }

        public static Signatory ToSignatory(this Sql.PersonalDb source)
        {
            return new Signatory(source.Title, source.FirstName, source.LastName, source.Email);
        }

        public static Address ToAdress(this Sql.PersonalDb source)
        {
            return new Address(source.Street, source.Complements, source.ZipCode, source.City, source.Country);
        }

        public static Collection ToModel(this Sql.CollectionDb source)
        {
            Company company = new Company(
                source.Company != null ? source.Company.Id : Guid.Empty,
                source.Company?.Name!,
                source.Company?.SiretNumber!,
                source.Company?.ErpId,
                source.Company?.JeDeclareFolder?.JdcDossierId,
                source.Personal?.ToSignatory(),
                source.Personal?.ToAdress());

            Bank? bank = source.Bank?.ToModel();

            Bban? bban = new Bban(source.BankCode!, source.BranchCode!, source.AccountNumber!, source.CheckDigits!, source.JeDeclareCollection?.JdcRibId, bank);

            var currentStatus = source.Statuses?.SingleOrDefault(i => i.IsCurrent);
            var creationStatus = source.Statuses?.SingleOrDefault(i => i.StatusCode == -1);

            Status status = new Status(
                (CollectionStatus)currentStatus?.RefStatusCode!.PulseCode!,
                currentStatus.RefStatusCode?.StatusNameFr!);

            return new Collection(
                id: source.Id,
                collectionServicesProviderId: source.JeDeclareCollection?.JdcReleveId,
                company: company,
                bban: bban,
                creationDate: (creationStatus?.StatusDate!).Value,
                modificationDate: (currentStatus.StatusDate!).Value,
                status: status);
        }

        public static Company ToModel(this Sql.CompanyDb source)
        {
            var address = new Address(source.Personal?.Street, source.Personal?.Complements, source.Personal?.ZipCode, source.Personal?.City, source.Personal?.Country);
            var signatory = new Signatory(source.Personal?.Title, source.Personal?.FirstName, source.Personal?.LastName, source.Personal?.Email);
            return new Company(
                source.Id,
                source.Name,
                source.SiretNumber,
                source.ErpId,
                source.JeDeclareFolder?.JdcDossierId,
                signatory,
                address);
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

        public static Sql.CollectionDb ToSql(this Bban source, Guid companyId)
        {
            return new Sql.CollectionDb()
            {
                AccountNumber = source.AccountNumber,
                BankCode = source.BankCode,
                BranchCode = source.BranchCode,
                CheckDigits = source.CheckDigits,
                CompanyId = companyId,
                RejectReason = null,
            };
        }

        public static Sql.PersonalDb ToPersonalCollection(this CollectionCreationCommand source)
        {
            return new Sql.PersonalDb()
            {
                Title = source.Signatory.Title,
                FirstName = source.Signatory.FirstName,
                LastName = source.Signatory.LastName,
                Email = source.Signatory.Email,
                Street = source.Address.Street,
                Complements = source.Address.Complements,
                ZipCode = source.Address.ZipCode,
                City = source.Address.City,
                Country = source.Address.Country,
            };
        }

        public static Status ToModel(this Sql.StatusDb source)
        {
            return new Status((CollectionStatus)source.StatusCode, source.RefStatusCode.StatusNameFr);
        }

        public static Sql.StatusDb DefaultStatus()
        {
            return new Sql.StatusDb()
            {
                IsCurrent = true,
                StatusCode = (int)JdcCollectionStatus.InitialCreate,
                StatusDate = DateTime.UtcNow,
            };
        }
    }
}