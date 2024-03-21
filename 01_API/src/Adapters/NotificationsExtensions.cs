// <copyright file="NotificationsExtensions.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Adapters
{
    using Kpmg.Constellation.Notifications.V2.Client;

    public static class NotificationsExtensions
    {
        public static EmailRequest ToEmailRequest(this EmailCommand emailCommand)
        {
            return new EmailRequest
            {
                Subject = emailCommand!.Subject,
                TemplateName = emailCommand!.TemplateName,
                From = emailCommand!.From,
                To = emailCommand!.To,
                Cc = emailCommand!.Cc,
                Attachements = emailCommand!.Attachements.Select(a => a.ToAttachmentFile()).ToList(),
                Variables = emailCommand!.Variables,
            };
        }

        public static AttachmentFile ToAttachmentFile(this AttachmentFileCommand attachmentFileCommand)
        {
            return new AttachmentFile
            {
                FileName = attachmentFileCommand!.FileName,
                Content = attachmentFileCommand!.Content,
            };
        }
    }
}
