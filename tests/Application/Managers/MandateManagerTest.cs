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

    [Collection("SerialExecutionPublishDb")]
    public class MandateManagerTest
    {
        private readonly Mock<IDatabaseService> mockDatabaseService;
        private readonly Mock<ICompanyManager> mockCompanyManager;
        private readonly Mock<IJeDeclareService> mockJeDeclareService;
        private readonly Mock<IAsposeHelper> mockAsposeHelper;
        private readonly IOptions<SqlMandateRepositoryOptions> options;

        public MandateManagerTest()
        {
            this.mockDatabaseService = new Mock<IDatabaseService>(MockBehavior.Strict);
            this.mockCompanyManager = new Mock<ICompanyManager>(MockBehavior.Strict);
            this.mockJeDeclareService = new Mock<IJeDeclareService>(MockBehavior.Strict);
            this.mockAsposeHelper = new Mock<IAsposeHelper>(MockBehavior.Strict);
            this.options = Options.Create(new SqlMandateRepositoryOptions()
            {
                ConnectionString = Sql.Implementation.Tests.SqlServerFixture.ConnectionString,
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

            var mandateManager = new MandateManager(this.mockDatabaseService.Object, this.mockCompanyManager.Object, this.mockJeDeclareService.Object, this.mockAsposeHelper.Object);

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

            var mandateManager = new MandateManager(this.mockDatabaseService.Object, this.mockCompanyManager.Object, this.mockJeDeclareService.Object, this.mockAsposeHelper.Object);

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

            var mandateManager = new MandateManager(this.mockDatabaseService.Object, this.mockCompanyManager.Object, this.mockJeDeclareService.Object, this.mockAsposeHelper.Object);

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

            var mandateManager = new MandateManager(this.mockDatabaseService.Object, this.mockCompanyManager.Object, this.mockJeDeclareService.Object, this.mockAsposeHelper.Object);

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
                  string.Empty,
                  null,
                  null,
                  null,
                  null,
                  null,
                  10,
                  0,
                  Mandate.SortOrder.Ascending,
                  Mandate.CollectionSortCriteria.Name,
                  Guid.Empty);

            Company company = new Company(
                new Guid("00000001-0000-0000-0000-000000000000"),
                "cn",
                "12345678910",
                "123456789",
                string.Empty,
                null,
                null);

            Bank bank = new Bank("12345", "bn", "bg", string.Empty, new BankAgreement(Mandate.JdcPartnership.NonPartner));

            Bban bban = new Bban("12345", "54321", "12345678901", "55", string.Empty, bank);

            Collection collection = new Collection(
                    new Guid("00000002-0000-0000-0000-000000000000"),
                    string.Empty,
                    company,
                    bban,
                    new DateTime(2022, 1, 1),
                    new DateTime(2022, 1, 1),
                    new Status(CollectionStatus.InProgress, "En cours"));

            var counters = new Counters(1, 1, 0, 0, 0, 0);
            var pm = new PagedMandate(counters, new List<Collection> { collection });

            var database = new Mock<IDatabaseService>(MockBehavior.Strict);
            database.Setup(i => i.GetAllCollectionsAsync(query))
                .ReturnsAsync(pm)
                .Verifiable();

            MandateManager manager = new MandateManager(
                database.Object,
                new Mock<ICompanyManager>(MockBehavior.Strict).Object,
                new Mock<IJeDeclareService>(MockBehavior.Strict).Object,
                null!);

            var result = await manager.GetAllCollectionsAsync(query);

            result.Should().BeEquivalentTo(pm);

            database.VerifyAll();
        }

        [Fact]
        public async Task GetAllCollectionsAsync_When_Service_Throw_Exception()
        {
            var query = new CollectionQueryDto(
                  string.Empty,
                  null,
                  null,
                  null,
                  null,
                  null,
                  10,
                  0,
                  Mandate.SortOrder.Ascending,
                  Mandate.CollectionSortCriteria.Name,
                  Guid.Empty);

            var database = new Mock<IDatabaseService>(MockBehavior.Strict);
            database.Setup(i => i.GetAllCollectionsAsync(query))
                .ThrowsAsync(new Exception("message"))
                .Verifiable();

            MandateManager manager = new MandateManager(
                database.Object,
                new Mock<ICompanyManager>(MockBehavior.Strict).Object,
                new Mock<IJeDeclareService>(MockBehavior.Strict).Object,
                null!);

            Func<Task> action = async () => await manager.GetAllCollectionsAsync(query);
            await action.Should().ThrowAsync<Exception>().WithMessage("message");

            database.VerifyAll();
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

            var mandateManager = new MandateManager(this.mockDatabaseService.Object, this.mockCompanyManager.Object, this.mockJeDeclareService.Object, this.mockAsposeHelper.Object);

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

            var mandateManager = new MandateManager(this.mockDatabaseService.Object, this.mockCompanyManager.Object, this.mockJeDeclareService.Object, this.mockAsposeHelper.Object);

            // Act
            var result = await mandateManager.UploadSignedMandateAsync(collectionId, fileStream);

            // Assert
            result.Should().BeNull("signedMandateId");
            this.mockDatabaseService.Verify(m => m.GetCollectionById(collectionId), Times.Once);
            this.mockJeDeclareService.Verify(m => m.UploadSignedMandate(It.IsAny<Collection>(), It.IsAny<byte[]>()), Times.Never);
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

            #region RefStatusCode
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
            #endregion
            #region Company
            var companyDb = EntityDbFactory.CompanyDb;
            await context.Company.AddAsync(companyDb);
            await context.SaveChangesAsync();
            #endregion
            #region bank
            var bankRef = EntityDbFactory.RefBankDb;
            bankRef.BankCode = "CodeB";
            bankRef.JdcPartnership = (JdcPartnership)3;
            await context.RefBank.AddAsync(bankRef);
            await context.SaveChangesAsync();
            #endregion

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
                    this.CompareSignatory(c.Signatory!, signature) &&
                    this.CompareAdress(c.Address!, adresse))))
                .ReturnsAsync(dossierClient)
                .Verifiable();

            this.mockJeDeclareService.Setup(item => item.AddRibToFolderAsync("folderId", command, It.Is<Bank>(b => b.Code == "CodeB")))
                .ReturnsAsync(rib)
                .Verifiable();

            this.mockJeDeclareService.Setup(item => item.CreateCollecteConfigurationAsync(dossierClient, rib))
               .ReturnsAsync("releveId")
               .Verifiable();

            var mandateManager = new MandateManager(adapter, companyManager, this.mockJeDeclareService.Object, this.mockAsposeHelper.Object);

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
            collection.Company.Should().NotBeNull();
            collection.Company.JeDeclareFolder.Should().NotBeNull();
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

        private bool CompareAdress(Address address1, Address address2)
        {
            return address1.City == address2.City &&
                address1.Country == address2.Country &&
                address1.Complements == address2.Complements &&
                address1.Street == address2.Street &&
                address1.ZipCode == address2.ZipCode;
        }

        private bool CompareSignatory(Signatory signatory1, Signatory signatory2)
        {
            return signatory1.FirstName == signatory2.FirstName &&
                signatory1.LastName == signatory2.LastName &&
                signatory1.Email == signatory2.Email &&
                signatory1.Title == signatory2.Title;
        }
    }
}