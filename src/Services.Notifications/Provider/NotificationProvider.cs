// <copyright file="NotificationProvider.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Notifications
{
    using Kpmg.Constellation.Notifications.V2.Client;

    public class NotificationProvider : INotificationProvider
    {
        private readonly INotificationsClientFactory factory;
        private readonly IAuthenticationContext authenticationContext;

        public NotificationProvider(INotificationsClientFactory factory, IAuthenticationContext authenticationContext)
        {
            this.factory = factory;
            this.authenticationContext = authenticationContext;
        }

        public async Task SendEmailAsync(EmailRequest emailRequest)
        {
            var client = this.factory.Create(this.authenticationContext.BearerToken);

            await client.SendEmailAsync(emailRequest);
        }
    }
}
