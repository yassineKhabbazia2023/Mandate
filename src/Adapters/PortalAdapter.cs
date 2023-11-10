// <copyright file="PortalAdapter.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Adapters
{
    using KPMG.Pulse.Back.Accounting.Mandate.Interfaces;
    using KPMG.Pulse.Back.Accounting.Mandate.Portal;

    public class PortalAdapter : IPortalManager
    {
        private readonly IPortalProvider portalProvider;

        public PortalAdapter(IPortalProvider portalProvider)
        {
            this.portalProvider = portalProvider;
        }

        public async Task<List<Company>> GetAccountsODataWithoutCache(int top = 1000, int skip = 0, string? query = null, bool count = false)
        {
            return (await this.portalProvider.GetAccountsODataWithoutCache(top, skip, query, count))
                .Accounts.Select(item => item.ToModel()).ToList();
        }
    }
}
