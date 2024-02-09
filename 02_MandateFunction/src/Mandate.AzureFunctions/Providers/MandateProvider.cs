// <copyright file="MandateProvider.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.AzureFunctions
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Kpmg.Constellation.IdentityService.Client;
    using KPMG.Pulse.Back.Accounting.Mandate.Client;

    /// <inheritdoc/>
    public class MandateProvider : IMandateProvider
    {
        private readonly IMandateClientFactory factory;
        private readonly ISystemAccountAuthenticationProvider systemAccountAuthenticationProvider;

        public MandateProvider(IMandateClientFactory factory, ISystemAccountAuthenticationProvider systemAccountAuthenticationProvider)
        {
            this.factory = factory;
            this.systemAccountAuthenticationProvider = systemAccountAuthenticationProvider;
        }

        /// <inheritdoc/>
        public async Task<PagedTechnicalMandate> GetCollectionsAsync(int skip, int limit, List<int> statusCodes)
        {
            string token = await this.systemAccountAuthenticationProvider.GetTokenAsync();
            var client = this.factory.Create(token);
            return await client.GetTechnicalCollectionSummaryAsync(skip, limit, statusCodes);
        }

        /// <inheritdoc/>
        public async Task<CollectionSummary> GetRecoveryAsync(Bban rib)
        {
            string token = await this.systemAccountAuthenticationProvider.GetTokenAsync();
            var client = this.factory.Create(token);
            return await client.GetRecoveryAsync(rib);
        }

        /// <inheritdoc/>
        public async Task RefreshCollectionsStatuses(List<TechnicalCollectionSummary> payload)
        {
            string token = await this.systemAccountAuthenticationProvider.GetTokenAsync();
            var client = this.factory.Create(token);
            await client.RefreshMandatsStatusesAsync(payload);
        }
    }
}
