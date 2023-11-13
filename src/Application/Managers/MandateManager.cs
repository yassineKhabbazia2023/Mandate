// <copyright file="MandateManager.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Application
{
    public class MandateManager : IMandateManager
    {
        private readonly IDatabaseService databaseService;
        private readonly ICompanyManager companyManager;
        private readonly IJeDeclareService jeDeclareService;

        public MandateManager(IDatabaseService databaseService, ICompanyManager companyManager, IJeDeclareService jeDeclareService)
        {
            this.databaseService = databaseService;
            this.companyManager = companyManager;
            this.jeDeclareService = jeDeclareService;
        }

        public async Task<Guid> CreateMandate(MandateCreation mandateCreation)
        {
            Company company = await this.companyManager.GetCompanyByErpId(mandateCreation.ErpId);

            Bank bank = await this.databaseService.GetBankByCodeAsync(mandateCreation.Bban.BankCode);

            Company dossierClient = await this.jeDeclareService.CreateFolderAsync(company);

            // Création du dossier coté SQL
            await this.databaseService.CreateFolderAsync(dossierClient.BankServicesProviderId!, dossierClient.Id);

            // Verification du bank partenaire ou non partenaire
            if (bank.JdcAgreement.JdcPartnership != JdcPartnership.Partner && string.IsNullOrWhiteSpace(bank.EbicsCardId))
            {
                throw new ApplicationException($"L'établissement bancaire {bank.Code} n'est pas partenaire de JeDeclare.com mais est défini sans connexion à une carte EBICs.");
            }

            // vérifier si la collecte existe
            if (await this.databaseService.CheckCollecteConfigExist(mandateCreation.Bban))
            {
                throw new ApplicationException($"Il existe une configuration de collecte pour ce RIB {StringExtensions.Concat(mandateCreation.Bban.BankCode, mandateCreation.Bban.BranchCode, mandateCreation.Bban.AccountNumber, mandateCreation.Bban.CheckDigits)}.");
            }

            // Création du rib coté jeDeclare
            Bban rib = await this.jeDeclareService.AddRibToFolderAsync(
                dossierClient.BankServicesProviderId,
                mandateCreation.Bban,
                mandateCreation.Signatory);

            // Création de la collecte
            Collection collection = await this.databaseService.CreateCollection(mandateCreation.ErpId, dossierClient.Id, mandateCreation.Bban);

            // Creation du Status -1
            Status initStatus = new Status(CollectionStatus.ToDo, "En Cours");
            await this.databaseService.CreateStatus(collection.Id, initStatus);

            // création de la collecte coté jeDeclare
            Collection createdReleve = await this.jeDeclareService.CreateCollecteConfigurationAsync(
                dossierClient.BankServicesProviderId!,
                rib);

            // modification collect pour LinkType
            await this.databaseService.UpdateCollection(collection.Id, createdReleve);

            // crétaion JeDeclareCollection coté sql
            await this.databaseService.InsertServicesProviderIds(collection.Id, createdReleve?.CollectionServicesProviderId!, rib?.BbanServicesProviderId!);

            // Creation mandate Status 10
            Status createdStatus = new Status(CollectionStatus.InProgress, "Actif");
            await this.databaseService.CreateStatus(collection.Id, createdStatus);

            return collection.Id!;
        }

        public async Task<IEnumerable<Collection>> GetAllCollectionsAsync(CollectionQueryDto query)
        {
            return await this.databaseService.GetAllCollectionsAsync(query).ConfigureAwait(false);
        }

        public Task<Collaborator> GetCollaboratorByEmail(string email)
        {
            throw new NotImplementedException();
        }
    }
}