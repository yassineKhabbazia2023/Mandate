// <copyright file="HttpFormioClientFactory.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Formio.Client.Http
{
    using Kpmg.Constellation.Net.Http;
    using Microsoft.Extensions.Options;

    public class HttpFormioClientFactory : IFormioClientFactory
    {
        private readonly IOptions<FormioOptions> options;
        private readonly IHttpClientFactory factory;

        public HttpFormioClientFactory(IOptions<FormioOptions> options, IHttpClientFactory factory)
        {
            this.options = options ?? throw new ArgumentNullException(nameof(options));
            this.factory = factory ?? throw new ArgumentNullException(nameof(factory));

            options.Value.Validate();
        }

        public IHttpClient Create()
        {
            var client = this.factory.Create(this.options.Value.BaseUri);

            client.DefaultRequestHeaders.Accept.Clear();

            return client;
        }

        public IHttpClient Create(FormioAuthToken authToken)
        {
            var client = this.factory.Create(this.options.Value.BaseUri);

            client.EnsureHttpClientCreated();
            client.DefaultRequestHeaders.Accept.Clear();

            switch (authToken.Type)
            {
                case FormioTokenType.User:
                    client.DefaultRequestHeaders.Add("x-jwt-token", authToken.Value);
                    break;
                case FormioTokenType.App:
                    client.DefaultRequestHeaders.Add("x-token", this.options.Value.FormioApiKey);
                    break;
            }

            return client;
        }
    }
}
