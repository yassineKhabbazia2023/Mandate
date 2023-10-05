// <copyright file="IMandateManager.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate
{
    using KPMG.Pulse.Back.Accounting.Mandate.Models;

    public interface IMandateManager
    {
        Task<IEnumerable<Collection>> GetAllCollections(CollectionQueryDto query);
    }
}
