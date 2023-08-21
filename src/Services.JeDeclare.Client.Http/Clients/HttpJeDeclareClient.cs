// <copyright file="HttpJeDeclareClient.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.JeDeclare.Client.Http
{
    public class HttpJeDeclareClient : IJeDeclareClient
    {
        public async Task<ListeReleves> GetAllConfigurationFromFolderAsync(string jdcCompteId, string jdcFolderId)
        {
            // TODO
            // send http request to jedeclare.com to get data
            throw new NotImplementedException();
        }
    }
}
