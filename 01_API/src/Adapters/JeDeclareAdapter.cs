// <copyright file="JeDeclareAdapter.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Adapters
{
    using KPMG.Pulse.Back.Accounting.Mandate.JeDeclare.Client;
    using Microsoft.Extensions.Logging;

    public class JeDeclareAdapter : IJeDeclareService
    {
        private readonly IJeDeclareClient _jedeclareClient;
        private readonly ILogger<JeDeclareAdapter> _logger;

        public JeDeclareAdapter(ILogger<JeDeclareAdapter> logger, IJeDeclareClient jedeclareClient)
        {
            _jedeclareClient = jedeclareClient;
            _logger = logger;
        }

        public async Task<Bban> AddRibToFolderAsync(string? bankServicesProviderId, CollectionCreationCommand mandateCreation, Bank bank)
        {
            var rib = mandateCreation.ToRibClient();

            var ribSaved = await _jedeclareClient.AddRibToFolderAsync(
                jdcFolderId: bankServicesProviderId!,
                ribClient: rib);

            return ribSaved.ToModel(bank);
        }

        public async Task<string> CreateCollecteConfigurationAsync(Company dossier, Bban rib, Signatory signatory, string bankServicesProviderId, string? destinationToolId = null)
        {
            var releve = rib.ToReleve(signatory);

            var collectConfigurationCreated = await _jedeclareClient.CreateCollecteConfigurationAsync(
                jdcFolderId: bankServicesProviderId,
                releve: releve,
                bankCode: rib.Bank?.Code!,
                isPartner: rib.Bank!.JdcAgreement.JdcPartnership == JdcPartnership.Partner,
                ebicsCardId: rib.Bank?.EbicsCardId!,
                destinationToolId: destinationToolId);

            return collectConfigurationCreated.Id!;
        }

        public async Task<Company> CreateFolderAsync(Company company, Address address, Signatory signatory)
        {
            var dossierClient = company.ToDossierClient(address, signatory);

            var createdFolder = await _jedeclareClient.CreateFolderAsync(dossierClient);

            return createdFolder.ToCompany();
        }

        public async Task<byte[]> GetMandatPdfAsync(string jdcFolderId, string jdcRibId)
        {
            try
            {
                return await _jedeclareClient.GetMandatPdfAsync(jdcFolderId, jdcRibId);
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
                return await _jedeclareClient.UploadSignedMandat(folderId!, ribId!, mandateFile);
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
                var partnership = collection.Bban?.Bank?.JdcAgreement?.JdcPartnership;

                return await _jedeclareClient.DeactivateCollection(
                    folderId!,
                    releveId!,
                    partnership.HasValue && partnership.Value != JdcPartnership.NonPartner);
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
                return await _jedeclareClient.GetSignedMandatPdfAsync(jdcFolderId, jdcRibId);
            }
            catch (JeDeclareApiException ex)
            {
                throw new ServicesProviderException(ex.Message, ex);
            }
        }

        public async Task<List<TechnicalCollection>> GetAllConfigurationFromFolderAsync(string jdcFolderId)
        {
            try
            {
                var releves = await _jedeclareClient.GetAllConfigurationFromFolderAsync(jdcFolderId);

                if (releves.Releve is null)
                {
                    return [];
                }

                return releves.Releve.Select(r => r.ToModel()).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "");
            }

            // You might want to return a default value or null if the exception is caught
            return [];
        }

        public async Task<bool> CheckSignedMandatExists(string jdcFolderId, string jdcRibId)
        {
            try
            {
                return await _jedeclareClient.CheckSignedMandatExists(jdcFolderId, jdcRibId);
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