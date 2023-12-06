// <copyright file="IFormioClient.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Formio.Client
{
    using Newtonsoft.Json.Linq;

    public interface IFormioClient
    {
        Task<FormioSubmissionCollection?> GetSubmissionsAsync(string formId, int skip, int? limit, FormioAuthToken authToken);

        Task<JToken?> CheckJdcPartnerBankAsync(string formId, string codeBank);

        Task<bool> CheckMadateDematSupportedAsync(string formId, string codeBank);

        Task<bool> CheckCollecteConfigExistAsync(string formId, string codeBank, string bankAccountNumber, string bankSortCode, FormioAuthToken authToken);

        Task<JToken?> GetTemplateShemaAsync(string projectId, string codeBank, FormioAuthToken authToken);

        Task<JToken> GetSubmissionByIdAsync(string formId, string submissionId, FormioAuthToken authToken);

        Task<JToken> GetProjectDefinitionAsync(string formioProjectId, FormioAuthToken authToken);

        Task<FormioSubmissionPdf> DownloadSubmissionAsPDFWithTemplate(JToken form, JToken data, string downloadUrl, string pdfFileToken);

        Task<FormioSubmissionCollection> GetSubmissionMandateAsync(string bankCode, string bankSortCode, string bankAccountNumber, string bankCheckNumber, FormioAuthToken authToken);
    }
}
