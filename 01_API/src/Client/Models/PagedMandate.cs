// <copyright file="PagedMandate.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Client
{
    using Newtonsoft.Json;

    public class PagedMandate
    {
        [JsonConstructor]
        public PagedMandate(Counters counters, IReadOnlyList<CollectionSummary> data)
        {
            this.Counters = counters;
            this.Data = data;
        }

        [JsonProperty("counters")]
        public Counters Counters { get; }

        [JsonProperty("data")]
        public IReadOnlyList<CollectionSummary> Data { get; }
    }
}
