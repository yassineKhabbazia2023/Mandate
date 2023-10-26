// <copyright file="CompanyManager.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Application
{
    internal class CompanyManager : ICompanyManager
    {
        public Task<Company> GetCompanyByErpId(string erpId)
        {
            throw new NotImplementedException();
        }
    }
}
