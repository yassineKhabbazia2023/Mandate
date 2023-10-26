// <copyright file="PortalProvider.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.PortalApi
{
    using KPMG.Constellation.Function.AccessRight.Helper;
    using KPMG.Constellation.Portal.Client;
    using KPMG.Constellation.Portal.Model;

    public class PortalProvider : IPortalProvider
    {
        private readonly IPortalClientFactory factory;

        private readonly IAuthenticationContext authenticationContext;

        public PortalProvider(IPortalClientFactory factory, IAuthenticationContext authenticationContext)
        {
            this.factory = factory;
            this.authenticationContext = authenticationContext;
        }

        public async Task<AccountWithoutCacheRootResponseJson> GetAccountsODataWithoutCache(string queryString)
        {
            var client = this.factory.Create(this.authenticationContext.BearerToken);
            return await client.GetAccountsODataWithoutCache(queryString);
        }
    }
}
