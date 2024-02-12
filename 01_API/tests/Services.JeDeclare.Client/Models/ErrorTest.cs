// <copyright file="ErrorTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.JeDeclare.Client.Tests
{
    using System.Text.Json;

    public class ErrorTest
    {
        [Fact]
        public void Constructor_ShouldCorrectlyAssignProperties()
        {
            // Arrange
            var errorType = "ValidationError";
            var logReference = "123456";
            var message = "An error occurred";

            // Act
            var error = new Error(errorType, logReference, message);

            // Assert
            error.ErrorType.Should().Be(errorType);
            error.LogReference.Should().Be(logReference);
            error.Message.Should().Be(message);
        }

        [Fact]
        public void Error_ShouldSerializeAndDeserializeCorrectly()
        {
            // Arrange
            var error = new Error("ValidationError", "123456", "An error occurred");
            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

            // Act
            var json = JsonSerializer.Serialize(error, options);
            var deserializedError = JsonSerializer.Deserialize<Error>(json, options);

            // Assert
            deserializedError.Should().NotBeNull();
            deserializedError?.ErrorType.Should().Be(error.ErrorType);
            deserializedError?.LogReference.Should().Be(error.LogReference);
            deserializedError?.Message.Should().Be(error.Message);
        }
    }
}
