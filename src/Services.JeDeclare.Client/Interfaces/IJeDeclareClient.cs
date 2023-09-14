// <copyright file="IJeDeclareClient.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.JeDeclare.Client
{
    public interface IJeDeclareClient
    {
        Task<ListeReleves> GetAllConfigurationFromFolderAsync(string jdcCompteId, string jdcFolderId);

        Task<byte[]> GetSignedMandatPdfAsync(string jdcCompteId, string jdcFolderId, string jdcRibId);

        Task<byte[]> GetMandatPdfAsync(string jdcCompteId, string jdcFolderId, string jdcRibId);

        Task<DossierClient> CreateFolderAsync(string jdcCompteId, DossierClient folderClient);

        Task<Rib> AddRibToFolderAsync(string jdcCompteId, string jdcFolderId, Rib ribClient);

        Task<Releve> CreateCollecteConfigurationAsync(string jdcCompteId, string jdcFolderId, Releve releve);

        Task<bool> UpdateCollecteConfigurationAsync(string jdcCompteId, string jdcFolderId, Releve releve);

        Task<string> UploadSignedMandat(string jdcCompteId, string jdcFolderId, string jdcRibId, byte[] mandat);

        Task<bool> CheckSignedMandatExists(string jdcCompteId, string jdcFolderId, string jdcRibId);

    }
}
