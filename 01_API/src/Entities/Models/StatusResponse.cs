// <copyright file="CollectionStatus.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Models;

using Newtonsoft.Json;
public class StatusResponse
{
    public StatusResponse(CollectionStatus? statusCode, string? errorMessage)
    {
        this.StatusCode = statusCode;
        this.ErrorMessage = errorMessage;
    }

    public CollectionStatus? StatusCode { get; }

    public string? ErrorMessage { get; }

}
