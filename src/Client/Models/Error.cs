// <copyright file="Error.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Client
{
    using Newtonsoft.Json;

    public class Error
    {
        [JsonConstructor]
        public Error(string errorType, string logReference, string message)
        {
            this.ErrorType = errorType;
            this.LogReference = logReference;
            this.Message = message;
        }

        [JsonProperty("errorType")]
        public string ErrorType { get; }

        [JsonProperty("logReference")]
        public string LogReference { get; }

        [JsonProperty("message")]
        public string Message { get; }
    }
}
