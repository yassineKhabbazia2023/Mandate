// <copyright file="MandateManagerTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.AzureFunctions.Tests
{
    using global::Mandate.AzureFunctions.Managers;
    using KPMG.Pulse.Back.Accounting.Mandate.Client;

    public class MandateManagerTest
    {
        private readonly Mock<IMandateProvider> mockMandateProvider;
        private readonly MandateFunctionManager mandateManager;

        public MandateManagerTest()
        {
            this.mockMandateProvider = new Mock<IMandateProvider>();
            this.mandateManager = new MandateFunctionManager(this.mockMandateProvider.Object);
        }

        [Fact]
        public async Task GetCollectionsAsync_CallsProviderWithCorrectParameters()
        {
            // Arrange
            var expectedResponse = new PagedTechnicalMandate(new List<TechnicalCollectionSummary> { /* ... populate test data ... */ });
            int skip = 0, limit = 10;
            var statusCodes = new List<int> { 1, 2, 3 };
            this.mockMandateProvider.Setup(p => p.GetCollectionsAsync(skip, limit, statusCodes))
                                    .ReturnsAsync(expectedResponse);

            // Act
            var result = await this.mandateManager.GetCollectionsAsync(skip, limit, statusCodes);

            // Assert
            result.Should().BeEquivalentTo(expectedResponse);
            this.mockMandateProvider.Verify(p => p.GetCollectionsAsync(skip, limit, statusCodes), Times.Once);
        }

        [Fact]
        public async Task RefreshCollectionsStatuses_CallsProviderWithCorrectPayload()
        {
            // Arrange
            var payload = new List<TechnicalCollectionSummary> { /* ... populate test data ... */ };
            this.mockMandateProvider.Setup(p => p.RefreshCollectionsStatuses(payload))
                                    .Returns(Task.CompletedTask);

            // Act
            await this.mandateManager.RefreshCollectionsStatuses(payload);

            // Assert
            this.mockMandateProvider.Verify(p => p.RefreshCollectionsStatuses(payload), Times.Once);
        }
    }
}
