// <copyright file="MonitoringJdcStatusTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.AzureFunctions.Tests
{
    using global::Mandate.AzureFunctions.Functions;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.DurableTask;
    using Microsoft.Azure.WebJobs.Extensions.Timers;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    public class MonitoringJdcStatusTest
    {
        private readonly Mock<IDurableOrchestrationClient> mockStarter;
        private readonly Mock<ILogger> mockLog;
        private readonly Mock<IConfiguration> mockConfiguration;

        public MonitoringJdcStatusTest()
        {
            this.mockStarter = new Mock<IDurableOrchestrationClient>();
            this.mockLog = new Mock<ILogger>();
            this.mockConfiguration = new Mock<IConfiguration>();
        }

        [Fact]
        public async Task StatusesMonitoringDailyRunScheduleStart_ShouldStartOrchestration()
        {
            // Arrange
            var timerInfo = new TimerInfo(null, new ScheduleStatus(), false);
            this.mockStarter.Setup(s => s.StartNewAsync("MappingStatus", It.IsAny<object>()))
                       .ReturnsAsync("instanceId");

            // Setup configuration values
            this.mockConfiguration.Setup(c => c["LimitDaily"]).Returns("100");
            this.mockConfiguration.Setup(c => c["StatusCodesDaily"]).Returns("1,2,3");

            // Act
            await MonitoringJdcStatus.StatusesMonitoringDailyRunScheduleStart(timerInfo, this.mockStarter.Object, this.mockLog.Object);

            // Assert
            this.mockStarter.Verify(s => s.StartNewAsync("MappingStatus", It.IsAny<object>()), Times.Once);
        }

        [Fact]
        public async Task StatusesMonitoringHourlyRunScheduleStart_ShouldStartOrchestration()
        {
            // Arrange
            var timerInfo = new TimerInfo(null, new ScheduleStatus(), false);
            this.mockStarter.Setup(s => s.StartNewAsync("MappingStatus", It.IsAny<object>()))
                       .ReturnsAsync("instanceId");

            // Setup configuration values
            this.mockConfiguration.Setup(c => c["LimitHourly"]).Returns("100");
            this.mockConfiguration.Setup(c => c["StatusCodesHourly"]).Returns("1,2,3");

            // Act
            await MonitoringJdcStatus.StatusesMonitoringHourlyRunSchedulStart(timerInfo, this.mockStarter.Object, this.mockLog.Object);

            // Assert
            this.mockStarter.Verify(s => s.StartNewAsync("MappingStatus", It.IsAny<object>()), Times.Once);
        }

        [Fact]
        public async Task HttpStart_ShouldStartOrchestrationAndReturnResponse()
        {
            // Arrange
            var httpRequestMessage = new HttpRequestMessage();
            this.mockStarter.Setup(s => s.StartNewAsync("MappingStatus", It.IsAny<object>()))
                       .ReturnsAsync("instanceId");

            // Setup configuration values
            this.mockConfiguration.Setup(c => c["LimitHttp"]).Returns("100");
            this.mockConfiguration.Setup(c => c["StatusCodesHttp"]).Returns("1,2,3");

            // Act
            var response = await MonitoringJdcStatus.HttpStart(httpRequestMessage, this.mockStarter.Object, this.mockLog.Object);

            // Assert
            this.mockStarter.Verify(s => s.StartNewAsync("MappingStatus", It.IsAny<object>()), Times.Once);
        }
    }
}
