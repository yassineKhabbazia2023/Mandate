// <copyright file="NotificationsProviderTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Notifications.Tests
{
    using AutoFixture;
    using global::Notifications.Commons.WebApi.QueryParams;
    using KPMG.Pulse.Back.Accounting.Mandate.Application;
    using KPMG.Pulse.Back.Accounting.Mandate.Portal;
    using Microsoft.Extensions.Options;

    public class NotificationsProviderTest
    {
        private readonly Mock<IAuthenticationContext> mockAuthContext;
        private readonly NotificationsProvider provider;

        public NotificationsProviderTest()
        {
            this.mockAuthContext = new Mock<IAuthenticationContext>();

            var notifOptions = Options.Create(new NotificationOptions() { BaseUrl = "http://notifications" });
            // Initialize the provider with the mocked dependencies

            this.provider = new NotificationsProvider(this.mockAuthContext.Object, notifOptions);
        }

        [Fact]
        public async Task SendEmailAsync_ShouldReturnFuncTask()
        {
            // Arrange
            var emailRequest = new EmailRequest(); // Assuming EmailRequest is a valid class
            var token = "test-token";
            this.mockAuthContext.SetupGet(ctx => ctx.BearerToken).Returns(token);

            // Act
            var action = async () => await this.provider.SendEmailAsync(emailRequest);

            // Assert
            action.Should().BeOfType<Func<Task>>();
        }


        [Fact]
        public void TrailUrl_ShouldThrowNull_IfUrlNull()
        {
            string url = string.Empty;
            string api = "/api/notif";
            var action = () => this.provider.TrailUrl(url, api);
            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void TrailUrl_ShouldThrowNull_ifApiIsNull()
        {
            string url = "https://hakounamata.com";
            string api = string.Empty;
            var action = () => this.provider.TrailUrl(url, api);
            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void TrailUrl_ShouldReturn_TrailedString()
        {
            string url = "https://www.hakounamatata.com/api/";
            string api = "/notifications/timon";
            string result = this.provider.TrailUrl(url, api);
            result.Should().Be("https://www.hakounamatata.com/api/notifications/timon");
        }
    }
}
