// <copyright file="Form.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Mongo
{
    using Newtonsoft.Json;

    public class Form
    {
        [JsonProperty("data")]
        public FormData? Data { get; set; }

        [JsonProperty("created")]
        public string? Created { get; set; }

        [JsonProperty("modified")]
        public string? Modified { get; set; }
    }
}
