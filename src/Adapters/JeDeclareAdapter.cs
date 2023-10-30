// <copyright file="JeDeclareAdapter.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Adapters
{
    public class JeDeclareAdapter : IJeDeclareService
    {
        public Task<Bban> AddRibToFolderAsync(string? bankServicesProviderId, Bban bban, Signatory signatory)
        {
            throw new NotImplementedException();
        }

        public Task<Collection> CreateCollecteConfigurationAsync(string bankServicesProviderId, Bban rib)
        {
            throw new NotImplementedException();
        }

        public Task<Company> CreateFolderAsync(Company company)
        {
            throw new NotImplementedException();
        }
    }
}