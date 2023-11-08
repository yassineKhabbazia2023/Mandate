// <copyright file="PortalAdapter.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Adapters
{
    using KPMG.Pulse.Back.Accounting.Mandate.Interfaces;
    using KPMG.Pulse.Back.Accounting.Mandate.PortalApi;

    public class PortalAdapter : IPortalManager
    {
        private readonly IPortalProvider portalProvider;

        public PortalAdapter(IPortalProvider portalProvider)
        {
            this.portalProvider = portalProvider;
        }

        public async Task GetAccountsODataWithoutCache(string? filer = null, int top = 1000, int skip = 0, bool count = false)
        {
            await this.portalProvider.GetAccountsODataWithoutCache(top, skip, filer, count);
        }
    }
}
