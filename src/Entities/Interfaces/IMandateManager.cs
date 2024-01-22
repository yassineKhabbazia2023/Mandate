// <copyright file="IMandateManager.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate
{
    public interface IMandateManager
    {
        Task<PagedMandate> GetAllCollectionsAsync(CollectionQueryDto query);

        Task<Guid> CreateMandate(CollectionCreationCommand mandateCreation);

        Task<byte[]> DownloadUnsignedAsync(Guid id);

        Task<string?> UploadSignedMandateAsync(Guid collectionId, Stream mandateFileStream);

        Task<byte[]> DownloadSignedAsync(Guid id);

        Task<bool> DeactivateCollectionAsync(Guid collectionId);
    }
}
