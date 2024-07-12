// <copyright file="FormioApiExceptionTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Formio.Client.Tests;

public class FormioApiExceptionTest
{
    [Fact]
    public void DefaultConstructor_ShouldInstantiateWithDefaultMessage()
    {
        // Act
        var exception = new FormioApiException();

        // Assert
        exception.Message.Should().Be("Exception of type 'KPMG.Pulse.Back.Accounting.Mandate.Formio.Client.FormioApiException' was thrown.");
    }

    [Fact]
    public void ConstructorWithMessage_ShouldSetMessage()
    {
        // Arrange
        var customMessage = "Custom error message";

        // Act
        var exception = new FormioApiException(customMessage);

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
        var exception = new FormioApiException(customMessage, innerException);

        // Assert
        exception.Message.Should().Be(customMessage);
        exception.InnerException.Should().Be(innerException);
    }
}
