// <copyright file="NotificationsProvider.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Notifications
{
    using global::Notifications.Commons.WebApi.QueryParams;
    using KPMG.Pulse.Back.Accounting.Mandate.Application;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;
    using System.Net.Http;
    using System.Text;

    public class NotificationsProvider : INotificationsProvider
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<NotificationsProvider> _logger;
        private readonly IOptions<NotificationOptions> _options;

        public NotificationsProvider(IHttpClientFactory httpClientFactory, ILogger<NotificationsProvider> logger, IOptions<NotificationOptions> options)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public async Task SendEmailAsync(EmailRequest emailRequest)
        {
            if (emailRequest == null) { throw new ArgumentNullException(nameof(emailRequest)); }

            using var http = _httpClientFactory.CreateClient();
            http.BaseAddress = new Uri(_options.Value.BaseUrl);
            string body = JsonConvert.SerializeObject(emailRequest);
            _logger.LogInformation(body);

            HttpContent content = new StringContent(body, encoding: Encoding.UTF8, "application/json");
            var response = await http.PostAsync("notifications/SendEmail", content);
            response.EnsureSuccessStatusCode();
        }
    }
}
