// <copyright file="MonitoringJdcStatusTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.AzureFunctions.Tests
{
    using System;
    using System.Net;
    using System.Threading.Tasks;
    using global::Mandate.AzureFunctions;
    using global::Mandate.AzureFunctions.Functions;
    using Microsoft.Azure.Functions.Worker;
    using Microsoft.Azure.Functions.Worker.Http;
    using Microsoft.DurableTask;
    using Microsoft.DurableTask.Client;
    using Microsoft.DurableTask.Internal;
    using Microsoft.Extensions.Logging;

    public class MonitoringJdcStatusTest
    {
        private readonly Mock<ILogger> mockLogger;
        private readonly Mock<IOrchestrationSubmitter> mockDurableClient;

        public MonitoringJdcStatusTest()
        {
            this.mockLogger = new Mock<ILogger>();
            this.mockDurableClient = new Mock<IOrchestrationSubmitter>();
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

            this.mockDurableClient
                .Setup(x => x.ScheduleNewOrchestrationInstanceAsync("MappingStatus", It.IsAny<OrchestratorInput>(), It.IsAny<StartOrchestrationOptions>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync("instanceId");

            // Act
            await MonitoringJdcStatus.StatusesMonitoringDailyRunScheduleStart(timerInfo, this.mockDurableClient.Object, this.mockLogger.Object);

            // Assert
            this.mockDurableClient.Verify(
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

            this.mockDurableClient
                .Setup(x => x.ScheduleNewOrchestrationInstanceAsync("MappingStatus", It.IsAny<OrchestratorInput>(), It.IsAny<StartOrchestrationOptions>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync("instanceId");

            // Act
            await MonitoringJdcStatus.StatusesMonitoringHourlyRunSchedulStart(timerInfo, this.mockDurableClient.Object, this.mockLogger.Object);

            // Assert
            this.mockDurableClient.Verify(
                x => x.ScheduleNewOrchestrationInstanceAsync(
                    "MappingStatus",
                    It.Is<OrchestratorInput>(input => input.LimitConfig == limitConfig && input.StatusCodesConfig == statusCodesConfig),
                    It.IsAny<StartOrchestrationOptions>(),
                    It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
