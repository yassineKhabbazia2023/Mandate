// <copyright file="NotificationsProvider.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Notifications
{
    using global::Notifications.Commons.WebApi.QueryParams;
    using KPMG.Pulse.Back.Accounting.Mandate.Application;
    using KPMG.Pulse.Back.Accounting.Mandate.Portal;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;
    using System.Text;

    public class NotificationsProvider : INotificationsProvider
    {
        private readonly IAuthenticationContext authenticationContext;
        private readonly IOptions<NotificationOptions> options;

        public NotificationsProvider(IAuthenticationContext authenticationContext, IOptions<NotificationOptions> options)
        {
            this.authenticationContext = authenticationContext;
            this.options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public async Task SendEmailAsync(EmailRequest emailRequest)
        {
            string token = this.authenticationContext.BearerToken;
            string url = this.TrailUrl(this.options.Value.BaseUrl, "/notifications/SendEmail");
            try
            {
                using var http = new HttpClient();
                string body = JsonConvert.SerializeObject(emailRequest);
                HttpContent content = new StringContent(body, encoding: Encoding.UTF8, "application/json");
                http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                var response = await http.PostAsync(url, content);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string TrailUrl(string url, string api)
        {
            if (string.IsNullOrEmpty(url)) { throw new ArgumentNullException(nameof(url)); }
            if (string.IsNullOrEmpty(api)) { throw new ArgumentNullException(nameof(api)); }
            if (url.EndsWith('/'))
            {
                url = url.Substring(0, url.Length - 1);
            }

            if (api.StartsWith('/'))
            {
                api = api.Substring(1, api.Length - 1);
            }

            return $"{url}/{api}";
        }
    }
}
