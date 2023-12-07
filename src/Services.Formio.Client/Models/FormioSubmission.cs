// <copyright file="FormioSubmission.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Formio.Client
{
    using Newtonsoft.Json;

    public class FormioSubmission
    {
        [JsonProperty("_id")]
        public string Id { get; set; } = null!;

        [JsonProperty("data")]
        public FormioBban Data { get; set; } = null!;

        [JsonProperty("owner")]
        public string Owner { get; set; } = null!;

        [JsonProperty("created")]
        public string Created { get; set; } = null!;

        [JsonProperty("modified")]
        public string Modified { get; set; } = null!;
    }
}
