// <copyright file="MonitoringJdcStatusOrchestratorTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.AzureFunctions.Tests
{
    using System.Collections.Generic;
    using global::Mandate.AzureFunctions;
    using global::Mandate.AzureFunctions.Activities;
    using global::Mandate.AzureFunctions.Interfaces;
    using KPMG.Pulse.Back.Accounting.Mandate.Client;
    using Microsoft.DurableTask;
    using Microsoft.DurableTask.Client;
    using Microsoft.Extensions.Logging;

    public class MonitoringJdcStatusOrchestratorTest
    {
        private readonly Mock<TaskOrchestrationContext> mockContext;
        private readonly Mock<IMandateFunctionManager> mockMandateManager;
        private readonly Mock<ILogger<MonitoringJdcStatusOrchestrator>> mockLogger;
        private readonly MonitoringJdcStatusOrchestrator orchestrator;

        public MonitoringJdcStatusOrchestratorTest()
        {
            var technicalCollectionSummaryList = new List<TechnicalCollectionSummary>()
            {
                new TechnicalCollectionSummary(
                Guid.Empty,
                "12345",
                "12347",
                new BankDetails(
                    "99999",
                    "00000",
                    "77340082511",
                    "99"),
                20),
                new TechnicalCollectionSummary(
                Guid.Empty,
                "12345",
                null!,
                new BankDetails(
                    "99999",
                    "00000",
                    "77340082511",
                    "99"),
                20),
            };

            this.mockContext = new Mock<TaskOrchestrationContext>();
            var pagedTechnicalMandate = new PagedTechnicalMandate(technicalCollectionSummaryList);

            // Setup the mock for CallActivityAsync
            this.mockContext.Setup(x => x.CallActivityAsync<PagedTechnicalMandate>(
                nameof(MonitoringJdcStatusOrchestrator.GetCollections),
                It.IsAny<Payload>(), 
                null))
                .ReturnsAsync(pagedTechnicalMandate)
                .Verifiable();

            this.mockContext.Setup(x => x.CallActivityAsync(
                nameof(MonitoringJdcStatusOrchestrator.RefreshCollectionsStatuses),
                technicalCollectionSummaryList,
                null))
                .Returns(Task.CompletedTask)
                .Verifiable();

            this.mockMandateManager = new Mock<IMandateFunctionManager>();
            this.mockLogger = new Mock<ILogger<MonitoringJdcStatusOrchestrator>>();

            this.orchestrator = new MonitoringJdcStatusOrchestrator(this.mockMandateManager.Object, this.mockLogger.Object);
        }

        [Fact]
        public async Task RunOrchestrator_CallsActivitiesWithCorrectParameters()
        {
            // Arrange
            var expectedInput = new OrchestratorInput
            {
                LimitConfig = "100",
                StatusCodesConfig = "1,2,3",
            };
            this.mockContext.Setup(ctx => ctx.GetInput<OrchestratorInput>()).Returns(expectedInput);

            // Act
            await this.orchestrator.RunOrchestrator(this.mockContext.Object);

            // Assert
            this.mockContext.Verify(
               x => x.CallActivityAsync<PagedTechnicalMandate>(
               nameof(MonitoringJdcStatusOrchestrator.GetCollections),
               It.IsAny<Payload>(), 
               null), Times.Once);

            this.mockContext.Verify(
                 x => x.CallActivityAsync<Task>(
                 nameof(MonitoringJdcStatusOrchestrator.RefreshCollectionsStatuses),
                 It.Is<List<TechnicalCollectionSummary>>(l => l.Count == 1), null), Times.Once);
        }

        [Fact]
        public async Task GetCollections_ReturnsExpectedResult()
        {
            // Arrange
            var payload = new Payload(0, 100, new List<int> { 1, 2, 3 });
            var expectedMandate = new PagedTechnicalMandate(new List<TechnicalCollectionSummary>());
            this.mockMandateManager.Setup(m => m.GetCollectionsAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<List<int>>()))
                              .ReturnsAsync(expectedMandate);

            // Act
            var result = await this.orchestrator.GetCollections(payload, this.mockLogger.Object);

            // Assert
            result.Should().BeEquivalentTo(expectedMandate);
        }

        [Fact]
        public async Task RefreshCollectionsStatuses_CallsMandateManagerCorrectly()
        {
            // Arrange
            var payload = new List<TechnicalCollectionSummary>();

            // Act
            await this.orchestrator.RefreshCollectionsStatuses(payload, this.mockLogger.Object);

            // Assert
            this.mockMandateManager.Verify(m => m.RefreshCollectionsStatuses(payload), Times.Once);
        }
    }
}
