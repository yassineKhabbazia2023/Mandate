// <copyright file="PortalProvider.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Portal
{
    using KPMG.Constellation.Portal.Client;

    public class PortalProvider : IPortalProvider
    {
        private readonly IPortalClientFactory factory;
        private readonly IAuthenticationContext authenticationContext;

        public PortalProvider(IPortalClientFactory factory, IAuthenticationContext authenticationContext)
        {
            this.factory = factory;
            this.authenticationContext = authenticationContext;
        }

        public async Task<AccountWithoutCacheRootResponseJson> GetAccountsODataWithoutCache(int top, int skip, string? query, bool count)
        {
            var client = this.factory.Create(this.authenticationContext.BearerToken);
            return await client.GetAccountsODataWithoutCache(this.ConstructQuery(top, skip, query, count));
        }

        private string ConstructQuery(int top, int skip, string? query, bool count)
        {
            string url = $"$top={top}&$skip={skip}";
            if (!string.IsNullOrEmpty(query))
            {
                url += $"&{query}";
            }

            url += count ? "&$count=true" : string.Empty;

            return url;
        }
    }
}