// <copyright file="FormioSubmissionCollection.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Formio.Client
{
    using Newtonsoft.Json;

    [JsonObject]
    public class FormioSubmissionCollection
    {
        public FormioSubmissionCollection()
        {
            this.Submissions = new List<FormioSubmission> { };
        }

        [JsonProperty("data")]
        public List<FormioSubmission> Submissions { get; }

        [JsonProperty("limit")]
        public int Limit { get; set; }

        [JsonProperty("skip")]
        public int Skip { get; set; }

        [JsonProperty("total")]
        public int Total { get; set; }
    }
}
