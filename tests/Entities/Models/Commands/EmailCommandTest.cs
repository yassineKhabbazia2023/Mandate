// <copyright file="EmailCommandTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Tests
{
    public class EmailCommandTest
    {
        [Fact]
        public void Constructor_WithNonNullAttachments_SetsAllPropertiesCorrectly()
        {
            // Arrange
            var subject = "Test Subject";
            var templateName = "TemplateName";
            var from = "from@example.com";
            var to = "to@example.com";
            var cc = new List<string> { "cc1@example.com", "cc2@example.com" };
            var attachments = new List<AttachmentFileCommand>
            {
                new AttachmentFileCommand("file1.txt", "content1"),
                new AttachmentFileCommand("file2.txt", "content2"),
            };
            var variables = new Dictionary<string, string> { { "key", "value" } };

            // Act
            var emailCommand = new EmailCommand(subject, templateName, from, to, cc, attachments, variables);

            // Assert
            emailCommand.Subject.Should().Be(subject);
            emailCommand.TemplateName.Should().Be(templateName);
            emailCommand.From.Should().Be(from);
            emailCommand.To.Should().Be(to);
            emailCommand.Cc.Should().BeEquivalentTo(cc);
            emailCommand.Attachements.Should().BeEquivalentTo(attachments);
            emailCommand.Variables.Should().BeEquivalentTo(variables);
        }

        [Fact]
        public void Constructor_WithNullAttachments_InitializesEmptyAttachmentsList()
        {
            // Arrange
            var subject = "Test Subject";
            var templateName = "TemplateName";
            var from = "from@example.com";
            var to = "to@example.com";
            var cc = new List<string> { "cc1@example.com", "cc2@example.com" };
            var variables = new Dictionary<string, string> { { "key", "value" } };

            // Act
            var emailCommand = new EmailCommand(subject, templateName, from, to, cc, null!, variables);

            // Assert
            emailCommand.Attachements.Should().BeEmpty(because: "Attachments should initialize to an empty list when null is passed in the constructor.");
        }
    }
}
