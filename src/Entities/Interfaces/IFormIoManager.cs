// <copyright file="IFormIoManager.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate
{
    public interface IFormIoManager
    {
        Task<Collection?> GetCollectionByBban(Bban bban);
    }
}
