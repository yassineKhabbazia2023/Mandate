// <copyright file="HttpMandateClient.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Client.Http
{
    using System.Text;
    using System.Threading.Tasks;
    using Kpmg.Constellation.Net.Http;
    using Newtonsoft.Json;

    public class HttpMandateClient : IMandateClient
    {
        private readonly Uri baseUri;
        private readonly HttpClientAuthentication authentication;
        private readonly Kpmg.Constellation.Net.Http.IHttpClientFactory clientFactory;

        public HttpMandateClient(Uri baseUri, HttpClientAuthentication authentication, Kpmg.Constellation.Net.Http.IHttpClientFactory clientFactory)
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
            return JsonConvert.DeserializeObject<CollectionSummary>(responseBody) !;
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
