// <copyright file="HttpMandateClient.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Client.Http
{
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;
    using Kpmg.Constellation.Net.Http;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public class HttpMandateClient : IMandateClient
    {
        private readonly Uri baseUri;
        private readonly HttpClientAuthentication authentication;
        private readonly IHttpClientFactory clientFactory;

        public HttpMandateClient(Uri baseUri, HttpClientAuthentication authentication, IHttpClientFactory clientFactory)
        {
            this.baseUri = baseUri ?? throw new ArgumentNullException(nameof(baseUri));
            this.authentication = authentication;
            this.clientFactory = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory));
        }

        public async Task<CollectionSummary> GetRecoveryAsync(Bban ribEntry)
        {
            this.EnsuresUserToken();
            const string? requestUri = "mandate/recovery";

            using var message = new HttpRequestMessage(HttpMethod.Post, requestUri)
            {
                Content = new StringContent(
                    JsonConvert.SerializeObject(ribEntry),
                    encoding: Encoding.UTF8,
                    mediaType: "application/json"),
            };

            using var client = this.clientFactory.Create(this.baseUri, this.authentication);
            var response = await client.SendAsync(message);

            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return DeserializeCollectionSummary(responseBody) !;

        }

        public async Task<PagedTechnicalMandate> GetTechnicalCollectionSummaryAsync()
        {
            return await this.GetTechnicalCollectionSummaryAsync(0, 100, new List<int>());
        }

        public async Task<PagedTechnicalMandate> GetTechnicalCollectionSummaryAsync(int skip, int limit)
        {
            return await this.GetTechnicalCollectionSummaryAsync(skip, limit, new List<int>());
        }

        public async Task<PagedTechnicalMandate> GetTechnicalCollectionSummaryAsync(int skip, int limit, List<int> statusCodes)
        {
            this.EnsuresUserToken();
            string? requestUri = $"mandate/technical?skip={skip}&limit={limit}&{Helpers.ConvertListToQueryString(statusCodes)}";

            using var message = new HttpRequestMessage(HttpMethod.Get, requestUri);

            using var client = this.clientFactory.Create(this.baseUri, this.authentication);
            var response = await client.SendAsync(message);

            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return JsonConvert.DeserializeObject<PagedTechnicalMandate>(responseBody)!;
        }

        public async Task<PagedRecoveryMandate> RecoveryFormIoAsync(int skip, int limit)
        {
            this.EnsuresUserToken();
            string? requestUri = $"mandate/recovery-form-io?skip={skip}&limit={limit}";

            using var message = new HttpRequestMessage(HttpMethod.Post, requestUri);

            using var client = this.clientFactory.Create(this.baseUri, this.authentication);
            var response = await client.SendAsync(message);

            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return DeserializePagedRecoveryMandate(responseBody);
        }

        public async Task<bool> RefreshMandatsStatusesAsync(List<TechnicalCollectionSummary> mandates)
        {
            this.EnsuresUserToken();
            const string? requestUri = $"mandate/refresh-mandates-statuses";

            using var message = new HttpRequestMessage(HttpMethod.Post, requestUri)
            {
                Content = new StringContent(
                                JsonConvert.SerializeObject(mandates),
                                encoding: Encoding.UTF8,
                                mediaType: "application/json"),
            };

            using var client = this.clientFactory.Create(this.baseUri, this.authentication);
            var response = await client.SendAsync(message);

            response.EnsureSuccessStatusCode();

            return true;
        }

        private static PagedRecoveryMandate DeserializePagedRecoveryMandate(string json)
        {
            var jsonObject = JObject.Parse(json);
            int imported = (int)jsonObject["imported"]!;
            var failed = jsonObject["failed"]!.ToObject<List<JObject>>()!.Select(failJson =>
            {
                var bankInfo = new CollectionBankInfo(
                    bankName: (string)failJson["bankName"]!,
                    accountNumber: (string)failJson["accountNumber"]!,
                    jdcPartnership: (int)failJson["jdcPartnership"]!);

                return new CollectionSummary(
                    id: (Guid)failJson["id"]!,
                    erpId: (string)failJson["erpId"]!,
                    companyName: (string)failJson["companyName"]!,
                    collectionBankInfo: bankInfo,
                    creationDate: (DateTime)failJson["creationDate"]!,
                    modificationDate: (DateTime)failJson["modificationDate"]!,
                    statusCode: (int)failJson["statusCode"]!);
            }).ToList().AsReadOnly();
            return new PagedRecoveryMandate(imported, failed) !;
        }

        private static CollectionSummary DeserializeCollectionSummary(string json)
        {
            var jsonObject = JObject.Parse(json);

            var collectionBankInfo = new CollectionBankInfo(
                bankName: (string)jsonObject["bankName"]!,
                accountNumber: (string)jsonObject["accountNumber"]!,
                jdcPartnership: (int)jsonObject["jdcPartnership"]!);

            var collectionSummary = new CollectionSummary(
                id: (Guid)jsonObject["id"]!,
                erpId: (string)jsonObject["erpId"]!,
                companyName: (string)jsonObject["companyName"]!,
                collectionBankInfo: collectionBankInfo,
                creationDate: (DateTime)jsonObject["creationDate"]!,
                modificationDate: (DateTime)jsonObject["modificationDate"]!,
                statusCode: (int)jsonObject["statusCode"]!);

            return collectionSummary;
        }

        private void EnsuresUserToken()
        {
            if (this.authentication == null)
            {
                throw new InvalidOperationException("The API call requires an authentification.");
            }
        }
    }
}
