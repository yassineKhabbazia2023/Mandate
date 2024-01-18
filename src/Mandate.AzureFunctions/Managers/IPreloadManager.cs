// <copyright file="IPreloadManager.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.AzureFunctions
{
    using System.Threading.Tasks;
    using KPMG.Pulse.Back.Accounting.Mandate.Client;

    public interface IPreloadManager
    {
        Task GetRecoveryAsync(Bban rib);
    }
}
