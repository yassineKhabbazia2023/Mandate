// <copyright file="IMandateRepository.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Sql
{
    public interface IMandateRepository
    {
        Task<CompanyDb> GetCompanyBySiretAsync(string siret);

        /// <summary>
        /// Finds a Bank in the reference table.
        /// </summary>
        /// <param name="bankCode">The bank code to retrieve (first paramater of the French BBAN).</param>
        /// <returns>A Bank entry.</returns>
        Task<RefBankDb> GetRefBankByCodeAsync(string bankCode);

        /// <summary>
        /// Finds a pdf template in the reference table.
        /// </summary>
        /// <param name="bankCode">The bank code to retrieve (first paramater of the French BBAN).</param>
        /// <returns>The pdf template file</returns>
        Task<byte[]> GetPdfTemplateByCodeAsync(string bankCode);

        /// <summary>
        /// Searches Collection and Company tables to find entries that match a set of criteria.
        /// </summary>
        /// <param name="query">An object that encapsulates the query parameters, including pagination and search terms.</param>
        /// <returns>All entries that match the query. All Collection instances returned contain their parent Company instance.</returns>
        Task<IReadOnlyList<CollectionDb>> SearchCollectionsAsync(CollectionQuery query);
    }
}
