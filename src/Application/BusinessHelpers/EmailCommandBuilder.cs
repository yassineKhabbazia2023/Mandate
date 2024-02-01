// <copyright file="EmailCommandBuilder.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Application
{
    public static class EmailCommandBuilder
    {
        public static EmailCommand CreateMandateCancellationEmail(Collection collection, MandateEmailOptions options)
        {
            var emailData = ExtractEmailData(collection);

            string emailBody = $@"
                Bonjour,<br><br>
                Désactivation de collecte<br><br>
                La collecte a été désactivée pour le compte suivant :<br><br>

               {GenerateEmailListContent(emailData)}


                L'équipe myPulse<br><br>
                Ce message est envoyé automatiquement, merci de ne pas répondre.
               ";

            return GenerateEmailCommand(emailData, emailBody, options.MandateCancellationSubject, options, null!, EmailType.MandateCancellation);
        }

        public static EmailCommand CreateSignedMandateUploadedEmail(Collection collection, MandateEmailOptions options, string fileContent, string fileName)
        {
            var emailData = ExtractEmailData(collection);

            string emailBody = $@"
                Bonjour,<br><br>
                Nouvelle demande de mandat Non Dématérialisé<br><br>
                Une nouvelle demande de mandat a été soumise pour une banque non dématérialisée pour le compte suivant :

               {GenerateEmailListContent(emailData)}

                Vous trouverez ci-joint le PDF du mandat signé.<br><br>
                L'équipe myPulse<br><br>
                Ce message est envoyé automatiquement, merci de ne pas répondre.";

            var attachments = new List<AttachmentFileCommand>()
            {
                new AttachmentFileCommand(fileName: fileName, content: fileContent),
            };

            return GenerateEmailCommand(emailData, emailBody, options!.MandateUploadedSubject, options, attachments, EmailType.MandateUploaded);
        }

        private static EmailData ExtractEmailData(Collection collection)
        {
            SignatoryDetails signatoryDetails = new SignatoryDetails(
                collaboratorEmail: collection.GetSignatoryEmail(),
                signatoryName: collection.GetSignatoryFullName(),
                siretNumber: collection.GetSiretNumber());

            Bban bban = new Bban(
                 bankCode: collection.GetBankCode(),
                 branchCode: collection.GetBranchCode(),
                 accountNumber: collection.GetAccountNumber(),
                 checkDigits: collection.GetCheckDigits(),
                 bbanServicesProviderId: null!,
                 bank: new Bank(
                     null!,
                     name: collection.GetBankName(),
                     group: null!,
                     ebicsCardId: null!,
                     null!));

            return new EmailData(
                signatoryDetails: signatoryDetails,
                bban: bban,
                ibs: collection.GetErpId());
        }

        private static string GenerateEmailListContent(EmailData data)
        {
            return $@"
            <ul>
                <li>Collaborateur: {data.CollaboratorEmail}</li>
                <li>Raison sociale du client: {data.Ibs}</li>
                <li>Siret : {data.SiretNumber}</li>
                <li>RIB:
                  <ul>
                    <li>Titulaire: {data.SignatoryName}</li>
                    <li>Libellé: {data.BankName}</li>
                    <li>Code établissement: {data.BankCode}</li>
                    <li>Guichet: {data.BranchCode}</li>
                    <li>Numéro de compte: {data.AccountNumber}</li>
                    <li>Clé: {data.CheckDigits}</li>
                  </ul>
                </li>
            </ul>";
        }

        private static EmailCommand GenerateEmailCommand(
            EmailData emailData,
            string? emailBody,
            string? subjectPrefix,
            MandateEmailOptions options,
            List<AttachmentFileCommand> attachments,
            EmailType emailType)
        {
            var (templateName, from, to, cc) = GetEmailProperties(options, emailType);

            return new EmailCommand(
                subject: subjectPrefix + $"{emailData.Ibs} - {emailData.BranchCode} {emailData.AccountNumber} {emailData.CheckDigits}",
                templateName: templateName,
                from: from,
                to: to,
                cc: cc,
                attachements: attachments ?? new List<AttachmentFileCommand>(),
                variables: new Dictionary<string, string> { { "body", emailBody! } });
        }

        private static (string templateName, string from, string to, List<string> cc) GetEmailProperties(MandateEmailOptions options, EmailType emailType)
        {
            return emailType switch
            {
                EmailType.MandateCancellation =>
                    (options.MandateCancellationTemplateName, options.MandateCancellationFromEmail, options.MandateCancellationToEmail, options.MandateCancellationCcEmails),
                EmailType.MandateUploaded =>
                    (options.MandateUploadedTemplateName, options.MandateUploadedFromEmail, options.MandateUploadedToEmail, options.MandateUploadedCcEmails),
                _ => default,
            };
        }
    }
}
