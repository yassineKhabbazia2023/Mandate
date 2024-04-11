// <copyright file="JeDeclareAdapter.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Adapters
{
    using KPMG.Pulse.Back.Accounting.Mandate.JeDeclare.Client;

    public class JeDeclareAdapter : IJeDeclareService
    {
        private readonly IJeDeclareClient jedeclareClient;

        public JeDeclareAdapter(IJeDeclareClient jedeclareClient)
        {
            this.jedeclareClient = jedeclareClient;
        }

        public async Task<Bban> AddRibToFolderAsync(string? bankServicesProviderId, CollectionCreationCommand mandateCreation, Bank bank)
        {
            var rib = mandateCreation.ToRibClient();

            var ribSaved = await this.jedeclareClient.AddRibToFolderAsync(
                jdcFolderId: bankServicesProviderId!,
                ribClient: rib);

            return ribSaved.ToModel(bank);
        }

        public async Task<string> CreateCollecteConfigurationAsync(Company dossier, Bban rib, string bankServicesProviderId)
        {
            var releve = rib.ToReleve(dossier?.Signatory!);

            var collectConfigurationCreated = await this.jedeclareClient.CreateCollecteConfigurationAsync(
                jdcFolderId: bankServicesProviderId,
                releve: releve,
                bankCode: rib.Bank?.Code!,
                ebicsCardId: rib.Bank?.EbicsCardId!);

            return collectConfigurationCreated.Id!;
        }

        public async Task<Company> CreateFolderAsync(Company company)
        {
            var dossierClient = company.ToDossierClient();

            var createdFolder = await this.jedeclareClient.CreateFolderAsync(dossierClient);

            return createdFolder.ToCompany();
        }

        public async Task<byte[]> GetMandatPdfAsync(string jdcFolderId, string jdcRibId)
        {
            try
            {
                return await this.jedeclareClient.GetMandatPdfAsync(jdcFolderId, jdcRibId);
            }
            catch (JeDeclareApiException ex)
            {
                throw new ServicesProviderException(ex.Message, ex);
            }
        }

        public async Task<string> UploadSignedMandate(Collection collection, byte[] mandateFile)
        {
            try
            {
                var folderId = collection?.Company?.BankServicesProviderId;
                var ribId = collection?.Bban?.BbanServicesProviderId;
                return await this.jedeclareClient.UploadSignedMandat(folderId!, ribId!, mandateFile);
            }
            catch (JeDeclareApiException ex)
            {
                throw new ServicesProviderException(ex.Message, ex);
            }
        }

        public async Task<bool> DeactivateCollection(Collection collection)
        {
            try
            {
                var folderId = collection.Company?.BankServicesProviderId ?? throw new ServicesProviderException($"BankServicesProviderId is null for {collection.Id}");
                var releveId = collection.CollectionServicesProviderId ?? throw new ServicesProviderException($"CollectionServicesProviderId is null for {collection.Id}");

                return await this.jedeclareClient.DeactivateCollection(folderId!, releveId!);
            }
            catch (JeDeclareApiException ex)
            {
                throw new ServicesProviderException(ex.Message, ex);
            }
        }

        public async Task<byte[]> GetSignedMandatPdfAsync(string jdcFolderId, string jdcRibId)
        {
            try
            {
                return await this.jedeclareClient.GetSignedMandatPdfAsync(jdcFolderId, jdcRibId);
            }
            catch (JeDeclareApiException ex)
            {
                throw new ServicesProviderException(ex.Message, ex);
            }
        }

        public async Task<List<TechnicalCollection>?> GetAllConfigurationFromFolderAsync(string jdcFolderId)
        {
            try
            {
                var releves = await this.jedeclareClient.GetAllConfigurationFromFolderAsync(jdcFolderId);
                return releves.Releve?.Select(r => r.ToModel()).ToList();
            }
            catch (JeDeclareApiException)
            {
                // Optionally log the exception or take other actions
                // Ignoring the exception
            }

            // You might want to return a default value or null if the exception is caught
            return null;
        }

        public async Task<bool> CheckSignedMandatExists(string jdcFolderId, string jdcRibId)
        {
            try
            {
                return await this.jedeclareClient.CheckSignedMandatExists(jdcFolderId, jdcRibId);
            }
            catch (JeDeclareApiException)
            {
                // Optionally log the exception or take other actions
                // Ignoring the exception
            }

            // You might want to return a default value or null if the exception is caught
            return false;
        }
    }
}