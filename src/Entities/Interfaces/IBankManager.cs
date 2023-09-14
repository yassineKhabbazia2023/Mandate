// <copyright file="IBankManager.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate
{
    public interface IBankManager
    {
        Task<Bank> GetByCodeAsync(string bankCode);
    }
}
