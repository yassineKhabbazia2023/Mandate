// <copyright file="RecoveryFormIOOrchestrator.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace Mandate.AzureFunctions.Activities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using global::Mandate.AzureFunctions;
    using KPMG.Pulse.Back.Accounting.Mandate.AzureFunctions;
    using KPMG.Pulse.Back.Accounting.Mandate.Client;
    using KPMG.Pulse.Back.Accounting.Mandate.Function.Helper;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.DurableTask;
    using Microsoft.Extensions.Logging;

    public class RecoveryFormIOOrchestrator
    {
        private readonly IPreloadManager preloadManager;
        private readonly ILogger<RecoveryFormIOOrchestrator> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="RecoveryFormIOOrchestrator"/> class.
        /// </summary>
        /// <param name="preloadManager">A instance of the <see cref="IPreloadManager"/> class.</param>
        /// <param name="logger">A instance of the <see cref="ILogger"/> class.</param>
        public RecoveryFormIOOrchestrator(IPreloadManager preloadManager, ILogger<RecoveryFormIOOrchestrator> logger)
        {
            this.preloadManager = preloadManager;
            this.logger = logger;
        }

        /// <summary>
        /// Orchestrator for monitoring status.
        /// </summary>
        /// <param name="context">instance of the <see cref="IDurableOrchestrationContext"/> class.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [FunctionName("RecoveryFormIoOrchestrator")]
        public async Task RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            int failed = 0, success = 0;
            List<string> failedMandate = new List<string>();

            try
            {
                int limit;
                var input = context!.GetInput<OrchestratorInput>();
                string limitConfig = input?.LimitConfig;
                limit = int.TryParse(limitConfig, out limit) ? limit : 50;

                int skip = 0;
                int imported = limit;

                while (imported == limit)
                {
                    var page = await context.CallActivityAsync<PagedRecoveryMandate>(
                        nameof(this.RecoverPage),
                        (skip, limit));

                    skip += page.Imported;
                    imported = page.Imported;
                    failed += page.Failed.Count;
                    success = limit - failed;

                    failedMandate.AddRange(page.Failed.ToList().Select(item => item.Stringify()).ToList());
                }

                this.logger.LogInformation("Finish {functionname} with {failed} failed and {success} success.", nameof(this.RunOrchestrator), failed, success);
            }
            catch (Exception ex)
            {
                string message = !string.IsNullOrEmpty(ex.InnerException?.Message) ? ex.InnerException.Message : ex.Message;
                this.logger.LogError(ex, "MandateFunction - {functionName} : {message}", nameof(this.RecoverPage), message);
                throw;
            }
            finally
            {
                this.logger.LogWarning("Finish {functionname} failted mandate {}", nameof(this.RunOrchestrator), string.Join(',', failedMandate));
            }
        }

        /// <summary>
        /// Activity to Get list of collection.
        /// </summary>
        /// <param name="tuple">tuple which contains skip and limit.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [FunctionName(nameof(RecoverPage))]
        public async Task<PagedRecoveryMandate> RecoverPage(
            [ActivityTrigger] (int, int) tuple)
        {
            try
            {
                this.logger.LogInformation("Starting {functionname} with skip {skip} and limit {limit}.", nameof(this.RecoverPage), tuple.Item1, tuple.Item2);
                return await this.preloadManager.RecoveryAsync(tuple.Item1, tuple.Item2);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "MandateFunction - {functionName} : {message}", nameof(this.RecoverPage), ex.Message);
                throw;
            }
        }
    }
}
