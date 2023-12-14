// <copyright file="FormIoSubmissionCollection.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Formio.Client
{
    using Newtonsoft.Json;

    [JsonObject]
    public class FormIoSubmissionCollection
    {
        public FormIoSubmissionCollection()
        {
            this.Submissions = new List<FormIoSubmission> { };
        }

        [JsonProperty("data")]
        public List<FormIoSubmission> Submissions { get; set; }

        [JsonProperty("limit")]
        public int Limit { get; set; }

        [JsonProperty("skip")]
        public int Skip { get; set; }

        [JsonProperty("total")]
        public int Total { get; set; }
    }
}
