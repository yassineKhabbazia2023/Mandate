// <copyright file="HttpJeDeclareClientFactory.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.JeDeclare.Client.Http
{
    using System.Net.Http.Headers;
    using Kpmg.Constellation.Net.Http;
    using Microsoft.Extensions.Options;

    public class HttpJeDeclareClientFactory : IJeDeclareClientFactory
    {
        private readonly IOptions<JeDeclareOptions> options;
        private readonly IHttpClientFactory factory;

        public HttpJeDeclareClientFactory(IOptions<JeDeclareOptions> options, IHttpClientFactory factory)
        {
            this.options = options ?? throw new ArgumentNullException(nameof(options));
            this.factory = factory ?? throw new ArgumentNullException(nameof(factory));
            options.Value.Validate();
        }

        public IHttpClient Create(bool allowAcceptXml = true)
        {
            var authentication = new BasicHttpClientAuthentication(this.options.Value.Login, this.options.Value.Password);

            var client = this.factory.Create(this.options.Value.BaseUri, authentication);

            client.EnsureHttpClientCreated();

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(allowAcceptXml ? "application/xml" : "application/*"));

            return client;
        }
    }
}
