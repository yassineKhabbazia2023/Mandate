// <copyright file="IPortalManager.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate
{
    public interface IPortalManager
    {
        Task<List<Company>> GetAccountsODataWithoutCache(int top = 1000, int skip = 0, string? query = null, bool count = false);
    }
}