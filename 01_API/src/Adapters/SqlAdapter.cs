// <copyright file="SqlAdapter.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Adapters
{
    using KPMG.Pulse.Back.Accounting.Mandate.Sql;

    public class SqlAdapter : IDatabaseService
    {
        private readonly Sql.IMandateRepository mandateRepository;
        private readonly Sql.IPaymentPreferenceRepository? paymentPreferenceRepository;
        private readonly Sql.ISepaMandateRepository? sepaMandateRepository;

        public SqlAdapter(Sql.IMandateRepository mandateRepository)
        {
            this.mandateRepository = mandateRepository;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlAdapter"/> class with payment preference support.
        /// </summary>
        /// <param name="mandateRepository">The mandate repository.</param>
        /// <param name="paymentPreferenceRepository">The payment preference repository.</param>
        public SqlAdapter(
            Sql.IMandateRepository mandateRepository,
            Sql.IPaymentPreferenceRepository paymentPreferenceRepository)
            : this(mandateRepository)
        {
            this.paymentPreferenceRepository = paymentPreferenceRepository;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlAdapter"/> class with payment preference and SEPA mandate support.
        /// </summary>
        /// <param name="mandateRepository">The mandate repository.</param>
        /// <param name="paymentPreferenceRepository">The payment preference repository.</param>
        /// <param name="sepaMandateRepository">The SEPA mandate repository.</param>
        public SqlAdapter(
            Sql.IMandateRepository mandateRepository,
            Sql.IPaymentPreferenceRepository paymentPreferenceRepository,
            Sql.ISepaMandateRepository sepaMandateRepository)
            : this(mandateRepository, paymentPreferenceRepository)
        {
            this.sepaMandateRepository = sepaMandateRepository;
        }

        public async Task<Guid> GetCollectionIfAlreadyExistingInIncidentStatus(string bankCode, string branchCode, string accountNumber, string erpId)
        {
            var collectionId = await this.mandateRepository.GetCollectionIfAlreadyExistingInIncidentStatus(bankCode, branchCode, accountNumber, erpId);
            return collectionId;
        }

        public async Task<Company> GetCompanyBySiretAsync(string siret)
        {
            var companyDb = await this.mandateRepository.GetCompanyBySiretAsync(siret).ConfigureAwait(false);

            var address = CreateAddressFromDb(companyDb!.Personal);
            var signatory = CreateSignatoryFromDb(companyDb!.Personal);

            return new Company(companyDb.Id, companyDb.Name, companyDb.SiretNumber, companyDb.ErpId, companyDb.JeDeclareFolder?.JdcDossierId, signatory, address);
        }

        public async Task<Bank> GetBankByCodeAsync(string bankCode)
        {
            try
            {
                var refBankDb = await this.mandateRepository.GetRefBankByCodeAsync(bankCode).ConfigureAwait(false);
                return refBankDb.ToModel();
            }
            catch (BankCodeNotFoundException e)
            {
                throw new Mandate.CustomBankCodeNotFoundException((Mandate.ExceptionType)ExceptionType.BankCodeNotFound, e.Message);
            }
        }

        public async Task<Status> GetRefStatusCodeByJdcCodeAsync(string jdcStatusCode)
        {
            var statusDb = await this.mandateRepository.GetRefStatusCodeByJdcCodeAsync(jdcStatusCode).ConfigureAwait(false);
            return statusDb.ToModel();
        }

        public async Task<byte[]> GetPdfTemplateByBankCodeAsync(string bankCode)
        {
            return await this.mandateRepository.GetPdfTemplateByCodeAsync(bankCode).ConfigureAwait(false);
        }

        public async Task<PagedMandate> GetAllCollectionsAsync(CollectionQueryDto query, int collaboratorId)
        {
            (List<Sql.CollectionDb>, int) tuple = await this.mandateRepository.SearchCollectionsAsync(query.ToSql(collaboratorId));

            return new PagedMandate(
                new Counters(tuple.Item2, 0, 0, 0, 0, 0),
                tuple.Item1.Select(i => i.ToModel()).ToList());
        }

        public async Task<PagedTechnicalMandate> GetAllTechnicalCollectionsAsync(CollectionQueryDto query)
        {
            (List<Sql.CollectionDb>, int) tuple = await this.mandateRepository.SearchCollectionsAsync(query.ToSql(default));

            return new PagedTechnicalMandate(
                new Counters(tuple.Item2, 0, 0, 0, 0, 0),
                tuple.Item1.Select(i => i.ToModel()).ToList());
        }

        public async Task<Company> GetCompanyByErpIdAsync(string erpId, string userEmail)
        {
            var company = await this.mandateRepository.GetCompanyByErpIdAsync(erpId, userEmail);
            return company.ToModel();
        }

        public async Task<Company> GetCompanyByErpIdAsync(string erpId, int contactId)
        {
            var company = await this.mandateRepository.GetCompanyByErpIdAsync(erpId, contactId);
            return company.ToModel();
        }

        public async Task CreateOrUpdateFolderAsync(string bankServicesProviderId, int companyId)
        {
            await this.mandateRepository.CreateOrUpdateFolderAsync(bankServicesProviderId, companyId);
        }
      
        public async Task<Status?> CreateStatusWithMessageAsync(Guid collectionId, int statusCode, string errorMessage)
        {
            return await InnerCreateStatusAsync(collectionId, statusCode, errorMessage);
        }

        public async Task<Status?> CreateStatusAsync(Guid collectionId, int statusCode)
        {
            return await InnerCreateStatusAsync(collectionId, statusCode);
        }

        public async Task<bool> CheckJdcStatusCodeIsPendingAsync(Guid collectionId)
        {
            return await this.mandateRepository.CheckJdcStatusCodeIsPendingAsync(collectionId);
        }

        public async Task InsertServicesProviderIds(Guid collectionId, string collectionServicesProviderId, string bbanServicesProviderId)
        {
            // Création de JeDeclare Collection
            await this.mandateRepository.InsertServicesProviderIdsAsync(collectionId, collectionServicesProviderId, bbanServicesProviderId);
        }

        public async Task<bool> CheckCollecteConfigExistAsync(Bban bban)
        {
            return await this.mandateRepository.CheckCollecteConfigExistAsync(bban!.BankCode, bban!.BranchCode, bban!.AccountNumber);
        }

        public async Task<Guid> CreateCollectionAsync(Bban bban, int companyId, int? createdBy, string? destinationTool)
        {
            CollectionDb collection = bban.ToSql(companyId, createdBy);
            collection.Statuses = new List<StatusDb>() { SqlExtensions.CreationInProgress() };
            collection.DestinationTool = destinationTool;
            return (await this.mandateRepository.CreateCollectionAsync(collection)).Id;
        }

        public Task<Collection> UpdateCollection(Guid id, Collection collection)
        {
            throw new NotImplementedException();
        }

        public async Task<Collaborator?> GetCollaboratorByEmail(string collaboratorEmail)
        {
            var collabDb = await this.mandateRepository.GetCollaboratorByEmailAsync(collaboratorEmail).ConfigureAwait(false);

            return collabDb?.ToModel();
        }

        public async Task<Collaborator?> GetCollaboratorById(int contactId)
        {
            var collabDb = await this.mandateRepository.GetContactByIdAsync(contactId).ConfigureAwait(false);

            return collabDb?.ToModel();
        }

        public async Task<Collection> GetCollectionById(Guid collectionId)
        {
            var collectionDb = await this.mandateRepository.GetCollectionById(collectionId).ConfigureAwait(false);
            return collectionDb.ToModel();
        }

        public async Task SaveSignatoryAsync(int? companyId, Guid? collectionId, Signatory signatory, Address address)
        {
            var newPersonalDb = new PersonalDb()
            {
                CompanyId = companyId,
                CollectionId = collectionId,
                City = address!.City,
                Country = address!.Country,
                Street = address!.Street,
                ZipCode = address!.ZipCode,
                Complements = address!.Complements,
                FirstName = signatory!.FirstName,
                LastName = signatory!.LastName,
                Email = signatory!.Email,
                Title = signatory!.Title,
            };

            await this.mandateRepository.SaveSignatoryAsync(newPersonalDb);
        }

        public async Task CreateFakeRefAsync()
        {
            await this.mandateRepository.CreateFakeRefAsync().ConfigureAwait(false);
        }

        public async Task DeleteFakeRefAsync()
        {
            await this.mandateRepository.DeleteFakeRefAsync().ConfigureAwait(false);
        }

        public async Task CreateFakeAuthAsync()
        {
            await this.mandateRepository.CreateFakeAuthAsync().ConfigureAwait(false);
        }

        public async Task DeleteFakeAuthAsync()
        {
            await this.mandateRepository.DeleteFakeAuthAsync().ConfigureAwait(false);
        }

        public async Task AddFakeDataAsync()
        {
            await this.mandateRepository.AddFakeDataAsync().ConfigureAwait(false);
        }

        public async Task DeleteFakeDataAsync()
        {
            await this.mandateRepository.DeleteFakeDataAsync().ConfigureAwait(false);
        }

        public async Task<Company> GetCompanyByErpIdSiretAsync(string erpId, string siret)
        {
            try
            {
                var company = await this.mandateRepository.GetCompanyByErpIdSiretAsync(erpId, siret);
                return company.ToModel();
            }
            catch (CustomCompanyNotFoundException e)
            {
                throw new Mandate.CustomCompanyNotFoundException((Mandate.ExceptionType)e.Type, e.Message);
            }
        }

        public async Task InsertMandateLogAsync(Collection collection, CustomException exception)
        {
            MandateLogDb mandateLog = collection.ToMandateLogDB(exception);
            await this.mandateRepository.InsertMandateLogAsync(mandateLog);
        }

        public async Task SaveMandateCreationLogMessageAsync(MandateCreationLogMessage mandateCreationLogMessage)
        {
            var messageDb = mandateCreationLogMessage.ToMandateCreationLogMessageDB();
            await this.mandateRepository.SaveMandateCreationLogMessageAsync(messageDb);
        }

        /// <inheritdoc />
        public async Task<bool> AccountExistsAsync(int accountId)
        {
            return await this.GetPaymentPreferenceRepository().AccountExistsAsync(accountId);
        }

        /// <inheritdoc />
        public async Task<PaymentPreference?> GetPaymentPreferenceByAccountIdAsync(int accountId)
        {
            var preference = await this.GetPaymentPreferenceRepository().GetByAccountIdAsync(accountId);
            return preference is null
                ? null
                : new PaymentPreference(
                    preference.Id,
                    preference.AccountId,
                    preference.PaymentType,
                    preference.CreatedAt,
                    preference.CreatedBy);
        }

        /// <inheritdoc />
        public async Task SavePaymentPreferenceAsync(PaymentPreference paymentPreference)
        {
            ArgumentNullException.ThrowIfNull(paymentPreference);

            await this.GetPaymentPreferenceRepository().SaveAsync(new Sql.PaymentPreferenceDb
            {
                Id = paymentPreference.Id,
                AccountId = paymentPreference.AccountId,
                PaymentType = paymentPreference.PaymentType,
                CreatedAt = paymentPreference.CreatedAt,
                CreatedBy = paymentPreference.CreatedBy
            });
        }

        /// <inheritdoc />
        public async Task SaveSepaMandateWithPaymentPreferenceAsync(
            SepaMandate sepaMandate,
            PaymentPreference paymentPreference)
        {
            ArgumentNullException.ThrowIfNull(sepaMandate);
            ArgumentNullException.ThrowIfNull(paymentPreference);

            await this.GetSepaMandateRepository().SaveWithPaymentPreferenceAsync(
                new Sql.SepaMandateDb
                {
                    Id = sepaMandate.Id,
                    AccountId = sepaMandate.AccountId,
                    RibDocumentId = sepaMandate.DocumentId,
                    AccountHolder = sepaMandate.AccountHolder,
                    Iban = sepaMandate.Iban,
                    Bic = sepaMandate.Bic,
                    Address = sepaMandate.Address,
                    SignatureRequestId = sepaMandate.SignatureRequestId,
                    SignatureUrl = sepaMandate.SignatureUrl,
                    SignatureStatus = (int)sepaMandate.SignatureStatus,
                    IsSentToAkuiteo = sepaMandate.IsSentToAkuiteo,
                    SentToAkuiteoAt = sepaMandate.SentToAkuiteoAt,
                    CreatedAt = sepaMandate.CreatedAt,
                    CreatedBy = sepaMandate.CreatedBy
                },
                new Sql.PaymentPreferenceDb
                {
                    Id = paymentPreference.Id,
                    AccountId = paymentPreference.AccountId,
                    PaymentType = paymentPreference.PaymentType,
                    CreatedAt = paymentPreference.CreatedAt,
                    CreatedBy = paymentPreference.CreatedBy
                });
        }

        /// <summary>
        /// Gets the configured payment preference repository.
        /// </summary>
        /// <returns>The payment preference repository.</returns>
        private Sql.IPaymentPreferenceRepository GetPaymentPreferenceRepository()
        {
            return this.paymentPreferenceRepository
                ?? throw new InvalidOperationException("Payment preference repository is not configured.");
        }

        /// <summary>
        /// Gets the configured SEPA mandate repository.
        /// </summary>
        /// <returns>The SEPA mandate repository.</returns>
        private Sql.ISepaMandateRepository GetSepaMandateRepository()
        {
            return this.sepaMandateRepository
                ?? throw new InvalidOperationException("SEPA mandate repository is not configured.");
        }

        private async Task<Status?> InnerCreateStatusAsync(Guid collectionId, int statusCode, string? errorMessage = null)
        {
            var currentJdcStatusCode = await this.mandateRepository.GetCurrentJdcStatusCodeAsync(collectionId);

            if (IsCurrentJdcSignedMandateUploadedAndNewJdcPending(currentJdcStatusCode!.StatusCode, statusCode))
            {
                return null;
            }

            // update current Status to false
            await this.mandateRepository.UpdateCurrentStatusAsync(collectionId);

            // select de la ref pour avoir
            StatusDb statusDb = new Sql.StatusDb()
            {
                CollectionId = collectionId,
                IsCurrent = true,
                StatusCode = statusCode,
                StatusDate = DateTime.UtcNow,
                CreatedBy = string.Empty,
                ErrorMessage = errorMessage,
            };

            // Création d'un status relié a une collecte
            return (await this.mandateRepository.CreateStatusAsync(collectionId, statusDb)).ToModel();
        }

        private static Address CreateAddressFromDb(PersonalDb? personal)
        {
            return new Address(personal?.Street, personal?.Complements, personal?.ZipCode, personal?.City, personal?.Country);
        }

        private static Signatory CreateSignatoryFromDb(PersonalDb? personal)
        {
            return new Signatory(personal?.Title, personal?.FirstName, personal?.LastName, personal?.Email);
        }

        private static bool IsCurrentJdcSignedMandateUploadedAndNewJdcPending(int currentJdcStatusCode, int? newJdcStatusCode)
        {
            return newJdcStatusCode == (int)JdcCollectionStatus.Activation_Requested_Collection_Pending
                    && currentJdcStatusCode == (int)JdcCollectionStatus.Activation_Requested_Signed_Mandate_Uploaded;
        }
    }
}
