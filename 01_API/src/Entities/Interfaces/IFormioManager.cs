// <copyright file="IFormioManager.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate
{
    public interface IFormioManager
    {
        Task<Collection?> GetCollectionByBban(Bban bban);
    }
}
