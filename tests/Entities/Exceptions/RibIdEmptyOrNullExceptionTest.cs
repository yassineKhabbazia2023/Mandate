// <copyright file="RibIdEmptyOrNullExceptionTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Tests.Exceptions
{
    using Newtonsoft.Json;

    public class RibIdEmptyOrNullExceptionTest
    {
        [Fact]
        public void DefaultConstructor_ShouldInstantiateWithDefaultMessage()
        {
            // Act
            var exception = new RibIdEmptyOrNullException();

            // Assert
            exception.Message.Should().Be("Exception of type 'KPMG.Pulse.Back.Accounting.Mandate.RibIdEmptyOrNullException' was thrown.");
        }

        [Fact]
        public void ConstructorWithMessage_ShouldSetMessage()
        {
            // Arrange
            var customMessage = "Custom error message";

            // Act
            var exception = new RibIdEmptyOrNullException(customMessage);

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
            var exception = new RibIdEmptyOrNullException(customMessage, innerException);

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
            var exception = new RibIdEmptyOrNullException(customMessage, innerException);

            string jsonString;
            RibIdEmptyOrNullException deserializedException;

            // Act
            jsonString = JsonConvert.SerializeObject(exception);
            deserializedException = JsonConvert.DeserializeObject<RibIdEmptyOrNullException>(jsonString!);

            // Assert
            deserializedException?.Message.Should().Be(customMessage);
            deserializedException?.InnerException?.Message.Should().Be(innerException.Message);
        }
    }
}
