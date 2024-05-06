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
            return JsonConvert.DeserializeObject<PagedTechnicalMandate>(responseBody) !;
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

            int imported = (int)jsonObject["Imported"] !;
            var failed = jsonObject["Failed"] !.ToObject<List<JObject>>() !.Select(failJson =>
            {
                var bankInfo = new CollectionBankInfo(
                    bankName: (string)failJson["BankName"] !,
                    accountNumber: (string)failJson["AccountNumber"] !,
                    jdcPartnership: (int)failJson["JdcPartnership"] !);

                return new CollectionSummary(
                    id: (Guid)failJson["Id"] !,
                    erpId: (string)failJson["ErpId"] !,
                    companyName: (string)failJson["CompanyName"] !,
                    collectionBankInfo: bankInfo,
                    creationDate: (DateTime)failJson["CreationDate"] !,
                    modificationDate: (DateTime)failJson["ModificationDate"] !,
                    statusCode: (int)failJson["StatusCode"] !);
            }).ToList().AsReadOnly();
            return new PagedRecoveryMandate(imported, failed) !;
        }

        private static CollectionSummary DeserializeCollectionSummary(string json)
        {
            var jsonObject = JObject.Parse(json);

            var collectionBankInfo = new CollectionBankInfo(
                bankName: (string)jsonObject["BankName"] !,
                accountNumber: (string)jsonObject["AccountNumber"] !,
                jdcPartnership: (int)jsonObject["JdcPartnership"] !);

            var collectionSummary = new CollectionSummary(
                id: (Guid)jsonObject["Id"] !,
                erpId: (string)jsonObject["ErpId"] !,
                companyName: (string)jsonObject["CompanyName"] !,
                collectionBankInfo: collectionBankInfo,
                creationDate: (DateTime)jsonObject["CreationDate"] !,
                modificationDate: (DateTime)jsonObject["ModificationDate"] !,
                statusCode: (int)jsonObject["StatusCode"] !);

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
