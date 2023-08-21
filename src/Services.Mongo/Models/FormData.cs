// <copyright file="FormData.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Mongo
{
    using Newtonsoft.Json;

    public class FormData
    {
        [JsonProperty("accountNumber")]
        public string? AccountNumber { get; set; }

        [JsonProperty("companyName")]
        public string? CompanyName { get; set; }

        [JsonProperty("SIRETNumber")]
        public string? SiretNumber { get; set; }

        [JsonProperty("signatoryTitle")]
        public string? SignatoryTitle { get; set; }

        [JsonProperty("signatoryLastName")]
        public string? SignatoryLastName { get; set; }

        [JsonProperty("signatoryFirstName")]
        public string? SignatoryFirstName { get; set; }

        [JsonProperty("headOffice")]
        public FormDataAddress? HeadOffice { get; set; }

        [JsonProperty("ebicsStatus")]
        public string? EbicsStatus { get; set; }

        [JsonProperty("jdcRibId")]
        public string? JdcRibId { get; set; }

        [JsonProperty("jdcReleveId")]
        public string? JdcReleveId { get; set; }

        [JsonProperty("bankSortCode")]
        public string? BankSortCode { get; set; }

        [JsonProperty("bankCode")]
        public string? BankCode { get; set; }

        [JsonProperty("jdcDossierId")]
        public string? JdcDossierId { get; set; }

        [JsonProperty("companyId")]
        public string? CompanyId { get; set; }

        [JsonProperty("bankAccountNumber")]
        public string? BankAccountNumber { get; set; }

        [JsonProperty("bankCheckNumber")]
        public string? BankCheckNumber { get; set; }
    }
}
