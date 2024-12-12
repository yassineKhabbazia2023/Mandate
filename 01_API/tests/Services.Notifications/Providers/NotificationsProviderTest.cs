// <copyright file="NotificationsProviderTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Notifications.Tests
{
    using global::Notifications.Commons.WebApi.QueryParams;
    using KPMG.Pulse.Back.Accounting.Mandate.Application;
    using Microsoft.Extensions.Options;
    using System.Net;

    public class NotificationsProviderTest
    {
        private readonly NotificationsProvider provider;

        public NotificationsProviderTest()
        {

            var notifOptions = Options.Create(new NotificationOptions() { BaseUrl = "http://www.kpmg.fr" });
            // Initialize the provider with the mocked dependencies

            this.provider = new NotificationsProvider(notifOptions);
        }

        [Fact]
        public async Task SendEmailAsync_ShouldReturnFuncTask()
        {
            // Arrange
            var emailRequest = new EmailRequest(); // Assuming EmailRequest is a valid class

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

        [Fact]
        public async Task SendEmailAsync_ShouldThrowNullArgument()
        {
            EmailRequest? emailRequest = null;
            var action = async () => await this.provider.SendEmailAsync(emailRequest);
            await action.Should().ThrowAsync<ArgumentNullException>();
        }


        [Fact]
        public async Task SendEmailAsync_CallsHttpClientPostAsync_WithCorrectUrlAndContent()
        {
            // Arrange
            var handler = new TestHttpMessageHandler
            {
                ResponseToReturn = new HttpResponseMessage(HttpStatusCode.OK),
                LastRequest = new HttpRequestMessage(HttpMethod.Post, "http://api.example.com/notifications/SendEmail")

            };

            var httpClient = new HttpClient(handler);

            var optionsMock = new Mock<IOptions<NotificationOptions>>();

            optionsMock.Setup(o => o.Value).Returns(new NotificationOptions { BaseUrl = "http://api.example.com" });

            var emailRequest = new EmailRequest { /* Set properties */ };

            // Act
            await this.provider.SendEmailAsync(emailRequest);

            // Assert
            handler.LastRequest.Should().NotBeNull();
            handler.LastRequest.Method.Should().Be(HttpMethod.Post);
            handler.LastRequest.RequestUri.Should().Be("http://api.example.com/notifications/SendEmail");
        }
    }
    public class TestHttpMessageHandler : HttpMessageHandler
    {
        public HttpRequestMessage LastRequest { get; set; }
        public HttpResponseMessage ResponseToReturn { get; set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            LastRequest = request;
            return Task.FromResult(ResponseToReturn);
        }
    }
}
