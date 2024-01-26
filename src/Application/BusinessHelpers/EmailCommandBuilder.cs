// <copyright file="EmailCommandBuilder.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Application
{
    public static class EmailCommandBuilder
    {
        public static EmailCommand CreateMandateCancellationEmail(Guid collectionId)
        {
            string emailBody = $@"
                Bonjour,<br><br>
                Nous vous informons que le mandat pour la collection avec l'ID <b>{collectionId}</b> a été annulé.<br><br>
                Si vous avez des questions ou avez besoin d'assistance, n'hésitez pas à nous contacter.<br><br>
                Cordialement,<br>
                L’équipe KPMG Pulse";

            return new EmailCommand(
                subject: "Annulation de Mandat",
                templateName: "Generique-Mypulsev2",
                from: "contact@kpmg.fr",
                to: "",
                cc: new List<string>
                {
                    "smedini+mandat@kpmg.onmicrosoft.com",
                    "clementprati+mandat@kpmg.onmicrosoft.com",
                    "fmanadi+mandat@kpmg.onmicrosoft.com",
                    "katiasana+mandat@kpmg.onmicrosoft.com",
                },
                attachements: null!,
                variables: new Dictionary<string, string>
                {
                    { "title", $"Annulation de Mandat pour la Collection {collectionId}" },
                    { "body", emailBody },
                });
        }

        public static EmailCommand CreateSignedMandateUploadedEmail(Guid collectionId)
        {
            string emailBody = $@"
                Bonjour,<br><br>
                Nous sommes heureux de vous informer qu'un mandat signé pour la collection avec l'ID <b>{collectionId}</b> a été téléchargé avec succès.<br><br>
                Vous pouvez vérifier et gérer le mandat dans votre espace client.<br><br>
                Cordialement,<br>
                L’équipe KPMG Pulse";

            return new EmailCommand(
                subject: "Mandat Signé Téléchargé",
                templateName: "Generique-Mypulsev2",
                from: "contact@kpmg.fr",
                to: "",
                cc: new List<string>
                {
                    "smedini+mandat@kpmg.onmicrosoft.com",
                    "clementprati+mandat@kpmg.onmicrosoft.com",
                    "fmanadi+mandat@kpmg.onmicrosoft.com",
                    "katiasana+mandat@kpmg.onmicrosoft.com",
                },
                attachements: null!,
                variables: new Dictionary<string, string>
                {
                    { "title", $"Mandat Signé pour la Collection {collectionId}" },
                    { "body", emailBody },
                });
        }
    }
}
