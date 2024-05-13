// <copyright file="StatusNotFoundExceptionTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Sql.Tests.Exceptions;

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
    public void FromId_WithGuid_ShouldCreateExceptionWithCorrectMessage()
    {
        // Arrange
        var guid = Guid.NewGuid();

        // Act
        var exception = StatusNotFoundException.FromId(guid);

        // Assert
        exception.Message.Should().Be($"La collection avec l\'id '{guid}' n'a pas de status en cours");
    }

    [Fact]
    public void FromId_WithString_ShouldCreateExceptionWithCorrectMessage()
    {
        // Arrange
        var id = "5";

        // Act
        var exception = StatusNotFoundException.FromId(id);

        // Assert
        exception.Message.Should().Be($"Le status code jdc '5' n'a pas été trouvé dans la collection");
    }
}
