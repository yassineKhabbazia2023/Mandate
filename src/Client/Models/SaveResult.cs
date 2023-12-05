// <copyright file="SaveResult.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Client
{
    using Newtonsoft.Json;

    public class SaveResult
    {
        [JsonConstructor]
        public SaveResult(Guid id)
        {
            this.Id = id;
        }

        [JsonProperty("id")]
        public Guid Id { get; }
    }
}
