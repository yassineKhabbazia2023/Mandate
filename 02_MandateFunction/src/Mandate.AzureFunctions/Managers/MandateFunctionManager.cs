// <copyright file="MandateFunctionManager.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace Mandate.AzureFunctions.Managers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using KPMG.Pulse.Back.Accounting.Mandate.AzureFunctions;
    using KPMG.Pulse.Back.Accounting.Mandate.Client;
    using Mandate.AzureFunctions.Interfaces;

    /// <inheritdoc/>
    public class MandateFunctionManager : IMandateFunctionManager
    {
        private readonly IMandateProvider mandateProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="MandateFunctionManager"/> class.
        /// </summary>
        /// <param name="mandateProvider">A instance of the <see cref="IMandateProvider"/> class.</param>
        public MandateFunctionManager(IMandateProvider mandateProvider)
        {
            this.mandateProvider = mandateProvider;
        }

        /// <inheritdoc/>
        public Task<PagedTechnicalMandate> GetCollectionsAsync(int skip, int limit, List<int> statusCodes)
        {
            return this.mandateProvider.GetCollectionsAsync(skip, limit, statusCodes);
        }

        /// <inheritdoc/>
        public async Task RefreshCollectionsStatuses(List<TechnicalCollectionSummary> payload)
        {
            await this.mandateProvider.RefreshCollectionsStatuses(payload);
        }
    }
}
