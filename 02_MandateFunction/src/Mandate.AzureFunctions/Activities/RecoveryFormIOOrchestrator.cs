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
    using Microsoft.Azure.Functions.Worker;
    using Microsoft.DurableTask;
    using Microsoft.Extensions.Logging;

    public class RecoveryFormIOOrchestrator
    {
        private readonly IPreloadManager preloadManager;
        private readonly ILogger<RecoveryFormIOOrchestrator> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="RecoveryFormIOOrchestrator"/> class.
        /// </summary>
        /// <param name="preloadManager">A instance of the <see cref="IPreloadManager"/> class.</param>
        /// <param name="logger">A instance of the <see cref="ILogger{RecoveryFormIOOrchestrator}"/> class.</param>
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
        [Function("RecoveryFormIo")]
        public async Task RunOrchestrator(
            [OrchestrationTrigger] TaskOrchestrationContext context)
        {
            int failed = 0, success = 0;
            List<string> failedMandate = new List<string>();

            try
            {
                var input = context!.GetInput<RecoveryOrchestratorInput>();
                int limit = input.LimitConfig;

                int skip = input.Skip;
                int imported = limit;

                while (imported == limit && limit > 0)
                {
                    var page = await context.CallActivityAsync<PagedRecoveryMandate>(
                        nameof(this.RecoverPage),
                        (skip, limit));

                    skip += page.Imported;
                    imported = page.Imported;
                    failed += page.Failed.Count;
                    success = limit - failed;

                    failedMandate.AddRange(page.Failed.Select(item => item.Stringify()).ToList());
                }
            }
            catch (Exception ex)
            {
                string message = !string.IsNullOrEmpty(ex.InnerException?.Message) ? ex.InnerException.Message : ex.Message;
                this.logger.LogError(ex, "MandateFunction - {functionName} : {message}", nameof(this.RecoverPage), message);
                throw;
            }
            finally
            {
                this.logger.LogWarning("Finish {functionname} with {failed} failed and {success} success.", nameof(this.RunOrchestrator), failed, success);
                this.logger.LogWarning("Finish {functionname} failted mandate {failedMandate}", nameof(this.RunOrchestrator), string.Join(',', failedMandate));
            }
        }

        /// <summary>
        /// Activity to Get list of collection.
        /// </summary>
        /// <param name="tuple">tuple which contains skip and limit.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [Function(nameof(RecoverPage))]
        public async Task<PagedRecoveryMandate> RecoverPage(
            [ActivityTrigger] (int, int) tuple)
        {
            try
            {
                this.logger.LogInformation("Starting {Functionname} with skip {Skip} and limit {Limit}.", nameof(this.RecoverPage), tuple.Item1, tuple.Item2);
                return await this.preloadManager.RecoveryAsync(tuple.Item1, tuple.Item2);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "MandateFunction - {FunctionName} : {Message}", nameof(this.RecoverPage), ex.Message);
                throw;
            }
        }
    }
}
