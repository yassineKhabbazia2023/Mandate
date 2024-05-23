// <copyright file="RecoveryFormIOFunction.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Function.Functions;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
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
        var input = await FetchConfiguration(req);

        // Function input comes from the request content.
        string instanceId = await starter.ScheduleNewOrchestrationInstanceAsync("RecoveryFormIo", input);

        log.LogInformation("Started orchestration with ID = '{InstanceId}'.", instanceId);

        return starter.CreateCheckStatusResponse(req, instanceId);
    }

    [ExcludeFromCodeCoverage]
    private static async Task<RecoveryOrchestratorInput> FetchConfiguration(HttpRequestData req)
    {
        var json = await req.ReadAsStringAsync();

        RecoveryOrchestratorInput input = null!;
        var option = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        };

        if (!string.IsNullOrEmpty(json))
        {
            input = JsonSerializer.Deserialize<RecoveryOrchestratorInput>(json, option) !;
        }

        var limitConfig = Environment.GetEnvironmentVariable("LimitRecoveryFormIo");

        int limit;
        limit = int.TryParse(limitConfig, out limit) ? limit : 50;

        if (input != null)
        {
            input.LimitConfig = limit;
        }
        else
        {
            input = new RecoveryOrchestratorInput()
            {
                LimitConfig = limit,
                Skip = 0,
            };
        }

        return input;
    }
}