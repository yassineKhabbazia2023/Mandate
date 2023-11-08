// <copyright file="IPortalManager.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Interfaces
{
    public interface IPortalManager
    {
        Task GetAccountsODataWithoutCache(string? filer = null, int top = 1000, int skip = 0, bool count = false);
    }
}