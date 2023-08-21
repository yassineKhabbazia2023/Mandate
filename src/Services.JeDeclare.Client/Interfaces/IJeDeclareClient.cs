// <copyright file="IJeDeclareClient.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.JeDeclare.Client
{
    public interface IJeDeclareClient
    {
        Task<ListeReleves> GetAllConfigurationFromFolderAsync(string jdcCompteId, string jdcFolderId);

        // TODO : https://dev.azure.com/kpmgfr/Constellation/_git/Constellation?path=/KPMG.Constellation.Bankin.Services/JeDeclare/Interface/IJeDeclareService.cs&version=GBbank-develop&_a=contents
    }
}
