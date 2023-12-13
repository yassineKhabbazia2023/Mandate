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

        public async Task<Bban> AddRibToFolderAsync(string? bankServicesProviderId, CollectionCreationCommand mandateCreation)
        {
            var rib = mandateCreation.ToRibClient();

            var ribSaved = await this.jedeclareClient.AddRibToFolderAsync(
                jdcFolderId: bankServicesProviderId !,
                ribClient: rib);

            return new Bban(
                bankCode: ribSaved.Etablissement !,
                branchCode: ribSaved.Guichet !,
                accountNumber: ribSaved.NumCompte !,
                checkDigits: ribSaved.Cle !,
                bbanServicesProviderId: ribSaved.Id,
                bank: null);
        }

        public async Task<Collection> CreateCollecteConfigurationAsync(string bankServicesProviderId, Bban rib, Signatory signatory)
        {
            var releve = rib.ToReleve(signatory);
            // bban => rib
            //rib.construct..

            var collectConfigurationCreated = await this.jedeclareClient.CreateCollecteConfigurationAsync(
                jdcFolderId: bankServicesProviderId,
                releve: releve);

            
            throw new NotImplementedException();
        }

        public async Task<Company> CreateFolderAsync(Company company)
        {
            var dossierClient = company.ToDossierClient();

            var createdFolder = await this.jedeclareClient.CreateFolderAsync(dossierClient);

            return createdFolder.ToCompany();
        }

        public async Task<byte[]> GetMandatPdfAsync(string jdcFolderId, string jdcRibId)
        {
            return await this.jedeclareClient.GetMandatPdfAsync(jdcFolderId, jdcRibId);
        }
    }
}