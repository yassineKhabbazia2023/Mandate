// <copyright file="RecoveryFormIOOrchestratorTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.AzureFunctions.Tests.Activities
{
    using global::Mandate.AzureFunctions;
    using global::Mandate.AzureFunctions.Activities;
    using KPMG.Pulse.Back.Accounting.Mandate.Client;
    using Microsoft.DurableTask;
    using Microsoft.Extensions.Logging;
    using Moq;
    using System.Collections.Generic;

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

            (int, int) tuple = (0, 100);
            this.mockContext.Setup(x => x.CallActivityAsync<PagedRecoveryMandate>(
                "RecoverPage",
                tuple,
                null))
            .ReturnsAsync(GeneratePage(100))
            .Verifiable();

            tuple.Item1 += 100;
            this.mockContext.Setup(x => x.CallActivityAsync<PagedRecoveryMandate>(
                "RecoverPage",
                tuple,
                null))
            .ReturnsAsync(GeneratePage(100))
            .Verifiable();

            tuple.Item1 += 100;
            this.mockContext.Setup(x => x.CallActivityAsync<PagedRecoveryMandate>(
                "RecoverPage",
                tuple,
                null))
            .ReturnsAsync(GeneratePage(99))
            .Verifiable();

            RecoveryFormIOOrchestrator orchestrator = new RecoveryFormIOOrchestrator(this.preloadManager.Object, this.mockLogger.Object);
            await orchestrator.RunOrchestrator(this.mockContext.Object);

            this.mockContext.Verify(
               x => x.CallActivityAsync<PagedRecoveryMandate>(
               nameof(RecoveryFormIOOrchestrator.RecoverPage),
               It.IsAny<(int, int)>(),
               null), Times.Exactly(3));

            this.mockContext.Verify(
               x => x.CallActivityAsync<PagedRecoveryMandate>(
               nameof(RecoveryFormIOOrchestrator.RecoverPage),
               It.Is<(int, int)>(i => i.Item1 == 100 & i.Item2 == 100),
               null), Times.Once);

            this.mockContext.Verify(
               x => x.CallActivityAsync<PagedRecoveryMandate>(
               nameof(RecoveryFormIOOrchestrator.RecoverPage),
               It.Is<(int, int)>(i => i.Item1 == 0 & i.Item2 == 100),
               null), Times.Once);

            this.mockContext.Verify(
               x => x.CallActivityAsync<PagedRecoveryMandate>(
               nameof(RecoveryFormIOOrchestrator.RecoverPage),
               It.Is<(int, int)>(i => i.Item1 == 200 & i.Item2 == 100),
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

            (int, int) tuple = (0, 50);
            this.mockContext.Setup(x => x.CallActivityAsync<PagedRecoveryMandate>(
                "RecoverPage",
                tuple,
                null))
            .ReturnsAsync(GeneratePage(10))
            .Verifiable();

            RecoveryFormIOOrchestrator orchestrator = new RecoveryFormIOOrchestrator(this.preloadManager.Object, this.mockLogger.Object);
            await orchestrator.RunOrchestrator(this.mockContext.Object);

            this.mockContext.Verify(
               x => x.CallActivityAsync<PagedRecoveryMandate>(
               nameof(RecoveryFormIOOrchestrator.RecoverPage),
               It.Is<(int, int)>(i => i.Item1 == 0 & i.Item2 == 50),
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

            (int, int) tuple = (0, 10);
            this.mockContext.Setup(x => x.CallActivityAsync<PagedRecoveryMandate>(
                "RecoverPage",
                tuple))
            .ReturnsAsync(new PagedRecoveryMandate(1, collectionSummaries))
            .Verifiable();

            RecoveryFormIOOrchestrator orchestrator = new RecoveryFormIOOrchestrator(this.preloadManager.Object, this.mockLogger.Object);
            await orchestrator.RunOrchestrator(this.mockContext.Object);

            this.mockContext.Verify(
               x => x.CallActivityAsync<PagedRecoveryMandate>(
               nameof(RecoveryFormIOOrchestrator.RecoverPage),
               It.IsAny<(int, int)>()), Times.Once);
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
                tuple))
            .ThrowsAsync(new Exception("message"))
            .Verifiable();

            RecoveryFormIOOrchestrator orchestrator = new RecoveryFormIOOrchestrator(this.preloadManager.Object, this.mockLogger.Object);
            Func<Task> func = async () => await orchestrator.RunOrchestrator(this.mockContext.Object);

            await func.Should().ThrowExactlyAsync<Exception>().WithMessage("message");

            this.mockContext.Verify(
               x => x.CallActivityAsync<PagedRecoveryMandate>(
               nameof(RecoveryFormIOOrchestrator.RecoverPage),
               It.IsAny<(int, int)>()), Times.Once);
        }

        [Fact]
        public async Task RecoverPage_ReturnsExpectedResult()
        {
            var page = GeneratePage(100);

            this.preloadManager.Setup(m => m.RecoveryAsync(0, 100))
                              .ReturnsAsync(page)
                              .Verifiable();

            RecoveryFormIOOrchestrator orchestrator = new RecoveryFormIOOrchestrator(this.preloadManager.Object, this.mockLogger.Object);
            var result = await orchestrator.RecoverPage((0, 100));

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
            Func<Task> act = async () => await orchestrator.RecoverPage((0, 100));

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
                   "Crédit Agricole",
                   "98765432101",
                   new DateTime(2023, 10, 1, 0, 0, 0, DateTimeKind.Utc),
                   new DateTime(2023, 10, 2, 0, 0, 0, DateTimeKind.Utc),
                   10));
            }

            IReadOnlyList<CollectionSummary> collectionSummaries = list;

            return new PagedRecoveryMandate(nbr, collectionSummaries);
        }
    }
}
