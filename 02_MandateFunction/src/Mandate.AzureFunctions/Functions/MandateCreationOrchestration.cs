namespace KPMG.Pulse.Back.Accounting.Mandate.Function.Functions
{
    using System.Diagnostics.CodeAnalysis;
    using System.Net;
    using System.Text;
    using Azure.Messaging.ServiceBus;
    using KPMG.Pulse.Back.Accounting.Mandate.Application;
    using KPMG.Pulse.Back.Accounting.Mandate.Function.Models;
    using KPMG.Pulse.Back.Accounting.Mandate.JeDeclare.Client;
    using KPMG.Pulse.Back.Accounting.Mandate.Sql;
    using Microsoft.Azure.Functions.Worker;
    using Microsoft.DurableTask;
    using Microsoft.DurableTask.Client;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;

    public class MandateCreationOrchestration
    {
        private readonly IJeDeclareService jeDeclareClient;
        private readonly IMandateRepository mandateRepository;
        private readonly IDatabaseService databaseService;

        public MandateCreationOrchestration(IJeDeclareService jeDeclareClient, IMandateRepository mandateRepository, IDatabaseService databaseService)
        {
            this.jeDeclareClient = jeDeclareClient;
            this.mandateRepository = mandateRepository;
            this.databaseService = databaseService;
        }

        [Function(nameof(RunOrchestrator))]
        [ExcludeFromCodeCoverage]
        public async Task RunOrchestrator(
    [OrchestrationTrigger] TaskOrchestrationContext context,
    MandateCreationMessage message)
        {
            ILogger logger = context.CreateReplaySafeLogger(nameof(MandateCreationOrchestration));
            logger.LogInformation("Start orchestration.");

            var collectionId = await context.CallActivityAsync<Guid>(nameof(this.GetCollectionAndSaveMessage), message);
            var dossierClient = await context.CallActivityAsync<Company>(nameof(this.CreateJdcFolderAsync), new MandateCreationMessageAndCollectionId(message, collectionId));
            if (dossierClient == null)
            {
                return;
            }

            var collectionIdAndRib = await context.CallActivityAsync<CollectionIdAndRib>(nameof(this.AddRibToJdcFolderAsync), new MandateCreationMessageAndCompanyAndCollectionId(message, dossierClient, collectionId));
            await context.CallActivityAsync<CollectionIdAndRib>(nameof(this.StartCollectAsync), new MandateCreationMessageAndCompanyAndMore(message, dossierClient, collectionIdAndRib));
            await context.CallActivityAsync<object>(nameof(this.SaveSignatoryAndAddStatusAsync), new MandateCreationMessageAndCompanyAndMore(message, dossierClient, collectionIdAndRib));
        }

        [Function(nameof(GetCollectionAndSaveMessage))]
        public async Task<Guid> GetCollectionAndSaveMessage([ActivityTrigger] MandateCreationMessage message, FunctionContext executionContext)
        {
            ILogger logger = executionContext.GetLogger(nameof(GetCollectionAndSaveMessage));
            var collection = await mandateRepository.GetCollectionAsync(message.Bban.BankCode, message.Bban.BranchCode, message.Bban.AccountNumber, message.Bban.CheckDigits, message.Company.Id);

            if (collection is null)
            {
                logger.LogError("collection not found");
                throw new CollectionNotFoundException($"The collection with the company id {message.Company.Id} and accountNumber : {message.Bban.AccountNumber} is not found");
            }

            var messageToSave = new MandateCreationLogMessage(collection.Id, JsonConvert.SerializeObject(message));
            await this.databaseService.SaveMandateCreationLogMessageAsync(messageToSave);

            return collection.Id;
        }

        [Function(nameof(CreateJdcFolderAsync))]
        public async Task<Company?> CreateJdcFolderAsync([ActivityTrigger] MandateCreationMessageAndCollectionId message, FunctionContext executionContext)
        {
            Company dossierClient;

            try
            {
                dossierClient = await this.jeDeclareClient.CreateFolderAsync(message.MandateCreationMessage.Company, message.MandateCreationMessage.Address, message.MandateCreationMessage.Signatory);
            }
            catch (JeDeclareApiException ex) when (ex.HttpStatusCode.HasValue && new[] { HttpStatusCode.BadRequest, HttpStatusCode.PreconditionFailed }.Contains(ex.HttpStatusCode.Value))
            {
                await this.databaseService.CreateStatusWithMessageAsync(message.CollectionId, (int)JdcCollectionStatus.Creation_Failed, ex.Message);
                return null;
            }
            catch
            {
                await this.databaseService.CreateStatusAsync(message.CollectionId, (int)JdcCollectionStatus.Creation_Failed);
                throw;
            }

            // Création du dossier coté SQL
            await this.mandateRepository.CreateOrUpdateFolderAsync(dossierClient!.BankServicesProviderId!, message.MandateCreationMessage.Company.Id);

            return dossierClient;
        }

        [Function(nameof(AddRibToJdcFolderAsync))]
        public async Task<CollectionIdAndRib> AddRibToJdcFolderAsync([ActivityTrigger] MandateCreationMessageAndCompanyAndCollectionId messageAndCompany, FunctionContext executionContext)
        {
            var message = messageAndCompany.MandateCreationMessage;

            Bban rib;

            try
            {
                // Création du rib coté jeDeclare
                rib = await this.jeDeclareClient.AddRibToFolderAsync(
                    messageAndCompany.Company.BankServicesProviderId,
                    new CollectionCreationCommand(message.ErpId, message.Signatory, message.Address, message.Bban),
                    message.Bank);
            }
            catch (JeDeclareApiException ex) when (ex.HttpStatusCode == HttpStatusCode.BadRequest)
            {
                await this.databaseService.CreateStatusWithMessageAsync(messageAndCompany.CollectionId, (int)JdcCollectionStatus.Creation_Failed, ex.Message);
                throw;
            }
            catch
            {
                await this.databaseService.CreateStatusAsync(messageAndCompany.CollectionId, (int)JdcCollectionStatus.Creation_Failed);
                throw;
            }

            return new CollectionIdAndRib(messageAndCompany.CollectionId, rib);
        }

        [Function(nameof(StartCollectAsync))]
        public async Task<Company> StartCollectAsync([ActivityTrigger] MandateCreationMessageAndCompanyAndMore mandateCreationMessageAndCompany, FunctionContext executionContext)
        {
            ILogger logger = executionContext.GetLogger(nameof(StartCollectAsync));

            var servideProviderIds = await mandateRepository.GetServicesProviderIdsAsync(mandateCreationMessageAndCompany.CollectionIdAndRib.CollectionId);

            if (servideProviderIds is not null)
            {
                logger.LogInformation("Collect already exists for {CollectionId}", mandateCreationMessageAndCompany.CollectionIdAndRib.CollectionId);
                return mandateCreationMessageAndCompany.Company;
            }

            string createdReleveId;

            try
            {
                // Création de la collecte coté jeDeclare
                createdReleveId = await this.jeDeclareClient.CreateCollecteConfigurationAsync(
                mandateCreationMessageAndCompany.Company,
                mandateCreationMessageAndCompany.CollectionIdAndRib.Bban,
                mandateCreationMessageAndCompany.MandateCreationMessage.Signatory!,
                mandateCreationMessageAndCompany.Company.BankServicesProviderId!);
            }
            catch (JeDeclareApiException ex) when (ex.HttpStatusCode == HttpStatusCode.BadRequest)
            {
                await this.databaseService.CreateStatusWithMessageAsync(mandateCreationMessageAndCompany.CollectionIdAndRib.CollectionId, (int)JdcCollectionStatus.Creation_Failed, ex.Message);
                throw;
            }
            catch
            {
                await this.databaseService.CreateStatusAsync(mandateCreationMessageAndCompany.CollectionIdAndRib.CollectionId, (int)JdcCollectionStatus.Creation_Failed);
                throw;
            }

            // Création JeDeclareCollection coté sql
            await this.mandateRepository.InsertServicesProviderIdsAsync(mandateCreationMessageAndCompany.CollectionIdAndRib.CollectionId, createdReleveId, mandateCreationMessageAndCompany.CollectionIdAndRib.Bban?.BbanServicesProviderId!);

            return mandateCreationMessageAndCompany.Company;
        }

        [Function(nameof(SaveSignatoryAndAddStatusAsync))]
        public async Task SaveSignatoryAndAddStatusAsync([ActivityTrigger] MandateCreationMessageAndCompanyAndMore mandateCreationMessageAndCompany, FunctionContext executionContext)
        {
            ILogger logger = executionContext.GetLogger(nameof(SaveSignatoryAndAddStatusAsync));

            var signatory = await mandateRepository.GetCollectionSignatoryAsync(mandateCreationMessageAndCompany.CollectionIdAndRib.CollectionId);

            if (signatory is not null)
            {
                logger.LogInformation("Signatory already exists for {CollectionId}", mandateCreationMessageAndCompany.CollectionIdAndRib.CollectionId);
            }
            else
            {
                // Save Signatory
                await this.mandateRepository.SaveSignatoryAsync(new PersonalDb
                {
                    FirstName = mandateCreationMessageAndCompany.MandateCreationMessage.Signatory.FirstName,
                    LastName = mandateCreationMessageAndCompany.MandateCreationMessage.Signatory.LastName,
                    Email = mandateCreationMessageAndCompany.MandateCreationMessage.Signatory.Email,
                    Title = mandateCreationMessageAndCompany.MandateCreationMessage.Signatory.Title,
                    CollectionId = mandateCreationMessageAndCompany.CollectionIdAndRib.CollectionId,
                    Street = mandateCreationMessageAndCompany.MandateCreationMessage.Address!.Street,
                    Complements = mandateCreationMessageAndCompany.MandateCreationMessage.Address!.Complements,
                    ZipCode = mandateCreationMessageAndCompany.MandateCreationMessage.Address!.ZipCode,
                    City = mandateCreationMessageAndCompany.MandateCreationMessage.Address!.City,
                    Country = mandateCreationMessageAndCompany.MandateCreationMessage.Address!.Country,
                });
            }

            // Creation mandate Status 10
            await this.databaseService.CreateStatusAsync(mandateCreationMessageAndCompany.CollectionIdAndRib.CollectionId, (int)JdcCollectionStatus.Activation_Requested_Collection_Pending);
        }

        [Function("MandateCreationOrchestration_Start")]
        [ExcludeFromCodeCoverage]
        public async Task Run(
            [ServiceBusTrigger("%MandateCreationQueue%", Connection = "serviceBusNameSpace:fullyQualifiedNamespace")] ServiceBusReceivedMessage message,
            [DurableClient] DurableTaskClient client,
            FunctionContext executionContext)
        {
            ILogger logger = executionContext.GetLogger("MandateCreationOrchestration_Start");

            var mandateCreationMessage = JsonConvert.DeserializeObject<MandateCreationMessage>(Encoding.UTF8.GetString(message.Body));

            if (mandateCreationMessage is null)
            {
                logger.LogError("Deserialization of the body : {body} failed .", message.Body);
                return;
            }

            // Function input comes from the request content.
            string instanceId = await client.ScheduleNewOrchestrationInstanceAsync(
                nameof(RunOrchestrator),
                mandateCreationMessage);

            logger.LogInformation("Started orchestration with ID = '{InstanceId}'.", instanceId);
        }
    }
}