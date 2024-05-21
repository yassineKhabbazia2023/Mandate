// <copyright file="IMandateFunctionManager.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace Mandate.AzureFunctions.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using KPMG.Pulse.Back.Accounting.Mandate.Client;

    /// <summary>
    /// A service to get list of collection.
    /// </summary>
    public interface IMandateFunctionManager
    {
        /// <summary>
        /// Get List of collections.
        /// </summary>
        /// <param name="skip">for skipping elements.</param>
        /// <param name="limit">the limit of the list.</param>
        /// <param name="statusCodes">the status codes to fitler with.</param>
        /// <returns>returns List of collections.</returns>
        Task<PagedTechnicalMandate> GetCollectionsAsync(int skip, int limit, List<int> statusCodes);

        Task RefreshCollectionsStatuses(List<TechnicalCollectionSummary> payload);
    }
}
