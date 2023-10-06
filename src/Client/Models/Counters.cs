// <copyright file="Counters.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Client
{
    using Newtonsoft.Json;

    public class Counters
    {
        [JsonConstructor]
        public Counters(int all, int status10, int status20, int status30, int status40, int status50)
        {
            this.All = all;
            this.Status10 = status10;
            this.Status20 = status20;
            this.Status30 = status30;
            this.Status40 = status40;
            this.Status50 = status50;
        }

        [JsonProperty("all")]
        public int All { get; }

        [JsonProperty("status10")]
        public int Status10 { get; }

        [JsonProperty("status20")]
        public int Status20 { get; }

        [JsonProperty("status30")]
        public int Status30 { get; }

        [JsonProperty("status40")]
        public int Status40 { get; }

        [JsonProperty("status50")]
        public int Status50 { get; }
    }
}
