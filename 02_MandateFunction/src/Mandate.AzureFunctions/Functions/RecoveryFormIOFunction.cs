// <copyright file="RecoveryFormIOFunction.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Function.Functions
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;
    using global::Mandate.AzureFunctions;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.DurableTask;
    using Microsoft.Azure.WebJobs.Extensions.Http;
    using Microsoft.Extensions.Logging;

    public class RecoveryFormIOFunction
    {
        [FunctionName("RecoveryFormIOFunction_HttpStart")]
        public static async Task<HttpResponseMessage> HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestMessage req,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {
            var limitConfig = Environment.GetEnvironmentVariable("LimitRecoveryFormIo");

            // Function input comes from the request content.
            string instanceId = await starter.StartNewAsync("RecoveryFormIo", new OrchestratorInput { LimitConfig = limitConfig, });

            log.LogInformation("Started orchestration with ID = '{instanceId}'.", instanceId);

            return starter.CreateCheckStatusResponse(req, instanceId);
        }
    }
}