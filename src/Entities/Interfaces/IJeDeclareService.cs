// <copyright file="IJeDeclareService.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate
{
    public interface IJeDeclareService
    {
        Task<Bban> AddRibToFolderAsync(string? bankServicesProviderId, Bban bban, Signatory signatory);

        Task<Collection> CreateCollecteConfigurationAsync(string bankServicesProviderId, Bban rib);

        Task<Company> CreateFolderAsync(Company company);
    }
}
