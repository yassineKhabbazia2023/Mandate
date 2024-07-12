// <copyright file="IMandateClient.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Client
{
    public interface IMandateClient
    {
        Task<CollectionSummary> GetRecoveryAsync(Bban ribEntry);

        Task<PagedTechnicalMandate> GetTechnicalCollectionSummaryAsync();

        Task<PagedTechnicalMandate> GetTechnicalCollectionSummaryAsync(int skip, int limit);

        Task<PagedTechnicalMandate> GetTechnicalCollectionSummaryAsync(int skip, int limit, List<int> statusCodes);

        Task<bool> RefreshMandatsStatusesAsync(List<TechnicalCollectionSummary> mandates);

        Task<PagedRecoveryMandate> RecoveryFormIoAsync(int skip, int limit);
    }
}
