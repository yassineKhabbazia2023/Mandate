// <copyright file="NotificationsAdapter.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Adapters
{
    using KPMG.Pulse.Back.Accounting.Mandate.Notifications;

    public class NotificationsAdapter : INotificationsService
    {
        private readonly INotificationsProvider notificationsProvider;

        public NotificationsAdapter(INotificationsProvider notificationsProvider)
        {
            this.notificationsProvider = notificationsProvider;
        }

        public async Task SendEmailAsync(EmailCommand emailCommand)
        {
            await this.notificationsProvider.SendEmailAsync(emailCommand.ToEmailRequest());
        }
    }
}
