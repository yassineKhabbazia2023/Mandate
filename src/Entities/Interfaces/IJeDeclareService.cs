// <copyright file="IJeDeclareService.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate
{
    public interface IJeDeclareService
    {
        Task<Bban> AddRibToFolderAsync(string? bankServicesProviderId, CollectionCreationCommand mandateCreation);

        Task<Collection> CreateCollecteConfigurationAsync(string bankServicesProviderId, Bban rib, Signatory signatory);

        Task<Company> CreateFolderAsync(Company company);

        Task<byte[]> GetMandatPdfAsync(string jdcFolderId, string jdcRibId);
    }
}
