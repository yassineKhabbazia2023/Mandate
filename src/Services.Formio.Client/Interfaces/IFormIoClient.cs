// <copyright file="IFormIoClient.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Formio.Client
{
    using Newtonsoft.Json.Linq;

    public interface IFormIoClient
    {
        Task<FormIoSubmissionCollection?> GetSubmissionsAsync(string formId, int skip, int? limit, FormIoAuthToken authToken);

        Task<JToken?> CheckJdcPartnerBankAsync(string formId, string codeBank);

        Task<bool> CheckMadateDematSupportedAsync(string formId, string codeBank);

        Task<bool> CheckCollecteConfigExistAsync(string formId, string codeBank, string bankAccountNumber, string bankSortCode, FormIoAuthToken authToken);

        Task<JToken?> GetTemplateShemaAsync(string projectId, string codeBank, FormIoAuthToken authToken);

        Task<JToken> GetSubmissionByIdAsync(string formId, string submissionId, FormIoAuthToken authToken);

        Task<JToken> GetProjectDefinitionAsync(string formioProjectId, FormIoAuthToken authToken);

        Task<FormIoSubmissionPdf> DownloadSubmissionAsPDFWithTemplate(JToken form, JToken data, string downloadUrl, string pdfFileToken);

        Task<FormIoSubmissionCollection> GetSubmissionMandateAsync(string bankCode, string bankSortCode, string bankAccountNumber, string bankCheckNumber, FormIoAuthToken authToken);
    }
}
