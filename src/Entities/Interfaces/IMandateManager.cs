// <copyright file="IMandateManager.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate
{
    public interface IMandateManager
    {
        Task<PagedMandate> GetAllCollectionsAsync(CollectionQueryDto query);

        Task<Guid> CreateMandate(MandateCreation mandateCreation);

        Task<byte[]> DownloadUnsignedAsync(Guid id);
    }
}
