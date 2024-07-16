// <copyright file="MonitoringJdcStatusTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.AzureFunctions.Tests
{
    using System;
    using System.Threading.Tasks;
    using global::Mandate.AzureFunctions;
    using global::Mandate.AzureFunctions.Functions;
    using Microsoft.Azure.Functions.Worker;
    using Microsoft.DurableTask;

    public class MonitoringJdcStatusTest
    {
        private readonly MyFunctionContextStub functionContext;
        private readonly Mock<DurableTaskClientStub> mockDurableTaskClient;

        public MonitoringJdcStatusTest()
        {
            this.functionContext = new MyFunctionContextStub();
            this.mockDurableTaskClient = new Mock<DurableTaskClientStub>();
        }

        [Fact]
        public async Task StatusesMonitoringDailyRunScheduleStart_Should_Start_Daily_Orchestration()
        {
            // Arrange
            var timerInfo = new TimerInfo()
            {
                IsPastDue = false,
                ScheduleStatus = null,
            };
            var limitConfig = "100";
            var statusCodesConfig = "200,201";
            Environment.SetEnvironmentVariable("LimitDaily", limitConfig);
            Environment.SetEnvironmentVariable("StatusCodesDaily", statusCodesConfig);

            this.mockDurableTaskClient
                .Setup(x => x.ScheduleNewOrchestrationInstanceAsync("MappingStatus", It.IsAny<OrchestratorInput>(), It.IsAny<StartOrchestrationOptions>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync("instanceId");


            // Act
            await MonitoringJdcStatus.StatusesMonitoringDailyRunScheduleStart(timerInfo, this.mockDurableTaskClient.Object, this.functionContext);

            // Assert
            this.mockDurableTaskClient.Verify(
                x => x.ScheduleNewOrchestrationInstanceAsync(
                    "MappingStatus",
                    It.Is<OrchestratorInput>(input => input.LimitConfig == limitConfig && input.StatusCodesConfig == statusCodesConfig),
                    It.IsAny<StartOrchestrationOptions>(),
                    It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task StatusesMonitoringHourlyRunSchedulStart_Should_Start_Hourly_Orchestration()
        {
            // Arrange
            var timerInfo = new TimerInfo()
            {
                IsPastDue = false,
                ScheduleStatus = null,
            };
            var limitConfig = "50";
            var statusCodesConfig = "200,202";
            Environment.SetEnvironmentVariable("LimitHourly", limitConfig);
            Environment.SetEnvironmentVariable("StatusCodesHourly", statusCodesConfig);

            this.mockDurableTaskClient
                .Setup(x => x.ScheduleNewOrchestrationInstanceAsync("MappingStatus", It.IsAny<OrchestratorInput>(), It.IsAny<StartOrchestrationOptions>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync("instanceId");

            // Act
            await MonitoringJdcStatus.StatusesMonitoringHourlyRunSchedulStart(timerInfo, this.mockDurableTaskClient.Object, this.functionContext);

            // Assert
            this.mockDurableTaskClient.Verify(
                x => x.ScheduleNewOrchestrationInstanceAsync(
                    "MappingStatus",
                    It.Is<OrchestratorInput>(input => input.LimitConfig == limitConfig && input.StatusCodesConfig == statusCodesConfig),
                    It.IsAny<StartOrchestrationOptions>(),
                    It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
