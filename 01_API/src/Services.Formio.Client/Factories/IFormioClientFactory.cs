// <copyright file="IFormioClientFactory.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Formio.Client
{
    using Kpmg.Constellation.Net.Http;

    public interface IFormioClientFactory
    {
        IHttpClient Create();

        IHttpClient Create(FormioAuthToken authToken);
    }
}
