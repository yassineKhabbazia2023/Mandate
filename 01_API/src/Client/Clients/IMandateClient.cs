// <copyright file="IMandateClient.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Client
{
    public interface IMandateClient
    {
        Task<CollectionSummary> GetRecoveryAsync(Bban ribEntry);
    }
}
