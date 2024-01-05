// <copyright file="StatusNotFoundExceptionTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Sql.Tests.Exceptions
{
    using Newtonsoft.Json;

    public class StatusNotFoundExceptionTest
    {
        [Fact]
        public void DefaultConstructor_ShouldInstantiateWithDefaultMessage()
        {
            // Act
            var exception = new StatusNotFoundException();

            // Assert
            exception.Message.Should().Be("Exception of type 'KPMG.Pulse.Back.Accounting.Mandate.Sql.StatusNotFoundException' was thrown.");
        }

        [Fact]
        public void ConstructorWithMessage_ShouldSetMessage()
        {
            // Arrange
            var customMessage = "Custom error message";

            // Act
            var exception = new StatusNotFoundException(customMessage);

            // Assert
            exception.Message.Should().Be(customMessage);
        }

        [Fact]
        public void ConstructorWithMessageAndInnerException_ShouldSetMessageAndInnerException()
        {
            // Arrange
            var customMessage = "Custom error message";
            var innerException = new Exception("Inner exception message");

            // Act
            var exception = new StatusNotFoundException(customMessage, innerException);

            // Assert
            exception.Message.Should().Be(customMessage);
            exception.InnerException.Should().Be(innerException);
        }

        [Fact]
        public void NewtonsoftJson_Serialization_ShouldPreserveExceptionProperties()
        {
            // Arrange
            var customMessage = "Custom error message";
            var innerException = new Exception("Inner exception message");
            var exception = new StatusNotFoundException(customMessage, innerException);

            string jsonString;
            StatusNotFoundException deserializedException;

            // Act
            jsonString = JsonConvert.SerializeObject(exception);
            deserializedException = JsonConvert.DeserializeObject<StatusNotFoundException>(jsonString!) !;

            // Assert
            deserializedException?.Message.Should().Be(customMessage);
            deserializedException?.InnerException?.Message.Should().Be(innerException.Message);
        }
    }
}
