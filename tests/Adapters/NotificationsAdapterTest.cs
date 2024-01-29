// <copyright file="NotificationsAdapterTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Adapters.Tests
{
    using Kpmg.Constellation.Notifications.V2.Client;
    using KPMG.Pulse.Back.Accounting.Mandate.Notifications;

    public class NotificationsAdapterTest
    {
        [Fact]
        public async Task SendEmailAsync_ShouldCallProviderWithCorrectEmailRequest()
        {
            // Arrange
            var mockNotificationProvider = new Mock<INotificationProvider>();
            var adapter = new NotificationsAdapter(mockNotificationProvider.Object);

            var emailCommand = new EmailCommand(
                subject: "Test Subject",
                templateName: "Test Template",
                from: "from@example.com",
                to: "to@example.com",
                cc: new List<string> { "cc1@example.com", "cc2@example.com" },
                attachements: new List<AttachmentFileCommand>(),
                variables: new Dictionary<string, string> { { "key", "value" } }
            );

            // Act
            await adapter.SendEmailAsync(emailCommand);

            // Assert
            mockNotificationProvider.Verify(
                p => p.SendEmailAsync(
                    It.Is<EmailRequest>(
                        req => req.Subject == emailCommand.Subject &&
                               req.TemplateName == emailCommand.TemplateName &&
                               req.From == emailCommand.From &&
                               req.To == emailCommand.To &&
                               req.Cc.Count == emailCommand.Cc.Count && !req.Cc.Except(emailCommand.Cc).Any() &&
                               req.Variables["key"] == "value")),
                Times.Once
            );
        }
    }
}
