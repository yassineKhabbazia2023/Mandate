// <copyright file="SqlAdapter.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Adapters
{
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

            var address = new Address(companyDb.CompanyPersonal?.Street, companyDb.CompanyPersonal?.Complements, companyDb.CompanyPersonal?.ZipCode, companyDb.CompanyPersonal?.City, companyDb.CompanyPersonal?.Country);
            var signatory = new Signatory(companyDb.CompanyPersonal?.Title, companyDb.CompanyPersonal?.FirstName, companyDb.CompanyPersonal?.LastName, companyDb.CompanyPersonal?.Email);
            var company = new Company(companyDb.Id, companyDb.Name, companyDb.SiretNumber, companyDb.ErpId, companyDb.BankServicesProviderId, signatory, address);
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

        public async Task<IEnumerable<Collection>> GetAllCollectionsAsync(CollectionQueryDto query)
        {
            return (await this.mandateRepository.SearchCollectionsAsync(query.ToSql())).Select(i => i.ToModel());
        }

        public Task<JeDeclareFolder> CreateFolderAsync(string jdcDossierId, Guid companyId)
        {
            // Creation JeDeclare Folder
            throw new NotImplementedException();
        }

        public Task<Collection> CreateCollection(MandateCreationDto mandateCreation)
        {
            // Creation mandate collection
            throw new NotImplementedException();
        }

        public Task<Status> CreateStatus(Guid collectionId, Status status)
        {
            // Création d'un status relié a une collecte
            throw new NotImplementedException();
        }

        public Task<JeDeclareCollection> CreateJeDeclareCollection(Guid collectionId, string jdcReleveId, string jdcRibId)
        {
            // Création de JeDeclare Collection
            throw new NotImplementedException();
        }

        public Task<bool> CheckCollecteConfigExist(Bban bban)
        {
            // Vérifier si le rib existe déja dans la base
            throw new NotImplementedException();
        }
    }
}
