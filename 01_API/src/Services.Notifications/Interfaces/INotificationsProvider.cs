// <copyright file="INotificationsProvider.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Notifications
{
    using Kpmg.Constellation.Notifications.V2.Client;

    public interface INotificationsProvider
    {
        Task SendEmailAsync(EmailRequest emailRequest);
    }
}
