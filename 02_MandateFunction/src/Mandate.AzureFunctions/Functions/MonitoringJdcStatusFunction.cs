namespace Mandate.AzureFunctions.Functions
{
    using Mandate.AzureFunctions.Interfaces;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Azure.Functions.Worker;
    using Microsoft.Azure.Functions.Worker.Http;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;

    public class MonitoringJdcStatusFunction
    {
        private readonly List<int> _dailyStatusCodes;
        private readonly List<int> _hourlyStatusCodes;
        private readonly ILogger<MonitoringJdcStatusFunction> _logger;
        private readonly IUpdateMandateStatusesHandler _mandateManager;

        public MonitoringJdcStatusFunction(IOptions<UpdateStatusesConfiguration> options, ILoggerFactory loggerFactory, IUpdateMandateStatusesHandler mandateManager)
        {
            _dailyStatusCodes = options.Value.DailyStatusCodes ?? throw new ArgumentNullException(nameof(options));
            _hourlyStatusCodes = options.Value.HourlyStatusCodes ?? throw new ArgumentNullException(nameof(options));
            _logger = loggerFactory.CreateLogger<MonitoringJdcStatusFunction>();
            _mandateManager = mandateManager;
        }

        /// <summary>
        /// Triggered by a timer to start the daily statuses monitoring process.
        /// </summary>
        /// <remarks>
        /// This function is invoked on a schedule defined by the "StatusesMonitoringDailyRunSchedule" setting.
        /// It initiates an orchestrator function for the daily monitoring of statuses.
        /// </remarks>
        /// <param name="myTimer">Timer information, including the schedule.</param>
        [Function("StatusesMonitoringDailyRunSchedule_Start")]
        public async Task StatusesMonitoringDailyRunScheduleStart([TimerTrigger("%StatusesMonitoringDailyRunSchedule%")] TimerInfo myTimer)
        {
            _logger.LogInformation("Started statuses monitoring daily run : {MyTimer}", myTimer);

            await UpdateMandateStatusAsync(_dailyStatusCodes);

            _logger.LogInformation("Finished statuses monitoring daily run");
        }

        /// <summary>
        /// Triggered by a timer to start the hourly statuses monitoring process.
        /// </summary>
        /// <remarks>
        /// This function is invoked on a schedule defined by the "StatusesMonitoringHourlyRunSchedule" setting.
        /// It initiates an orchestrator function for the hourly monitoring of statuses.
        /// </remarks>
        /// <param name="myTimer">Timer information, including the schedule.</param>
        [Function("StatusesMonitoringHourlyRunSchedule_Start")]
        public async Task StatusesMonitoringHourlyRunScheduleStart([TimerTrigger("%StatusesMonitoringHourlyRunSchedule%")] TimerInfo myTimer)
        {
            _logger.LogInformation("Started statuses monitoring hourly run : {MyTimer}", myTimer);

            await UpdateMandateStatusAsync(_hourlyStatusCodes);

            _logger.LogInformation("Finished statuses monitoring hourly run");
        }

        private async Task UpdateMandateStatusAsync(List<int> statusCodes)
        {
            await _mandateManager.UpdateMandateStatusAsync(statusCodes);
        }

        [ExcludeFromCodeCoverage]
        [Function("ActivationFonction_HttpStart")]
        public async Task<IActionResult> HttpStart(
           [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req)
        {
            await UpdateMandateStatusAsync(_hourlyStatusCodes);
            return new OkResult();
        }
    }
}