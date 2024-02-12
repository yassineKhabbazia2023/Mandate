// <copyright file="AddStatusCommandTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Sql.Tests
{
    public class AddStatusCommandTest
    {
        [Fact]
        public void AddStatusCommand_ShouldInitializePropertiesCorrectly()
        {
            // Arrange
            var collectionId = Guid.NewGuid();
            var statusCode = 200;
            var mandateFile = new byte[] { 1, 2, 3 };
            var createdBy = "testUser";

            // Act
            var command = new AddStatusCommand
            {
                CollectionId = collectionId,
                StatusCode = statusCode,
                MandateFile = mandateFile,
                CreatedBy = createdBy,
            };

            // Assert
            command.CollectionId.Should().Be(collectionId);
            command.StatusCode.Should().Be(statusCode);
            command.MandateFile.Should().Equal(mandateFile);
            command.CreatedBy.Should().Be(createdBy);
        }
    }
}
