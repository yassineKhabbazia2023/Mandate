// <copyright file="IMandateManager.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate
{
    public interface IMandateManager
    {
        Task<PagedMandate> GetAllCollectionsAsync(CollectionQueryDto query);

        Task<PagedTechnicalMandate> GetAllTechnicalCollectionsAsync(CollectionQueryDto query);

        Task<Guid> CreateMandateAsync(CollectionCreationCommand mandateCreation, int contactId);

        Task<byte[]> DownloadUnsignedAsync(Guid id);

        Task<string?> UploadSignedMandateAsync(Guid collectionId, Stream mandateFileStream, string userEmail);

        Task<byte[]> DownloadSignedAsync(Guid id);
        
        Task<bool> DeactivateCollectionAsync(Guid collectionId, string userEmail);

        Task<CollectionStatus?> GetMandateStatusAsync(Guid collectionId);
    }
}
