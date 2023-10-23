// <copyright file="IMandateRepository.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Sql
{
    public interface IMandateRepository
    {
        /// <summary>
        /// Asynchronously retrieves a company by its SIRET number.
        /// </summary>
        /// <param name="siret">The SIRET number of the company to retrieve.</param>
        /// <returns>A company associated with a siret</returns>
        Task<CompanyDb> GetCompanyBySiretAsync(string siret);

        /// <summary>
        /// Asynchronously retrieves all companies associated with a specific collaborator based on their email.
        /// </summary>
        /// <param name="email">The email address of the collaborator.</param>
        /// <returns>A list of companies associated with the collaborator.</returns>
        Task<List<CompanyDb?>> GetAllCompaniesByCollaboratorAsync(string email);

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
