// <copyright file="MonitoringJdcStatusOrchestrator.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace Mandate.AzureFunctions.Activities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using KPMG.Pulse.Back.Accounting.Mandate.Client;
    using Mandate.AzureFunctions.Interfaces;
    using Microsoft.Azure.Functions.Worker;
    using Microsoft.DurableTask;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;

    /// <summary>
    /// class impléménts the orchastrator and activities of the fonction.
    /// </summary>
    public class MonitoringJdcStatusOrchestrator
    {
        private readonly IMandateFunctionManager mandateManager;
        private readonly ILogger<MonitoringJdcStatusOrchestrator> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="MonitoringJdcStatusOrchestrator"/> class.
        /// </summary>
        /// <param name="mandateManager">A instance of the <see cref="IMandateFunctionManager"/> class.</param>
        /// <param name="logger">A instance of the <see cref="ILogger"/> class.</param>
        public MonitoringJdcStatusOrchestrator(IMandateFunctionManager mandateManager, ILogger<MonitoringJdcStatusOrchestrator> logger)
        {
            this.mandateManager = mandateManager;
            this.logger = logger;
        }

        /// <summary>
        /// Orchestrator for monitoring status.
        /// </summary>
        /// <param name="context">instance of the <see cref="IDurableOrchestrationContext"/> class.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [Function("MappingStatus")]
        public async Task RunOrchestrator(
            [OrchestrationTrigger] TaskOrchestrationContext context)
        {
            var input = context!.GetInput<OrchestratorInput>();
            string? limitConfig = input?.LimitConfig;
            string? statusCodesConfig = input?.StatusCodesConfig;

            int count = 0;
            int skip = 0;
            int limit;
            if (!int.TryParse(limitConfig, out limit))
            {
                limit = 100;
            }

            List<int> statusCodes = ParseStatusCodes(statusCodesConfig!);
            do
            {
                // Call activities without direct logging in the orchestrator
                PagedTechnicalMandate page = await context.CallActivityAsync<PagedTechnicalMandate>(
                    nameof(this.GetCollections),
                    new Payload(skip, limit, statusCodes));

                List<TechnicalCollectionSummary> collections = page.Data.ToList();
                collections.RemoveAll(item => item.RibId == null);

                await context.CallActivityAsync<Task>(
                    nameof(this.RefreshCollectionsStatuses),
                    collections);

                count = page.Data.Count;
                skip += limit;
            }
            while (limit <= count);
        }

        /// <summary>
        /// Activity to Get list of collection.
        /// </summary>
        /// <param name="payload">instance of the <see cref="Payload"/> class.</param>
        /// <param name="log">instance of the <see cref="ILogger"/> class.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [Function(nameof(GetCollections))]
        public async Task<PagedTechnicalMandate> GetCollections(
            [ActivityTrigger] Payload payload,
            ILogger log)
        {
            try
            {
                log.LogInformation("Starting {Functionname} with payload {Payload}.", nameof(this.GetCollections), JsonConvert.SerializeObject(payload!));
                return await this.mandateManager.GetCollectionsAsync(payload!.Skip, payload!.Limit, payload!.StatusCodes);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "MandateFunction - {FunctionName} : {Message}", nameof(this.GetCollections), ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Activity to Get list of collection.
        /// </summary>
        /// <param name="payload">instance of the <see cref="Payload"/> class.</param>
        /// <param name="log">instance of the <see cref="ILogger"/> class.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [Function(nameof(RefreshCollectionsStatuses))]
        public async Task RefreshCollectionsStatuses(
            [ActivityTrigger] List<TechnicalCollectionSummary> payload,
            ILogger log)
        {
            try
            {
                log.LogInformation("Starting {Functionname} with payload {Nbr} elements.", nameof(this.RefreshCollectionsStatuses), payload!.Count);
                await this.mandateManager.RefreshCollectionsStatuses(payload!);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "MandateFunction - {FunctionName} : {Message}", nameof(this.RefreshCollectionsStatuses), ex.Message);
            }
        }

        private static List<int> ParseStatusCodes(string codesString)
        {
            List<int> statusCodes = new List<int>();

            foreach (var codeString in codesString.Split(","))
            {
                if (int.TryParse(codeString, out int code))
                {
                    statusCodes.Add(code);
                }
            }

            return statusCodes;
        }
    }
}
