// <copyright file="IPortalManager.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Interfaces
{
    public interface IPortalManager
    {
        Task GetAccountsODataWithoutCache(string odataQueryString);
    }
}