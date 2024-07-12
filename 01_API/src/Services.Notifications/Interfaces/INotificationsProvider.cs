// <copyright file="INotificationsProvider.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

using Notifications.Commons.WebApi.QueryParams;

namespace KPMG.Pulse.Back.Accounting.Mandate.Notifications
{

    public interface INotificationsProvider
    {
        Task SendEmailAsync(EmailRequest emailRequest);
    }
}
