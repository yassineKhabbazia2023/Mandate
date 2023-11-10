// <copyright file="IPortalProvider.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Portal
{
    using KPMG.Constellation.Portal.Client;

    public interface IPortalProvider
    {
        Task<AccountWithoutCacheRootResponseJson> GetAccountsODataWithoutCache(int top, int skip, string? query, bool count);
    }
}