// <copyright file="HttpFormioClientFactory.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Formio.Client.Http
{
    using Kpmg.Constellation.Net.Http;
    using Microsoft.Extensions.Options;

    public class HttpFormIoClientFactory : IFormIoClientFactory
    {
        private readonly IOptions<FormIoOptions> options;
        private readonly IHttpClientFactory factory;

        public HttpFormIoClientFactory(IOptions<FormIoOptions> options, IHttpClientFactory factory)
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

        public IHttpClient Create(FormIoAuthToken authToken)
        {
            var client = this.factory.Create(this.options.Value.BaseUri);

            client.EnsureHttpClientCreated();
            client.DefaultRequestHeaders.Accept.Clear();

            switch (authToken.Type)
            {
                case FormIoTokenType.User:
                    client.DefaultRequestHeaders.Add("x-jwt-token", authToken.Value);
                    break;
                case FormIoTokenType.App:
                    client.DefaultRequestHeaders.Add("x-token", this.options.Value.FormioApiKey);
                    break;
            }

            return client;
        }
    }
}
