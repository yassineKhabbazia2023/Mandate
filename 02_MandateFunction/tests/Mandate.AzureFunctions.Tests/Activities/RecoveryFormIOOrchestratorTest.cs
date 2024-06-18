// <copyright file="RecoveryFormIOOrchestratorTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.AzureFunctions.Tests.Activities
{
    using System.Collections.Generic;
    using global::Mandate.AzureFunctions;
    using global::Mandate.AzureFunctions.Activities;
    using KPMG.Pulse.Back.Accounting.Mandate.Client;
    using Microsoft.DurableTask;
    using Microsoft.Extensions.Logging;
    using Moq;

    public class RecoveryFormIOOrchestratorTest
    {
        private readonly Mock<TaskOrchestrationContext> mockContext;
        private readonly Mock<ILogger<RecoveryFormIOOrchestrator>> mockLogger;
        private readonly Mock<IPreloadManager> preloadManager;

        public RecoveryFormIOOrchestratorTest()
        {
            this.mockContext = new Mock<TaskOrchestrationContext>(MockBehavior.Strict);
            this.mockLogger = new Mock<ILogger<RecoveryFormIOOrchestrator>>(MockBehavior.Loose);
            this.preloadManager = new Mock<IPreloadManager>(MockBehavior.Strict);
        }

        [Fact]
        public async Task RunOrchestrator_CallsActivitiesWithCorrectParameters()
        {
            // Arrange
            var expectedInput = new RecoveryOrchestratorInput
            {
                LimitConfig = 100,
            };

            this.mockContext.Setup(ctx => ctx.GetInput<RecoveryOrchestratorInput>()).Returns(expectedInput);

            this.mockContext.Setup(x => x.CallActivityAsync<PagedRecoveryMandate>(
                "RecoverPage",
                It.Is<Function.Models.ActivityInput>(t => t.Skip == 0 && t.Limit == 100),
                null))
            .ReturnsAsync(GeneratePage(100))
            .Verifiable();

            this.mockContext.Setup(x => x.CallActivityAsync<PagedRecoveryMandate>(
                "RecoverPage",
                It.Is<Function.Models.ActivityInput>(t => t.Skip == 100 && t.Limit == 100),
                null))
            .ReturnsAsync(GeneratePage(100))
            .Verifiable();

            this.mockContext.Setup(x => x.CallActivityAsync<PagedRecoveryMandate>(
                "RecoverPage",
                It.Is<Function.Models.ActivityInput>(t => t.Skip == 200 && t.Limit == 100),
                null))
            .ReturnsAsync(GeneratePage(99))
            .Verifiable();

            RecoveryFormIOOrchestrator orchestrator = new RecoveryFormIOOrchestrator(this.preloadManager.Object, this.mockLogger.Object);
            await orchestrator.RunOrchestrator(this.mockContext.Object);

            this.mockContext.Verify(
               x => x.CallActivityAsync<PagedRecoveryMandate>(
               nameof(RecoveryFormIOOrchestrator.RecoverPage),
               It.IsAny<Function.Models.ActivityInput>(),
               null), Times.Exactly(3));

            this.mockContext.Verify(
               x => x.CallActivityAsync<PagedRecoveryMandate>(
               nameof(RecoveryFormIOOrchestrator.RecoverPage),
               It.Is<Function.Models.ActivityInput>(i => i.Skip == 100 & i.Limit == 100),
               null), Times.Once);

            this.mockContext.Verify(
               x => x.CallActivityAsync<PagedRecoveryMandate>(
               nameof(RecoveryFormIOOrchestrator.RecoverPage),
               It.Is<Function.Models.ActivityInput>(i => i.Skip == 0 & i.Limit == 100),
               null), Times.Once);

            this.mockContext.Verify(
               x => x.CallActivityAsync<PagedRecoveryMandate>(
               nameof(RecoveryFormIOOrchestrator.RecoverPage),
               It.Is<Function.Models.ActivityInput>(i => i.Skip == 200 & i.Limit == 100),
               null), Times.Once);
        }

        [Fact]
        public async Task RunOrchestrator_CallsActivitiesWithDefaultParameters_WhenNoConfig()
        {
            // Arrange
            var expectedInput = new RecoveryOrchestratorInput()
            {
                LimitConfig = 50,
            };

            this.mockContext.Setup(ctx => ctx.GetInput<RecoveryOrchestratorInput>()).Returns(expectedInput);

            this.mockContext.Setup(x => x.CallActivityAsync<PagedRecoveryMandate>(
                "RecoverPage",
                It.Is<Function.Models.ActivityInput>(t => t.Skip == 0 && t.Limit == 50),
                null))
            .ReturnsAsync(GeneratePage(10))
            .Verifiable();

            RecoveryFormIOOrchestrator orchestrator = new RecoveryFormIOOrchestrator(this.preloadManager.Object, this.mockLogger.Object);
            await orchestrator.RunOrchestrator(this.mockContext.Object);

            this.mockContext.Verify(
               x => x.CallActivityAsync<PagedRecoveryMandate>(
               nameof(RecoveryFormIOOrchestrator.RecoverPage),
               It.Is<Function.Models.ActivityInput>(t => t.Skip == 0 && t.Limit == 50),
               null), Times.Once);
        }

        [Fact]
        public async Task RunOrchestrator_NoFormatString_WhenNoCollection()
        {
            // Arrange
            var expectedInput = new RecoveryOrchestratorInput
            {
                LimitConfig = 10,
            };

            List<CollectionSummary> summary = new List<CollectionSummary>() { null! };
            IReadOnlyList<CollectionSummary> collectionSummaries = summary;

            this.mockContext.Setup(ctx => ctx.GetInput<RecoveryOrchestratorInput>()).Returns(expectedInput);

            this.mockContext.Setup(x => x.CallActivityAsync<PagedRecoveryMandate>(
                "RecoverPage",
                It.Is<Function.Models.ActivityInput>(t => t.Skip == 0 && t.Limit == 10),
                null))
            .ReturnsAsync(new PagedRecoveryMandate(1, collectionSummaries))
            .Verifiable();

            RecoveryFormIOOrchestrator orchestrator = new RecoveryFormIOOrchestrator(this.preloadManager.Object, this.mockLogger.Object);
            await orchestrator.RunOrchestrator(this.mockContext.Object);

            this.mockContext.Verify(
               x => x.CallActivityAsync<PagedRecoveryMandate>(
               nameof(RecoveryFormIOOrchestrator.RecoverPage),
               It.IsAny<Function.Models.ActivityInput>(),
               It.IsAny<TaskOptions>()),
               Times.Once);
        }

        [Fact]
        public async Task RunOrchestrator_ShouldThrow_WhenCallActivityThrow()
        {
            // Arrange
            var expectedInput = new RecoveryOrchestratorInput
            {
                LimitConfig = 10,
            };

            this.mockContext.Setup(ctx => ctx.GetInput<RecoveryOrchestratorInput>()).Returns(expectedInput);

            (int, int) tuple = (0, 10);
            this.mockContext.Setup(x => x.CallActivityAsync<PagedRecoveryMandate>(
                "RecoverPage",
                It.Is<Function.Models.ActivityInput>(t => t.Skip == 0 && t.Limit == 10),
                null))
            .ThrowsAsync(new Exception("message"))
            .Verifiable();

            RecoveryFormIOOrchestrator orchestrator = new RecoveryFormIOOrchestrator(this.preloadManager.Object, this.mockLogger.Object);
            Func<Task> func = async () => await orchestrator.RunOrchestrator(this.mockContext.Object);

            await func.Should().ThrowExactlyAsync<Exception>().WithMessage("message");

            this.mockContext.Verify(
               x => x.CallActivityAsync<PagedRecoveryMandate>(
               nameof(RecoveryFormIOOrchestrator.RecoverPage),
               It.IsAny<Function.Models.ActivityInput>(),
               It.IsAny<TaskOptions>()),
               Times.Once);
        }

        [Fact]
        public async Task RecoverPage_ReturnsExpectedResult()
        {
            var page = GeneratePage(100);

            this.preloadManager.Setup(m => m.RecoveryAsync(0, 100))
                              .ReturnsAsync(page)
                              .Verifiable();

            RecoveryFormIOOrchestrator orchestrator = new RecoveryFormIOOrchestrator(this.preloadManager.Object, this.mockLogger.Object);
            var result = await orchestrator.RecoverPage(new Function.Models.ActivityInput(0, 100));

            // Assert
            result.Should().BeEquivalentTo(page);

            this.preloadManager.Verify(i => i.RecoveryAsync(0, 100), Times.Once);
        }

        [Fact]
        public async Task RecoverPage_ShouldThow_WhenRecoveryAsyncThrowException()
        {
            this.preloadManager.Setup(m => m.RecoveryAsync(0, 100))
                              .ThrowsAsync(new Exception("message"))
                              .Verifiable();

            RecoveryFormIOOrchestrator orchestrator = new RecoveryFormIOOrchestrator(this.preloadManager.Object, this.mockLogger.Object);
            Func<Task> act = async () => await orchestrator.RecoverPage(new Function.Models.ActivityInput(0, 100));

            await act.Should().ThrowExactlyAsync<Exception>().WithMessage("message");

            this.preloadManager.Verify(i => i.RecoveryAsync(0, 100), Times.Once);
        }

        private static PagedRecoveryMandate GeneratePage(int nbr)
        {
            var list = new List<CollectionSummary>();

            for (int i = 0; i < nbr; i++)
            {
                list.Add(new CollectionSummary(
                   Guid.NewGuid(),
                   (i + 1).ToString("00000000000"),
                   "Weyland Corporation",
                   new CollectionBankInfo("bankName", "accountNumber", 1),
                   new DateTime(2023, 10, 1, 0, 0, 0, DateTimeKind.Utc),
                   new DateTime(2023, 10, 2, 0, 0, 0, DateTimeKind.Utc),
                   10));
            }

            IReadOnlyList<CollectionSummary> collectionSummaries = list;

            return new PagedRecoveryMandate(nbr, collectionSummaries);
        }
    }
}
