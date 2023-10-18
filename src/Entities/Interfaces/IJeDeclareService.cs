// <copyright file="IJeDeclareService.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Interfaces
{
    public interface IJeDeclareService
    {
        Task<Bban> AddRibToFolderAsync(string? bankServicesProviderId, MandateCreationDto mandateCreation);

        Task<Collection> CreateCollecteConfigurationAsync(string bankServicesProviderId, Bban rib);

        Task<Company> CreateFolderAsync(Company company);
    }
}
