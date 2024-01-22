// <copyright file="IJeDeclareClient.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.JeDeclare.Client
{
    public interface IJeDeclareClient
    {
        Task<ListeReleves> GetAllConfigurationFromFolderAsync(string jdcFolderId);

        Task<byte[]> GetSignedMandatPdfAsync(string jdcFolderId, string jdcRibId);

        Task<byte[]> GetMandatPdfAsync(string jdcFolderId, string jdcRibId);

        Task<DossierClient> CreateFolderAsync(DossierClient folderClient);

        Task<Rib> AddRibToFolderAsync(string jdcFolderId, Rib ribClient);

        Task<Releve> CreateCollecteConfigurationAsync(string jdcFolderId, Releve releve, string bankCode, string ebicsCardId);

        Task<bool> UpdateCollecteConfigurationAsync(string jdcFolderId, Releve releve);

        Task<string> UploadSignedMandat(string jdcFolderId, string jdcRibId, byte[] mandat);

        Task<bool> CheckSignedMandatExists(string jdcFolderId, string jdcRibId);

        Task<bool> DeactivateCollection(string jdcFolderId, string jdcReleveId);
    }
}
