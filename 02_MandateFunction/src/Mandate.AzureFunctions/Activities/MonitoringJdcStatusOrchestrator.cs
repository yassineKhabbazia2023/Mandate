// <copyright file="MonitoringJdcStatusOrchestrator.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace Mandate.AzureFunctions.Activities
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Interfaces;
    using Microsoft.Azure.Functions.Worker;
    using Microsoft.DurableTask;

    public class MonitoringJdcStatusOrchestrator
    {
        private readonly IUpdateMandateStatusesHandler _mandateManager;

        public MonitoringJdcStatusOrchestrator(IUpdateMandateStatusesHandler mandateManager)
        {
            _mandateManager = mandateManager;
        }

        [Function("MappingStatus")]
        public async Task RunOrchestrator(
            [OrchestrationTrigger] TaskOrchestrationContext context, List<int> statusCodes)
        {
            await _mandateManager.UpdateMandateStatusAsync(statusCodes);
        }
    }
}