// <copyright file="NotificationProviderTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Notifications.Tests
{
    using Kpmg.Constellation.Notifications.V2.Client;

    public class NotificationProviderTest
    {
        private readonly Mock<INotificationsClientFactory> mockFactory;
        private readonly Mock<IAuthenticationContext> mockAuthContext;
        private readonly Mock<INotificationsClient> mockClient;
        private readonly NotificationProvider provider;

        public NotificationProviderTest()
        {
            this.mockFactory = new Mock<INotificationsClientFactory>();
            this.mockAuthContext = new Mock<IAuthenticationContext>();
            this.mockClient = new Mock<INotificationsClient>();

            // Setup the factory to return the mock client
            this.mockFactory.Setup(f => f.Create(It.IsAny<string>())).Returns(this.mockClient.Object);

            // Initialize the provider with the mocked dependencies
            this.provider = new NotificationProvider(this.mockFactory.Object, this.mockAuthContext.Object);
        }

        [Fact]
        public async Task SendEmailAsync_CallsClientSendEmail()
        {
            // Arrange
            var emailRequest = new EmailRequest(); // Assuming EmailRequest is a valid class
            var token = "test-token";
            this.mockAuthContext.SetupGet(ctx => ctx.BearerToken).Returns(token);

            // Act
            await this.provider.SendEmailAsync(emailRequest);

            // Assert
            this.mockFactory.Verify(f => f.Create(token), Times.Once);
            this.mockClient.Verify(c => c.SendEmailAsync(emailRequest), Times.Once);
        }
    }
}
