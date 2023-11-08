// <copyright file="IPortalProvider.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.PortalApi
{
    using KPMG.Constellation.Portal.Client;

    public interface IPortalProvider
    {
        Task<AccountWithoutCacheRootResponseJson> GetAccountsODataWithoutCache(int top, int skip, string? filer, bool count);
    }
}
