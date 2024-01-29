// <copyright file="EmailCommandBuilder.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Application
{
    public static class EmailCommandBuilder
    {
        public static EmailCommand CreateMandateCancellationEmail(Collection collection, MandateEmailOptions options)
        {
            var (collaboratorEmail, ibs, siretNumber, signatoryName, bankName, bankCode, branchCode, accountNumber, checkDigits) = ExtractEmailData(collection);

            string emailBody = $@"
                Bonjour,<br><br>
                Désactivation de collecte<br><br>
                La collecte a été désactivée pour le compte suivant :<br><br>

               {GenerateEmailListContent(collaboratorEmail, ibs, siretNumber, signatoryName, bankName, bankCode, branchCode, accountNumber, checkDigits)}


                L'équipe myPulse<br><br>
                Ce message est envoyé automatiquement, merci de ne pas répondre.
               ";

            return GenerateEmailCommand(ibs, branchCode, accountNumber, checkDigits, emailBody, options.MandateCancellationSubject, options, null!, EmailType.MandateCancellation);
        }

        public static EmailCommand CreateSignedMandateUploadedEmail(Collection collection, MandateEmailOptions options, string fileContent, string fileName)
        {
            var (collaboratorEmail, ibs, siretNumber, signatoryName, bankName, bankCode, branchCode, accountNumber, checkDigits) = ExtractEmailData(collection);

            string emailBody = $@"
                Bonjour,<br><br>
                Nouvelle demande de mandat Non Dématérialisé<br><br>
                Une nouvelle demande de mandat a été soumise pour une banque non dématérialisée pour le compte suivant :

               {GenerateEmailListContent(collaboratorEmail, ibs, siretNumber, signatoryName, bankName, bankCode, branchCode, accountNumber, checkDigits)}

                Vous trouverez ci-joint le PDF du mandat signé.<br><br>
                L'équipe myPulse<br><br>
                Ce message est envoyé automatiquement, merci de ne pas répondre.";

            var attachments = new List<AttachmentFileCommand>()
            {
                new AttachmentFileCommand(fileName: fileName, content: fileContent),
            };

            return GenerateEmailCommand(ibs, branchCode, accountNumber, checkDigits, emailBody, options.MandateUploadedSubject, options, attachments, EmailType.MandateUploaded);
        }

        private static (string? collaboratorEmail, string? ibs, string? siretNumber, string? signatoryName, string? bankName, string? bankCode, string? branchCode, string? accountNumber, string? checkDigits) ExtractEmailData(Collection collection)
        {
            return (
                collection!.Company?.Signatory?.Email,
                collection!.Company?.ErpId,
                collection!.Company?.SiretNumber,
                $"{collection!.Company?.Signatory?.LastName} {collection!.Company?.Signatory?.FirstName}",
                collection!.Bban?.Bank!.Name,
                collection!.Bban?.BankCode,
                collection!.Bban?.BranchCode,
                collection!.Bban?.AccountNumber,
                collection!.Bban?.CheckDigits);
        }

        private static string GenerateEmailListContent(string? collaboratorEmail, string? ibs, string? siretNumber, string? signatoryName, string? bankName, string? bankCode, string? branchCode, string? accountNumber, string? checkDigits)
        {
            return $@"
            <ul>
                <li>Collaborateur: {collaboratorEmail}</li>
                <li>Raison sociale du client: {ibs}</li>
                <li>Siret : {siretNumber}</li>
                <li>RIB:
                  <ul>
                    <li>Titulaire: {signatoryName}</li>
                    <li>Libellé: {bankName}</li>
                    <li>Code établissement: {bankCode}</li>
                    <li>Guichet: {branchCode}</li>
                    <li>Numéro de compte: {accountNumber}</li>
                    <li>Clé: {checkDigits}</li>
                  </ul>
                </li>
            </ul>";
        }

        private static EmailCommand GenerateEmailCommand(
            string? ibs,
            string? branchCode,
            string? accountNumber,
            string? checkDigits,
            string? emailBody,
            string? subjectPrefix,
            MandateEmailOptions options,
            List<AttachmentFileCommand> attachments,
            EmailType emailType)
        {
            var (templateName, from, to, cc) = GetEmailProperties(options, emailType);

            return new EmailCommand(
                subject: subjectPrefix + $"{ibs} - {branchCode} {accountNumber} {checkDigits}",
                templateName: templateName,
                from: from,
                to: to,
                cc: cc,
                attachements: attachments ?? new List<AttachmentFileCommand>(),
                variables: new Dictionary<string, string> { { "body", emailBody } }
            );
        }

        private static (string templateName, string from, string to, List<string> cc) GetEmailProperties(MandateEmailOptions options, EmailType emailType)
        {
            return emailType switch
            {
                EmailType.MandateCancellation =>
                    (options.MandateCancellationTemplateName, options.MandateCancellationFromEmail, options.MandateCancellationToEmail, options.MandateCancellationCcEmails),
                EmailType.MandateUploaded =>
                    (options.MandateUploadedTemplateName, options.MandateUploadedFromEmail, options.MandateUploadedToEmail, options.MandateUploadedCcEmails),
            };
        }
    }
}
