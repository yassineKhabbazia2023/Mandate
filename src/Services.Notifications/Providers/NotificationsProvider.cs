// <copyright file="NotificationsProvider.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Notifications
{
    using Kpmg.Constellation.Notifications.V2.Client;
    using KPMG.Pulse.Back.Accounting.Mandate.Portal;

    public class NotificationsProvider : INotificationsProvider
    {
        private readonly INotificationsClientFactory factory;
        private readonly IAuthenticationContext authenticationContext;

        public NotificationsProvider(INotificationsClientFactory factory, IAuthenticationContext authenticationContext)
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
