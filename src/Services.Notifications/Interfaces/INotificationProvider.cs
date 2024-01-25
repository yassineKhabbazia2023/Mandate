// <copyright file="INotificationProvider.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Notifications
{
    using Kpmg.Constellation.Notifications.V2.Client;

    public interface INotificationProvider
    {
        Task SendEmailAsync(EmailRequest emailRequest);
    }
}
