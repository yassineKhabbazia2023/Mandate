// <copyright file="FormIoApiExceptionTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Formio.Client.Tests
{
    using Newtonsoft.Json;

    public class FormIoApiExceptionTest
    {
        [Fact]
        public void DefaultConstructor_ShouldInstantiateWithDefaultMessage()
        {
            // Act
            var exception = new FormIoApiException();

            // Assert
            exception.Message.Should().Be("Exception of type 'KPMG.Pulse.Back.Accounting.Mandate.Formio.Client.FormIoApiException' was thrown.");
        }

        [Fact]
        public void ConstructorWithMessage_ShouldSetMessage()
        {
            // Arrange
            var customMessage = "Custom error message";

            // Act
            var exception = new FormIoApiException(customMessage);

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
            var exception = new FormIoApiException(customMessage, innerException);

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
            var exception = new FormIoApiException(customMessage, innerException);

            string jsonString;

            // Act
            jsonString = JsonConvert.SerializeObject(exception);
            FormIoApiException? deserializedException = JsonConvert.DeserializeObject<FormIoApiException>(jsonString);

            // Assert
            deserializedException?.Message.Should().Be(customMessage);
            deserializedException?.InnerException?.Message.Should().Be(innerException.Message);
        }
    }
}
