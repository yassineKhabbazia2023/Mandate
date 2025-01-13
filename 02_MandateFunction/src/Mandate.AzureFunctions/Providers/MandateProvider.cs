// <copyright file="MandateProvider.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.AzureFunctions
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using KPMG.Pulse.Back.Accounting.Mandate.Client;

    /// <inheritdoc/>
    public class MandateProvider : IMandateProvider
    {
        private readonly IMandateClientFactory factory;

        public MandateProvider(IMandateClientFactory factory)
        {
            this.factory = factory;
        }

        /// <inheritdoc/>
        public async Task<PagedTechnicalMandate> GetCollectionsAsync(int skip, int limit, List<int> statusCodes)
        {
            var client = this.factory.Create();
            return await client.GetTechnicalCollectionSummaryAsync(skip, limit, statusCodes);
        }

        /// <inheritdoc/>
        public async Task<CollectionSummary> GetRecoveryAsync(Bban rib)
        {
            var client = this.factory.Create();
            return await client.GetRecoveryAsync(rib);
        }

        /// <inheritdoc/>
        public async Task RefreshCollectionsStatuses(List<TechnicalCollectionSummary> payload)
        {
            var client = this.factory.Create();
            await client.RefreshMandatsStatusesAsync(payload);
        }
    }
}
