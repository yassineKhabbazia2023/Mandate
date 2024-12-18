// <copyright file="NotificationsProviderTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Notifications.Tests
{
    using Castle.Core.Logging;
    using global::Notifications.Commons.WebApi.QueryParams;
    using KPMG.Pulse.Back.Accounting.Mandate.Application;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;
    using Microsoft.Extensions.Options;
    using System.Net;

    public class NotificationsProviderTest
    {
        private readonly NotificationsProvider provider;

        public NotificationsProviderTest()
        {

            var notifOptions = Options.Create(new NotificationOptions() { BaseUrl = "http://www.kpmg.fr" });
            // Initialize the provider with the mocked dependencies

            IHttpClientFactory httpClientFactory = new Mock<IHttpClientFactory>().Object;
            ILogger<NotificationsProvider> logger = new NullLogger<NotificationsProvider>();

            this.provider = new NotificationsProvider(httpClientFactory, logger, notifOptions);
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
        public async Task SendEmailAsync_ShouldThrowNullArgument()
        {
            EmailRequest? emailRequest = null;
            var action = async () => await this.provider.SendEmailAsync(emailRequest);
            await action.Should().ThrowAsync<ArgumentNullException>();
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
