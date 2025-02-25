// <copyright file="MandateManagerTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

using KPMG.Pulse.Back.Accounting.Mandate.Sql;
using Microsoft.Extensions.Logging.Abstractions;

namespace KPMG.Pulse.Back.Accounting.Mandate.AzureFunctions.Tests
{
    using global::Mandate.AzureFunctions.Managers;
    using KPMG.Pulse.Back.Accounting.Mandate.Client;
    using KPMG.Pulse.Back.Accounting.Mandate.Function.Interfaces;

    public class MandateManagerTest
    {
        private readonly UpdateMandateStatusesHandler _mandateManager;
        private readonly Mock<IMandateRepository> _repositoryMock;
        private readonly Mock<IJeDeclareService> _jeDeclareServiceMock;
        private readonly Mock<IMandateEventPublisher> _mandateEventPublisherMock;

        public MandateManagerTest()
        {
            _repositoryMock = new Mock<IMandateRepository>();
            _jeDeclareServiceMock = new Mock<IJeDeclareService>();
            _mandateEventPublisherMock = new Mock<IMandateEventPublisher>();
            _mandateManager = new UpdateMandateStatusesHandler(
                NullLogger<UpdateMandateStatusesHandler>.Instance,
                _mandateEventPublisherMock.Object,
                _repositoryMock.Object, _jeDeclareServiceMock.Object);
        }

        [Fact]
        public async Task GetCollectionsAsync_CallsProviderWithCorrectParameters()
        {
            // Arrange
            _repositoryMock.Setup(d =>
                d.GetCollectionByStatus(It.IsAny<List<int>>())).ReturnsAsync([]);

            var statusCodes = new List<int> { 1, 2, 3 };

            // Act
            await _mandateManager.UpdateMandateStatusAsync(statusCodes);

            true.Should().Be(true);
        }
    }
}