// <copyright file="FormDataAddress.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Mongo
{
    using Newtonsoft.Json;

    public class FormDataAddress
    {
        [JsonProperty("signatoryStreetAddress")]
        public string? SignatoryStreetAddress { get; set; }

        [JsonProperty("signatoryAddressComplements")]
        public string? SignatoryAddressComplements { get; set; }

        [JsonProperty("signatoryAddressZipCode")]
        public string? SignatoryAddressZipCode { get; set; }

        [JsonProperty("signatoryAddressCity")]
        public string? SignatoryAddressCity { get; set; }

        [JsonProperty("signatoryAddressCountry")]
        public string? SignatoryAddressCountry { get; set; }
    }
}
