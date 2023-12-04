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

        public async Task<PagedMandate> GetAllCollectionsAsync(CollectionQueryDto query)
        {
            (List<Sql.CollectionDb>, int) tuple = await this.mandateRepository.SearchCollectionsAsync(query.ToSql());

            return new PagedMandate(
                new Counters(tuple.Item2, 0, 0, 0, 0, 0),
                tuple.Item1.Select(i => i.ToModel()).ToList());
        }

        public async Task<Company> GetCompanyByErpIdAsync(string erpId)
        {
            var company = await this.mandateRepository.GetCompanyByErpIdAsync(erpId);
            return company.ToModel();
        }

        public Task<Company> CreateFolderAsync(string bankServicesProviderId, Guid companyId)
        {
            // Creation JeDeclare Folder
            throw new NotImplementedException();
        }

        public Task<Status> CreateStatus(Guid collectionId, Status status)
        {
            // Création d'un status relié a une collecte
            throw new NotImplementedException();
        }

        public Task<Collection> InsertServicesProviderIds(Guid collectionId, string collectionServicesProviderId, string bbanServicesProviderId)
        {
            // Création de JeDeclare Collection
            throw new NotImplementedException();
        }

        public Task<bool> CheckCollecteConfigExist(Bban bban)
        {
            // Vérifier si le rib existe déja dans la base
            throw new NotImplementedException();
        }

        public Task<Collection> CreateCollection(string erpId, Guid companyId, Bban bban)
        {
            throw new NotImplementedException();
        }

        public Task<Collection> UpdateCollection(Guid id, Collection collection)
        {
            throw new NotImplementedException();
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
    }
}
