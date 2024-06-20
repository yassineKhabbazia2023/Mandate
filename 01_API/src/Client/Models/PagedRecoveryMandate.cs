// <copyright file="PagedRecoveryMandate.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Client
{
    using Newtonsoft.Json;

    public class PagedRecoveryMandate
    {
        public PagedRecoveryMandate(int imported, IReadOnlyList<CollectionSummary> failed)
        {
            this.Imported = imported;
            this.Failed = failed;
        }

        [JsonProperty("imported")]
        public int Imported { get; }

        [JsonProperty("failed")]
        public IReadOnlyList<CollectionSummary> Failed { get; }
    }
}
