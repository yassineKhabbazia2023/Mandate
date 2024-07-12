// <copyright file="INotificationsService.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate
{
    public interface INotificationsService
    {
        Task SendEmailAsync(EmailCommand emailCommand);
    }
}
