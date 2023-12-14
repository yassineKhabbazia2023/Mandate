// <copyright file="IFormIoClientFactory.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Formio.Client
{
    using Kpmg.Constellation.Net.Http;

    public interface IFormIoClientFactory
    {
        IHttpClient Create();

        IHttpClient Create(FormIoAuthToken authToken);
    }
}
