// <copyright file="PayloadTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.AzureFunctions.Tests
{
    using global::Mandate.AzureFunctions;

    public class PayloadTest
    {
        [Fact]
        public void Constructor()
        {
            // Arrange
            int skip = 10;
            int limit = 20;
            var statusCodes = new List<int> { 1, 2, 3 };

            // Act
            var payload = new Payload(skip, limit, statusCodes);

            // Assert
            payload.Skip.Should().Be(skip);
            payload.Limit.Should().Be(limit);
            payload.StatusCodes.Should().BeEquivalentTo(statusCodes);
        }
    }
}
