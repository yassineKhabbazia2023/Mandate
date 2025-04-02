# Requêtes à utiliser sur AppInsight pour retrouver les erreurs liées aux appels à JDC

## Création de mandat

```KQL
exceptions 
| where problemId == "KPMG.Pulse.Back.Accounting.Mandate.JeDeclare.Client.JeDeclareApiException"
    and customDimensions.Method in ("CreateFolderAsync", "AddRibToFolderAsync", "CreateCollecteConfigurationAsync")
| project timestamp, method = customDimensions.Method, statusCode = customDimensions.StatusCode, error = customDimensions.ErrorMessage, folder = customDimensions.SerializedFolder, rib = customDimensions.SerializedRib, releve = customDimensions.SerializedReleve
```


## Upload/download de mandat

```KQL
exceptions 
| where problemId == "KPMG.Pulse.Back.Accounting.Mandate.JeDeclare.Client.JeDeclareApiException"
    and customDimensions.Method in ("UploadSignedMandat", "GetSignedMandatPdfAsync", "GetMandatPdfAsync")
    and customDimensions.JdcRibId != ""
| project timestamp, method = customDimensions.Method, statusCode = customDimensions.StatusCode, jdcFolderId = customDimensions.JdcFolderId, jdcRibId = customDimensions.JdcRibId, error = customDimensions.ErrorMessage
```

## Endpoint appelé à la mise à jour des mandats

```KQL
exceptions 
| where problemId == "KPMG.Pulse.Back.Accounting.Mandate.JeDeclare.Client.JeDeclareApiException"
    and customDimensions.Method !in ("CreateFolderAsync", "AddRibToFolderAsync", "CreateCollecteConfigurationAsync")
    and customDimensions.Method == "GetAllConfigurationFromFolderAsync"
| project timestamp, method = customDimensions.Method, severityLevel
```

# Requêtes à utiliser sur AppInsight pour retrouver les erreurs liées aux appels de l'azure fonction StatusesMonitoringDailyRunSchedule_Starts.
## Liste des appels de la fonction (StatusesMonitoringDailyRunSchedule_Start)
```
requests
| project
    timestamp,
    id,
    operation_Name,
    success,
    resultCode,
    duration,
    operation_Id,
    cloud_RoleName,
    invocationId=customDimensions['InvocationId']
| where timestamp > ago(30d)
| where cloud_RoleName =~ 'appcegpulsemdtprd0102' and operation_Name =~ 'StatusesMonitoringDailyRunSchedule_Start'
| order by timestamp desc
| take 20
```

## Détail des logs d'une execution de la fonction (StatusesMonitoringDailyRunSchedule_Start)
### Avant d'éxectuer, il faut mettre à jour l'operationId et l'invocationId.
```
union traces| union exceptions| where timestamp > ago(30d)| where operation_Id == 'e074cdcf214ec537f7704e3861e3d9ce'| where customDimensions['InvocationId'] == 'cb4452b6-da01-4710-b396-debcc3ceeb6f'| order by timestamp asc| project timestamp, message = iff(message != '', message, iff(innermostMessage != '', innermostMessage, customDimensions.['prop__{OriginalFormat}'])), logLevel = customDimensions.['LogLevel'], severityLevel
```