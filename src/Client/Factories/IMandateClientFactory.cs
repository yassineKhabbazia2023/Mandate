// <copyright file="IMandateClientFactory.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Client
{
    public interface IMandateClientFactory
    {
        IMandateClient Create();

        IMandateClient Create(string userToken);
    }
}
