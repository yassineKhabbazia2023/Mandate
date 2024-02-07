// <copyright file="MandateProvider.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.AzureFunctions
{
    using System.Threading.Tasks;
    using Kpmg.Constellation.IdentityService.Client;
    using KPMG.Pulse.Back.Accounting.Mandate.Client;

    public class MandateProvider : IMandateProvider
    {
        private readonly IMandateClientFactory factory;
        private readonly ISystemAccountAuthenticationProvider systemAccountAuthenticationProvider;

        public MandateProvider(IMandateClientFactory factory, ISystemAccountAuthenticationProvider systemAccountAuthenticationProvider)
        {
            this.factory = factory;
            this.systemAccountAuthenticationProvider = systemAccountAuthenticationProvider;
        }

        public async Task<CollectionSummary> GetRecoveryAsync(Bban rib)
        {
            var client = this.factory.Create(await this.systemAccountAuthenticationProvider.GetTokenAsync());
            return await client.GetRecoveryAsync(rib);
        }
    }
}
