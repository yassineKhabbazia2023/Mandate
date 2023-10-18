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

        Task<IEnumerable<Collection>> GetAllCollectionsAsync(CollectionQueryDto query);

        Task<Collection> CreateCollection(string erpId, Guid companyId, Bban bban);

        Task<JeDeclareFolder> CreateFolderAsync(string jdcDossierId, Guid companyId);

        Task<Status> CreateStatus(Guid collectionId, Status status);

        Task<JeDeclareCollection> CreateJeDeclareCollection(Guid collectionId, string jdcReleveId, string jdcRibId);

        Task<bool> CheckCollecteConfigExist(Bban bban);

        Task<Collection> UpdateCollection(Guid id, Collection collection);
    }
}
