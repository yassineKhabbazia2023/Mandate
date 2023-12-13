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
        private readonly IAsposeHelper asposeHelper;

        public MandateManager(IDatabaseService databaseService, ICompanyManager companyManager, IJeDeclareService jeDeclareService, IAsposeHelper asposeHelper)
        {
            this.databaseService = databaseService;
            this.companyManager = companyManager;
            this.jeDeclareService = jeDeclareService;
            this.asposeHelper = asposeHelper;
        }

        public async Task<Guid> CreateMandate(CollectionCreationCommand mandateCreation)
        {
            Company company = await this.companyManager.GetCompanyByErpIdAsync(mandateCreation.ErpId);

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

            // save Signatory
            await this.databaseService.SaveSignatoryAsync(company.Id, collection.Id, mandateCreation.Signatory, mandateCreation.Address);

            return collection.Id!;
        }

        public async Task<PagedMandate> GetAllCollectionsAsync(CollectionQueryDto query)
        {
            return await this.databaseService.GetAllCollectionsAsync(query).ConfigureAwait(false);
        }

        public async Task<string?> UploadSignedMandateAsync(Guid collectionId, Stream mandateFileStream)
        {
            using var memoryStream = new MemoryStream();
            await mandateFileStream.CopyToAsync(memoryStream);
            byte[] fileBytes = memoryStream.ToArray() !;

            var collection = await this.databaseService.GetCollectionById(collectionId);
            var isJdcPartner = this.IsJdcPartner(collection);

            if (isJdcPartner)
            {
                return await this.jeDeclareService.UploadSignedMandate(collection, fileBytes);
            }

            return null;
        }

        public async Task<byte[]> DownloadUnsignedAsync(Guid id)
        {
            var collection = await this.databaseService.GetCollectionById(id);
            var isJdcPartner = this.IsJdcPartner(collection);

            if (isJdcPartner)
            {
                return await this.DownloadPdfForJdcPartner(collection);
            }
            else
            {
                return await this.GeneratePdfForNonPartner(collection);
            }
        }

        private bool IsJdcPartner(Collection collection)
        {
            return collection.Bban?.Bank?.JdcAgreement.JdcPartnership == JdcPartnership.Partner;
        }

        private void ValidatePartnerCollection(Collection collection)
        {
            var folderId = collection.Company?.BankServicesProviderId;
            var ribId = collection.Bban?.BbanServicesProviderId;

            if (string.IsNullOrEmpty(folderId))
            {
                throw new FolderIdEmptyOrNullException();
            }

            if (string.IsNullOrEmpty(ribId))
            {
                throw new RibIdEmptyOrNullException();
            }
        }

        private async Task<byte[]> DownloadPdfForJdcPartner(Collection collection)
        {
            var folderId = collection?.Company?.BankServicesProviderId;
            var ribId = collection?.Bban?.BbanServicesProviderId;
            this.ValidatePartnerCollection(collection!);
            return await this.jeDeclareService.GetMandatPdfAsync(folderId!, ribId!);
        }

        private async Task<byte[]> GeneratePdfForNonPartner(Collection collection)
        {
            return await this.asposeHelper.GeneratePdfFromTemplateAsync(collection);
        }
    }
}