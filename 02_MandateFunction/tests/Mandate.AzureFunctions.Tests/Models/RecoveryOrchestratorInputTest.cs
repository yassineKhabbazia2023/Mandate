// <copyright file="RecoveryOrchestratorInputTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.AzureFunctions.Tests.Models
{
    using global::Mandate.AzureFunctions;

    public class RecoveryOrchestratorInputTest
    {
        [Fact]
        public void Constructor()
        {
            // Arrange
            int skip = 10;
            int limit = 20;

            // Act
            var payload = new RecoveryOrchestratorInput()
            {
                Skip = skip,
                LimitConfig = limit,
            };

            // Assert
            payload.Skip.Should().Be(skip);
            payload.LimitConfig.Should().Be(limit);
        }
    }
}
