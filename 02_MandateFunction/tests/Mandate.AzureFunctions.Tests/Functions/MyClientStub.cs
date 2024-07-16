// <copyright file="MonitoringJdcStatusTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.AzureFunctions.Tests
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.DurableTask;
    using Microsoft.DurableTask.Client;

    public class MyClientStub : DurableTaskClient
    {
        public MyClientStub()
            : base("MyAmazingClient")
        {
        }

        public override ValueTask DisposeAsync()
        {
            throw new NotImplementedException();
        }

        public override AsyncPageable<OrchestrationMetadata> GetAllInstancesAsync(OrchestrationQuery? filter = null)
        {
            throw new NotImplementedException();
        }

        public override Task<OrchestrationMetadata?> GetInstancesAsync(string instanceId, bool getInputsAndOutputs = false, CancellationToken cancellation = default)
        {
            throw new NotImplementedException();
        }

        public override Task RaiseEventAsync(string instanceId, string eventName, object? eventPayload = null, CancellationToken cancellation = default)
        {
            throw new NotImplementedException();
        }

        public override Task ResumeInstanceAsync(string instanceId, string? reason = null, CancellationToken cancellation = default)
        {
            throw new NotImplementedException();
        }

        public override Task<string> ScheduleNewOrchestrationInstanceAsync(TaskName orchestratorName, object? input = null, StartOrchestrationOptions? options = null, CancellationToken cancellation = default)
        {
            throw new NotImplementedException();
        }

        public override Task SuspendInstanceAsync(string instanceId, string? reason = null, CancellationToken cancellation = default)
        {
            throw new NotImplementedException();
        }

        public override Task<OrchestrationMetadata> WaitForInstanceCompletionAsync(string instanceId, bool getInputsAndOutputs = false, CancellationToken cancellation = default)
        {
            throw new NotImplementedException();
        }

        public override Task<OrchestrationMetadata> WaitForInstanceStartAsync(string instanceId, bool getInputsAndOutputs = false, CancellationToken cancellation = default)
        {
            throw new NotImplementedException();
        }
    }
}
