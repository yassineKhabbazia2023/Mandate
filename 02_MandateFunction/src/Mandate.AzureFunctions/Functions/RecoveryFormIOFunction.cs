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
            var input = await FetchConfiguration(req);

            // Function input comes from the request content.
            string instanceId = await starter.StartNewAsync("RecoveryFormIoOrchestrator", input);

            log.LogInformation("Started orchestration with ID = '{instanceId}'.", instanceId);

            return starter.CreateCheckStatusResponse(req, instanceId);
        }

        private static async Task<RecoveryOrchestratorInput> FetchConfiguration(HttpRequestMessage req)
        {
            var input = await req.Content.ReadAsAsync<RecoveryOrchestratorInput>();
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
}