// <copyright file="MandateCreationMessageTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>
namespace KPMG.Pulse.Back.Accounting.Mandate.Tests.Models
{
    public class MandateCreationMessageTest
    {
        [Fact]
        public void Constructor_Should_Set_Properties_Correctly()
        {
            // Arrange
            var collectionId = Guid.NewGuid();
            var messageContent = "This is a test message.";

            // Act
            var mandateCreationLogMessage = new MandateCreationLogMessage(collectionId, messageContent);

            // Assert
            mandateCreationLogMessage.CollectionId.Should().Be(collectionId);
            mandateCreationLogMessage.MessageContent.Should().Be(messageContent);
            mandateCreationLogMessage.CreatedDate.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public void Should_Have_All_Properties()
        {
            // Arrange
            var entity = new MandateCreationLogMessage(Guid.NewGuid(), "Sample message");

            // Act & Assert
            entity.GetType().GetProperties().Length.Should().Be(4);

            // Test all properties individually
            entity.CollectionId.Should().NotBeEmpty();
            entity.MessageContent.Should().NotBeNullOrEmpty();
            entity.CreatedDate.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(1));
        }
    }
}
