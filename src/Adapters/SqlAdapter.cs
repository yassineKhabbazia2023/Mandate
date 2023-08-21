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
            var companyDb = await this.mandateRepository.GetCompanyBySiretAsync(siret);

            var address = new Address(companyDb.CompanyPersonal?.Street, companyDb.CompanyPersonal?.Complements, companyDb.CompanyPersonal?.ZipCode, companyDb.CompanyPersonal?.City, companyDb.CompanyPersonal?.Country);
            var signatory = new Signatory(companyDb.CompanyPersonal?.Title, companyDb.CompanyPersonal?.FirstName, companyDb.CompanyPersonal?.LastName, address);
            var company = new Company(companyDb.Id, companyDb.Name, companyDb.SiretNumber, companyDb.ErpId, companyDb.BankServicesProviderId, signatory);
            return company;
        }
    }
}
