// <copyright file="IJeDeclareService.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate
{
    public interface IJeDeclareService
    {
        Task<Bban> AddRibToFolderAsync(string? bankServicesProviderId, CollectionCreationCommand mandateCreation, Bank bank);

        Task<Collection> CreateCollecteConfigurationAsync(string bankServicesProviderId, Bban rib, Company dossier, Guid collectionId, Status initStatus);

        Task<Company> CreateFolderAsync(Company company);

        Task<byte[]> GetMandatPdfAsync(string jdcFolderId, string jdcRibId);

        Task<string> UploadSignedMandate(Collection collection, byte[] mandateFile);
    }
}
