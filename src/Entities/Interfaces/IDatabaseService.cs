// <copyright file="IDatabaseService.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate
{
    public interface IDatabaseService
    {
        Task<Company> GetCompanyBySiretAsync(string siret);

        Task<Bank> GetBankByCodeAsync(string bankCode);

        Task<byte[]> GetPdfTemplateByBankCodeAsync(string bankCode);

        Task<PagedMandate> GetAllCollectionsAsync(CollectionQueryDto query);

        Task<Collection> CreateCollection(string erpId, Guid companyId, Bban bban);

        Task<Company> CreateFolderAsync(string bankServicesProviderId, Guid companyId);

        Task<Status> CreateStatus(Guid collectionId, Status status);

        Task<Collection> InsertServicesProviderIds(Guid collectionId, string collectionServicesProviderId, string bbanServicesProviderId);

        Task<bool> CheckCollecteConfigExist(Bban bban);

        Task<Collection> UpdateCollection(Guid id, Collection collection);

        Task SaveSignatoryAsync(Guid companyId, Guid collectionId, Signatory signatory, Address address);
    }
}
