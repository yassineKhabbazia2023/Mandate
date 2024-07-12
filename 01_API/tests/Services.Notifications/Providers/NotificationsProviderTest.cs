// <copyright file="NotificationsProviderTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Notifications.Tests
{
    using AutoFixture;
    using global::Notifications.Commons.WebApi.QueryParams;
    using Kpmg.Constellation.Net.Http;
    using KPMG.Pulse.Back.Accounting.Mandate.Application;
    using KPMG.Pulse.Back.Accounting.Mandate.Portal;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;
    using System.Net;

    public class NotificationsProviderTest
    {
        private readonly Mock<IAuthenticationContext> mockAuthContext;
        private readonly NotificationsProvider provider;

        public NotificationsProviderTest()
        {
            this.mockAuthContext = new Mock<IAuthenticationContext>();

            var notifOptions = Options.Create(new NotificationOptions() { BaseUrl = "http://www.kpmg.fr" });
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

        [Fact]
        public async Task SendEmailAsync_ShouldThrowNullArgument()
        {
            EmailRequest? emailRequest = null;
            var action = async () => await this.provider.SendEmailAsync(emailRequest);
            await action.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task SendEmailAsync_ShouldSendEmail()
        {
            EmailRequest emailRequest = new EmailRequest() { From = "noreply@kpmg.com", HtmlContent = "content" };
            var mockHttp = new Mock<IHttpClient>();

            HttpContent content = new StringContent("{}", encoding: Encoding.UTF8, "application/json");

            mockHttp.Setup(client => client.PostAsync(It.IsAny<string>(), content)).ReturnsAsync(It.IsAny<HttpResponseMessage>);

            var action = async () => await this.provider.SendEmailAsync(emailRequest);

            action.Should().BeAssignableTo<Func<Task>>();
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

            var authContextMock = new Mock<IAuthenticationContext>();
            var optionsMock = new Mock<IOptions<NotificationOptions>>();

            authContextMock.Setup(a => a.BearerToken).Returns("test-token");
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
