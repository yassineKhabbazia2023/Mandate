// <copyright file="SqlAdapter.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Adapters
{
    using KPMG.Pulse.Back.Accounting.Mandate.Sql;

    public class SqlAdapter : IDatabaseService
    {
        private readonly Sql.IMandateRepository mandateRepository;

        public SqlAdapter(Sql.IMandateRepository mandateRepository)
        {
            this.mandateRepository = mandateRepository;
        }

        public async Task<Company> GetCompanyBySiretAsync(string siret)
        {
            var companyDb = await this.mandateRepository.GetCompanyBySiretAsync(siret).ConfigureAwait(false);

            var address = new Address(companyDb.Personal?.Street, companyDb.Personal?.Complements, companyDb.Personal?.ZipCode, companyDb.Personal?.City, companyDb.Personal?.Country);
            var signatory = new Signatory(companyDb.Personal?.Title, companyDb.Personal?.FirstName, companyDb.Personal?.LastName, companyDb.Personal?.Email);
            var company = new Company(companyDb.Id, companyDb.Name, companyDb.SiretNumber, companyDb.ErpId, companyDb.JeDeclareFolder?.JdcDossierId, signatory, address);
            return company;
        }

        public async Task<Bank> GetBankByCodeAsync(string bankCode)
        {
            var refBankDb = await this.mandateRepository.GetRefBankByCodeAsync(bankCode).ConfigureAwait(false);
            return refBankDb.ToModel();
        }

        public async Task<byte[]> GetPdfTemplateByBankCodeAsync(string bankCode)
        {
            return await this.mandateRepository.GetPdfTemplateByCodeAsync(bankCode).ConfigureAwait(false);
        }

        public async Task<PagedMandate> GetAllCollectionsAsync(CollectionQueryDto query, Guid collaboratorId)
        {
            (List<Sql.CollectionDb>, int) tuple = await this.mandateRepository.SearchCollectionsAsync(query.ToSql(collaboratorId));

            return new PagedMandate(
                new Counters(tuple.Item2, 0, 0, 0, 0, 0),
                tuple.Item1.Select(i => i.ToModel()).ToList());
        }

        public async Task<Company> GetCompanyByErpIdAsync(string erpId)
        {
            var company = await this.mandateRepository.GetCompanyByErpIdAsync(erpId);
            return company.ToModel();
        }

        public async Task CreateOrUpdateFolderAsync(string bankServicesProviderId, Guid companyId)
        {
            await this.mandateRepository.CreateOrUpdateFolderAsync(bankServicesProviderId, companyId);
        }

        public async Task<Status> CreateStatus(Guid collectionId, int statusCode)
        {
            // update current Status to false
            await this.mandateRepository.UpdateCurrentStatusAsync(collectionId);

            // select de la ref pour avoir
            StatusDb statusDb = new Sql.StatusDb()
            {
                CollectionId = collectionId,
                IsCurrent = true,
                StatusCode = statusCode,
                StatusDate = DateTime.UtcNow,
                CreatedBy = string.Empty,
            };

            // Création d'un status relié a une collecte
            return (await this.mandateRepository.CreateStatusAsync(collectionId, statusDb)).ToModel();
        }

        public async Task InsertServicesProviderIds(Guid collectionId, string collectionServicesProviderId, string bbanServicesProviderId)
        {
            // Création de JeDeclare Collection
            await this.mandateRepository.InsertServicesProviderIdsAsync(collectionId, collectionServicesProviderId, bbanServicesProviderId);
        }

        public async Task<bool> CheckCollecteConfigExistAsync(Bban bban)
        {
            return await this.mandateRepository.CheckCollecteConfigExistAsync(bban.BankCode, bban.BranchCode, bban.AccountNumber);
        }

        public async Task<Guid> CreateCollectionAsync(Bban bban, Guid companyId)
        {
            CollectionDb collection = bban.ToSql(companyId);
            collection.Statuses = new List<StatusDb>() { SqlExtensions.DefaultStatus() };
            return (await this.mandateRepository.CreateCollectionAsync(collection)).Id;
        }

        public Task<Collection> UpdateCollection(Guid id, Collection collection)
        {
            throw new NotImplementedException();
        }

        public async Task<Collaborator> GetCollaboratorByEmail(string collaboratorEmail)
        {
            var collabDb = await this.mandateRepository.GetCollaboratorByEmailAsync(collaboratorEmail).ConfigureAwait(false);

            return collabDb.ToModel();
        }

        public async Task<Collection> GetCollectionById(Guid collectionId)
        {
            var collectionDb = await this.mandateRepository.GetCollectionById(collectionId).ConfigureAwait(false);
            return collectionDb.ToModel();
        }

        public async Task SaveSignatoryAsync(Guid? companyId, Guid? collectionId, Signatory signatory, Address address)
        {
            var newPersonalDb = new PersonalDb()
            {
                CompanyId = companyId,
                CollectionId = collectionId,
                City = address.City,
                Country = address.Country,
                Street = address.Street,
                ZipCode = address.ZipCode,
                Complements = address.Complements,
                FirstName = signatory.FirstName,
                LastName = signatory.LastName,
                Email = signatory.Email,
                Title = signatory.Title,
            };

            await this.mandateRepository.SaveSignatoryAsync(newPersonalDb);
        }

        public async Task CreateFakeRefAsync()
        {
            await this.mandateRepository.CreateFakeRefAsync().ConfigureAwait(false);
        }

        public async Task DeleteFakeRefAsync()
        {
            await this.mandateRepository.DeleteFakeRefAsync().ConfigureAwait(false);
        }

        public async Task CreateFakeAuthAsync()
        {
            await this.mandateRepository.CreateFakeAuthAsync().ConfigureAwait(false);
        }

        public async Task DeleteFakeAuthAsync()
        {
            await this.mandateRepository.DeleteFakeAuthAsync().ConfigureAwait(false);
        }

        public async Task AddFakeDataAsync()
        {
            await this.mandateRepository.AddFakeDataAsync().ConfigureAwait(false);
        }

        public async Task DeleteFakeDataAsync()
        {
            await this.mandateRepository.DeleteFakeDataAsync().ConfigureAwait(false);
        }
    }
}
