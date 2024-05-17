// <copyright file="MandateManager.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("KPMG.Pulse.Back.Accounting.Mandate.Application.Tests")]

namespace KPMG.Pulse.Back.Accounting.Mandate.Application
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    public class MandateManager : IMandateManager
    {
        private readonly IDatabaseService databaseService;
        private readonly ICompanyManager companyManager;
        private readonly IJeDeclareService jeDeclareService;
        private readonly IAsposeHelper asposeHelper;
        private readonly INotificationsService notificationsService;
        private readonly IOptions<MandateEmailOptions> options;
        private readonly ILogger<MandateManager> logger;

        public MandateManager(IDatabaseService databaseService, ICompanyManager companyManager, IJeDeclareService jeDeclareService, IAsposeHelper asposeHelper, INotificationsService notificationsService, IOptions<MandateEmailOptions> options, ILogger<MandateManager> logger)
        {
            this.databaseService = databaseService;
            this.companyManager = companyManager;
            this.jeDeclareService = jeDeclareService;
            this.asposeHelper = asposeHelper;
            this.notificationsService = notificationsService;
            this.options = options ?? throw new ArgumentNullException(nameof(options));
            this.logger = logger;
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
                toAdd,
                rib,
                dossierClient.BankServicesProviderId!);

            // save Signatory
            await this.databaseService.SaveSignatoryAsync(null, collectionId, mandateCreation.Signatory, mandateCreation.Address);

            // crétaion JeDeclareCollection coté sql
            await this.databaseService.InsertServicesProviderIds(collectionId, createdReleveId, rib?.BbanServicesProviderId!);

            // Creation mandate Status 10
            await this.databaseService.CreateStatusAsync(collectionId, (int)JdcCollectionStatus.Activation_Requested_Collection_Pending);

            return collectionId;
        }

        public async Task<PagedMandate> GetAllCollectionsAsync(CollectionQueryDto query)
        {
            Collaborator collaborator = await this.databaseService.GetCollaboratorByEmail(query!.CollaboratorEmail);
            return await this.databaseService.GetAllCollectionsAsync(query, collaborator.Id).ConfigureAwait(false);
        }

        public async Task<PagedTechnicalMandate> GetAllTechnicalCollectionsAsync(CollectionQueryDto query)
        {
            return await this.databaseService.GetAllTechnicalCollectionsAsync(query).ConfigureAwait(false);
        }

        public async Task RefreshMandatsStatusesAsync(List<TechnicalCollection> mandats)
        {
            foreach (var mandat in mandats!)
            {
                var jdcCollection = await this.GetJdcCollectionAsync(mandat);
                if (jdcCollection == null)
                {
                    continue;
                }

                var status = await this.databaseService.GetRefStatusCodeByJdcCodeAsync(jdcCollection.StatusCode);
                var collection = await this.databaseService.GetCollectionById(mandat.Id);

                if (!HasStatusChanged((int)status.StatusCode, (int)collection.Status.StatusCode))
                {
                    continue;
                }

                await this.UpdateCollectionStatusAsync(collection, jdcCollection.StatusCode);
            }
        }

        public async Task<string?> UploadSignedMandateAsync(Guid collectionId, Stream mandateFileStream, string userEmail)
        {
            using var memoryStream = new MemoryStream();
            await mandateFileStream.CopyToAsync(memoryStream);
            byte[] fileBytes = memoryStream.ToArray()!;

            var collection = await this.databaseService.GetCollectionById(collectionId);
            var isJdcPartner = IsJdcPartner(collection);

            if (isJdcPartner)
            {
                var signedMandateContent = await this.jeDeclareService.UploadSignedMandate(collection, fileBytes);
                var folderId = collection!.Company?.BankServicesProviderId;
                var ribId = collection!.Bban?.BbanServicesProviderId;
                var isUploaded = await this.jeDeclareService.CheckSignedMandatExists(folderId!, ribId!);

                if (!string.IsNullOrEmpty(signedMandateContent) && isUploaded)
                {
                    await this.databaseService.CreateStatusAsync(collection.Id, (int)JdcCollectionStatus.Activation_Requested_Signed_Mandate_Uploaded);
                }
                else
                {
                    this.logger.LogError(
                        "{MethodName}, the upload of the signed mandate = {CollectionId} / folderId = {FolderId} and ribId = {RibId} failed / isUploaded = {IsUploaded}, signedMandateContent = {SignedMandateContent}",
                        nameof(this.UploadSignedMandateAsync),
                        collection.Id,
                        folderId,
                        ribId,
                        isUploaded,
                        string.IsNullOrEmpty(signedMandateContent));
                }

                return signedMandateContent;
            }
            else
            {
                string fileName = $@"uploaded-signed-mandate-{collection.Id}.pdf";
                string fileContent = Convert.ToBase64String(fileBytes);
                var emailCommand = EmailCommandBuilder.CreateSignedMandateUploadedEmail(collection, this.options.Value, fileContent, fileName, userEmail);
                await this.notificationsService.SendEmailAsync(emailCommand);
                await this.databaseService.CreateStatusAsync(collection.Id, (int)JdcCollectionStatus.Activation_Requested_Signed_Mandate_Uploaded);
                return null;
            }
        }

        public async Task<byte[]> DownloadUnsignedAsync(Guid id)
        {
            var collection = await this.databaseService.GetCollectionById(id);
            var isJdcPartner = IsJdcPartner(collection);

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
            ValidatePartnerCollection(collection!);

            return await this.jeDeclareService.GetSignedMandatPdfAsync(folderId!, ribId!);
        }

        public async Task<bool> DeactivateCollectionAsync(Guid collectionId, string userEmail)
        {
            var collection = await this.databaseService.GetCollectionById(collectionId);

            var emailCommand = EmailCommandBuilder.CreateMandateCancellationEmail(collection, this.options.Value, userEmail);
            await this.notificationsService.SendEmailAsync(emailCommand);

            return collection.Id != Guid.Empty;
        }

        public async Task InsertFormIOCollectionAsync(Collection collection)
        {
            var siret = collection.Company!.SiretNumber;
            var companyId = (await this.databaseService.GetCompanyBySiretAsync(siret)).Id;
            if (await this.databaseService.CheckCollecteConfigExistAsync(collection.Bban!))
            {
                throw new ApplicationException($"Il existe une configuration de collecte pour ce RIB {StringExtensions.Concat(collection.Bban!.BankCode, collection.Bban!.BranchCode, collection.Bban!.AccountNumber, collection.Bban!.CheckDigits)}.");
            }
            else
            {
                await this.databaseService.InsertFormIOCollectionAsync(collection, companyId);
            }
        }

        internal async Task<byte[]> DownloadPdfForJdcPartner(Collection collection)
        {
            var folderId = collection?.Company?.BankServicesProviderId;
            var ribId = collection?.Bban?.BbanServicesProviderId;
            ValidatePartnerCollection(collection!);
            var pdfBytes = await this.jeDeclareService.GetMandatPdfAsync(folderId!, ribId!);

            using var stream = new MemoryStream(pdfBytes);
            return this.asposeHelper.DeleteFirstPageFromPdf(stream);
        }

        private static bool IsJdcPartner(Collection collection)
        {
            return collection.Bban?.Bank?.JdcAgreement.JdcPartnership == JdcPartnership.Partner;
        }

        private static void ValidatePartnerCollection(Collection collection)
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

        private static bool MatchesMandat(TechnicalCollection jdcCollection, TechnicalCollection mandat)
        {
            bool ribIdMatches = jdcCollection.RibId == mandat.RibId;
            bool bankAndBranchMatches = jdcCollection.BankDetails.BankCode == mandat.BankDetails.BankCode && jdcCollection.BankDetails.BranchCode == mandat.BankDetails.BranchCode;
            bool accountDetailsMatches = jdcCollection.BankDetails.AccountNumber == mandat.BankDetails.AccountNumber && jdcCollection.BankDetails.CheckDigits == mandat.BankDetails.CheckDigits;

            return ribIdMatches && bankAndBranchMatches && accountDetailsMatches;
        }

        private static bool HasStatusChanged(int currentStatusCode, int previousStatusCode)
        {
            return currentStatusCode != previousStatusCode;
        }

        private async Task<TechnicalCollection?> GetJdcCollectionAsync(TechnicalCollection mandat)
        {
            var jdcCollections = await this.jeDeclareService.GetAllConfigurationFromFolderAsync(mandat.FolderId);
            var jdcCollection = jdcCollections?.SingleOrDefault(c => MatchesMandat(c, mandat));

            if (jdcCollection == null)
            {
                this.logger.LogError("The collection with Id={CollectionId} is not found in jedeclare", mandat.Id);
            }

            return jdcCollection;
        }

        private async Task UpdateCollectionStatusAsync(Collection collection, string jdcStatusCodeStr)
        {
            if (!int.TryParse(jdcStatusCodeStr, out int jdcStatusCode))
            {
                // Handle the parse failure. For example, log an error and return from the method.
                this.logger.LogError("Failed to parse JDC status code '{JdcStatusCodeStr}' for collection ID {CollectionId}.", jdcStatusCodeStr, collection.Id);
                return;
            }

            var newStatus = await this.databaseService.CreateStatusAsync(collection.Id, statusCode: jdcStatusCode!);
            var jdcStatusCodePending = await this.databaseService.CheckJdcStatusCodeIsPendingAsync(collection.Id);

            if (newStatus != null && jdcStatusCodePending)
            {
                await this.UpdateStatusOnSignedMandateUploadAsync(collection);
            }
        }

        private async Task UpdateStatusOnSignedMandateUploadAsync(Collection collection)
        {
            var folderId = collection.Company?.BankServicesProviderId;
            var ribId = collection.Bban?.BbanServicesProviderId;
            var isUploaded = await this.jeDeclareService.CheckSignedMandatExists(folderId!, ribId!);

            if (isUploaded)
            {
                await this.databaseService.CreateStatusAsync(collection.Id, (int)JdcCollectionStatus.Activation_Requested_Signed_Mandate_Uploaded);
            }
            else
            {
                this.logger.LogInformation(
                    "{MethodName}, the upload of the signed mandate = {CollectionId} / folderId = {FolderId} and ribId = {RibId} failed / isUploaded = {IsUploaded}",
                    nameof(this.UpdateStatusOnSignedMandateUploadAsync),
                    collection.Id,
                    folderId,
                    ribId,
                    isUploaded);
            }
        }

        private async Task<byte[]> GeneratePdfForNonPartner(Collection collection)
        {
            return await this.asposeHelper.GeneratePdfFromTemplateAsync(collection);
        }
    }
}