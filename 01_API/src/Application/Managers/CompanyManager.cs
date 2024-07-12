// <copyright file="CompanyManager.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Application
{
    public class CompanyManager : ICompanyManager
    {
        private readonly IDatabaseService databaseService;

        public CompanyManager(IDatabaseService databaseService)
        {
            this.databaseService = databaseService;
        }

        public async Task<Company> GetCompanyByErpIdAsync(string erpId)
        {
           return await this.databaseService.GetCompanyByErpIdAsync(erpId).ConfigureAwait(false);
        }
    }
}
