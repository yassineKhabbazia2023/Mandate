// <copyright file="NotificationsExtensionsTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Adapters.Tests
{
    public class NotificationsExtensionsTest
    {
        [Fact]
        public void ToEmailRequest_ShouldConvertEmailCommandToEmailRequestCorrectly()
        {
            // Arrange
            var emailCommand = new EmailCommand(
                subject: "Test Subject",
                templateName: "Test Template",
                from: "from@example.com",
                to: "to@example.com",
                cc: new List<string> { "cc1@example.com", "cc2@example.com" },
                attachements: new List<AttachmentFileCommand>
                {
                    new AttachmentFileCommand("file1.txt", "content1"),
                    new AttachmentFileCommand("file2.txt", "content2"),
                },
                variables: new Dictionary<string, string> { { "key", "value" } });

            // Act
            var emailRequest = emailCommand.ToEmailRequest();

            // Assert
            emailRequest.Subject.Should().Be("Test Subject");
            emailRequest.TemplateName.Should().Be("Test Template");
            emailRequest.From.Should().Be("from@example.com");
            emailRequest.To.Should().Contain("to@example.com");
            emailRequest.Cc.Should().BeEquivalentTo(new List<string> { "cc1@example.com", "cc2@example.com" });
            emailRequest.Attachements.Should().HaveCount(2);
            emailRequest.Attachements.Select(a => a.FileName).Should().Contain(new List<string> { "file1.txt", "file2.txt" });
            emailRequest.Variables.Should().Contain(new Dictionary<string, string> { { "key", "value" } });
        }

        [Fact]
        public void ToAttachmentFile_ShouldConvertAttachmentFileCommandToAttachmentFileCorrectly()
        {
            // Arrange
            var attachmentFileCommand = new AttachmentFileCommand("file.txt", "content");

            // Act
            var attachmentFile = attachmentFileCommand.ToAttachmentFile();

            // Assert
            attachmentFile.FileName.Should().Be("file.txt");
            attachmentFile.Content.Should().Be("content");
        }
    }
}
