// <copyright file="MandateManager.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Application
{
    using KPMG.Pulse.Back.Accounting.Mandate.Adapters;
    using KPMG.Pulse.Back.Accounting.Mandate.JeDeclare.Client;
    using KPMG.Pulse.Back.Accounting.Mandate.Sql;

    public class MandateManager : IMandateManager
    {
        private readonly IDatabaseService databaseService;
        private readonly ICompanyManager companyManager;
        private readonly IJeDeclareClient jeDeclareProvider;
        private readonly string historyDateEnabledBanks = string.Empty;

        public MandateManager(IDatabaseService databaseService, ICompanyManager companyManager, IJeDeclareClient jeDeclareProvider)
        {
            this.databaseService = databaseService;
            this.companyManager = companyManager;
            this.jeDeclareProvider = jeDeclareProvider;
        }

        public async Task<Collection> CreateMandate(MandateCreationDto mandateCreation)
        {
            Company company = await this.companyManager.GetCompanyByErpId(mandateCreation.ErpId);

            if (company == null)
            {
                throw CompanyNotFoundException.FromId(mandateCreation.ErpId);
            }

            Bank bank = await this.databaseService.GetBankByCodeAsync(mandateCreation.Bban.BankCode);

            if (bank == null)
            {
                throw BankCodeNotFoundException.FromId(mandateCreation.Bban.BankCode);
            }

            DossierClient dossierClient =
                await this.jeDeclareProvider.CreateFolderAsync(
                    string.Empty,
                    company.ToDossierClient());

            // Création du dossier coté SQL
            await this.databaseService.CreateFolderAsync(dossierClient.Client?.Id!, company.Id);

            // Verification du bank partenaire ou non partenaire
            if (!bank.JdcAgreement.IsJdcPartner && string.IsNullOrWhiteSpace(bank.EbicsCardId))
            {
                throw new ApplicationException($"L'établissement bancaire {bank.Code} n'est pas partenaire de JeDeclare.com mais est défini sans connexion à une carte EBICs.");
            }

            // vérifier si la collecte existe
            if (await this.databaseService.CheckCollecteConfigExist(mandateCreation.Bban))
            {
                throw new ApplicationException($"Il existe une configuration de collecte pour ce RIB {StringExtensions.Concat(mandateCreation.Bban.BankCode, mandateCreation.Bban.BranchCode, mandateCreation.Bban.AccountNumber, mandateCreation.Bban.CheckDigits)}.");
            }

            // Création du rib coté jeDeclare
            var rib = await this.jeDeclareProvider.AddRibToFolderAsync(
                string.Empty,
                dossierClient.Client?.Id!,
                mandateCreation.ToRibClient());

            // Création de la collecte
            Collection collection = await this.databaseService.CreateCollection(mandateCreation);

            // Creation du Status -1
            Status initStatus = new Status(CollectionStatus.ToDo, "En cours");
            await this.databaseService.CreateStatus(collection.Id, initStatus);

            // création de la collecte coté jeDeclare
            Releve createdReleve = await this.jeDeclareProvider.CreateCollecteConfigurationAsync(
                string.Empty,
                dossierClient.Client?.Id!,
                rib.ConstructReleve(bank.Code, bank.EbicsCardId, this.historyDateEnabledBanks));

            // crétaion JeDeclareCollection coté sql
            await this.databaseService.CreateJeDeclareCollection(collection.Id, createdReleve.Id!, createdReleve.Id!);

            // Creation mandate Status 10
            Status createdStatus = new Status(CollectionStatus.InProgress, "Actif");
            await this.databaseService.CreateStatus(collection.Id, createdStatus);

            return createdReleve.ToModel(company, bank, collection);
        }

        public async Task<IEnumerable<Collection>> GetAllCollectionsAsync(CollectionQueryDto query)
        {
            return await this.databaseService.GetAllCollectionsAsync(query).ConfigureAwait(false);
        }
    }
}