// <copyright file="MandateManager.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Application
{
    using KPMG.Pulse.Back.Accounting.Mandate.Models;

    public class MandateManager : IMandateManager
    {
        private readonly IDatabaseService databaseService;

        public MandateManager(IDatabaseService databaseService)
        {
            this.databaseService = databaseService;
        }

        public async Task<IEnumerable<Collection>> GetAllCollections(CollectionQueryDto query)
        {
            return await this.databaseService.GetAllCollections(query).ConfigureAwait(false);
        }
    }
}