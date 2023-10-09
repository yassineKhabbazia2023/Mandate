// <copyright file="HttpFormioClient.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Formio.Client.Http
{
    using System.Collections.Generic;
    using System.Text;
    using System.Web;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public class HttpFormioClient : IFormioClient
    {
        private readonly ILogger logger;
        private readonly IFormioClientFactory factory;

        public HttpFormioClient(ILogger logger, IFormioClientFactory factory)
        {
            this.logger = logger;
            this.factory = factory;
        }

        public async Task<FormioSubmissionCollection?> GetSubmissionsAsync(string formId, int skip, int? limit, FormioAuthToken authToken)
        {
            var queryStringDic = HttpUtility.ParseQueryString(string.Empty);
            queryStringDic["skip"] = $"{skip}";
            queryStringDic["limit"] = limit == null ? "50" : $"{limit}";

            var allSubs = new FormioSubmissionCollection();

            using var client = this.factory.Create(authToken);

            var requestUri = $"constellation/{formId.Trim('/')}/submission?{queryStringDic}";

            var response = await client.GetAsync(requestUri);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                var exception = new FormioApiException($"Exception was thrown : status code : {response.StatusCode} - Message : '{responseBody}'");

                this.logger.LogError(
                    exception,
                    "{class} - '{method}': Exception was thrown : status code : '{statusCode}' - formiId: {formiId} - Message : '{eroorMessage}'",
                    nameof(HttpFormioClient),
                    nameof(this.GetSubmissionsAsync),
                    response.StatusCode,
                    formId,
                    responseBody);

                throw exception;
            }

            var submissions = DeserializeSubmissions(responseBody);

            allSubs.Submissions.AddRange(submissions);
            allSubs.Skip = skip;
            allSubs.Limit = 50;

            return allSubs;
        }

        public async Task<JToken?> CheckJdcPartnerBankAsync(string formId, string codeBank)
        {
            using var client = this.factory.Create();

            var queryStringDic = HttpUtility.ParseQueryString(string.Empty);
            queryStringDic["data.bankCode"] = $"{codeBank}";

            var requestUri = $"constellation/{formId.Trim('/')}/submission?{queryStringDic}";

            var response = await client.GetAsync(requestUri);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                var exception = new FormioApiException($"Exception was thrown : status code : {response.StatusCode} - Message : '{responseBody}'");

                this.logger.LogError(
                    exception,
                    "{class} - '{method}': Exception was thrown : status code : '{statusCode}' - formiId: {formiId} - Message : '{eroorMessage}'",
                    nameof(HttpFormioClient),
                    nameof(this.CheckJdcPartnerBankAsync),
                    response.StatusCode,
                    formId,
                    responseBody);

                throw exception;
            }

            var submissions = DeserializeSubmissions(responseBody);

            if (submissions.Count == 0)
            {
                return null;
            }

            var result = JObject.Parse(submissions.Single().Data.ToString() !);

            return result;
        }

        public async Task<bool> CheckMadateDematSupportedAsync(string formId, string codeBank)
        {
            using var client = this.factory.Create();

            var queryStringDic = HttpUtility.ParseQueryString(string.Empty);
            queryStringDic["data.bankCode"] = $"{codeBank}";
            queryStringDic["data.supportDigitalMandate"] = "true";

            var requestUri = $"constellation/{formId.Trim('/')}/submission?{queryStringDic}";

            var response = await client.GetAsync(requestUri);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                var exception = new FormioApiException($"Exception was thrown : status code : {response.StatusCode} - Message : '{responseBody}'");

                this.logger.LogError(
                    exception,
                    "{class} - '{method}': Exception was thrown : status code : '{statusCode}' - formiId: {formiId} - Message : '{eroorMessage}'",
                    nameof(HttpFormioClient),
                    nameof(this.CheckMadateDematSupportedAsync),
                    response.StatusCode,
                    formId,
                    responseBody);

                throw exception;
            }

            var submissions = DeserializeSubmissions(responseBody);

            return submissions.Count > 0;
        }

        public async Task<bool> CheckCollecteConfigExistAsync(string formId, string codeBank, string bankAccountNumber, string bankSortCode, FormioAuthToken authToken)
        {
            using var client = this.factory.Create(authToken);

            var queryStringDic = HttpUtility.ParseQueryString(string.Empty);
            queryStringDic["data.bankCode"] = $"{codeBank}";
            queryStringDic["data.bankAccountNumber"] = $"{bankAccountNumber}";
            queryStringDic["data.bankSortCode"] = $"{bankSortCode}";

            var requestUri = $"constellation/{formId.Trim('/')}/exists?{queryStringDic}";

            var response = await client.GetAsync(requestUri);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                var exception = new FormioApiException($"Exception was thrown : status code : {response.StatusCode} - Message : '{responseBody}'");

                this.logger.LogError(
                    exception,
                    "{class} - '{method}': Exception was thrown : status code : '{statusCode}' - formiId: {formiId} - codeBank: {codeBank} - bankAccountNumber: {bankAccountNumber} - bankSortCode: {bankSortCode} - Message : '{eroorMessage}'",
                    nameof(HttpFormioClient),
                    nameof(this.CheckCollecteConfigExistAsync),
                    response.StatusCode,
                    formId,
                    codeBank,
                    bankAccountNumber,
                    bankSortCode,
                    responseBody);

                throw exception;
            }

            return true;
        }

        public async Task<JToken?> GetTemplateShemaAsync(string projectId, string codeBank, FormioAuthToken authToken)
        {
            using var client = this.factory.Create(authToken);

            var queryStringDic = HttpUtility.ParseQueryString(string.Empty);
            queryStringDic["limit"] = "100";
            queryStringDic["properties.bankCode__regex"] = $"{codeBank}";
            queryStringDic["select"] = "components,settings,title,path";

            var requestUri = $"project/{projectId.Trim('/')}/form?{queryStringDic}";

            var response = await client.GetAsync(requestUri);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                var exception = new FormioApiException($"Exception was thrown : status code : {response.StatusCode} - Message : '{responseBody}'");

                this.logger.LogError(
                    exception,
                    "{class} - '{method}': Exception was thrown : status code : '{statusCode}' - projectId: {projectId} - codeBank: {codeBank} - Message : '{eroorMessage}'",
                    nameof(HttpFormioClient),
                    nameof(this.GetTemplateShemaAsync),
                    response.StatusCode,
                    projectId,
                    codeBank,
                    responseBody);

                throw exception;
            }

            var templateSchemas = JArray.Parse(responseBody);

            return templateSchemas[0];
        }

        public async Task<JToken> GetSubmissionByIdAsync(string formId, string submissionId, FormioAuthToken authToken)
        {
            using var client = this.factory.Create(authToken);

            var requestUri = $"constellation/{formId.Trim('/')}/submission/{submissionId}";

            var response = await client.GetAsync(requestUri);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                var exception = new FormioApiException($"Exception was thrown : status code : {response.StatusCode} - Message : '{responseBody}'");

                this.logger.LogError(
                    exception,
                    "{class} - '{method}': Exception was thrown : status code : '{statusCode}' - formiId: {formiId} - submissionId: {submissionId} - Message : '{eroorMessage}'",
                    nameof(HttpFormioClient),
                    nameof(this.GetSubmissionsAsync),
                    response.StatusCode,
                    formId,
                    submissionId,
                    responseBody);

                throw exception;
            }

            var result = JObject.Parse(responseBody);

            return result;
        }

        public async Task<FormioSubmissionPdf> DownloadSubmissionAsPDFWithTemplate(JToken form, JToken data, string downloadUrl, string pdfFileToken)
        {
            using var client = this.factory.Create();

            client.DefaultRequestHeaders.Add("x-file-token", pdfFileToken);

            var request = JToken.FromObject(new
            {
                form,
                submission = data,
            });

            var content = new StringContent(
               JsonConvert.SerializeObject(request),
               Encoding.UTF8,
               "application/json");

            var response = await client.PostAsync(downloadUrl, content);

            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                var exception = new FormioApiException($"Exception was thrown : status code : {response.StatusCode} - Message : '{responseBody}'");

                this.logger.LogError(
                    exception,
                    "{class} - '{method}': Exception was thrown : status code : '{statusCode}' - downloadUrl: {downloadUrl} - Message : '{eroorMessage}'",
                    nameof(HttpFormioClient),
                    nameof(this.DownloadSubmissionAsPDFWithTemplate),
                    response.StatusCode,
                    downloadUrl,
                    responseBody);

                throw exception;
            }

            return await GetFormioSubmissionPdfAsync(data, response);
        }

        public async Task<JToken> GetProjectDefinitionAsync(string formioProjectId, FormioAuthToken authToken)
        {
            using var client = this.factory.Create(authToken);

            var requestUri = $"project/{formioProjectId.Trim('/')}";

            var response = await client.GetAsync(requestUri);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                var exception = new FormioApiException($"Exception was thrown : status code : {response.StatusCode} - Message : '{responseBody}'");

                this.logger.LogError(
                    exception,
                    "{class} - '{method}': Exception was thrown : status code : '{statusCode}' - formioProjectId: {formioProjectId} - Message : '{eroorMessage}'",
                    nameof(HttpFormioClient),
                    nameof(this.GetProjectDefinitionAsync),
                    response.StatusCode,
                    formioProjectId,
                    responseBody);

                throw exception;
            }

            var result = JObject.Parse(responseBody);

            return result;
        }

        private static async Task<FormioSubmissionPdf> GetFormioSubmissionPdfAsync(JToken data, HttpResponseMessage response)
        {
            Stream responseStream = await response.Content.ReadAsStreamAsync();
            byte[] pdf = new byte[responseStream.Length];
            responseStream.Read(pdf, 0, (int)responseStream.Length);

            var formioSubmissionPdf = new FormioSubmissionPdf
            {
                Data = pdf,
                Created = data["created"]?.Value<string>()!,
                Modified = data["modified"]?.Value<string>()!,
                Id = data["_id"]?.Value<string>()!,
                Owner = data["owner"]?.Value<string>()!,
            };

            return formioSubmissionPdf;
        }

        private static IList<FormioSubmission> DeserializeSubmissions(string responseBody)
        {
            return responseBody.Deserialize<IList<FormioSubmission>>();
        }
    }
}