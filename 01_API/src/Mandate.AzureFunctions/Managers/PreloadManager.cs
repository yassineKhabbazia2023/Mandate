// <copyright file="PreloadManager.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.AzureFunctions
{
    using System.Threading.Tasks;
    using KPMG.Pulse.Back.Accounting.Mandate.Client;

    public class PreloadManager : IPreloadManager
    {
        private readonly IMandateProvider mandateProvider;

        public PreloadManager(IMandateProvider mandateProvider)
        {
            this.mandateProvider = mandateProvider;
        }

        public async Task GetRecoveryAsync(Bban rib)
        {
            await this.mandateProvider.GetRecoveryAsync(rib);
        }
    }
}
