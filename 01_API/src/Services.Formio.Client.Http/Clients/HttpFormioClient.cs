// <copyright file="HttpFormioClient.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Formio.Client.Http
{
    using System.Collections.Generic;
    using System.Text;
    using System.Web;
    using Kpmg.Constellation.Net.Http;
    using Microsoft.AspNetCore.Http.Extensions;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public class HttpFormioClient : IFormioClient
    {
        private const string FormId = "demandemandat";
        private readonly ILogger<HttpFormioClient> logger;
        private readonly IFormioClientFactory factory;
        private readonly FormioOptions options;

        public HttpFormioClient(ILogger<HttpFormioClient> logger, IFormioClientFactory factory, IOptions<FormioOptions> options)
        {
            this.logger = logger;
            this.factory = factory;
            this.options = options?.Value ?? throw new InvalidOperationException($"Instance of {nameof(FormioOptions)} is null.");
        }

        public async Task<FormioSubmissionCollection?> GetSubmissionsAsync(string formId, int skip, int? limit, FormioAuthToken authToken)
        {
            var queryStringDic = HttpUtility.ParseQueryString(string.Empty);
            queryStringDic["skip"] = $"{skip}";
            queryStringDic["limit"] = limit == null ? "50" : $"{limit}";

            var subs = new FormioSubmissionCollection();

            using var client = this.factory.Create(authToken);

            var requestUri = $"constellation/{formId.Trim('/')}/submission?{queryStringDic}";

            var responseBody = await this.GetResponseBodyAsync(formId, client, requestUri);

            var submissions = DeserializeSubmissions(responseBody);

            subs.Submissions.AddRange(submissions);
            subs.Skip = skip;

            subs.Limit = limit == null ? 50 : (int)limit;

            return subs;
        }

        public async Task<FormioSubmissionCollection?> GetSubmissionsAsync(int skip, int? limit, FormioAuthToken authToken)
        {
            return await this.GetSubmissionsAsync($"form/{this.options.DemandeMandateFormId}", skip, limit, authToken);
        }

        public async Task<JToken?> CheckJdcPartnerBankAsync(string formId, string codeBank)
        {
            using var client = this.factory.Create();

            var queryStringDic = HttpUtility.ParseQueryString(string.Empty);
            queryStringDic["data.bankCode"] = $"{codeBank}";

            var requestUri = $"constellation/{formId.Trim('/')}/submission?{queryStringDic}";
            string responseBody = await this.GetResponseBodyAsync(formId, client, requestUri);

            var submissions = DeserializeSubmissions(responseBody);

            if (submissions.Count == 0)
            {
                return null;
            }

            return JObject.Parse(submissions.Single().Data.ToString()!);
        }

        public async Task<bool> CheckMadateDematSupportedAsync(string formId, string codeBank)
        {
            using var client = this.factory.Create();

            var queryStringDic = HttpUtility.ParseQueryString(string.Empty);
            queryStringDic["data.bankCode"] = $"{codeBank}";
            queryStringDic["data.supportDigitalMandate"] = "true";

            var requestUri = $"constellation/{formId.Trim('/')}/submission?{queryStringDic}";

            var responseBody = await this.GetResponseBodyAsync(formId, client, requestUri);

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

            var responseBody = await this.GetResponseBodyAsync(formId, client, requestUri);

            return JArray.Parse(responseBody).Count > 0;
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
                    "{Class} - '{Method}': Exception was thrown : status code : '{StatusCode}' - projectId: {ProjectId} - codeBank: {CodeBank} - Message : '{ErrorMessage}'",
                    nameof(HttpFormioClient),
                    nameof(this.GetTemplateShemaAsync),
                    response.StatusCode,
                    projectId,
                    codeBank,
                    responseBody);

                throw exception;
            }

            var templateSchemas = JArray.Parse(responseBody);

            return templateSchemas.Count > 0 ? templateSchemas[0] : null;
        }

        public async Task<JToken> GetSubmissionByIdAsync(string formId, string submissionId, FormioAuthToken authToken)
        {
            using var client = this.factory.Create(authToken);

            var requestUri = $"constellation/{formId.Trim('/')}/submission/{submissionId}";

            var responseBody = await this.GetResponseBodyAsync(formId, client, requestUri);

            return JObject.Parse(responseBody);
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
                    "{Class} - '{Method}': Exception was thrown : status code : '{StatusCode}' - downloadUrl: {DownloadUrl} - Message : '{ErrorMessage}'",
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

            var responseBody = await this.GetResponseBodyAsync(formioProjectId, client, requestUri);

            var result = JObject.Parse(responseBody);

            return result;
        }

        public async Task<FormioSubmissionCollection> GetSubmissionMandateAsync(string bankCode, string bankSortCode, string bankAccountNumber, string bankCheckNumber, FormioAuthToken authToken)
        {
            var queryBuilder = new QueryBuilder();

            if (bankCode != null)
            {
                queryBuilder.Add("data.bankCode", bankCode);
            }

            if (bankSortCode != null)
            {
                queryBuilder.Add("data.bankSortCode", bankSortCode);
            }

            if (bankAccountNumber != null)
            {
                queryBuilder.Add("data.bankAccountNumber", bankAccountNumber);
            }

            if (bankCheckNumber != null)
            {
                queryBuilder.Add("data.bankCheckNumber", bankCheckNumber);
            }

            var uriQuery = queryBuilder.ToQueryString();

            var subs = new FormioSubmissionCollection();

            using var client = this.factory.Create(authToken);

            var requestUri = $"constellation/{FormId}/submission{uriQuery}";

            var responseBody = await this.GetResponseBodyAsync(FormId, client, requestUri);

            var submissions = DeserializeSubmissions(responseBody);

            subs.Submissions.AddRange(submissions);

            return subs;
        }

        private static async Task<FormioSubmissionPdf> GetFormioSubmissionPdfAsync(JToken data, HttpResponseMessage response)
        {
            Stream responseStream = await response.Content.ReadAsStreamAsync();
            byte[] pdf = new byte[responseStream.Length];
            var bytesRead = responseStream.Read(pdf, 0, (int)responseStream.Length);

            var formioSubmissionPdf = new FormioSubmissionPdf
            {
                Data = bytesRead > 0 ? pdf : Array.Empty<byte>(),
                Created = data["created"]?.Value<string>()!,
                Modified = data["modified"]?.Value<string>()!,
                Id = data["_id"]?.Value<string>()!,
                Owner = data["owner"]?.Value<string>()!,
            };

            return formioSubmissionPdf;
        }

        private static IList<FormioSubmission> DeserializeSubmissions(string responseBody)
        {
            return JsonConvert.DeserializeObject<IList<FormioSubmission>>(responseBody)!;
        }

        private async Task<string> GetResponseBodyAsync(string formId, IHttpClient client, string requestUri)
        {
            var response = await client.GetAsync(requestUri);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                var exception = new FormioApiException($"Exception was thrown : status code : {response.StatusCode} - Message : '{responseBody}'");

                this.logger.LogError(
                    exception,
                    "{Class} - '{Method}': Exception was thrown : status code : '{StatusCode}' - formiId: {FormiId} - Message : '{ErrorMessage}'",
                    nameof(HttpFormioClient),
                    nameof(this.CheckJdcPartnerBankAsync),
                    response.StatusCode,
                    formId,
                    responseBody);

                throw exception;
            }

            return responseBody;
        }
    }
}