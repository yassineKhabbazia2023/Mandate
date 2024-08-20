// <copyright file="SqlExtensions.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

using KPMG.Pulse.Back.Accounting.Mandate.Sql;

namespace KPMG.Pulse.Back.Accounting.Mandate.Adapters
{
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

        public static Bank ToModel(this Sql.RefBankDb source)
        {
            var bankagreement = new BankAgreement((JdcPartnership)source.JdcPartnership);
            return new Bank(source.BankCode, source.BankName, source.BankGroup, source.EbicsCardId, bankagreement);
        }

        public static Collection ToModel(this Sql.CollectionDb source)
        {
            Company company = new Company(
                source.Company != null ? source.Company.Id : default,
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

        public static Sql.CollectionQuery ToSql(this CollectionQueryDto source, int collaboratorId)
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

        public static Sql.CollectionDb ToSql(this Bban source, int companyId)
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

        public static Sql.CompanyDb ToSql(this Company company)
        {
            return new Sql.CompanyDb()
            {
                Id = company!.Id,
                Name = company.Name,
                ErpId = company.ErpId,
                SiretNumber = company.SiretNumber,
                CompanyCollaborators = new List<Sql.CompanyCollaboratorDb>(),
                JeDeclareFolder = company.ToJeDeclareFolderDb(),
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
                StatusCode = (int)CollectionStatus.Creation_Inprogress,
                StatusDate = DateTime.UtcNow,
            };
        }

        public static Sql.JeDeclareFolderDb ToJeDeclareFolderDb(this Company company)
        {
            return new Sql.JeDeclareFolderDb()
            {
                CompanyId = company!.Id,
                JdcDossierId = company!.BankServicesProviderId,
            };
        }

        public static Sql.JeDeclareCollectionDb ToDeclareCollectionDb(this Collection collection)
        {
            return new Sql.JeDeclareCollectionDb()
            {
                CollectionId = collection!.Id,
                JdcReleveId = collection!.CollectionServicesProviderId,
                JdcRibId = collection!.Bban?.BbanServicesProviderId,
            };
        }

        public static Sql.PersonalDb ToPersonalDb(this Company company)
        {
            return new Sql.PersonalDb()
            {
                City = company.Address?.City,
                Country = company.Address?.Country,
                Email = company.Signatory?.Email!,
                FirstName = company.Signatory?.FirstName,
                LastName = company.Signatory?.LastName,
                Street = company.Address?.Street,
                ZipCode = company.Address?.ZipCode,
                Complements = company.Address?.Complements,
                Title = company.Signatory?.Title,
            };
        }

        public static List<Sql.StatusDb> ToStatusesDB(this Collection collection)
        {
            return new List<Sql.StatusDb>()
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    CollectionId = collection.Id,
                    StatusCode = -1,
                    IsCurrent = true,
                    StatusDate = DateTime.UtcNow,
                },
            };
        }

        public static Sql.CollectionDb ToCollectionDB(this Collection collection, int companyId)
        {
            return new Sql.CollectionDb()
            {
                Id = collection.Id,
                AccountNumber = collection.Bban!.AccountNumber.ToString(),
                BranchCode = collection.Bban!.BranchCode,
                CheckDigits = collection.Bban!.CheckDigits,
                BankCode = collection.Bban!.BankCode,
                JeDeclareCollection = collection.ToDeclareCollectionDb(),
                Personal = collection.Company!.ToPersonalDb(),
                CompanyId = companyId,
                Statuses = collection.ToStatusesDB(),
            };
        }

        public static Sql.MandateLogDb ToMandateLogDB(this Collection collection, CustomException exception)
        {
            return new Sql.MandateLogDb
            {
                ErpId = collection.Company?.ErpId!,
                SiretNumber = collection.Company?.SiretNumber!,
                BankCode = collection.Bban?.BankCode!,
                BranchCode = collection.Bban?.BranchCode!,
                AccountNumber = collection.Bban?.AccountNumber!,
                CheckDigits = collection.Bban?.CheckDigits!,
                JdcDossierId = collection.Company?.BankServicesProviderId!,
                JdcReleveId = collection.CollectionServicesProviderId!,
                JdcRibId = collection.Bban?.BbanServicesProviderId!,
                CreationDate = DateTime.Now,
                ExceptionType = (Sql.ExceptionType)exception.Type,
                ExceptionMessage = exception.Message,
                InnerExceptionMessage = exception.InnerException?.Message,
            };
        }

        public static CollectionDb ToCollectionDb(this Bban source, int companyId)
        {
            return new CollectionDb()
            {
                JdcRibId = source.BbanServicesProviderId,
                AccountNumber = source.AccountNumber,
                BankCode = source.BankCode,
                BranchCode = source.BranchCode,
                CheckDigits = source.CheckDigits,
                CompanyId = companyId,
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