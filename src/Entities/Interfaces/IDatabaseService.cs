// <copyright file="IDatabaseService.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate
{
    public interface IDatabaseService
    {
        Task<Company> GetCompanyBySiretAsync(string siret);

        Task<Company> GetCompanyByErpIdAsync(string erpId);

        Task<Bank> GetBankByCodeAsync(string bankCode);

        Task<byte[]> GetPdfTemplateByBankCodeAsync(string bankCode);

        Task<PagedMandate> GetAllCollectionsAsync(CollectionQueryDto query);

        Task<Collection> GetCollectionById(Guid collectionId);

        Task<Collection> CreateCollectionAsync(string erpId, Guid companyId, Bban bban);

        Task CreateFolderAsync(string bankServicesProviderId, Guid companyId);

        Task<Status> CreateStatus(Guid collectionId, Status status);

        Task<Collection> InsertServicesProviderIds(Guid collectionId, string collectionServicesProviderId, string bbanServicesProviderId);

        Task<bool> CheckCollecteConfigExist(Bban bban);

        Task<Collection> UpdateCollection(Guid id, Collection collection);

        Task SaveSignatoryAsync(Guid? companyId, Guid? collectionId, Signatory signatory, Address address);

        Task CreateFakeRefAsync();

        Task DeleteFakeRefAsync();

        Task CreateFakeAuthAsync();

        Task DeleteFakeAuthAsync();

        Task AddFakeDataAsync();

        Task DeleteFakeDataAsync();
    }
}
