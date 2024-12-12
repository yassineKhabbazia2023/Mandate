// <copyright file="NotificationsExtensions.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

using Notifications.Commons.WebApi;
using Notifications.Commons.WebApi.QueryParams;

namespace KPMG.Pulse.Back.Accounting.Mandate.Adapters
{

    public static class NotificationsExtensions
    {
        public static EmailRequest ToEmailRequest(this EmailCommand emailCommand)
        {
            return new EmailRequest
            {
                Subject = emailCommand!.Subject,
                TemplateName = emailCommand!.TemplateName,
                From = emailCommand!.From,
                To = new List<string>() { emailCommand.To },
                Cc = emailCommand!.Cc,
                Attachements = emailCommand!.Attachements.Select(a => a.ToAttachmentFile()).ToList(),
                Variables = emailCommand!.Variables,
            };
        }

        public static AttachmentFileDto ToAttachmentFile(this AttachmentFileCommand attachmentFileCommand)
        {
            return new AttachmentFileDto
            {
                FileName = attachmentFileCommand!.FileName,
                Content = attachmentFileCommand!.Content,
                Type = MailAttachmentType.Pdf,
            };
        }
    }
}
