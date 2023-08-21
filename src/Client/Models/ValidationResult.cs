// <copyright file="ValidationResult.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Client
{
    using Newtonsoft.Json;

    public class ValidationResult
    {
        [JsonConstructor]
        public ValidationResult(bool valid)
        {
            this.Valid = valid;
        }

        [JsonProperty("valid")]
        public bool Valid { get; }
    }
}
