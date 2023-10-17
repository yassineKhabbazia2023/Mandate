// <copyright file="ICompanyManager.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate
{
    public interface ICompanyManager
    {
        Task<Company> GetCompanyByErpId(string erpId);
    }
}