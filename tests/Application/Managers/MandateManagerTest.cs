// <copyright file="MandateManagerTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Application.Tests.Managers
{
    using System.IO;
    using KPMG.Pulse.Back.Accounting.Mandate.Adapters;
    using KPMG.Pulse.Back.Accounting.Mandate.Sql;
    using KPMG.Pulse.Back.Accounting.Mandate.Sql.Implementation;
    using KPMG.Pulse.Back.Accounting.Mandate.Sql.Implementation.Tests;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Options;
    using Moq;

    [Collection("SerialExecutionPublishDb")]
    public class MandateManagerTest
    {
        private readonly Mock<IDatabaseService> mockDatabaseService;
        private readonly Mock<ICompanyManager> mockCompanyManager;
        private readonly Mock<IJeDeclareService> mockJeDeclareService;
        private readonly Mock<IAsposeHelper> mockAsposeHelper;
        private readonly Mock<INotificationsService> mockNotificationsService;
        private readonly IOptions<SqlMandateRepositoryOptions> options;
        private readonly IOptions<MandateEmailOptions> emailOptions;

        public MandateManagerTest()
        {
            this.mockDatabaseService = new Mock<IDatabaseService>(MockBehavior.Strict);
            this.mockCompanyManager = new Mock<ICompanyManager>(MockBehavior.Strict);
            this.mockJeDeclareService = new Mock<IJeDeclareService>(MockBehavior.Strict);
            this.mockAsposeHelper = new Mock<IAsposeHelper>(MockBehavior.Strict);
            this.mockNotificationsService = new Mock<INotificationsService>(MockBehavior.Strict);
            this.options = Options.Create(new SqlMandateRepositoryOptions()
            {
                ConnectionString = Sql.Implementation.Tests.SqlServerFixture.ConnectionString,
            });
            this.emailOptions = Options.Create(new MandateEmailOptions
            {
                MandateCancellationSubject = "Your Cancellation Subject",
                MandateCancellationTemplateName = "CancellationTemplate",
                MandateCancellationFromEmail = "cancel_from@example.com",
                MandateCancellationToEmail = "cancel_to@example.com",
                MandateCancellationCcEmails = new List<string> { "cancel_cc1@example.com", "cancel_cc2@example.com" },
                MandateUploadedSubject = "Your Uploaded Mandate Subject",
                MandateUploadedTemplateName = "UploadedTemplate",
                MandateUploadedFromEmail = "upload_from@example.com",
                MandateUploadedToEmail = "upload_to@example.com",
                MandateUploadedCcEmails = new List<string> { "upload_cc1@example.com", "upload_cc2@example.com" },
            });
        }

        [Fact]
        public async Task DownloadUnsignedAsync_ValidJdcPartner_ReturnsPdf()
        {
            // Arrange
            var id = new PredictableGuid().NewGuid();
            var companyId = new PredictableGuid().NewGuid();

            var company = TestHelper.GetCompany(companyId, "bankServicesProviderId");
            Bban bban = TestHelper.GetBban("ebicsCardId", true);
            Status status = TestHelper.GetStatus();
            var collection = new Collection(
                Guid.NewGuid(),
                "yourServiceProviderId",
                company,
                bban,
                DateTime.Now,
                DateTime.Now,
                status);
            var expectedBytes = new byte[] { /* byte array */ };

            this.mockDatabaseService
                .Setup(m => m.GetCollectionById(id))
                .ReturnsAsync(collection);

            this.mockJeDeclareService
                .Setup(m => m.GetMandatPdfAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(expectedBytes);

            var mandateManager = new MandateManager(this.mockDatabaseService.Object, this.mockCompanyManager.Object, this.mockJeDeclareService.Object, this.mockAsposeHelper.Object, null!, this.emailOptions);

            // Act
            var result = await mandateManager.DownloadUnsignedAsync(id);

            // Assert
            result.Should().BeEquivalentTo(expectedBytes, "because the service should return the expected PDF data");
            result.Should().NotBeNull("because the method should return a non-null PDF data");
            this.mockDatabaseService.Verify(m => m.GetCollectionById(id), Times.Once);
            this.mockJeDeclareService.Verify(m => m.GetMandatPdfAsync(company.BankServicesProviderId!, bban.BbanServicesProviderId!), Times.Once);
        }

        [Fact]
        public async Task DownloadUnsignedAsync_FolderIdEmpty_ThrowsFolderIdEmptyOrNullException()
        {
            // Arrange
            var id = Guid.NewGuid();
            var company = TestHelper.GetCompany(new Guid("00000000-0000-0000-0000-000000000001"), string.Empty);
            Bban bban = TestHelper.GetBban("ebicsCardId", true);
            Status status = TestHelper.GetStatus();
            var collection = new Collection(
                Guid.NewGuid(),
                "yourServiceProviderId",
                company,
                bban,
                DateTime.Now,
                DateTime.Now,
                status);
            var expectedBytes = new byte[] { /* byte array */ };

            this.mockDatabaseService
                .Setup(m => m.GetCollectionById(id))
                .ReturnsAsync(collection);

            var mandateManager = new MandateManager(this.mockDatabaseService.Object, this.mockCompanyManager.Object, this.mockJeDeclareService.Object, this.mockAsposeHelper.Object, null!, this.emailOptions);

            // Act & Assert
            Func<Task> act = async () => await mandateManager.DownloadUnsignedAsync(id);
            await act.Should().ThrowAsync<FolderIdEmptyOrNullException>("because the service should throw an exception in this scenario");
        }

        [Fact]
        public async Task DownloadUnsignedAsync_RibIdEmpty_ThrowsRibIdEmptyOrNullException()
        {
            // Arrange
            var id = Guid.NewGuid();
            var company = TestHelper.GetCompany(new Guid("00000000-0000-0000-0000-000000000001"), "bankServicesProviderId");
            Bban bban = TestHelper.GetBban(string.Empty, true);
            Status status = TestHelper.GetStatus();
            var collection = new Collection(
                Guid.NewGuid(),
                "yourServiceProviderId",
                company,
                bban,
                DateTime.Now,
                DateTime.Now,
                status);
            var expectedBytes = new byte[] { /* byte array */ };

            this.mockDatabaseService
                .Setup(m => m.GetCollectionById(id))
                .ReturnsAsync(collection);

            this.mockJeDeclareService
                .Setup(m => m.GetMandatPdfAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(expectedBytes);

            var mandateManager = new MandateManager(this.mockDatabaseService.Object, this.mockCompanyManager.Object, this.mockJeDeclareService.Object, this.mockAsposeHelper.Object, null!, this.emailOptions);

            // Act & Assert
            Func<Task> act = async () => await mandateManager.DownloadUnsignedAsync(id);
            await act.Should().ThrowAsync<RibIdEmptyOrNullException>("because the service should throw an exception in this scenario");
        }

        [Fact]
        public async Task DownloadUnsignedAsync_NotJdcPartner_ReturnsPdf()
        {
            // Arrange
            var id = Guid.NewGuid();
            var company = TestHelper.GetCompany(new Guid("00000000-0000-0000-0000-000000000001"), "bankServicesProviderId");
            Bban bban = TestHelper.GetBban("bbanServicesProviderId", false);
            Status status = TestHelper.GetStatus();
            var collection = new Collection(
                Guid.NewGuid(),
                "yourServiceProviderId",
                company,
                bban,
                DateTime.Now,
                DateTime.Now,
                status);
            var expectedBytes = new byte[] { /* byte array */ };

            this.mockDatabaseService
                .Setup(m => m.GetCollectionById(id))
                .ReturnsAsync(collection);

            this.mockAsposeHelper
                .Setup(m => m.GeneratePdfFromTemplateAsync(It.IsAny<Collection>()))
                .ReturnsAsync(expectedBytes);

            var mandateManager = new MandateManager(this.mockDatabaseService.Object, this.mockCompanyManager.Object, this.mockJeDeclareService.Object, this.mockAsposeHelper.Object, null!, this.emailOptions);

            // Act
            var result = await mandateManager.DownloadUnsignedAsync(id);

            // Assert
            result.Should().BeEquivalentTo(expectedBytes, "because the service should return the expected PDF data");
            result.Should().NotBeNull("because the method should return a non-null PDF data");
            this.mockDatabaseService.Verify(m => m.GetCollectionById(id), Times.Once);
            this.mockAsposeHelper.Verify(m => m.GeneratePdfFromTemplateAsync(It.IsAny<Collection>()), Times.Once);
        }

        [Fact]
        public async Task GetAllCollectionsAsync_Case_OK()
        {
            var query = new CollectionQueryDto(
                 "search",
                 new DateTime(2023, 10, 1, 0, 0, 0, DateTimeKind.Utc),
                 new DateTime(2023, 10, 2, 0, 0, 0, DateTimeKind.Utc),
                 new DateTime(2023, 10, 1, 0, 0, 0, DateTimeKind.Utc),
                 new DateTime(2023, 10, 2, 0, 0, 0, DateTimeKind.Utc),
                 new List<int>() { -1, 3 },
                 10,
                 0,
                 Mandate.SortOrder.Ascending,
                 Mandate.CollectionSortCriteria.AccountNumber,
                 "collab@email.com");

            Collaborator collaborator = EntityFactory.Collaborator;

            var databaseService = new Mock<IDatabaseService>(MockBehavior.Strict);
            databaseService.Setup(r => r.GetCollaboratorByEmail("collab@email.com"))
                .ReturnsAsync(collaborator)
                .Verifiable();

            Bank bank = new Bank("12345", "bn", "bg", string.Empty, new BankAgreement(Mandate.JdcPartnership.NonPartner));

            PagedMandate pagedMandate = EntityFactory.PagedMandate(new List<Collection> { EntityFactory.Collection });
            databaseService.Setup(r => r.GetAllCollectionsAsync(query, new Guid("00000001-0000-0000-0000-000000000000")))
                .ReturnsAsync(pagedMandate)
                .Verifiable();

            var mandateManager = new MandateManager(databaseService.Object, new Mock<ICompanyManager>(MockBehavior.Strict).Object, new Mock<IJeDeclareService>(MockBehavior.Strict).Object, null!, null!, this.emailOptions);

            var result = await mandateManager.GetAllCollectionsAsync(query);
            result.Should().BeEquivalentTo(pagedMandate);

            databaseService.VerifyAll();
        }

        [Fact]
        public async Task GetAllCollectionsAsync_When_GetCollaboratorByEmail_Throw_Exception()
        {
            var query = new CollectionQueryDto(
                 "search",
                 new DateTime(2023, 10, 1, 0, 0, 0, DateTimeKind.Utc),
                 new DateTime(2023, 10, 2, 0, 0, 0, DateTimeKind.Utc),
                 new DateTime(2023, 10, 1, 0, 0, 0, DateTimeKind.Utc),
                 new DateTime(2023, 10, 2, 0, 0, 0, DateTimeKind.Utc),
                 new List<int>() { -1, 3 },
                 10,
                 0,
                 Mandate.SortOrder.Ascending,
                 Mandate.CollectionSortCriteria.AccountNumber,
                 "collab@email.com");

            // Arrange
            var databaseService = new Mock<IDatabaseService>();
            databaseService.Setup(x => x.GetCollaboratorByEmail("collab@email.com"))
                               .ThrowsAsync(new Exception("message"));

            MandateManager mandateManager = new MandateManager(
                databaseService.Object,
                new Mock<ICompanyManager>(MockBehavior.Strict).Object,
                new Mock<IJeDeclareService>(MockBehavior.Strict).Object,
                null!,
                null!,
                this.emailOptions);

            Func<Task> action = async () => await mandateManager.GetAllCollectionsAsync(query);
            await action.Should().ThrowAsync<Exception>().WithMessage("message");

            databaseService.VerifyAll();
        }

        [Fact]
        public async Task GetAllCollectionsAsync_Throw_Exception()
        {
            var query = new CollectionQueryDto(
                 "search",
                 new DateTime(2023, 10, 1, 0, 0, 0, DateTimeKind.Utc),
                 new DateTime(2023, 10, 2, 0, 0, 0, DateTimeKind.Utc),
                 new DateTime(2023, 10, 1, 0, 0, 0, DateTimeKind.Utc),
                 new DateTime(2023, 10, 2, 0, 0, 0, DateTimeKind.Utc),
                 new List<int>() { -1, 3 },
                 10,
                 0,
                 Mandate.SortOrder.Ascending,
                 Mandate.CollectionSortCriteria.AccountNumber,
                 "collab@email.com");

            Collaborator collaborator = new Collaborator(new Guid("00000001-0000-0000-0000-000000000000"), "collab@email.com", "fname", "lname");

            var databaseService = new Mock<IDatabaseService>();
            databaseService.Setup(x => x.GetCollaboratorByEmail("collab@email.com"))
                               .ReturnsAsync(collaborator);

            databaseService.Setup(x => x.GetAllCollectionsAsync(query, new Guid("00000001-0000-0000-0000-000000000000")))
                   .ThrowsAsync(new Exception("message"));

            var mandateManager = new MandateManager(databaseService.Object, new Mock<ICompanyManager>(MockBehavior.Strict).Object, new Mock<IJeDeclareService>(MockBehavior.Strict).Object, null!, null!, this.emailOptions);

            Func<Task> action = async () => await mandateManager.GetAllCollectionsAsync(query);
            await action.Should().ThrowAsync<Exception>().WithMessage("message");

            databaseService.VerifyAll();
        }

        [Fact]
        public async Task UploadSignedMandateAsync_ValidJdcPartner_ReturnsSignedMandateId()
        {
            // Arrange
            var collectionId = new PredictableGuid().NewGuid();
            var companyId = new PredictableGuid().NewGuid();

            var company = TestHelper.GetCompany(companyId, "bankServicesProviderId");
            Bban bban = TestHelper.GetBban("ebicsCardId", true);
            Status status = TestHelper.GetStatus();
            var collection = new Collection(
                collectionId,
                "yourServiceProviderId",
                company,
                bban,
                DateTime.Now,
                DateTime.Now,
                status);

            var fileContent = Encoding.UTF8.GetBytes("This is a test file content");
            var fileStream = new MemoryStream(fileContent);

            this.mockDatabaseService
                .Setup(m => m.GetCollectionById(collectionId))
                .ReturnsAsync(collection);

            this.mockJeDeclareService
                .Setup(m => m.UploadSignedMandate(It.IsAny<Collection>(), It.IsAny<byte[]>()))
                .ReturnsAsync("signedMandateId");

            var mandateManager = new MandateManager(this.mockDatabaseService.Object, this.mockCompanyManager.Object, this.mockJeDeclareService.Object, this.mockAsposeHelper.Object, null!, this.emailOptions);

            // Act
            var result = await mandateManager.UploadSignedMandateAsync(collectionId, fileStream);

            // Assert
            result.Should().Be("signedMandateId");
            result.Should().NotBeNull("because the method should return a non-null PDF data");
            this.mockDatabaseService.Verify(m => m.GetCollectionById(collectionId), Times.Once);
            this.mockJeDeclareService.Verify(m => m.UploadSignedMandate(It.IsAny<Collection>(), It.IsAny<byte[]>()), Times.Once);
        }

        [Fact]
        public async Task UploadSignedMandateAsync_NotJdcPartner_ReturnsNull()
        {
            // Arrange
            var collectionId = new PredictableGuid().NewGuid();
            var companyId = new PredictableGuid().NewGuid();

            var company = TestHelper.GetCompany(companyId, "bankServicesProviderId");
            Bban bban = TestHelper.GetBban("ebicsCardId", false);
            Status status = TestHelper.GetStatus();
            var collection = new Collection(
                collectionId,
                "yourServiceProviderId",
                company,
                bban,
                DateTime.Now,
                DateTime.Now,
                status);

            var fileContent = Encoding.UTF8.GetBytes("This is a test file content");
            var fileStream = new MemoryStream(fileContent);

            this.mockDatabaseService
                .Setup(m => m.GetCollectionById(collectionId))
                .ReturnsAsync(collection);

            this.mockJeDeclareService
                .Setup(m => m.UploadSignedMandate(It.IsAny<Collection>(), It.IsAny<byte[]>()))
                .ReturnsAsync("signedMandateId");

            this.mockNotificationsService
                .Setup(m => m.SendEmailAsync(It.IsAny<EmailCommand>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            var mandateManager = new MandateManager(this.mockDatabaseService.Object, this.mockCompanyManager.Object, this.mockJeDeclareService.Object, this.mockAsposeHelper.Object, this.mockNotificationsService.Object, this.emailOptions);

            // Act
            var result = await mandateManager.UploadSignedMandateAsync(collectionId, fileStream);

            // Assert
            result.Should().BeNull("signedMandateId");
            this.mockDatabaseService.Verify(m => m.GetCollectionById(collectionId), Times.Once);
            this.mockJeDeclareService.Verify(m => m.UploadSignedMandate(It.IsAny<Collection>(), It.IsAny<byte[]>()), Times.Never);
            this.mockNotificationsService.Verify(m => m.SendEmailAsync(It.IsAny<EmailCommand>()), Times.Once);
        }

        [Fact]
        public async Task CreateMandate_Case_Ok()
        {
            var bank = new Bank("CodeB", "name", "group", "ebicsCardId", new BankAgreement(Mandate.JdcPartnership.Partner));
            var rib = new Bban("CodeB", "54321", "12345678901", "01", "ribId", bank);
            var signature = new Signatory("M", "marwen", "elleuch", "maroo@email.com");
            var adresse = new Address("LE ROUSSEL", "complements", "63520", "DOMAIZE", "France");

            var command = new CollectionCreationCommand(
                "1234567890",
                signature,
                adresse,
                rib);

            await using var database = SqlServerFixture.CreateDatabase();
            using var context = new MandateContext(this.options);

            await context.RefStatusCode.AddAsync(new RefStatusCodeDb()
            {
                StatusCode = -1,
                PulseCode = 30,
                StatusNameFr = "En Cours",
                StatusNameEn = "In Progress",
            });

            await context.RefStatusCode.AddAsync(new RefStatusCodeDb()
            {
                StatusCode = 10,
                PulseCode = 20,
                StatusNameFr = "En Cours",
                StatusNameEn = "In Progress",
            });
            await context.SaveChangesAsync();

            var companyDb = EntityDbFactory.CompanyDb;
            await context.Company.AddAsync(companyDb);
            await context.SaveChangesAsync();

            var bankRef = EntityDbFactory.RefBankDb;
            bankRef.BankCode = "CodeB";
            bankRef.JdcPartnership = (JdcPartnership)3;
            await context.RefBank.AddAsync(bankRef);
            await context.SaveChangesAsync();

            var sqlRepo = new SqlMandateRepository(this.options);

            var adapter = new SqlAdapter(sqlRepo);
            var companyManager = new CompanyManager(adapter);

            var dossierClient = new Company(companyDb.Id, "cn1", "12345678901234", "1234567890", "folderId", EntityFactory.Signatory, EntityFactory.Address);

            this.mockJeDeclareService.Setup(item => item.CreateFolderAsync(
                It.Is<Company>(c =>
                    c.Id == companyDb.Id &&
                    c.ErpId == "1234567890" &&
                    c.Name == "cn1" &&
                    c.SiretNumber == "12345678901234" &&
                    c.BankServicesProviderId == null &&
                    CompareSignatory(c.Signatory!, signature) &&
                    CompareAdress(c.Address!, adresse))))
                .ReturnsAsync(dossierClient)
                .Verifiable();

            this.mockJeDeclareService.Setup(item => item.AddRibToFolderAsync("folderId", command, It.Is<Bank>(b => b.Code == "CodeB")))
                .ReturnsAsync(rib)
                .Verifiable();

            this.mockJeDeclareService.Setup(item => item.CreateCollecteConfigurationAsync(dossierClient, rib))
               .ReturnsAsync("releveId")
               .Verifiable();

            var mandateManager = new MandateManager(adapter, companyManager, this.mockJeDeclareService.Object, this.mockAsposeHelper.Object, null!, this.emailOptions);

            Guid collectionId = await mandateManager.CreateMandate(command);

            collectionId.Should().NotBeEmpty();

            int count = context.Collection.Count();
            count.Should().Be(1);

            var collection = await context.Collection
                .Include(item => item.Company).ThenInclude(c => c!.JeDeclareFolder)
                .Include(item => item.Statuses)
                .Include(c => c.JeDeclareCollection)
                .Include(c => c.Personal)
                .FirstOrDefaultAsync(item => item.Id == collectionId);

            collection.Should().NotBeNull();
            collection!.Company.Should().NotBeNull();
            collection!.Company!.JeDeclareFolder.Should().NotBeNull();
            collection.JeDeclareCollection.Should().NotBeNull();
            collection.Personal.Should().NotBeNull();
            collection.Statuses.Should().NotBeNull();

            var companyId = collection!.CompanyId;

            collection!.Company!.JeDeclareFolder!.CompanyId.Should().Be(companyId);
            collection!.Company!.JeDeclareFolder!.JdcDossierId.Should().Be("folderId");

            collection!.JeDeclareCollection!.CollectionId.Should().Be(collectionId);
            collection!.JeDeclareCollection!.JdcReleveId.Should().Be("releveId");
            collection!.JeDeclareCollection!.JdcRibId.Should().Be("ribId");

            collection!.Personal!.Title.Should().Be("M");
            collection!.Personal!.FirstName.Should().Be("marwen");
            collection!.Personal!.LastName.Should().Be("elleuch");
            collection!.Personal!.Email.Should().Be("maroo@email.com");
            collection!.Personal!.Street.Should().Be("LE ROUSSEL");
            collection!.Personal!.Complements.Should().Be("complements");
            collection!.Personal!.ZipCode.Should().Be("63520");
            collection!.Personal!.City.Should().Be("DOMAIZE");
            collection!.Personal!.Country.Should().Be("France");

            collection.Statuses.Count.Should().Be(2);
            collection.Statuses.Count(s => s.IsCurrent).Should().Be(1);
            collection.Statuses.Count(s => !s.IsCurrent).Should().Be(1);

            var creationStatus = collection.Statuses.Where(item => !item.IsCurrent).FirstOrDefault();
            creationStatus.Should().NotBeNull();
            creationStatus!.StatusCode.Should().Be(-1);
            creationStatus!.CollectionId.Should().Be(collectionId);

            var currentStatus = collection.Statuses.Where(item => item.IsCurrent).FirstOrDefault();
            currentStatus.Should().NotBeNull();
            currentStatus!.StatusCode.Should().Be(10);
            currentStatus!.CollectionId.Should().Be(collectionId);

            this.mockJeDeclareService.VerifyAll();
            this.mockAsposeHelper.VerifyAll();
        }

        [Fact]
        public async Task CreateMandate_Case_JdcPartnership_NoCard()
        {
            var bank = new Bank("CodeB", "name", "group", null, new BankAgreement(Mandate.JdcPartnership.NonPartner));
            var rib = new Bban("CodeB", "54321", "12345678901", "01", "ribId", bank);
            var signature = new Signatory("M", "marwen", "elleuch", "maroo@email.com");
            var adresse = new Address("LE ROUSSEL", "complements", "63520", "DOMAIZE", "France");

            var command = new CollectionCreationCommand(
                "1234567890",
                signature,
                adresse,
                rib);

            await using var database = SqlServerFixture.CreateDatabase();
            using var context = new MandateContext(this.options);

            var companyDb = EntityDbFactory.CompanyDb;
            await context.Company.AddAsync(companyDb);
            await context.SaveChangesAsync();

            var bankRef = EntityDbFactory.RefBankDb;
            bankRef.BankCode = "CodeB";
            bankRef.JdcPartnership = (JdcPartnership)2;
            await context.RefBank.AddAsync(bankRef);
            await context.SaveChangesAsync();

            var sqlRepo = new SqlMandateRepository(this.options);

            var adapter = new SqlAdapter(sqlRepo);
            var companyManager = new CompanyManager(adapter);

            var dossierClient = new Company(companyDb.Id, "cn1", "12345678901234", "1234567890", "folderId", EntityFactory.Signatory, EntityFactory.Address);

            this.mockJeDeclareService.Setup(item => item.CreateFolderAsync(
                It.Is<Company>(c =>
                    c.Id == companyDb.Id &&
                    c.ErpId == "1234567890" &&
                    c.Name == "cn1" &&
                    c.SiretNumber == "12345678901234" &&
                    c.BankServicesProviderId == null &&
                    CompareSignatory(c.Signatory!, signature) &&
                    CompareAdress(c.Address!, adresse))))
                .ReturnsAsync(dossierClient)
                .Verifiable();

            var mandateManager = new MandateManager(adapter, companyManager, this.mockJeDeclareService.Object, this.mockAsposeHelper.Object, null!, this.emailOptions);

            Func<Task> acttion = () => mandateManager.CreateMandate(command);

            int count = context.Collection.Count();
            count.Should().Be(0);

            await acttion.Should().ThrowExactlyAsync<ApplicationException>()
                    .WithMessage("L'établissement bancaire CodeB n'est pas partenaire de JeDeclare.com mais est défini sans connexion à une carte EBICs.");

            this.mockJeDeclareService.VerifyAll();
            this.mockAsposeHelper.VerifyAll();
        }

        [Fact]
        public async Task CreateMandate_Case_Collection_Exist()
        {
            var bank = new Bank("CodeB", "name", "group", "cardId", new BankAgreement(Mandate.JdcPartnership.NonPartner));
            var rib = new Bban("CodeB", "23456", "12345678901", "55", "ribId", bank);
            var signature = new Signatory("M", "marwen", "elleuch", "maroo@email.com");
            var adresse = new Address("LE ROUSSEL", "complements", "63520", "DOMAIZE", "France");

            var command = new CollectionCreationCommand(
                "1234567890",
                signature,
                adresse,
                rib);

            await using var database = SqlServerFixture.CreateDatabase();
            using var context = new MandateContext(this.options);

            var companyDb = EntityDbFactory.CompanyDb;
            await context.Company.AddAsync(companyDb);
            await context.SaveChangesAsync();

            var bankRef = EntityDbFactory.RefBankDb;
            bankRef.BankCode = "CodeB";
            bankRef.JdcPartnership = (JdcPartnership)2;
            bankRef.EbicsCardId = "cardId";
            await context.RefBank.AddAsync(bankRef);
            await context.SaveChangesAsync();

            var coll = EntityDbFactory.CollectionDb;
            coll.BankCode = "CodeB";
            await context.Collection.AddAsync(coll);
            await context.SaveChangesAsync();

            var sqlRepo = new SqlMandateRepository(this.options);

            var adapter = new SqlAdapter(sqlRepo);
            var companyManager = new CompanyManager(adapter);

            var dossierClient = new Company(companyDb.Id, "cn1", "12345678901234", "1234567890", "folderId", EntityFactory.Signatory, EntityFactory.Address);

            this.mockJeDeclareService.Setup(item => item.CreateFolderAsync(
                It.Is<Company>(c =>
                    c.Id == companyDb.Id &&
                    c.ErpId == "1234567890" &&
                    c.Name == "cn1" &&
                    c.SiretNumber == "12345678901234" &&
                    c.BankServicesProviderId == null &&
                    CompareSignatory(c.Signatory!, signature) &&
                    CompareAdress(c.Address!, adresse))))
                .ReturnsAsync(dossierClient)
                .Verifiable();

            var mandateManager = new MandateManager(adapter, companyManager, this.mockJeDeclareService.Object, this.mockAsposeHelper.Object, null!, this.emailOptions);

            Func<Task> acttion = () => mandateManager.CreateMandate(command);

            int count = context.Collection.Count();
            count.Should().Be(1);

            await acttion.Should().ThrowExactlyAsync<ApplicationException>()
                    .WithMessage("Il existe une configuration de collecte pour ce RIB CodeB-23456-12345678901-55.");

            this.mockJeDeclareService.VerifyAll();
            this.mockAsposeHelper.VerifyAll();
        }

        [Fact]
        public async Task DownloadSignedAsync_ValidCollectionIdAndValidPartnerCollection_ReturnsPdf()
        {
            // Arrange
            var id = new PredictableGuid().NewGuid();
            var companyId = new PredictableGuid().NewGuid();

            var company = TestHelper.GetCompany(companyId, "bankServicesProviderId");
            Bban bban = TestHelper.GetBban("ebicsCardId", true);
            Status status = TestHelper.GetStatus();
            var collection = new Collection(
                Guid.NewGuid(),
                "yourServiceProviderId",
                company,
                bban,
                DateTime.Now,
                DateTime.Now,
                status);
            var expectedBytes = Array.Empty<byte>();

            this.mockDatabaseService
                .Setup(m => m.GetCollectionById(id))
                .ReturnsAsync(collection);

            this.mockJeDeclareService
                .Setup(m => m.GetSignedMandatPdfAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(expectedBytes);

            var mandateManager = new MandateManager(this.mockDatabaseService.Object, this.mockCompanyManager.Object, this.mockJeDeclareService.Object, this.mockAsposeHelper.Object, null!, this.emailOptions);

            // Act
            var result = await mandateManager.DownloadSignedAsync(id);

            // Assert
            result.Should().BeEquivalentTo(expectedBytes, "because the service should return the expected PDF data");
            result.Should().NotBeNull("because the method should return a non-null PDF data");
            this.mockDatabaseService.Verify(m => m.GetCollectionById(id), Times.Once);
            this.mockJeDeclareService.Verify(m => m.GetSignedMandatPdfAsync(company.BankServicesProviderId!, bban.BbanServicesProviderId!), Times.Once);
        }

        [Fact]
        public async Task DownloadSignedAsync_FolderIdEmptyAndCompanyNotNull_ThrowsFolderIdEmptyOrNullException()
        {
            // Arrange
            var id = Guid.NewGuid();
            var company = TestHelper.GetCompany(new Guid("00000000-0000-0000-0000-000000000001"), string.Empty);
            Bban bban = TestHelper.GetBban("ebicsCardId", true);
            Status status = TestHelper.GetStatus();
            var collection = new Collection(
                Guid.NewGuid(),
                "yourServiceProviderId",
                company,
                bban,
                DateTime.Now,
                DateTime.Now,
                status);
            var expectedBytes = Array.Empty<byte>();

            this.mockDatabaseService
                .Setup(m => m.GetCollectionById(id))
                .ReturnsAsync(collection);

            var mandateManager = new MandateManager(this.mockDatabaseService.Object, this.mockCompanyManager.Object, this.mockJeDeclareService.Object, this.mockAsposeHelper.Object, null!, this.emailOptions);

            // Act & Assert
            Func<Task> act = async () => await mandateManager.DownloadSignedAsync(id);
            await act.Should().ThrowAsync<FolderIdEmptyOrNullException>("because the service should throw an exception in this scenario");
        }

        [Fact]
        public async Task DownloadSignedAsync_FolderIdEmptyAndCompanyIsNull_ThrowsFolderIdEmptyOrNullException()
        {
            // Arrange
            var id = Guid.NewGuid();
            Bban bban = TestHelper.GetBban("ebicsCardId", true);
            Status status = TestHelper.GetStatus();
            var collection = new Collection(
                Guid.NewGuid(),
                "yourServiceProviderId",
                null,
                bban,
                DateTime.Now,
                DateTime.Now,
                status);
            var expectedBytes = Array.Empty<byte>();

            this.mockDatabaseService
                .Setup(m => m.GetCollectionById(id))
                .ReturnsAsync(collection);

            var mandateManager = new MandateManager(this.mockDatabaseService.Object, this.mockCompanyManager.Object, this.mockJeDeclareService.Object, this.mockAsposeHelper.Object, null!, this.emailOptions);

            // Act & Assert
            Func<Task> act = async () => await mandateManager.DownloadSignedAsync(id);
            await act.Should().ThrowAsync<FolderIdEmptyOrNullException>("because the service should throw an exception in this scenario");
        }

        [Fact]
        public async Task DownloadSignedAsync_RibIdEmptyAndBbanNotNull_ThrowsRibIdEmptyOrNullException()
        {
            // Arrange
            var id = Guid.NewGuid();
            var company = TestHelper.GetCompany(new Guid("00000000-0000-0000-0000-000000000001"), "bankServicesProviderId");
            Bban bban = TestHelper.GetBban(string.Empty, true);
            Status status = TestHelper.GetStatus();
            var collection = new Collection(
                Guid.NewGuid(),
                "yourServiceProviderId",
                company,
                bban,
                DateTime.Now,
                DateTime.Now,
                status);
            var expectedBytes = Array.Empty<byte>();

            this.mockDatabaseService
                .Setup(m => m.GetCollectionById(id))
                .ReturnsAsync(collection);

            this.mockJeDeclareService
                .Setup(m => m.GetSignedMandatPdfAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(expectedBytes);

            var mandateManager = new MandateManager(this.mockDatabaseService.Object, this.mockCompanyManager.Object, this.mockJeDeclareService.Object, this.mockAsposeHelper.Object, null!, this.emailOptions);

            // Act & Assert
            Func<Task> act = async () => await mandateManager.DownloadSignedAsync(id);
            await act.Should().ThrowAsync<RibIdEmptyOrNullException>("because the service should throw an exception in this scenario");
        }

        [Fact]
        public async Task DownloadSignedAsync_RibIdEmptyAndBbanIsNull_ThrowsRibIdEmptyOrNullException()
        {
            // Arrange
            var id = Guid.NewGuid();
            var company = TestHelper.GetCompany(new Guid("00000000-0000-0000-0000-000000000001"), "bankServicesProviderId");
            Status status = TestHelper.GetStatus();
            var collection = new Collection(
                Guid.NewGuid(),
                "yourServiceProviderId",
                company,
                null,
                DateTime.Now,
                DateTime.Now,
                status);
            var expectedBytes = Array.Empty<byte>();

            this.mockDatabaseService
                .Setup(m => m.GetCollectionById(id))
                .ReturnsAsync(collection);

            this.mockJeDeclareService
                .Setup(m => m.GetSignedMandatPdfAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(expectedBytes);

            var mandateManager = new MandateManager(this.mockDatabaseService.Object, this.mockCompanyManager.Object, this.mockJeDeclareService.Object, this.mockAsposeHelper.Object, null!, this.emailOptions);

            Func<Task> act = async () => await mandateManager.DownloadSignedAsync(id);
            await act.Should().ThrowAsync<RibIdEmptyOrNullException>("because the service should throw an exception in this scenario");
        }

        [Fact]
        public async Task DeactivateCollectionAsync_CaseOkAndIsPartner()
        {
            // Arrange
            var collectionId = new PredictableGuid().NewGuid();
            var companyId = new PredictableGuid().NewGuid();

            var mandateId = new PredictableGuid().NewGuid();
            var company = TestHelper.GetCompany(companyId, "bankServicesProviderId");
            Bban bban = TestHelper.GetBban("ebicsCardId", true);
            Status status = TestHelper.GetStatus();
            var collection = new Collection(
                collectionId,
                "yourServiceProviderId",
                company,
                bban,
                DateTime.Now,
                DateTime.Now,
                status);

            this.mockDatabaseService
                .Setup(m => m.GetCollectionById(mandateId))
                .ReturnsAsync(collection);

            this.mockJeDeclareService
                .Setup(m => m.DeactivateCollection(collection))
                .ReturnsAsync(true);

            this.mockNotificationsService
                .Setup(m => m.SendEmailAsync(It.IsAny<EmailCommand>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            var mandateManager = new MandateManager(this.mockDatabaseService.Object, null!, this.mockJeDeclareService.Object, null!, this.mockNotificationsService.Object, this.emailOptions);

            // Act
            var result = await mandateManager.DeactivateCollectionAsync(mandateId);

            // Assert
            result.Should().BeTrue();
            this.mockDatabaseService.Verify(m => m.GetCollectionById(It.IsAny<Guid>()), Times.Once);
            this.mockJeDeclareService.Verify(m => m.DeactivateCollection(It.IsAny<Collection>()), Times.Once);
            this.mockNotificationsService.Verify(m => m.SendEmailAsync(It.IsAny<EmailCommand>()), Times.Never);
        }

        [Fact]
        public async Task DeactivateCollectionAsync_CaseOkAndNotPartner()
        {
            // Arrange
            var mandateId = new PredictableGuid().NewGuid();
            var collection = new Collection(mandateId, null!, null!, null!, DateTime.MinValue, DateTime.MinValue, null!);

            this.mockDatabaseService
                .Setup(m => m.GetCollectionById(mandateId))
                .ReturnsAsync(collection);

            this.mockJeDeclareService
                .Setup(m => m.DeactivateCollection(collection))
                .ReturnsAsync(true);

            this.mockNotificationsService
                .Setup(m => m.SendEmailAsync(It.IsAny<EmailCommand>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            var mandateManager = new MandateManager(this.mockDatabaseService.Object, null!, this.mockJeDeclareService.Object, null!, this.mockNotificationsService.Object, this.emailOptions);

            // Act
            var result = await mandateManager.DeactivateCollectionAsync(mandateId);

            // Assert
            result.Should().BeTrue();
            this.mockDatabaseService.Verify(m => m.GetCollectionById(It.IsAny<Guid>()), Times.Once);
            this.mockJeDeclareService.Verify(m => m.DeactivateCollection(It.IsAny<Collection>()), Times.Never);
            this.mockNotificationsService.Verify(m => m.SendEmailAsync(It.IsAny<EmailCommand>()), Times.Once);
        }

        [Fact]
        public async Task InsertFormIOCollectionAsync()
        {
            // Arrange
            var id = Guid.NewGuid();
            var company = TestHelper.GetCompany(new Guid("00000000-0000-0000-0000-000000000001"), "bankServicesProviderId");
            Bban bban = TestHelper.GetBban(string.Empty, true);
            Status status = TestHelper.GetStatus();
            var collection = new Collection(
                Guid.NewGuid(),
                "yourServiceProviderId",
                company,
                bban,
                DateTime.Now,
                DateTime.Now,
                status);

            this.mockDatabaseService
               .Setup(m => m.InsertFormIOCollectionAsync(collection, company))
               .Returns(Task.CompletedTask);

            this.mockDatabaseService.Setup(r => r.CheckCollecteConfigExistAsync(bban))
                .ReturnsAsync(false)
                .Verifiable();

            this.mockDatabaseService.Setup(r => r.GetCompanyBySiretAsync(company.SiretNumber))
                .ReturnsAsync(company)
                .Verifiable();

            var mandateManager = new MandateManager(this.mockDatabaseService.Object, this.mockCompanyManager.Object, this.mockJeDeclareService.Object, this.mockAsposeHelper.Object, null!, this.emailOptions);

            // Act
            await mandateManager.InsertFormIOCollectionAsync(collection);

            // Assert
            this.mockDatabaseService.VerifyAll();
        }

        [Fact]
        public async Task InsertFormIOCollectionAsync_Throw_Exception_When_Collect_Exists()
        {
            // Arrange
            var id = Guid.NewGuid();
            var company = TestHelper.GetCompany(new Guid("00000000-0000-0000-0000-000000000001"), "bankServicesProviderId");
            Bban bban = TestHelper.GetBban(string.Empty, true);
            Status status = TestHelper.GetStatus();
            var collection = new Collection(
                Guid.NewGuid(),
                "yourServiceProviderId",
                company,
                bban,
                DateTime.Now,
                DateTime.Now,
                status);

            this.mockDatabaseService.Setup(r => r.CheckCollecteConfigExistAsync(bban))
                .ReturnsAsync(true)
                .Verifiable();

            this.mockDatabaseService.Setup(r => r.GetCompanyBySiretAsync(company.SiretNumber))
                .ReturnsAsync(company)
                .Verifiable();

            var mandateManager = new MandateManager(this.mockDatabaseService.Object, this.mockCompanyManager.Object, this.mockJeDeclareService.Object, this.mockAsposeHelper.Object, null!, this.emailOptions);

            // Act
            Func<Task> act = async () => await mandateManager.InsertFormIOCollectionAsync(collection);

            // Assert
            await act.Should().ThrowAsync<ApplicationException>();
            this.mockDatabaseService.VerifyAll();
        }

        private static bool CompareAdress(Address address1, Address address2)
        {
            return address1.City == address2.City &&
                address1.Country == address2.Country &&
                address1.Complements == address2.Complements &&
                address1.Street == address2.Street &&
                address1.ZipCode == address2.ZipCode;
        }

        private static bool CompareSignatory(Signatory signatory1, Signatory signatory2)
        {
            return signatory1.FirstName == signatory2.FirstName &&
                signatory1.LastName == signatory2.LastName &&
                signatory1.Email == signatory2.Email &&
                signatory1.Title == signatory2.Title;
        }
    }
}