// <copyright file="IJeDeclareClientFactory.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.JeDeclare.Client
{
    using Kpmg.Constellation.Net.Http;

    public interface IJeDeclareClientFactory
    {
        IHttpClient Create(bool allowAcceptXml);
    }
}
