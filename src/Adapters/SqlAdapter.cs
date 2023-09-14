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
            var signatory = new Signatory(companyDb.CompanyPersonal?.Title, companyDb.CompanyPersonal?.FirstName, companyDb.CompanyPersonal?.LastName, companyDb.CompanyPersonal?.Email, address);
            var company = new Company(companyDb.Id, companyDb.Name, companyDb.SiretNumber, companyDb.ErpId, companyDb.BankServicesProviderId, signatory);
            return company;
        }

        public async Task<Bank> GetBankByCodeAsync(string bankCode)
        {
            var refBankDb = await this.mandateRepository.GetRefBankByCodeAsync(bankCode).ConfigureAwait(false);
            return refBankDb.ToModel();
        }

        public async Task<byte[]> GetPdfTemplateByBankCodeAsync(string bankCode)
        {
            // TODO
            await Task.CompletedTask;
            throw new NotImplementedException();
        }
    }
}
