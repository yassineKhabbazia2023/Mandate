// <copyright file="HelpersTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Client.Http.Tests
{
    public class HelpersTest
    {
        [Fact]
        public void ConvertListToQueryString_WithNullStatusCodes_ReturnsEmptyString()
        {
            // Arrange
            List<int> statusCodes = null!;

            // Act
            var result = Helpers.ConvertListToQueryString(statusCodes);

            // Assert
            result.Should().Be(string.Empty);
        }

        [Fact]
        public void ConvertListToQueryString_WithEmptyStatusCodes_ReturnsEmptyString()
        {
            // Arrange
            var statusCodes = new List<int>();

            // Act
            var result = Helpers.ConvertListToQueryString(statusCodes);

            // Assert
            result.Should().Be(string.Empty);
        }

        [Fact]
        public void ConvertListToQueryString_WithMultipleStatusCodes_ReturnsCorrectQueryString()
        {
            // Arrange
            var statusCodes = new List<int> { 1, 2, 3 };

            // Act
            var result = Helpers.ConvertListToQueryString(statusCodes);

            // Assert
            result.Should().Be("statusCodes=1&statusCodes=2&statusCodes=3");
        }
    }
}
