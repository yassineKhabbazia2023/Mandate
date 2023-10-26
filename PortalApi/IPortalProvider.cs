// <copyright file="IPortalProvider.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.PortalApi
{

    public interface IPortalProvider
    {
        Task<AccountWithoutCacheRootResponseJson> GetAccountsODataWithoutCache(string queryString);
    }
}
