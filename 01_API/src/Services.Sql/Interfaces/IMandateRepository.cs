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
        /// <returns>A company associated with a siret.</returns>
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
        /// Finds a pulse status in the reference table.
        /// </summary>
        /// <param name="jdcStatusCode">The jdc status code to retrieve pulse status code.</param>
        /// <returns>A pulse status code.</returns>
        Task<StatusDb> GetRefStatusCodeByJdcCodeAsync(string jdcStatusCode);

        /// <summary>
        /// Finds a pdf template in the reference table.
        /// </summary>
        /// <param name="bankCode">The bank code to retrieve (first paramater of the French BBAN).</param>
        /// <returns>The pdf template file.</returns>
        Task<byte[]> GetPdfTemplateByCodeAsync(string bankCode);

        /// <summary>
        /// Searches Collection and Company tables to find entries that match a set of criteria.
        /// </summary>
        /// <param name="query">An object that encapsulates the query parameters, including pagination and search terms.</param>
        /// <returns>All entries that match the query. All Collection instances returned contain their parent Company instance.</returns>
        Task<(List<CollectionDb>, int)> SearchCollectionsAsync(CollectionQuery query);

        /// <summary>
        /// Saves Signatory.
        /// </summary>
        /// <param name="personalDb">the personal table where we save signatory informations.</param>
        /// <returns>an asynchronous task.</returns>
        Task SaveSignatoryAsync(PersonalDb personalDb);

        Task CreateFakeRefAsync();

        Task DeleteFakeRefAsync();

        Task CreateFakeAuthAsync();

        Task DeleteFakeAuthAsync();

        Task AddFakeDataAsync();

        Task DeleteFakeDataAsync();

        /// <summary>
        /// Retrieves a collection from the database based on the provided unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the collection to be retrieved.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains an instance of <see cref="CollectionDb"/> corresponding to the specified ID.
        /// </returns>
        Task<CollectionDb> GetCollectionById(Guid id);

        /// <summary>
        /// Asynchronously retrieves a company's details from the database based on the provided ERP identifier.
        /// </summary>
        /// <param name="erpId">The ERP identifier of the company to be retrieved.</param>
        /// <param name="userEmail">The connected user's email address</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains an instance of <see cref="CompanyDb"/> corresponding to the specified ERP ID.
        /// </returns>
        Task<CompanyDb> GetCompanyByErpIdAsync(string erpId, string userEmail);

        /// <summary>
        /// Asynchronously retrieves a collaborator by his email.
        /// </summary>
        /// <param name="collaboratorEmail">The email of the collaborator.</param>
        /// <returns>The task result contains an instance of <see cref="CollaboratorDb"/> corresponding to the specified email.</returns>
        Task<CollaboratorDb> GetCollaboratorByEmailAsync(string collaboratorEmail);

        /// <summary>
        /// Save JeDeclare folder identifier.
        /// </summary>
        /// <param name="bankServicesProviderId">JeDeclare Folder identifier.</param>
        /// <param name="companyId">the Ccompany identifier.</param>
        /// <returns>A task result.</returns>
        Task CreateOrUpdateFolderAsync(string bankServicesProviderId, int companyId);

        Task<CollectionDb> CreateCollectionAsync(CollectionDb collection);

        Task<bool> CheckCollecteConfigExistAsync(string bankCode, string branchCode, string accountNumber);

        Task InsertServicesProviderIdsAsync(Guid collectionId, string collectionServicesProviderId, string bbanServicesProviderId);

        Task<StatusDb> CreateStatusAsync(Guid collectionId, StatusDb statusDb);

        Task<StatusDb> GetCurrentJdcStatusCodeAsync(Guid collectionId);

        Task<bool> CheckJdcStatusCodeIsPendingAsync(Guid collectionId);

        Task UpdateCurrentStatusAsync(Guid collectionId);

        Task InsertFormIOCollectionAsync(CollectionDb collection);

        Task<CompanyDb> GetCompanyByErpIdSiretAsync(string erpId, string siretNumber);

        Task InsertMandateLogAsync(MandateLogDb mandateLog);

        Task CreateContactByEventAsync(CollaboratorDb contactDb);

        Task UpdateContactByEventAsync(CollaboratorDb contactDb);

        Task<CollaboratorDb?> GetActiveContactByIdAsync(int contactId);

        Task<CompanyDb?> GetActiveAccountByIdAsync(int accountId);

        Task<CollaboratorDb?> GetContactByIdAsync(int contactId);

        Task<CompanyDb?> GetAccountByIdAsync(int accountId);

        Task CreateRoleAsync(CompanyCollaboratorDb accountContactDb);

        Task DeleteRoleAsync(CompanyCollaboratorDb accountContactDb);

        Task<CompanyCollaboratorDb?> GetAccountContactByAccountIdAndContactIdAsync(int accountId, int contactId);

        Task UpdateCompanyAsync(CompanyDb accountDb);

        Task CreateCompanyAsync(CompanyDb accountDb);
    }
}
