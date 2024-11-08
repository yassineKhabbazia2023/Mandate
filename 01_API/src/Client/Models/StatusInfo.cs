// <copyright file="CollectionBankInfo.cs" company="PULSE">
// Copyright (c) PULSE. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Client;

using Newtonsoft.Json;

public class StatusInfo
{
    [JsonConstructor]
    public StatusInfo(int statusCode, string jdcStatusDescription, int? jdcStatusCode)
    {
        this.StatusCode = statusCode;
        this.JdcStatusDescription = jdcStatusDescription;
        this.JdcStatusCode = jdcStatusCode;
    }

    [JsonProperty("statusCode")]
    public int StatusCode { get; }

    [JsonProperty("jdcStatusDescription")]
    public string JdcStatusDescription { get; }

    [JsonProperty("jdcStatusCode")]
    public int? JdcStatusCode { get; }
}