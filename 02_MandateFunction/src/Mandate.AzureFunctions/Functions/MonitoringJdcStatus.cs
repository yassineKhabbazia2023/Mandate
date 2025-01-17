namespace Mandate.AzureFunctions.Functions
{
    using Microsoft.Azure.Functions.Worker;
    using Microsoft.Azure.Functions.Worker.Http;
    using Microsoft.DurableTask.Client;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;

    public class MonitoringJdcStatus
    {
        private readonly List<int> _dailyStatusCodes;
        private readonly List<int> _hourlyStatusCodes;

        public MonitoringJdcStatus(IOptions<UpdateStatusesConfiguration> options)
        {
            _dailyStatusCodes = options.Value.DailyStatusCodes ?? throw new ArgumentNullException(nameof(options));
            _hourlyStatusCodes = options.Value.HourlyStatusCodes ?? throw new ArgumentNullException(nameof(options));
        }

        /// <summary>
        /// Triggered by a timer to start the daily statuses monitoring process.
        /// </summary>
        /// <remarks>
        /// This function is invoked on a schedule defined by the "StatusesMonitoringDailyRunSchedule" setting.
        /// It initiates an orchestrator function for the daily monitoring of statuses.
        /// </remarks>
        /// <param name="myTimer">Timer information, including the schedule.</param>
        /// <param name="starter">Durable orchestration client to start orchestrations.</param>
        /// <param name="executionContext">Logger instance for logging purpose.</param>
        [Function("StatusesMonitoringDailyRunSchedule_Start")]
        public async Task StatusesMonitoringDailyRunScheduleStart(
            [TimerTrigger("%StatusesMonitoringDailyRunSchedule%")] TimerInfo myTimer,
            [DurableClient] DurableTaskClient starter,
            FunctionContext executionContext)
        {
            ILogger logger = executionContext.GetLogger("StatusesMonitoringDailyRunSchedule_Start");
            logger.LogInformation("Started statuses monitoring daily run : {MyTimer}", myTimer);

            string instanceId = await starter!.ScheduleNewOrchestrationInstanceAsync("MappingStatus", _dailyStatusCodes);

            logger.LogInformation("Started daily orchestration with ID = '{InstanceId}'.", instanceId);
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
        /// <param name="executionContext">Logger instance for logging purpose.</param>
        [Function("StatusesMonitoringHourlyRunSchedule_Start")]
        public async Task StatusesMonitoringHourlyRunSchedulStart(
            [TimerTrigger("%StatusesMonitoringHourlyRunSchedule%")] TimerInfo myTimer,
            [DurableClient] DurableTaskClient starter,
            FunctionContext executionContext)
        {
            ILogger logger = executionContext.GetLogger("StatusesMonitoringHourlyRunSchedule_Start");
            logger.LogInformation("Started statuses monitoring hourly run : {MyTimer}", myTimer);

            string instanceId = await starter.ScheduleNewOrchestrationInstanceAsync("MappingStatus", _hourlyStatusCodes);

            logger.LogInformation("Started hourly orchestration with ID = '{InstanceId}'.", instanceId);
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
        /// <param name="executionContext">Logger instance for logging purpose.</param>
        /// <returns>The HTTP response including the status of the request.</returns>
        [ExcludeFromCodeCoverage]
        [Function("ActivationFonction_HttpStart")]
        public async Task<HttpResponseData> HttpStart(
           [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req,
           [DurableClient] DurableTaskClient starter,
           FunctionContext executionContext)
        {
            ILogger logger = executionContext.GetLogger("ActivationFonction_HttpStart");
            logger.LogInformation($"Started http functions '.");

            string instanceId = await starter!.ScheduleNewOrchestrationInstanceAsync("MappingStatus", _hourlyStatusCodes);

            logger.LogInformation("Started http orchestration with ID = '{InstanceId}'.", instanceId);

            return await starter.CreateCheckStatusResponseAsync(req, instanceId);
        }
    }
}