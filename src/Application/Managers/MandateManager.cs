// <copyright file="MandateManager.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Application
{
    using KPMG.Pulse.Back.Accounting.Mandate.Models.Enums;

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

            Company toAdd = new Company(
                company.Id,
                company.Name,
                company.SiretNumber,
                mandateCreation.ErpId,
                null,
                mandateCreation.Signatory,
                mandateCreation.Address);

            Company dossierClient = await this.jeDeclareService.CreateFolderAsync(toAdd);

            // Création du dossier coté SQL
            await this.databaseService.CreateOrUpdateFolderAsync(dossierClient.BankServicesProviderId!, company.Id);

            // Verification du bank partenaire ou non partenaire
            if (bank.JdcAgreement.JdcPartnership != JdcPartnership.Partner && string.IsNullOrWhiteSpace(bank.EbicsCardId))
            {
                throw new ApplicationException($"L'établissement bancaire {bank.Code} n'est pas partenaire de JeDeclare.com mais est défini sans connexion à une carte EBICs.");
            }

            // vérifier si la collecte existe
            if (await this.databaseService.CheckCollecteConfigExistAsync(mandateCreation.Bban))
            {
                throw new ApplicationException($"Il existe une configuration de collecte pour ce RIB {StringExtensions.Concat(mandateCreation.Bban.BankCode, mandateCreation.Bban.BranchCode, mandateCreation.Bban.AccountNumber, mandateCreation.Bban.CheckDigits)}.");
            }

            // Création du rib coté jeDeclare
            Bban rib = await this.jeDeclareService.AddRibToFolderAsync(
                dossierClient.BankServicesProviderId,
                mandateCreation,
                bank);

            // Création de la collecte
            Guid collectionId = await this.databaseService.CreateCollectionAsync(rib, company.Id);

            // création de la collecte coté jeDeclare
            string createdReleveId = await this.jeDeclareService.CreateCollecteConfigurationAsync(
                dossierClient,
                rib);

            // save Signatory
            await this.databaseService.SaveSignatoryAsync(null, collectionId, mandateCreation.Signatory, mandateCreation.Address);

            // crétaion JeDeclareCollection coté sql
            await this.databaseService.InsertServicesProviderIds(collectionId, createdReleveId, rib?.BbanServicesProviderId!);

            // Creation mandate Status 10
            await this.databaseService.CreateStatus(collectionId, (int)JdcCollectionStatus.Activation_Requested_Coollection_Pending);

            return collectionId;
        }

        public async Task<PagedMandate> GetAllCollectionsAsync(CollectionQueryDto query)
        {
            Collaborator collaborator = await this.databaseService.GetCollaboratorByEmail(query.CollaboratorEmail);
            return await this.databaseService.GetAllCollectionsAsync(query, collaborator.Id).ConfigureAwait(false);
        }

        public async Task<string?> UploadSignedMandateAsync(Guid collectionId, Stream mandateFileStream)
        {
            using var memoryStream = new MemoryStream();
            await mandateFileStream.CopyToAsync(memoryStream);
            byte[] fileBytes = memoryStream.ToArray()!;

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

        public async Task<byte[]> DownloadSignedAsync(Guid id)
        {
            var collection = await this.databaseService.GetCollectionById(id);

            var folderId = collection!.Company?.BankServicesProviderId;
            var ribId = collection!.Bban?.BbanServicesProviderId;
            this.ValidatePartnerCollection(collection!);

            return await this.jeDeclareService.GetSignedMandatPdfAsync(folderId!, ribId!);
        }

        public async Task<bool> DeactivateCollectionAsync(Guid collectionId)
        {
            var collection = await this.databaseService.GetCollectionById(collectionId);
            return await this.jeDeclareService.DeactivateCollection(collection);
        }

        public async Task InsertFormIOCollectionAsync(Collection collection)
        {
            var siret = collection.Company!.SiretNumber;
            var company = await this.databaseService.GetCompanyBySiretAsync(siret);
            if (await this.databaseService.CheckCollecteConfigExistAsync(collection.Bban!))
            {
                throw new ApplicationException($"Il existe une configuration de collecte pour ce RIB {StringExtensions.Concat(collection.Bban!.BankCode, collection.Bban!.BranchCode, collection.Bban!.AccountNumber, collection.Bban!.CheckDigits)}.");
            }
            else
            {
                await this.databaseService.InsertFormIOCollectionAsync(collection, company);
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