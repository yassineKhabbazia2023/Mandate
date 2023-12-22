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

            return new Bban(
                bankCode: ribSaved.Etablissement!,
                branchCode: ribSaved.Guichet!,
                accountNumber: ribSaved.NumCompte!,
                checkDigits: ribSaved.Cle!,
                bbanServicesProviderId: ribSaved.Id,
                bank: bank);
        }

        public async Task<string> CreateCollecteConfigurationAsync(Company dossier, Bban rib)
        {
            var releve = rib.ToReleve(dossier?.Signatory!);

            var collectConfigurationCreated = await this.jedeclareClient.CreateCollecteConfigurationAsync(
                jdcFolderId: dossier?.BankServicesProviderId!,
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
    }
}