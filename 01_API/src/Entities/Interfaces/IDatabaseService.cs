// <copyright file="IDatabaseService.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate
{
    public interface IDatabaseService
    {
        Task<Company> GetCompanyBySiretAsync(string siret);

        Task<Company> GetCompanyByErpIdAsync(string erpId, string userEmail);

        Task<Company> GetCompanyByErpIdAsync(string erpId, int contactId);

        Task<Bank> GetBankByCodeAsync(string bankCode);

        Task<Status> GetRefStatusCodeByJdcCodeAsync(string jdcStatusCode);

        Task<byte[]> GetPdfTemplateByBankCodeAsync(string bankCode);

        Task<PagedTechnicalMandate> GetAllTechnicalCollectionsAsync(CollectionQueryDto query);

        Task<PagedMandate> GetAllCollectionsAsync(CollectionQueryDto query, int collaboratorId);

        Task<Collection> GetCollectionById(Guid collectionId);

        Task<Guid> CreateCollectionAsync(Bban bban, int companyId, int? createdBy);

        Task CreateOrUpdateFolderAsync(string bankServicesProviderId, int companyId);

        Task<Status?> CreateStatusWithMessageAsync(Guid collectionId, int statusCode, string errorMessage);

        Task<Status?> CreateStatusAsync(Guid collectionId, int statusCode);

        Task<bool> CheckJdcStatusCodeIsPendingAsync(Guid collectionId);

        Task InsertServicesProviderIds(Guid collectionId, string collectionServicesProviderId, string bbanServicesProviderId);

        Task<bool> CheckCollecteConfigExistAsync(Bban bban);

        Task<Collection> UpdateCollection(Guid id, Collection collection);

        Task<Collaborator?> GetCollaboratorByEmail(string collaboratorEmail);

        Task<Collaborator?> GetCollaboratorById(int contactId);

        Task SaveSignatoryAsync(int? companyId, Guid? collectionId, Signatory signatory, Address address);

        Task CreateFakeRefAsync();

        Task DeleteFakeRefAsync();

        Task CreateFakeAuthAsync();

        Task DeleteFakeAuthAsync();

        Task AddFakeDataAsync();

        Task DeleteFakeDataAsync();

        Task<Company> GetCompanyByErpIdSiretAsync(string erpId, string siret);

        Task InsertMandateLogAsync(Collection collection, CustomException exception);

        Task<Guid> GetCollectionIfAlreadyExistingInIncidentStatus(string bankCode, string branchCode, string accountNumber, string erpId);

        Task SaveMandateCreationLogMessageAsync(MandateCreationLogMessage mandateCreationLogMessage);
    }
}