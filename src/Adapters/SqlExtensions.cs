// <copyright file="SqlExtensions.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Adapters
{
    using System.Runtime.CompilerServices;
    using KPMG.Pulse.Back.Accounting.Mandate.Models.Enums;

    public static class SqlExtensions
    {
        public static Signatory ToSignatory(this Sql.PersonalDb source)
        {
            return new Signatory(source.Title, source.FirstName, source.LastName, source.Email);
        }

        public static Address ToAdress(this Sql.PersonalDb source)
        {
            return new Address(source.Street, source.Complements, source.ZipCode, source.City, source.Country);
        }

        public static Bank ToModel(this Sql.RefBankDb source)
        {
            var bankagreement = new BankAgreement((JdcPartnership)source.JdcPartnership);
            return new Bank(source.BankCode, source.BankName, source.BankGroup, source.EbicsCardId, bankagreement);
        }

        public static Bban ToBbanModel(this Sql.CollectionDb source)
        {
            return new Bban(
                source.BankCode!,
                source.BranchCode!,
                source.AccountNumber!,
                source.CheckDigits!,
                source.JeDeclareCollection?.JdcRibId,
                source.Bank?.ToModel());
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

            Bban? bban = source.ToBbanModel();

            var currentStatus = source.Statuses?.SingleOrDefault(i => i.IsCurrent);
            var creationStatus = source.Statuses?.SingleOrDefault(i => i.StatusCode == -1);

            ValidateStatuses(currentStatus, creationStatus);

            Status? status = currentStatus!.ToModel();

            return new Collection(
                id: source.Id,
                collectionServicesProviderId: source.JeDeclareCollection?.JdcReleveId,
                company: company,
                bban: bban,
                creationDate: GetCreationDate(creationStatus),
                modificationDate: GetModificationDate(currentStatus!, creationStatus!),
                status: status!);
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

        public static Collaborator ToModel(this Sql.CollaboratorDb source)
        {
            return new Collaborator(source.Id, source.Email, source.FirstName, source.LastName);
        }

        public static Status ToModel(this Sql.StatusDb source)
        {
            return new Status(
                (CollectionStatus)source?.RefStatusCode?.PulseCode!,
                source?.RefStatusCode?.StatusNameFr!);
        }

        public static Sql.CollectionQuery ToSql(this CollectionQueryDto source, Guid collaboratorId)
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
                CollaboratorId = collaboratorId,
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

        public static Sql.StatusDb DefaultStatus()
        {
            return new Sql.StatusDb()
            {
                IsCurrent = true,
                StatusCode = (int)JdcCollectionStatus.InitialCreate,
                StatusDate = DateTime.UtcNow,
            };
        }

        private static DateTime GetCreationDate(Sql.StatusDb? creationStatus)
        {
            return creationStatus!.StatusDate!.Value;
        }

        private static DateTime GetModificationDate(Sql.StatusDb currentStatus, Sql.StatusDb creationStatus)
        {
            if (currentStatus.StatusCode == creationStatus.StatusCode)
            {
                return creationStatus.StatusDate!.Value;
            }
            else
            {
                return currentStatus.StatusDate!.Value;
            }
        }

        private static void ValidateStatuses(Sql.StatusDb? currentStatus, Sql.StatusDb? creationStatus)
        {
            if (currentStatus == null)
            {
                throw new ApplicationException($"{nameof(SqlExtensions)} - {nameof(ToModel)} : Error while parsing Collection {nameof(currentStatus)} is null.");
            }

            if (creationStatus == null)
            {
                throw new ApplicationException($"{nameof(SqlExtensions)} - {nameof(ToModel)} : Error while parsing Collection {nameof(creationStatus)} is null.");
            }
        }
    }
}