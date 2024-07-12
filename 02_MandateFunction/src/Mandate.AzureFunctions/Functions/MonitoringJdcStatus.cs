// <copyright file="MonitoringJdcStatus.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace Mandate.AzureFunctions.Functions
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using Microsoft.Azure.Functions.Worker;
    using Microsoft.Azure.Functions.Worker.Http;
    using Microsoft.DurableTask.Client;
    using Microsoft.DurableTask.Internal;
    using Microsoft.Extensions.Logging;

    public static class MonitoringJdcStatus
    {
        /// <summary>
        /// Triggered by a timer to start the daily statuses monitoring process.
        /// </summary>
        /// <remarks>
        /// This function is invoked on a schedule defined by the "StatusesMonitoringDailyRunSchedule" setting.
        /// It initiates an orchestrator function for the daily monitoring of statuses.
        /// </remarks>
        /// <param name="myTimer">Timer information, including the schedule.</param>
        /// <param name="starter">Durable orchestration client to start orchestrations.</param>
        /// <param name="log">Logger instance for logging purpose.</param>
        [Function("StatusesMonitoringDailyRunSchedule_Start")]
        public static async Task StatusesMonitoringDailyRunScheduleStart(
            [TimerTrigger("%StatusesMonitoringDailyRunSchedule%")] TimerInfo myTimer,
            [DurableClient] IOrchestrationSubmitter starter,
            ILogger log)
        {
            log.LogInformation($"Started statuses monitoring daily run : {myTimer}");

            var limitConfig = Environment.GetEnvironmentVariable("LimitDaily");
            var statusCodesConfig = Environment.GetEnvironmentVariable("StatusCodesDaily");

            // Function input comes from the request content.
            string instanceId = await starter!.ScheduleNewOrchestrationInstanceAsync("MappingStatus", new OrchestratorInput { LimitConfig = limitConfig, StatusCodesConfig = statusCodesConfig });

            log.LogInformation($"Started daily orchestration with ID = '{instanceId}'.");
        }

        /// <summary>
        /// Triggered by a timer to start the hourly statuses monitoring process.
        /// </summary>
        /// <remarks>
        /// This function is invoked on a schedule defined by the "StatusesMonitoringHourlyRunSchedule" setting.
        /// It initiates an orchestrator function for the hourly monitoring of statuses.
        /// </remarks>
        /// <param name="myTimer">Timer information, including the schedule.</param>
        /// <param name="starter">Durable orchestration client to start orchestrations.</param>
        /// <param name="log">Logger instance for logging purpose.</param>
        [Function("StatusesMonitoringHourlyRunSchedule_Start")]
        public static async Task StatusesMonitoringHourlyRunSchedulStart(
            [TimerTrigger("%StatusesMonitoringHourlyRunSchedule%")] TimerInfo myTimer,
            [DurableClient] IOrchestrationSubmitter starter,
            ILogger log)
        {
            log.LogInformation($"Started statuses monitoring hourly run : {myTimer}");

            var limitConfig = Environment.GetEnvironmentVariable("LimitHourly");
            var statusCodesConfig = Environment.GetEnvironmentVariable("StatusCodesHourly");

            // Function input comes from the request content.
            string instanceId = await starter!.ScheduleNewOrchestrationInstanceAsync("MappingStatus", new OrchestratorInput { LimitConfig = limitConfig, StatusCodesConfig = statusCodesConfig });

            log.LogInformation($"Started hourly orchestration with ID = '{instanceId}'.");
        }

        /// <summary>
        /// Triggered by an HTTP request to start the statuses monitoring process.
        /// </summary>
        /// <remarks>
        /// This function can be triggered by either a GET or POST request.
        /// It starts an orchestrator function to monitor statuses based on the HTTP request content.
        /// </remarks>
        /// <param name="req">The HTTP request triggering this function.</param>
        /// <param name="starter">Durable orchestration client to start orchestrations.</param>
        /// <param name="log">Logger instance for logging purpose.</param>
        /// <returns>The HTTP response including the status of the request.</returns>
        [ExcludeFromCodeCoverage]
        [Function("ActivationFonction_HttpStart")]
        public static async Task<HttpResponseData> HttpStart(
           [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req,
           [DurableClient] DurableTaskClient starter,
           ILogger log)
        {
            log.LogInformation($"Started http functions '.");

            var limitConfig = Environment.GetEnvironmentVariable("LimitHttp");
            var statusCodesConfig = Environment.GetEnvironmentVariable("StatusCodesHttp");

            // Function input comes from the request content.
            string instanceId = await starter!.ScheduleNewOrchestrationInstanceAsync("MappingStatus", new OrchestratorInput { LimitConfig = limitConfig!, StatusCodesConfig = statusCodesConfig! });

            log.LogInformation($"Started http orchestration with ID = '{instanceId}'.");

            return starter.CreateCheckStatusResponse(req, instanceId);
        }
    }
}