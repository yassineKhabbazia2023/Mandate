// <copyright file="EmailCommandBuilder.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Application
{
    public static class EmailCommandBuilder
    {
        public static EmailCommand CreateMandateCancellationEmail(Collection collection, MandateEmailOptions options)
        {
            var emailData = EmailData.FromCollection(collection);
            return GenerateEmailCommand(emailData, options.MandateCancellationSubject, options, new List<AttachmentFileCommand>(), EmailType.MandateCancellation);
        }

        public static EmailCommand CreateSignedMandateUploadedEmail(Collection collection, MandateEmailOptions options, string fileContent, string fileName)
        {
            var emailData = EmailData.FromCollection(collection);
            List<AttachmentFileCommand> attachments =
            [
                new(fileName, fileContent)
            ];

            return GenerateEmailCommand(emailData, options!.MandateUploadedSubject, options, attachments, EmailType.MandateUploaded);
        }

        private static EmailCommand GenerateEmailCommand(
            EmailData emailData,
            string subjectPrefix,
            MandateEmailOptions options,
            List<AttachmentFileCommand> attachments,
            EmailType emailType)
        {
            var (templateName, from, to, cc) = GetEmailProperties(options, emailType);

            return new EmailCommand(
                subject: subjectPrefix +
                         $"{emailData.CompanyName} - {emailData.BranchCode} {emailData.AccountNumber} {emailData.CheckDigits}",
                templateName: templateName,
                from: from,
                to: to,
                cc: cc,
                attachements: attachments,
                variables: emailData);
        }

        private static (string templateName, string from, string to, List<string> cc) GetEmailProperties(
            MandateEmailOptions options, EmailType emailType)
        {
            return emailType switch
            {
                EmailType.MandateCancellation =>
                    (options.MandateCancellationTemplateName, options.MandateCancellationFromEmail,
                        options.MandateCancellationToEmail, options.MandateCancellationCcEmails),
                EmailType.MandateUploaded =>
                    (options.MandateUploadedTemplateName, options.MandateUploadedFromEmail,
                        options.MandateUploadedToEmail, options.MandateUploadedCcEmails),
                _ => default,
            };
        }
    }
}