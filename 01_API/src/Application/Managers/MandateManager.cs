// <copyright file="MandateManager.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("KPMG.Pulse.Back.Accounting.Mandate.Application.Tests")]

namespace KPMG.Pulse.Back.Accounting.Mandate.Application
{
    using KPMG.Pulse.Back.Accounting.Mandate.Application.Interfaces;
    using KPMG.Pulse.Back.Accounting.Mandate.Sql;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    public class MandateManager : IMandateManager
    {
        private readonly IDatabaseService databaseService;
        private readonly IJeDeclareService jeDeclareService;
        private readonly IAsposeHelper asposeHelper;
        private readonly INotificationsService notificationsService;
        private readonly IOptions<MandateEmailOptions> options;
        private readonly ILogger<MandateManager> logger;
        private readonly IEventManager eventManager;

        public MandateManager(
            IDatabaseService databaseService,
            IJeDeclareService jeDeclareService,
            IAsposeHelper asposeHelper,
            INotificationsService notificationsService,
            IOptions<MandateEmailOptions> options,
            ILogger<MandateManager> logger,
            IEventManager eventManager)
        {
            this.databaseService = databaseService;
            this.jeDeclareService = jeDeclareService;
            this.asposeHelper = asposeHelper;
            this.notificationsService = notificationsService;
            this.options = options ?? throw new ArgumentNullException(nameof(options));
            this.logger = logger;
            this.eventManager = eventManager;
        }

        public async Task<Guid> CreateMandateAsync(CollectionCreationCommand mandateCreation, int contactId)
        {
            var company = await this.databaseService.GetCompanyByErpIdAsync(mandateCreation.ErpId, contactId);

            if (string.IsNullOrWhiteSpace(company.SiretNumber))
            {
                throw new CompanyHasNoSiretException($"La Compagnie {company.Name} - {company.ErpId} n'a pas de SIRET");
            }

            await this.CheckIfCollectionWithSameBbanAlreadyExistsAsync(mandateCreation);
            Bank bank = await this.GetBankAndVerifyParnershipAsync(mandateCreation);

            var collectionId = await this.databaseService.GetCollectionIfAlreadyExistingInIncidentStatus(mandateCreation.Bban.BankCode, mandateCreation.Bban.BranchCode, mandateCreation.Bban.AccountNumber, mandateCreation.ErpId);

            // if the collection has not been found, we can create a new collection normally.
            if (collectionId == Guid.Empty)
            {
                collectionId = await this.databaseService.CreateCollectionAsync(mandateCreation.Bban, company.Id, contactId);
            }
            // If the collection has been found with the status Incident, then we can update the status to Creation_InProgress.
            else
            {
                await this.databaseService.CreateStatusAsync(collectionId, (int)JdcCollectionStatus.Creation_InProgress);

            }

            var collaborator = await this.databaseService.GetCollaboratorById(contactId);

            var message = new MandateCreationMessage
            {
                Id = company.Id,
                Name = company.Name,
                SiretNumber = company.SiretNumber,
                ErpId = company.ErpId,
                Address = mandateCreation.Address,
                Signatory = mandateCreation.Signatory,
                BankServicesProviderId = null,
                Bank = bank,
                Bban = mandateCreation.Bban,
                Company = company,
                Collaborator = collaborator,
            };

            await this.eventManager.PublishCreateMandateAsync(message);

            return collectionId;
        }

        public async Task<PagedMandate> GetAllCollectionsAsync(CollectionQueryDto query)
        {
            Collaborator collaborator = await this.databaseService.GetCollaboratorByEmail(query!.CollaboratorEmail);

            if (collaborator is null)
            {
                throw new UnauthorizedAccessException("Collaborator not authorized or does not exist.");
            }

            return await this.databaseService.GetAllCollectionsAsync(query, collaborator.Id).ConfigureAwait(false);
        }

        public async Task<PagedTechnicalMandate> GetAllTechnicalCollectionsAsync(CollectionQueryDto query)
        {
            return await this.databaseService.GetAllTechnicalCollectionsAsync(query).ConfigureAwait(false);
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
                var emailCommand = EmailCommandBuilder.CreateSignedMandateUploadedEmail(userEmail, collection, this.options.Value, fileContent, fileName);
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

            var emailCommand = EmailCommandBuilder.CreateMandateCancellationEmail(userEmail, collection, this.options.Value);
            await this.notificationsService.SendEmailAsync(emailCommand);

            return collection.Id != Guid.Empty;
        }
        
        public async Task<CollectionStatus?> GetMandateStatusAsync(Guid collectionId)
        {
            var collection = await this.databaseService.GetCollectionById(collectionId);
            return collection.Status.StatusCode;
        }

        internal async Task<byte[]> DownloadPdfForJdcPartner(Collection collection)
        {
            var folderId = collection?.Company?.BankServicesProviderId;
            var ribId = collection?.Bban?.BbanServicesProviderId;
            ValidatePartnerCollection(collection!);
            return await this.jeDeclareService.GetMandatPdfAsync(folderId!, ribId!);
        }

        private async Task CheckIfCollectionWithSameBbanAlreadyExistsAsync(CollectionCreationCommand mandateCreation)
        {
            if (await this.databaseService.CheckCollecteConfigExistAsync(mandateCreation.Bban))
            {
                throw new JdcCollecteConfigExistException(
                    $"Il existe une configuration de collecte pour ce RIB {StringExtensions.Concat(mandateCreation.Bban.BankCode, mandateCreation.Bban.BranchCode, mandateCreation.Bban.AccountNumber, mandateCreation.Bban.CheckDigits)}.");
            }
        }

        private async Task<Bank> GetBankAndVerifyParnershipAsync(CollectionCreationCommand mandateCreation)
        {
            var bank = await this.databaseService.GetBankByCodeAsync(mandateCreation.Bban.BankCode);

            // Verification du bank partenaire ou non partenaire
            if (bank.JdcAgreement.JdcPartnership != JdcPartnership.Partner && string.IsNullOrWhiteSpace(bank.EbicsCardId))
            {
                throw new BankHasNoJdcPartnershipException($"L'établissement bancaire {bank.Code} n'est pas partenaire de JeDeclare.com mais est défini sans connexion à une carte EBICs.");
            }

            return bank;
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
        
        private async Task<byte[]> GeneratePdfForNonPartner(Collection collection)
        {
            return await this.asposeHelper.GeneratePdfFromTemplateAsync(collection);
        }
    }
}