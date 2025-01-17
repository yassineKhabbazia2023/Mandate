// <copyright file="IMandateFunctionManager.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace Mandate.AzureFunctions.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using KPMG.Pulse.Back.Accounting.Mandate.Client;

    /// <summary>
    /// A service to get list of collection.
    /// </summary>
    public interface IUpdateMandateStatusesHandler
    {
        Task UpdateMandateStatusAsync(List<int> statusCodes);
    }
}
