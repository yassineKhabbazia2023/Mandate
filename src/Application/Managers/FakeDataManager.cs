// <copyright file="FakeDataManager.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Application
{
    public class FakeDataManager : IFakeDataManager
    {
        private readonly IDatabaseService databaseService;

        public FakeDataManager(IDatabaseService databaseService)
        {
            this.databaseService = databaseService;
        }

        public async Task CreateFakeRefAsync()
        {
            await this.databaseService.CreateFakeRefAsync().ConfigureAwait(false);
        }

        public async Task DeleteFakeRefAsync()
        {
            await this.databaseService.DeleteFakeRefAsync().ConfigureAwait(false);
        }

        public async Task CreateFakeAuthAsync()
        {
            await this.databaseService.CreateFakeAuthAsync().ConfigureAwait(false);
        }

        public async Task DeleteFakeAuthAsync()
        {
            await this.databaseService.DeleteFakeAuthAsync().ConfigureAwait(false);
        }

        public async Task AddFakeDataAsync()
        {
            await this.databaseService.AddFakeDataAsync().ConfigureAwait(false);
        }

        public async Task DeleteFakeDataAsync()
        {
            await this.databaseService.DeleteFakeDataAsync().ConfigureAwait(false);
        }
    }
}
