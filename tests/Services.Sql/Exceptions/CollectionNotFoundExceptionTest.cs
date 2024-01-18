// <copyright file="CollectionNotFoundExceptionTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Sql.Tests
{
    using Newtonsoft.Json;

    public class CollectionNotFoundExceptionTest
    {
        [Fact]
        public void DefaultConstructor_ShouldInstantiateWithDefaultMessage()
        {
            // Act
            var exception = new CollectionNotFoundException();

            // Assert
            exception.Message.Should().Be("Exception of type 'KPMG.Pulse.Back.Accounting.Mandate.Sql.CollectionNotFoundException' was thrown.");
        }

        [Fact]
        public void ConstructorWithMessage_ShouldSetMessage()
        {
            // Arrange
            var customMessage = "Custom error message";

            // Act
            var exception = new CollectionNotFoundException(customMessage);

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
            var exception = new CollectionNotFoundException(customMessage, innerException);

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
            var exception = new CollectionNotFoundException(customMessage, innerException);

            string jsonString;
            CollectionNotFoundException deserializedException;

            // Act
            jsonString = JsonConvert.SerializeObject(exception);
            deserializedException = JsonConvert.DeserializeObject<CollectionNotFoundException>(jsonString!) !;

            // Assert
            deserializedException?.Message.Should().Be(customMessage);
            deserializedException?.InnerException?.Message.Should().Be(innerException.Message);
        }
    }
}
