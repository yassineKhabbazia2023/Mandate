# Variables d'environnement

| Projet | Variable | Valeur | Description |
|---|---|---|---|
| Mandate.AspNetCore | APPINSIGHTS_INSTRUMENTATIONKEY | | Clé AppInsights |
| Mandate.AspNetCore | DbConnectionString | | Chaîne de connexion à la base de données SQL |
| Mandate.AspNetCore | JeDeclareBaseUri | | Uri de l'api de JeDeclare |
| Mandate.AspNetCore | JeDeclareLogin | | Utilisateur de l'api de JeDeclare |
| Mandate.AspNetCore | JeDeclarePassword | | Mot de passe de l'api de JeDeclare |
| Mandate.AspNetCore | JeDeclareCompteId | | Identifiant de Pulse pour JeDeclare |
| Mandate.AspNetCore | JeDeclareHistoryDateEnabledBanks | | Banques pour lesquelles la date de reprise est renseignée au premier janvier de l'année en cours lors de la création d'un mandat |
| Mandate.AspNetCore | MandateCancellationCcEmails | | Addresses à utiliser en CC des mails de résiliation d'un mandat, au format "adresse1,adresse2" |
| Mandate.AspNetCore | MandateCancellationSubject | | Objet du mail de résiliation |
| Mandate.AspNetCore | MandateCancellationTemplateName | | Template du mail de résiliation |
| Mandate.AspNetCore | MandateCancellationFromEmail | | Adresse d'envoi du mail de résiliation |
| Mandate.AspNetCore | MandateCancellationToEmail | |Adresse de destination du mail de résiliation |
| Mandate.AspNetCore | MandateUploadedCcEmails | | Addresses à utiliser en CC des mails de transmission d'un mandat signé, au format "adresse1,adresse2" |
| Mandate.AspNetCore | MandateUploadedSubject | | Objet du mail de transmission d'un mandat signé |
| Mandate.AspNetCore | MandateUploadedTemplateName | | Template du mail de transmission d'un mandat signé |
| Mandate.AspNetCore | MandateUploadedFromEmail | | Adresse d'envoi du mail de transmission d'un mandat signé |
| Mandate.AspNetCore | MandateUploadedToEmail | | Adresse de destiniation du mail de transmission d'un mandat signé |
| Mandate.AspNetCore | MANDATE_NOTIFICATION_V2_API_URL | | Url de Notification API (pour l'envoi de mail) |
| Mandate.AspNetCore | EnableSwagger | | Feature flip d'activation/désactivation du swagger |
| Mandate.AspNetCore | LogLevel | | Niveau de log minimal pour tracer dans AppInsight |
| Mandate.AspNetCore | ServiceBusQueueCreateQueueName | | Nom de la queue d'évènement à lire |
| Mandate.AspNetCore | hubServiceBus__fullyQualifiedNamespace | | Url du Service Bus |
| Mandate.AspNetCore | hubServiceBus__clientId | | Id de l'Identité Managé du Spoke |
| Mandate.AspNetCore | hubServiceBus__credential | | Type d'identification au Service Bus |
| Mandate.Function.Functions | DbConnectionString | | Chaîne de connexion à la base de données SQL |
| Mandate.Function.Functions | JeDeclareBaseUri | | Uri de l'api de JeDeclare |
| Mandate.Function.Functions | JeDeclareLogin | | Utilisateur de l'api de JeDeclare |
| Mandate.Function.Functions | JeDeclarePassword | | Mot de passe de l'api de JeDeclare |
| Mandate.Function.Functions | JeDeclareCompteId | | Identifiant de Pulse pour JeDeclare |
| Mandate.Function.Functions | JeDeclareHistoryDateEnabledBanks | | Banques pour lesquelles la date de reprise est renseignée au premier janvier de l'année en cours lors de la création d'un mandat |
| Mandate.Function.Functions | serviceBusNameSpace__fullyQualifiedNamespace | | Namespace du service bus utilisé par les Azure functions |
| Mandate.Function.Functions | serviceBusNameSpace__clientId | | Id de l'Identité Managé du Spoke |
| Mandate.Function.Functions | UpdateStatuses__DailyStatusCodes |  | Liste des statuts monitorés pour mise à jour des statuts par la fonction quotidienne |
| Mandate.Function.Functions | UpdateStatuses__HourlyStatusCodes |  | Liste des statuts monitorés pour mise à jour des statuts par la fonction lancée toutes les 4 heures |
| Mandate.Function.Functions | StatusesMonitoringDailyRunSchedule | | CRON pour la planification d'exécution de la fonction de mise à jour quotidienne des statuts des mandats |
| Mandate.Function.Functions | StatusesMonitoringHourlyRunSchedule | | CRON pour la planification d'exécution de la fonction de mise à jour chaque 4 heures des statuts des mandats |
| Mandate.Function.Functions | MandateCreationQueue | | Nom de la queue d'évènement sur les créations de mandat |