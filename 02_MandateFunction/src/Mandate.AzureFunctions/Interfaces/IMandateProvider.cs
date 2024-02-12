// <copyright file="IMandateProvider.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.AzureFunctions
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using KPMG.Pulse.Back.Accounting.Mandate.Client;

    /// <summary>
    /// A service to get call mandate Api endpoints.
    /// </summary>
    public interface IMandateProvider
    {
        /// <summary>
        /// Get List of collections.
        /// </summary>
        /// <param name="limit">the limit of the list.</param>
        /// <param name="skip">for skipping elements.</param>
        /// <param name="statusCodes">the status codes to fitler with.</param>
        /// <returns>returns List of collections.</returns>
        Task<PagedTechnicalMandate> GetCollectionsAsync(int skip, int limit, List<int> statusCodes);

        Task<CollectionSummary> GetRecoveryAsync(Bban rib);

        Task RefreshCollectionsStatuses(List<TechnicalCollectionSummary> payload);
    }
}
