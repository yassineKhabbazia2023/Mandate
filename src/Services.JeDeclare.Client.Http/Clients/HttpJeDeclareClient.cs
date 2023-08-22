// <copyright file="HttpJeDeclareClient.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.JeDeclare.Client.Http
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;

    public class HttpJeDeclareClient : IJeDeclareClient
    {
        private readonly Uri baseUri;
        private readonly Kpmg.Constellation.Net.Http.IHttpClientFactory clientFactory;

        public HttpJeDeclareClient(Uri baseUri, Kpmg.Constellation.Net.Http.IHttpClientFactory clientFactory)
        {
            this.baseUri = baseUri ?? throw new ArgumentNullException(nameof(baseUri));
            this.clientFactory = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory));
        }

        public async Task<ListeReleves> GetAllConfigurationFromFolderAsync(string jdcCompteId, string jdcFolderId)
        {
            using var client = this.clientFactory.Create(this.baseUri);
            var requestUri = $"compte/{jdcCompteId}/dossierClient/{jdcFolderId}/releve";

            var response = await client.GetAsync(requestUri);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var o = await response.Content.ReadAsAsync<ListeReleves>();
                return o;
            }

            await ProcessInvalidResponse(response).ConfigureAwait(false);

            throw new ApiException("The HTTP status code of the response was not expected (" + response.StatusCode + ").", response.StatusCode, (Error)null);
        }

        private static async Task ProcessInvalidResponse(HttpResponseMessage response)
        {
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new ApiException("Bearer token is missing or invalid", response.StatusCode, (Error)null);
            }

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var error = await response.Content.ReadAsAsync<Error>().ConfigureAwait(false);

                if (error == null)
                {
                    throw new ApiException("Response was null which was not expected.", response.StatusCode, (Error)null);
                }

                throw new ApiException("The query contains invalid arguments", response.StatusCode, error);
            }

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                var error = await response.Content.ReadAsAsync<Error>().ConfigureAwait(false);

                if (error == null)
                {
                    throw new ApiException("Response was null which was not expected.", response.StatusCode, (Error)null);
                }

                if (error.ErrorType.Equals("TechnicalError"))
                {
                    throw new ApiException("A technical error has occurred.", response.StatusCode, error);
                }

                throw new ApiException("Company or Employee or Job is missing", response.StatusCode, error);
            }
        }
    }
}
