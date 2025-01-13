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