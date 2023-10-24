// <copyright file="MandateManager.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Application
{
    public class MandateManager : IMandateManager
    {
        private readonly IDatabaseService databaseService;

        public MandateManager(IDatabaseService databaseService)
        {
            this.databaseService = databaseService;
        }

        public async Task<IEnumerable<Collection>> GetAllCollectionsAsync(CollectionQueryDto query)
        {
            return await this.databaseService.GetAllCollectionsAsync(query).ConfigureAwait(false);
        }

        public Task<Collaborator> GetCollaboratorByEmail(string email)
        {
            throw new NotImplementedException();
        }
    }
}