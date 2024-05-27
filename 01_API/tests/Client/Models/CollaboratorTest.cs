// <copyright file="CollaboratorTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Client.Tests;

public class CollaboratorTest
{
    [Fact]
    public void Constructor_ShouldInitializeProperties()
    {
        // Arrange
        int expectedId = 1;
        string expectedEmail = "test@example.com";
        string expectedFirstName = "John";
        string expectedLastName = "Doe";

        // Act
        var collaborator = new Collaborator(expectedId, expectedEmail, expectedFirstName, expectedLastName);

        // Assert
        collaborator.Id.Should().Be(expectedId);
        collaborator.Email.Should().Be(expectedEmail);
        collaborator.FirstName.Should().Be(expectedFirstName);
        collaborator.LastName.Should().Be(expectedLastName);
    }
}
