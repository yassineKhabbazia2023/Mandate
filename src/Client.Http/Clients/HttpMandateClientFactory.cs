// <copyright file="HttpMandateClientFactory.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Client.Http.Clients
{
    using Kpmg.Constellation.Net.Http;
    using Microsoft.Extensions.Options;

    internal class HttpMandateClientFactory : IMandateClientFactory
    {
        private readonly IOptions<MandateClientOptions> options;
        private readonly IHttpClientFactory factory;

        public HttpMandateClientFactory(IOptions<MandateClientOptions> options, IHttpClientFactory factory)
        {
            this.options = options ?? throw new ArgumentNullException(nameof(options));
            this.factory = factory ?? throw new ArgumentNullException(nameof(factory));
            options.Value.Validate();
        }

        // Overloaded method without userToken
        public IMandateClient Create()
        {
            return this.CreateInternal(null);
        }

        // Overloaded method with userToken
        public IMandateClient Create(string userToken)
        {
            return this.CreateInternal(userToken);
        }

        // Common method used by both overloads
        private IMandateClient CreateInternal(string? userToken)
        {
            var authentication = userToken != null ? new BearerHttpClientAuthentication(userToken) : (HttpClientAuthentication)null!;

            return new HttpMandateClient(this.options.Value.BaseUri, authentication, this.factory);
        }
    }
}
