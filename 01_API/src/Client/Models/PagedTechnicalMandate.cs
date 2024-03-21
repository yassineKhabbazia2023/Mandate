// <copyright file="PagedTechnicalMandate.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Client
{
    using Newtonsoft.Json;

    public class PagedTechnicalMandate
    {
        [JsonConstructor]
        public PagedTechnicalMandate(IReadOnlyList<TechnicalCollectionSummary> data)
        {
            this.Data = data;
        }

        [JsonProperty("data")]
        public IReadOnlyList<TechnicalCollectionSummary> Data { get; }
    }
}
