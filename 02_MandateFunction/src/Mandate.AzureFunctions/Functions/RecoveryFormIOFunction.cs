// <copyright file="RecoveryFormIOFunction.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Function.Functions;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Threading.Tasks;
using global::Mandate.AzureFunctions;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;

public class RecoveryFormIOFunction
{
    [ExcludeFromCodeCoverage]
    [Function("RecoveryFormIOFunction_HttpStart")]
    public static async Task<HttpResponseData> HttpStart(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req,
        [DurableClient] DurableTaskClient starter,
        ILogger log)
    {
        var limitConfig = Environment.GetEnvironmentVariable("LimitRecoveryFormIo");

        // Function input comes from the request content.
        string instanceId = await starter.ScheduleNewOrchestrationInstanceAsync("RecoveryFormIo", new OrchestratorInput { LimitConfig = limitConfig!, });

        log.LogInformation("Started orchestration with ID = '{InstanceId}'.", instanceId);

        return starter.CreateCheckStatusResponse(req, instanceId);
    }
}