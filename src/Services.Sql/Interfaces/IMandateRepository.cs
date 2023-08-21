// <copyright file="IMandateRepository.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Sql
{
    public interface IMandateRepository
    {
        Task<CompanyDb> GetCompanyBySiretAsync(string siret);
    }
}
