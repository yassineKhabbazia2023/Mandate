// <copyright file="MandateControllerTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.AspNetCore.Tests
{
    using System.Net;
    using KPMG.Pulse.Back.Accounting.Mandate.Adapters;
    using KPMG.Pulse.Back.Accounting.Mandate.Application;
    using KPMG.Pulse.Back.Accounting.Mandate.Sql.Implementation;
    using KPMG.Pulse.Back.Accounting.Mandate.Sql.Implementation.Tests;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;
    using Microsoft.Extensions.Options;

    [Collection("SerialExecutionPublishDb")]
    public class MandateControllerTest
    {
        private readonly Mock<IJeDeclareService> mockJeDeclareService;
        private readonly Mock<IAsposeHelper> mockAsposeHelper;
        private readonly IOptions<MandateEmailOptions> emailOptions;
        private readonly Mock<ILogger<MandateManager>> mockMandateLogger;
        private IOptions<SqlMandateRepositoryOptions> options;

        public MandateControllerTest()
        {
            this.options = Options.Create(new SqlMandateRepositoryOptions()
            {
                ConnectionString = Sql.Implementation.Tests.SqlServerFixture.ConnectionString,
            });
            this.mockJeDeclareService = new Mock<IJeDeclareService>(MockBehavior.Strict);
            this.mockAsposeHelper = new Mock<IAsposeHelper>(MockBehavior.Strict);
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
            this.mockMandateLogger = new Mock<ILogger<MandateManager>>(MockBehavior.Loose);
        }

        [Fact]
        public async Task GetCollectionsAsync_When_GetCollectionsAsync_OK()
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
                SortOrder.Ascending,
                CollectionSortCriteria.Name,
                "collab@email.com");

            var company = new Company(
                1,
                "cn",
                "12345678910",
                "123456789",
                string.Empty,
                null,
                null);

            Bank bank = new Bank("12345", "bn", "bg", string.Empty, new BankAgreement(JdcPartnership.NonPartner));

            Bban bban = new Bban("12345", "54321", "12345678901", "55", string.Empty, bank);

            Collection collection = new Collection(
                    new Guid("00000002-0000-0000-0000-000000000000"),
                    string.Empty,
                    company,
                    bban,
                    new DateTime(2022, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    new DateTime(2022, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    new Status(CollectionStatus.InProgress, "En cours"));

            var pm = new PagedMandate(
                new Counters(1, 1, 0, 0, 0, 0),
                new List<Collection>()
                {
                    collection,
                });

            var logger = new Mock<ILogger<MandateController>>(MockBehavior.Loose);
            var manager = new Mock<IMandateManager>(MockBehavior.Strict);
            manager.Setup(item =>
                item.GetAllCollectionsAsync(
                    It.Is<CollectionQueryDto>(item => item.SortCriteria == query.SortCriteria && item.SortOrder == query.SortOrder)))
                .ReturnsAsync(pm)
                .Verifiable();

            var authenticationContext = new Mock<IAuthenticationServices>(MockBehavior.Strict);
            authenticationContext.Setup(item => item.Email)
                .Returns("collab@email.com")
                .Verifiable();

            var guidGenerator = new Mock<IGuidGenerator>();

            var controller = new MandateController(logger.Object, manager.Object, authenticationContext.Object, guidGenerator.Object, null!);

            var result = await controller.GetCollectionsAsync(
                string.Empty,
                null,
                null,
                null,
                null,
                null,
                10,
                0,
                "Ascending",
                "Name");

            var expectedCounters = new Client.Counters(1, 1, 0, 0, 0, 0);

            var collectionBankInfo = new Client.CollectionBankInfo(
                bankName: "bn",
                accountNumber: "12345678901",
                jdcPartnership: 2);

            var expectedCollection = new Client.CollectionSummary(
                id: new Guid("00000002-0000-0000-0000-000000000000"),
                erpId: "123456789",
                companyName: "cn",
                collectionBankInfo: collectionBankInfo,
                creationDate: new DateTime(2022, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                modificationDate: new DateTime(2022, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                statusCode: (int)CollectionStatus.InProgress);

            var expectedCollections = new List<Client.CollectionSummary>()
            {
               expectedCollection,
            };

            result.As<OkObjectResult>().StatusCode.Should().Be(200);
            result.As<OkObjectResult>().Value.Should().BeEquivalentTo(new Client.PagedMandate(expectedCounters, expectedCollections));

            logger.VerifyAll();
            manager.VerifyAll();
        }

        [Fact]
        public async Task GetCollectionsAsync_When_GetCollectionsAsync_Throw_Exception()
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
                SortOrder.Ascending,
                CollectionSortCriteria.Name,
                "collab@email.com");

            var logger = new Mock<ILogger<MandateController>>(MockBehavior.Loose);
            var manager = new Mock<IMandateManager>(MockBehavior.Strict);
            manager.Setup(item =>
                item.GetAllCollectionsAsync(
                    It.Is<CollectionQueryDto>(item => item.SortCriteria == query.SortCriteria && item.SortOrder == query.SortOrder)))
                .ThrowsAsync(new Exception("message"))
                .Verifiable();

            var authenticationContext = new Mock<IAuthenticationServices>(MockBehavior.Strict);
            authenticationContext.Setup(item => item.Email)
                .Returns("collab@email.com")
                .Verifiable();

            var guidGenerator = new Mock<IGuidGenerator>();

            var controller = new MandateController(logger.Object, manager.Object, authenticationContext.Object, guidGenerator.Object, null!);

            var result = await controller.GetCollectionsAsync(
                string.Empty,
                null,
                null,
                null,
                null,
                null,
                10,
                0,
                "Ascending",
                "Name") as ObjectResult;

            result!.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);

            var errorType = result!.Value as Client.Error;
            errorType.Should().NotBeNull();
            errorType!.ErrorType.Should().Be("TechnicalError");
            errorType!.Message.Should().Be("message");

            logger.VerifyAll();
            manager.VerifyAll();
        }

        [Fact]
        public async Task GetTechnicalCollectionsAsync_CaseOK()
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
                SortOrder.Ascending,
                CollectionSortCriteria.Name,
                "collab@email.com");

            var company = new Company(
                1,
                "cn",
                "12345678910",
                "123456789",
                "12345",
                null,
                null);

            Bank bank = new Bank("12345", "bn", "bg", string.Empty, new BankAgreement(JdcPartnership.NonPartner));

            Bban bban = new Bban("12345", "54321", "12345678901", "55", "12347", bank);

            Collection collection = new Collection(
                    new Guid("00000002-0000-0000-0000-000000000000"),
                    string.Empty,
                    company,
                    bban,
                    new DateTime(2022, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    new DateTime(2022, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    new Status(CollectionStatus.InProgress, "En cours"));

            var pm = new PagedTechnicalMandate(
                new Counters(1, 1, 0, 0, 0, 0),
                new List<Collection>()
                {
                    collection,
                });

            var logger = new Mock<ILogger<MandateController>>(MockBehavior.Loose);
            var manager = new Mock<IMandateManager>(MockBehavior.Strict);
            manager.Setup(item =>
                item.GetAllTechnicalCollectionsAsync(
                    It.Is<CollectionQueryDto>(item => item.SortCriteria == query.SortCriteria && item.SortOrder == query.SortOrder)))
                .ReturnsAsync(pm)
                .Verifiable();

            var guidGenerator = new Mock<IGuidGenerator>();

            var controller = new MandateController(logger.Object, manager.Object, null!, guidGenerator.Object, null!);

            var result = await controller.GetTechnicalCollectionsAsync(
                string.Empty,
                null,
                null,
                null,
                null,
                null,
                10,
                0,
                "Ascending",
                "Name");

            var expectedCollection = new Client.TechnicalCollectionSummary(
                id: new Guid("00000002-0000-0000-0000-000000000000"),
                folderId: "12345",
                ribId: "12347",
                bankDetails: new Client.BankDetails(
                    bankCode: "12345",
                    branchCode: "54321",
                    accountNumber: "12345678901",
                    checkDigits: "55"),
                statusCode: 30);

            var expectedCollections = new List<Client.TechnicalCollectionSummary>()
            {
               expectedCollection,
            };

            result.As<OkObjectResult>().StatusCode.Should().Be(200);
            result.As<OkObjectResult>().Value.Should().BeEquivalentTo(new Client.PagedTechnicalMandate(expectedCollections));

            logger.VerifyAll();
            manager.VerifyAll();
        }

        [Fact]
        public async Task GetTechnicalCollectionsAsync_CaseThrowException()
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
                SortOrder.Ascending,
                CollectionSortCriteria.Name,
                "collab@email.com");

            var logger = new Mock<ILogger<MandateController>>(MockBehavior.Loose);
            var manager = new Mock<IMandateManager>(MockBehavior.Strict);
            manager.Setup(item =>
                item.GetAllTechnicalCollectionsAsync(
                    It.Is<CollectionQueryDto>(item => item.SortCriteria == query.SortCriteria && item.SortOrder == query.SortOrder)))
                .ThrowsAsync(new Exception("message"))
                .Verifiable();

            var guidGenerator = new Mock<IGuidGenerator>();

            var controller = new MandateController(logger.Object, manager.Object, null!, guidGenerator.Object, null!);

            var result = await controller.GetTechnicalCollectionsAsync(
                string.Empty,
                null,
                null,
                null,
                null,
                null,
                10,
                0,
                "Ascending",
                "Name") as ObjectResult;

            result!.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);

            var errorType = result!.Value as Client.Error;
            errorType.Should().NotBeNull();
            errorType!.ErrorType.Should().Be("TechnicalError");
            errorType!.Message.Should().Be("message");

            logger.VerifyAll();
            manager.VerifyAll();
        }

        [Fact]
        public async Task RefreshMandatsStatusesAsync_ShouldReturnOk_WhenSuccessful()
        {
            // Arrange
            var technicalCollectionSummaries = new List<Client.TechnicalCollectionSummary>
            {
                new Client.TechnicalCollectionSummary(
                    Guid.NewGuid(),
                    "folderId1",
                    "ribId1",
                    new Client.BankDetails(
                        "bankCode1",
                        "branchCode1",
                        "accountNumber1",
                        "checkDigits1"),
                    30),
                new Client.TechnicalCollectionSummary(
                    Guid.NewGuid(),
                    "folderId2",
                    "ribId2",
                    new Client.BankDetails(
                        "bankCode2",
                        "branchCode2",
                        "accountNumber2",
                        "checkDigits2"),
                    20),
            };

            var logger = new Mock<ILogger<MandateController>>(MockBehavior.Loose);
            var manager = new Mock<IMandateManager>(MockBehavior.Strict);
            var guidGenerator = new Mock<IGuidGenerator>();

            manager.Setup(m => m.RefreshMandatsStatusesAsync(It.IsAny<List<TechnicalCollection>>()))
                               .Returns(Task.CompletedTask)
                               .Verifiable();

            var controller = new MandateController(logger.Object, manager.Object, null!, guidGenerator.Object, null!);

            // Act
            var result = await controller.RefreshMandatsStatusesAsync(technicalCollectionSummaries);

            // Assert
            result.Should().BeOfType<OkResult>();
            manager.VerifyAll();
            logger.VerifyAll();
            guidGenerator.VerifyAll();
        }

        [Fact]
        public async Task RefreshMandatsStatusesAsync_ShouldReturnInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            var technicalCollectionSummaries = new List<Client.TechnicalCollectionSummary>
            {
                new Client.TechnicalCollectionSummary(
                    Guid.NewGuid(),
                    "folderId1",
                    "ribId1",
                    new Client.BankDetails(
                        "bankCode1",
                        "branchCode1",
                        "accountNumber1",
                        "checkDigits1"),
                    30),
                new Client.TechnicalCollectionSummary(
                    Guid.NewGuid(),
                    "folderId2",
                    "ribId2",
                    new Client.BankDetails(
                        "bankCode2",
                        "branchCode2",
                        "accountNumber2",
                        "checkDigits2"),
                    20),
            };

            var logger = new Mock<ILogger<MandateController>>(MockBehavior.Loose);
            var manager = new Mock<IMandateManager>(MockBehavior.Strict);
            var guidGenerator = new Mock<IGuidGenerator>();

            manager.Setup(m => m.RefreshMandatsStatusesAsync(It.IsAny<List<TechnicalCollection>>()))
                               .ThrowsAsync(new Exception())
                               .Verifiable();

            var controller = new MandateController(logger.Object, manager.Object, null!, guidGenerator.Object, null!);

            // Act
            var result = await controller.RefreshMandatsStatusesAsync(technicalCollectionSummaries) as ObjectResult;

            // Assert
            result.Should().BeOfType<ObjectResult>().Which.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
            result.Should().NotBeNull();
            result!.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);

            var errorType = result!.Value as Client.Error;
            errorType.Should().NotBeNull();
            errorType!.ErrorType.Should()
                               .Be("TechnicalError");

            manager.VerifyAll();
            logger.VerifyAll();
            guidGenerator.VerifyAll();
        }

        [Fact]
        public async Task DownloadUnsignedAsync_CaseOK()
        {
            var guidGenerator = new Mock<IGuidGenerator>();

            var mandateId = "00000001-0000-0000-0000-000000000000";
            byte[] expectedFileData = { 0, 16, 104, 213 };

            var mandateManager = new Mock<IMandateManager>();
            mandateManager.Setup(m => m.DownloadUnsignedAsync(It.IsAny<Guid>()))
                .Callback<Guid>(m =>
                {
                    m.Should().Be(Guid.Parse(mandateId));
                })
                .ReturnsAsync(expectedFileData);

            var logger = new Mock<ILogger<MandateController>>(MockBehavior.Strict);
            logger.Setup(x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsValueType>(),
                It.IsAny<Exception?>(),
                (Func<It.IsValueType, Exception?, string>)It.IsAny<object>())); // Ignore all logs
            var expectedContentType = "application/pdf";
            var expectedFileName = $"unsigned-mandate-{mandateId}.pdf";

            var controller = new MandateController(logger.Object, mandateManager.Object, null!, guidGenerator.Object, null!);

            var result = await controller.DownloadUnsignedAsync(mandateId);

            result.Should().NotBeNull();
            var fileResult = result.Should().BeOfType<FileContentResult>().Subject;
            fileResult.FileContents.Should().BeEquivalentTo(expectedFileData);
            fileResult.ContentType.Should().Be(expectedContentType);
            fileResult.FileDownloadName.Should().Be(expectedFileName);

            mandateManager.VerifyAll();
            guidGenerator.VerifyAll();
        }

        [Fact]
        public async Task DownloadUnsignedAsync_CaseCollectionNotFoundException()
        {
            var mandateId = "00000001-0000-0000-0000-000000000000";

            var mandateManager = new Mock<IMandateManager>();
            mandateManager.Setup(m => m.DownloadUnsignedAsync(It.IsAny<Guid>()))
                .Callback<Guid>(m =>
                {
                    m.Should().Be(Guid.Parse(mandateId));
                })
                .Throws(new Sql.CollectionNotFoundException());

            var logger = new Mock<ILogger<MandateController>>(MockBehavior.Strict);
            logger.Setup(x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsValueType>(),
                It.IsAny<Exception?>(),
                (Func<It.IsValueType, Exception?, string>)It.IsAny<object>())); // Ignore all logs

            var controller = new MandateController(logger.Object, mandateManager.Object, null!, null!, null!);

            var result = await controller.DownloadUnsignedAsync(mandateId) as ObjectResult;

            result.Should().NotBeNull();
            result!.StatusCode.Should().Be((int)HttpStatusCode.NotFound);

            var errorType = result!.Value as Client.Error;
            errorType.Should().NotBeNull();
            errorType!.ErrorType.Should()
                               .Be("CollectionNotFound");
            errorType!.LogReference.Should()
                                  .Be("0");

            mandateManager.VerifyAll();
            logger.VerifyAll();
        }

        [Fact]
        public async Task DownloadUnsignedAsync_CaseFolderIdEmptyOrNullException()
        {
            var mandateId = "00000001-0000-0000-0000-000000000000";

            var mandateManager = new Mock<IMandateManager>();
            mandateManager.Setup(m => m.DownloadUnsignedAsync(It.IsAny<Guid>()))
                .Callback<Guid>(m =>
                {
                    m.Should().Be(Guid.Parse(mandateId));
                })
                .Throws(new FolderIdEmptyOrNullException());

            var logger = new Mock<ILogger<MandateController>>(MockBehavior.Strict);
            logger.Setup(x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsValueType>(),
                It.IsAny<Exception?>(),
                (Func<It.IsValueType, Exception?, string>)It.IsAny<object>())); // Ignore all logs

            var controller = new MandateController(logger.Object, mandateManager.Object, null!, null!, null!);

            var result = await controller.DownloadUnsignedAsync(mandateId) as ObjectResult;

            result.Should().NotBeNull();
            result!.StatusCode.Should().Be((int)HttpStatusCode.NotFound);

            var errorType = result!.Value as Client.Error;
            errorType.Should().NotBeNull();
            errorType!.ErrorType.Should()
                               .Be("FolderIdEmptyOrNull");
            errorType!.LogReference.Should()
                                  .Be("0");

            mandateManager.VerifyAll();
            logger.VerifyAll();
        }

        [Fact]
        public async Task DownloadUnsignedAsync_CaseRibIdEmptyOrNullException()
        {
            var mandateId = "00000001-0000-0000-0000-000000000000";

            var mandateManager = new Mock<IMandateManager>();
            mandateManager.Setup(m => m.DownloadUnsignedAsync(It.IsAny<Guid>()))
                .Callback<Guid>(m =>
                {
                    m.Should().Be(Guid.Parse(mandateId));
                })
                .Throws(new RibIdEmptyOrNullException());

            var logger = new Mock<ILogger<MandateController>>(MockBehavior.Strict);
            logger.Setup(x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsValueType>(),
                It.IsAny<Exception?>(),
                (Func<It.IsValueType, Exception?, string>)It.IsAny<object>())); // Ignore all logs

            var controller = new MandateController(logger.Object, mandateManager.Object, null!, null!, null!);

            var result = await controller.DownloadUnsignedAsync(mandateId) as ObjectResult;

            result.Should().NotBeNull();
            result!.StatusCode.Should().Be((int)HttpStatusCode.NotFound);

            var errorType = result!.Value as Client.Error;
            errorType.Should().NotBeNull();
            errorType!.ErrorType.Should()
                               .Be("RibIdEmptyOrNull");
            errorType!.LogReference.Should()
                                  .Be("0");

            mandateManager.VerifyAll();
            logger.VerifyAll();
        }

        [Fact]
        public async Task UploadSignedMandateAsync_WithValidPdfFile_ReturnsOkResult()
        {
            // Arrange
            var validMandateId = "00000001-0000-0000-0000-000000000000";

            var authenticationContext = new Mock<IAuthenticationServices>(MockBehavior.Strict);
            authenticationContext.Setup(item => item.Email)
                .Returns("collab@email.com")
                .Verifiable();

            var mandateManagerMock = new Mock<IMandateManager>();
            mandateManagerMock.Setup(m => m.UploadSignedMandateAsync(It.IsAny<Guid>(), It.IsAny<Stream>(), It.IsAny<string>()))
                .ReturnsAsync("signedMandateId")
                .Verifiable();

            var logger = new Mock<ILogger<MandateController>>(MockBehavior.Strict);
            logger.Setup(x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsValueType>(),
                It.IsAny<Exception?>(),
                (Func<It.IsValueType, Exception?, string>)It.IsAny<object>())); // Ignore all logs

            var controller = new MandateController(logger.Object, mandateManagerMock.Object, authenticationContext.Object, null!, null!);

            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.ContentType).Returns("application/pdf");
            var content = "PDF file content";
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.Write(content);
            writer.Flush();
            ms.Position = 0;
            fileMock.Setup(f => f.OpenReadStream()).Returns(ms);

            // Act
            var result = await controller.UploadSignedMandateAsync(validMandateId, fileMock.Object);

            // Assert
            var okResult = result as OkObjectResult;
            okResult?.StatusCode.Should().Be(200);
            okResult?.Value.Should().Be("signedMandateId");
            okResult.Should().NotBeNull();
            okResult.Should().BeOfType<OkObjectResult>();
            mandateManagerMock.Verify(m => m.UploadSignedMandateAsync(It.IsAny<Guid>(), It.IsAny<Stream>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task UploadSignedMandateAsync_WithNullFile_ReturnsBadRquest()
        {
            // Arrange
            var newGuid = Guid.Parse("a0000000-0000-0000-0000-000000000000");
            var guidGenerator = new Mock<IGuidGenerator>();
            guidGenerator.Setup(g => g.NewGuid())
                .Returns(newGuid);

            var validMandateId = "00000001-0000-0000-0000-000000000000";

            var mandateManagerMock = new Mock<IMandateManager>();
            var logger = new Mock<ILogger<MandateController>>(MockBehavior.Strict);
            logger.Setup(x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsValueType>(),
                It.IsAny<Exception?>(),
                (Func<It.IsValueType, Exception?, string>)It.IsAny<object>())); // Ignore all logs

            var controller = new MandateController(logger.Object, mandateManagerMock.Object, null!, guidGenerator.Object, null!);

            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.ContentType).Returns("application/jpg");
            var content = "PDF file content";
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.Write(content);
            writer.Flush();
            ms.Position = 0;
            fileMock.Setup(f => f.OpenReadStream()).Returns(ms);

            // Act
            var result = await controller.UploadSignedMandateAsync(validMandateId, fileMock.Object);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult?.StatusCode.Should().Be(400);
            badRequestResult.Should().NotBeNull();
            badRequestResult.Should().BeOfType<BadRequestObjectResult>();
            mandateManagerMock.Verify(m => m.UploadSignedMandateAsync(It.IsAny<Guid>(), It.IsAny<Stream>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task UploadSignedMandateAsync_WithInValidFileType_ReturnsBadRequest()
        {
            // Arrange
            var newGuid = Guid.Parse("a0000000-0000-0000-0000-000000000000");
            var guidGenerator = new Mock<IGuidGenerator>();
            guidGenerator.Setup(g => g.NewGuid())
                .Returns(newGuid);

            var validMandateId = "00000001-0000-0000-0000-000000000000";

            var mandateManagerMock = new Mock<IMandateManager>();
            var logger = new Mock<ILogger<MandateController>>(MockBehavior.Strict);
            logger.Setup(x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsValueType>(),
                It.IsAny<Exception?>(),
                (Func<It.IsValueType, Exception?, string>)It.IsAny<object>())); // Ignore all logs

            var controller = new MandateController(logger.Object, mandateManagerMock.Object, null!, guidGenerator.Object, null!);

            // Act
            var result = await controller.UploadSignedMandateAsync(validMandateId, null!);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult?.StatusCode.Should().Be(400);
            badRequestResult.Should().NotBeNull();
            badRequestResult.Should().BeOfType<BadRequestObjectResult>();
            mandateManagerMock.Verify(m => m.UploadSignedMandateAsync(It.IsAny<Guid>(), It.IsAny<Stream>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task UploadSignedMandateAsync_WithInValidMandateId_ReturnsBadRequest()
        {
            // Arrange
            var newGuid = Guid.Parse("a0000000-0000-0000-0000-000000000000");
            var guidGenerator = new Mock<IGuidGenerator>();
            guidGenerator.Setup(g => g.NewGuid())
                .Returns(newGuid);

            var inValidMandateId = "00000001-0000-0000-0000-00000";

            var mandateManagerMock = new Mock<IMandateManager>();
            var logger = new Mock<ILogger<MandateController>>(MockBehavior.Strict);
            logger.Setup(x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsValueType>(),
                It.IsAny<Exception?>(),
                (Func<It.IsValueType, Exception?, string>)It.IsAny<object>())); // Ignore all logs

            var controller = new MandateController(logger.Object, mandateManagerMock.Object, null!, guidGenerator.Object, null!);

            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.ContentType).Returns("application/pdf");
            var content = "PDF file content";
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.Write(content);
            writer.Flush();
            ms.Position = 0;
            fileMock.Setup(f => f.OpenReadStream()).Returns(ms);

            // Act
            var result = await controller.UploadSignedMandateAsync(inValidMandateId, fileMock.Object);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult.Should().BeOfType<BadRequestObjectResult>();
            mandateManagerMock.Verify(m => m.UploadSignedMandateAsync(It.IsAny<Guid>(), It.IsAny<Stream>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task UploadSignedMandateAsync_WhenJeDeclareThrowJeDeclareApiException_ReturnsInternalServerError()
        {
            // Arrange
            var newGuid = Guid.Parse("a0000000-0000-0000-0000-000000000000");
            var guidGenerator = new Mock<IGuidGenerator>();
            guidGenerator.Setup(g => g.NewGuid())
                .Returns(newGuid);

            var validMandateId = "00000001-0000-0000-0000-000000000000";

            var authenticationContext = new Mock<IAuthenticationServices>(MockBehavior.Strict);
            authenticationContext.Setup(item => item.Email)
                .Returns("collab@email.com")
                .Verifiable();

            var mandateManagerMock = new Mock<IMandateManager>();
            mandateManagerMock.Setup(m => m.UploadSignedMandateAsync(It.IsAny<Guid>(), It.IsAny<Stream>(), It.IsAny<string>()))
                .ThrowsAsync(new ServicesProviderException("Test exception"))
                .Verifiable();

            var logger = new Mock<ILogger<MandateController>>(MockBehavior.Strict);
            logger.Setup(x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsValueType>(),
                It.IsAny<Exception?>(),
                (Func<It.IsValueType, Exception?, string>)It.IsAny<object>())); // Ignore all logs

            var controller = new MandateController(logger.Object, mandateManagerMock.Object, authenticationContext.Object, guidGenerator.Object, null!);

            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.ContentType).Returns("application/pdf");
            var content = "PDF file content";
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.Write(content);
            writer.Flush();
            ms.Position = 0;
            fileMock.Setup(f => f.OpenReadStream()).Returns(ms);

            // Act
            var result = await controller.UploadSignedMandateAsync(validMandateId, fileMock.Object);

            // Assert
            var internalServerErrorResult = result as ObjectResult;
            internalServerErrorResult?.StatusCode.Should().Be(500);
            internalServerErrorResult.Should().NotBeNull();
            internalServerErrorResult.Should().BeOfType<ObjectResult>();
            mandateManagerMock.Verify(m => m.UploadSignedMandateAsync(It.IsAny<Guid>(), It.IsAny<Stream>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task UploadSignedMandateAsync_ReturnsInternalServerError()
        {
            // Arrange
            var newGuid = Guid.Parse("a0000000-0000-0000-0000-000000000000");
            var guidGenerator = new Mock<IGuidGenerator>();
            guidGenerator.Setup(g => g.NewGuid())
                .Returns(newGuid);

            var validMandateId = "00000001-0000-0000-0000-000000000000";

            var authenticationContext = new Mock<IAuthenticationServices>(MockBehavior.Strict);
            authenticationContext.Setup(item => item.Email)
                .Returns("collab@email.com")
                .Verifiable();

            var mandateManagerMock = new Mock<IMandateManager>();
            mandateManagerMock.Setup(m => m.UploadSignedMandateAsync(It.IsAny<Guid>(), It.IsAny<Stream>(), It.IsAny<string>()))
                .ThrowsAsync(new Exception("Test exception"))
                .Verifiable();

            var logger = new Mock<ILogger<MandateController>>(MockBehavior.Strict);
            logger.Setup(x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsValueType>(),
                It.IsAny<Exception?>(),
                (Func<It.IsValueType, Exception?, string>)It.IsAny<object>())); // Ignore all logs

            var controller = new MandateController(logger.Object, mandateManagerMock.Object, authenticationContext.Object, guidGenerator.Object, null!);

            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.ContentType).Returns("application/pdf");
            var content = "PDF file content";
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.Write(content);
            writer.Flush();
            ms.Position = 0;
            fileMock.Setup(f => f.OpenReadStream()).Returns(ms);

            // Act
            var result = await controller.UploadSignedMandateAsync(validMandateId, fileMock.Object);

            // Assert
            var internalServerErrorResult = result as ObjectResult;
            internalServerErrorResult?.StatusCode.Should().Be(500);
            internalServerErrorResult.Should().NotBeNull();
            internalServerErrorResult.Should().BeOfType<ObjectResult>();
            mandateManagerMock.Verify(m => m.UploadSignedMandateAsync(It.IsAny<Guid>(), It.IsAny<Stream>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task DownloadSignedAsync_WithValidMandateId_ReturnsOkResult()
        {
            var guidGenerator = new Mock<IGuidGenerator>();

            var mandateId = "00000001-0000-0000-0000-000000000000";
            byte[] expectedFileData = { 0, 16, 104, 213 };

            var mandateManager = new Mock<IMandateManager>();
            mandateManager.Setup(m => m.DownloadSignedAsync(It.IsAny<Guid>()))
                .Callback<Guid>(m =>
                {
                    m.Should().Be(Guid.Parse(mandateId));
                })
                .ReturnsAsync(expectedFileData);

            var logger = new Mock<ILogger<MandateController>>(MockBehavior.Strict);
            logger.Setup(x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsValueType>(),
                It.IsAny<Exception?>(),
                (Func<It.IsValueType, Exception?, string>)It.IsAny<object>())); // Ignore all logs
            var expectedContentType = "application/pdf";
            var expectedFileName = $"signed-mandate-{mandateId}.pdf";

            var controller = new MandateController(logger.Object, mandateManager.Object, null!, guidGenerator.Object, null!);

            var result = await controller.DownloadSignedAsync(mandateId);

            result.Should().NotBeNull();
            var fileResult = result.Should().BeOfType<FileContentResult>().Subject;
            fileResult.FileContents.Should().BeEquivalentTo(expectedFileData);
            fileResult.ContentType.Should().Be(expectedContentType);
            fileResult.FileDownloadName.Should().Be(expectedFileName);

            mandateManager.VerifyAll();
            guidGenerator.VerifyAll();
        }

        [Fact]
        public async Task DownloadSignedAsync_CaseCollectionNotFoundException()
        {
            var mandateId = "00000001-0000-0000-0000-000000000000";

            var mandateManager = new Mock<IMandateManager>();
            mandateManager.Setup(m => m.DownloadSignedAsync(It.IsAny<Guid>()))
                .Callback<Guid>(m =>
                {
                    m.Should().Be(Guid.Parse(mandateId));
                })
                .Throws(new Sql.CollectionNotFoundException());

            var logger = new Mock<ILogger<MandateController>>(MockBehavior.Strict);
            logger.Setup(x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsValueType>(),
                It.IsAny<Exception?>(),
                (Func<It.IsValueType, Exception?, string>)It.IsAny<object>())); // Ignore all logs

            var controller = new MandateController(logger.Object, mandateManager.Object, null!, null!, null!);

            var result = await controller.DownloadSignedAsync(mandateId) as ObjectResult;

            result.Should().NotBeNull();
            result!.StatusCode.Should().Be((int)HttpStatusCode.NotFound);

            var errorType = result!.Value as Client.Error;
            errorType.Should().NotBeNull();
            errorType!.ErrorType.Should()
                               .Be("CollectionNotFound");
            errorType!.LogReference.Should()
                                  .Be("0");

            mandateManager.VerifyAll();
            logger.VerifyAll();
        }

        [Fact]
        public async Task DownloadSignedAsync_CaseFolderIdEmptyOrNullException()
        {
            var mandateId = "00000001-0000-0000-0000-000000000000";

            var mandateManager = new Mock<IMandateManager>();
            mandateManager.Setup(m => m.DownloadSignedAsync(It.IsAny<Guid>()))
                .Callback<Guid>(m =>
                {
                    m.Should().Be(Guid.Parse(mandateId));
                })
                .Throws(new FolderIdEmptyOrNullException());

            var logger = new Mock<ILogger<MandateController>>(MockBehavior.Strict);
            logger.Setup(x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsValueType>(),
                It.IsAny<Exception?>(),
                (Func<It.IsValueType, Exception?, string>)It.IsAny<object>())); // Ignore all logs

            var controller = new MandateController(logger.Object, mandateManager.Object, null!, null!, null!);

            var result = await controller.DownloadSignedAsync(mandateId) as ObjectResult;

            result.Should().NotBeNull();
            result!.StatusCode.Should().Be((int)HttpStatusCode.NotFound);

            var errorType = result!.Value as Client.Error;
            errorType.Should().NotBeNull();
            errorType!.ErrorType.Should()
                               .Be("FolderIdEmptyOrNull");
            errorType!.LogReference.Should()
                                  .Be("0");

            mandateManager.VerifyAll();
            logger.VerifyAll();
        }

        [Fact]
        public async Task DownloadSignedAsync_CaseRibIdEmptyOrNullException()
        {
            var mandateId = "00000001-0000-0000-0000-000000000000";

            var mandateManager = new Mock<IMandateManager>();
            mandateManager.Setup(m => m.DownloadSignedAsync(It.IsAny<Guid>()))
                .Callback<Guid>(m =>
                {
                    m.Should().Be(Guid.Parse(mandateId));
                })
                .Throws(new RibIdEmptyOrNullException());

            var logger = new Mock<ILogger<MandateController>>(MockBehavior.Strict);
            logger.Setup(x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsValueType>(),
                It.IsAny<Exception?>(),
                (Func<It.IsValueType, Exception?, string>)It.IsAny<object>())); // Ignore all logs

            var controller = new MandateController(logger.Object, mandateManager.Object, null!, null!, null!);

            var result = await controller.DownloadSignedAsync(mandateId) as ObjectResult;

            result.Should().NotBeNull();
            result!.StatusCode.Should().Be((int)HttpStatusCode.NotFound);

            var errorType = result!.Value as Client.Error;
            errorType.Should().NotBeNull();
            errorType!.ErrorType.Should()
                               .Be("RibIdEmptyOrNull");
            errorType!.LogReference.Should()
                                  .Be("0");

            mandateManager.VerifyAll();
            logger.VerifyAll();
        }

        [Fact]
        public async Task DownloadSignedAsync_CaseServicesProviderException()
        {
            var mandateId = "00000001-0000-0000-0000-000000000000";

            var mandateManager = new Mock<IMandateManager>();
            mandateManager.Setup(m => m.DownloadSignedAsync(It.IsAny<Guid>()))
                .Callback<Guid>(m =>
                {
                    m.Should().Be(Guid.Parse(mandateId));
                })
                .Throws(new ServicesProviderException());

            var logger = new Mock<ILogger<MandateController>>(MockBehavior.Strict);
            logger.Setup(x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsValueType>(),
                It.IsAny<Exception?>(),
                (Func<It.IsValueType, Exception?, string>)It.IsAny<object>())); // Ignore all logs

            var controller = new MandateController(logger.Object, mandateManager.Object, null!, null!, null!);

            var result = await controller.DownloadSignedAsync(mandateId) as ObjectResult;

            result.Should().NotBeNull();
            result!.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);

            var errorType = result!.Value as Client.Error;
            errorType.Should().NotBeNull();
            errorType!.ErrorType.Should()
                               .Be("ServicesProviderError");
            errorType!.LogReference.Should()
                                  .Be("0");

            mandateManager.VerifyAll();
            logger.VerifyAll();
        }

        [Fact]
        public async Task DownloadSignedAsync_CaseException()
        {
            var mandateId = "00000001-0000-0000-0000-000000000000";

            var mandateManager = new Mock<IMandateManager>();
            mandateManager.Setup(m => m.DownloadSignedAsync(It.IsAny<Guid>()))
                .Callback<Guid>(m =>
                {
                    m.Should().Be(Guid.Parse(mandateId));
                })
                .Throws(new Exception());

            var logger = new Mock<ILogger<MandateController>>(MockBehavior.Strict);
            logger.Setup(x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsValueType>(),
                It.IsAny<Exception?>(),
                (Func<It.IsValueType, Exception?, string>)It.IsAny<object>())); // Ignore all logs

            var controller = new MandateController(logger.Object, mandateManager.Object, null!, null!, null!);

            var result = await controller.DownloadSignedAsync(mandateId) as ObjectResult;

            result.Should().NotBeNull();
            result!.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);

            var errorType = result!.Value as Client.Error;
            errorType.Should().NotBeNull();
            errorType!.ErrorType.Should()
                               .Be("Exception");
            errorType!.LogReference.Should()
                                  .Be("0");

            mandateManager.VerifyAll();
            logger.VerifyAll();
        }

        [Fact]
        public async Task DeactivateAsync_Ok()
        {
            var mandateId = new PredictableGuid().NewGuid();

            var authenticationContext = new Mock<IAuthenticationServices>(MockBehavior.Strict);
            authenticationContext.Setup(item => item.Email)
                .Returns("collab@email.com")
                .Verifiable();

            var mandateManager = new Mock<IMandateManager>();
            mandateManager.Setup(m => m.DeactivateCollectionAsync(mandateId, It.IsAny<string>()))
                .ReturnsAsync(true);

            var controller = new MandateController((new NullLoggerFactory() as ILoggerFactory).CreateLogger<MandateController>(), mandateManager.Object, authenticationContext.Object, null!, null!);

            var result = await controller.DeactivateAsync(mandateId.ToString());

            result.Should().NotBeNull();
            result.Should().BeOfType<NoContentResult>();

            mandateManager.VerifyAll();
        }

        [Fact]
        public async Task DeactivateAsync_Ko()
        {
            var mandateId = new PredictableGuid().NewGuid();

            var authenticationContext = new Mock<IAuthenticationServices>(MockBehavior.Strict);
            authenticationContext.Setup(item => item.Email)
                .Returns("collab@email.com")
                .Verifiable();

            var mandateManager = new Mock<IMandateManager>();
            mandateManager.Setup(m => m.DeactivateCollectionAsync(mandateId, It.IsAny<string>()))
                .ReturnsAsync(false);

            var controller = new MandateController((new NullLoggerFactory() as ILoggerFactory).CreateLogger<MandateController>(), mandateManager.Object, authenticationContext.Object, null!, null!);

            var result = await controller.DeactivateAsync(mandateId.ToString());

            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundResult>();

            mandateManager.VerifyAll();
        }

        [Fact]
        public async Task DeactivateAsync_Throws()
        {
            var controller = new MandateController((new NullLoggerFactory() as ILoggerFactory).CreateLogger<MandateController>(), null!, null!, null!, null!);

            var result = await controller.DeactivateAsync("x");

            result.Should().NotBeNull();
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task RecoveryAsync_ReturnOK()
        {
            // Arrange
            var newGuid = Guid.Parse("a0000000-0000-0000-0000-000000000000");
            var guidGenerator = new Mock<IGuidGenerator>();
            guidGenerator.Setup(g => g.NewGuid())
                .Returns(newGuid);

            var logger = new Mock<ILogger<MandateController>>(MockBehavior.Strict);
            logger.Setup(x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsValueType>(),
                It.IsAny<Exception?>(),
                (Func<It.IsValueType, Exception?, string>)It.IsAny<object>()));

            var company = new Company(
                1,
                "cn",
                "12345678910",
                "123456789",
                string.Empty,
                null,
                null);

            Bank bank = new Bank("12345", "bn", "bg", string.Empty, new BankAgreement(JdcPartnership.NonPartner));

            Bban bban = new Bban("12345", "54321", "12345678901", "55", string.Empty, bank);

            Collection collection = new Collection(
                    new Guid("00000002-0000-0000-0000-000000000000"),
                    string.Empty,
                    company,
                    bban,
                    new DateTime(2022, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    new DateTime(2022, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    new Status(CollectionStatus.InProgress, "En cours"));

            var formIoManager = new Mock<IFormioManager>(MockBehavior.Strict);
            formIoManager.Setup(item =>
                item.GetCollectionByBban(
                    It.Is<Bban>(b =>
                        b.BankCode == "12345" &&
                        b.BranchCode == "54321" &&
                        b.AccountNumber == "12345678901" &&
                        b.CheckDigits == "01")))
               .ReturnsAsync(collection)
               .Verifiable();

            var mandateManager = new Mock<IMandateManager>();
            mandateManager.Setup(_ => _.InsertFormIOCollectionAsync(It.IsAny<Collection>()))
                            .Returns(Task.CompletedTask)
                            .Verifiable();

            var controller = new MandateController(logger.Object, mandateManager.Object, null!, guidGenerator.Object, formIoManager.Object);

            var clientBBan = new Client.Bban("12345", "54321", "12345678901", "01");

            // Act
            var result = await controller.Recovery(clientBBan);

            // Assert
            result.Should().NotBeNull();
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task RecoveryAsync_Return_NoContent()
        {
            // Arrange
            var newGuid = Guid.Parse("a0000000-0000-0000-0000-000000000000");
            var guidGenerator = new Mock<IGuidGenerator>();
            guidGenerator.Setup(g => g.NewGuid())
                .Returns(newGuid);

            var logger = new Mock<ILogger<MandateController>>(MockBehavior.Strict);
            logger.Setup(x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsValueType>(),
                It.IsAny<Exception?>(),
                (Func<It.IsValueType, Exception?, string>)It.IsAny<object>()));

            Collection? collection = null;

            var formIoManager = new Mock<IFormioManager>(MockBehavior.Strict);
            formIoManager.Setup(item =>
                item.GetCollectionByBban(
                    It.Is<Bban>(b =>
                        b.BankCode == "12345" &&
                        b.BranchCode == "54321" &&
                        b.AccountNumber == "12345678901" &&
                        b.CheckDigits == "01")))
               .ReturnsAsync(collection)
               .Verifiable();

            var mandateManager = new Mock<IMandateManager>();
            mandateManager.Setup(_ => _.InsertFormIOCollectionAsync(It.IsAny<Collection>()))
                            .Returns(Task.CompletedTask)
                            .Verifiable();

            var controller = new MandateController(logger.Object, mandateManager.Object, null!, guidGenerator.Object, formIoManager.Object);

            var clientBBan = new Client.Bban("12345", "54321", "12345678901", "01");

            // Act
            var result = await controller.Recovery(clientBBan);

            // Assert
            result.Should().NotBeNull();
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task RecoveryAsync_ThrowsException()
        {
            // Arrange
            var newGuid = Guid.Parse("a0000000-0000-0000-0000-000000000000");
            var guidGenerator = new Mock<IGuidGenerator>();
            guidGenerator.Setup(g => g.NewGuid())
                .Returns(newGuid);

            var logger = new Mock<ILogger<MandateController>>(MockBehavior.Strict);
            logger.Setup(x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsValueType>(),
                It.IsAny<Exception?>(),
                (Func<It.IsValueType, Exception?, string>)It.IsAny<object>()));

            var company = new Company(
                1,
                "cn",
                "12345678910",
                "123456789",
                string.Empty,
                null,
                null);

            Bank bank = new Bank("12345", "bn", "bg", string.Empty, new BankAgreement(JdcPartnership.NonPartner));

            Bban bban = new Bban("12345", "54321", "12345678901", "55", string.Empty, bank);

            Collection collection = new Collection(
                    new Guid("00000002-0000-0000-0000-000000000000"),
                    string.Empty,
                    company,
                    bban,
                    new DateTime(2022, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    new DateTime(2022, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    new Status(CollectionStatus.InProgress, "En cours"));

            var formIoManager = new Mock<IFormioManager>(MockBehavior.Strict);
            formIoManager.Setup(item =>
                item.GetCollectionByBban(
                    It.Is<Bban>(b =>
                        b.BankCode == "12345" &&
                        b.BranchCode == "54321" &&
                        b.AccountNumber == "12345678901" &&
                        b.CheckDigits == "01")))
               .ReturnsAsync(collection)
               .Verifiable();

            var mandateManager = new Mock<IMandateManager>();
            mandateManager.Setup(_ => _.InsertFormIOCollectionAsync(It.IsAny<Collection>()))
                            .Throws(new Exception());

            var controller = new MandateController(logger.Object, mandateManager.Object, null!, guidGenerator.Object, formIoManager.Object);

            var clientBBan = new Client.Bban("12345", "54321", "12345678901", "01");

            // Act
            ObjectResult result = (ObjectResult)await controller.Recovery(clientBBan);

            // Assert
            result.Should().NotBeNull();
            Assert.IsType<ObjectResult>(result);
            result!.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        }

        [Fact]
        public async Task RecoveryFormIOAsync_ShouldCallGetAllCollectionAsyncAndInsertFormIOCollectionAsync_AndReturnOkResult()
        {
            // Arrange
            await using var database = SqlServerFixture.CreateDatabase();

            this.options = Options.Create(new SqlMandateRepositoryOptions() { ConnectionString = SqlServerFixture.ConnectionString });
            using var context = new MandateContext(this.options);

            await context.Company.AddAsync(new Sql.CompanyDb()
            {
                Id = 1,
                Name = "cn",
                ErpId = "123456789",
                SiretNumber = "12345678910",
                IsActive = true,
            });

            await context.SaveChangesAsync();
            var sqlRepo = new SqlMandateRepository(this.options);

            await sqlRepo.CreateFakeRefAsync();
            var adapter = new SqlAdapter(sqlRepo);
            var companyManager = new CompanyManager(adapter);

            var mandateManager = new MandateManager(adapter, companyManager, this.mockJeDeclareService.Object, this.mockAsposeHelper.Object, null!, this.emailOptions, this.mockMandateLogger.Object);

            var newGuid = Guid.Parse("a0000000-0000-0000-0000-000000000000");
            var guidGenerator = new Mock<IGuidGenerator>();
            guidGenerator.Setup(g => g.NewGuid())
                .Returns(newGuid);

            var logger = new Mock<ILogger<MandateController>>(MockBehavior.Strict);
            logger.Setup(x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsValueType>(),
                It.IsAny<Exception?>(),
                (Func<It.IsValueType, Exception?, string>)It.IsAny<object>()));

            var formIoManager = new Mock<IFormioManager>(MockBehavior.Strict);
            formIoManager.Setup(item =>
                item.GetAllCollectionAsync(0, 1000))
               .ReturnsAsync(GetTestCollection())
               .Verifiable();

            var controller = new MandateController(logger.Object, mandateManager, null!, guidGenerator.Object, formIoManager.Object);

            // Act
            var result = (OkObjectResult)await controller.RecoveryFormIOAsync(0, 1000);

            // Assert
            var insertedCollections = await context.Collection.ToListAsync();
            insertedCollections.Count.Should().Be(100);

            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
            result.Value.Should().BeEquivalentTo(new Client.PagedRecoveryMandate(100,  new List<Client.CollectionSummary>()));
        }

        [Fact]
        public async Task RecoveryFormIOAsync_ShouldThrow_ApplicationException()
        {
            // Arrange
            var newGuid = Guid.Parse("a0000000-0000-0000-0000-000000000000");
            var guidGenerator = new Mock<IGuidGenerator>();
            guidGenerator.Setup(g => g.NewGuid())
                .Returns(newGuid);

            var logger = new Mock<ILogger<MandateController>>(MockBehavior.Strict);
            logger.Setup(x => x.Log(
               It.IsAny<LogLevel>(),
               It.IsAny<EventId>(),
               It.IsAny<It.IsValueType>(),
               It.IsAny<Exception?>(),
               (Func<It.IsValueType, Exception?, string>)It.IsAny<object>()));

            var formIoManager = new Mock<IFormioManager>(MockBehavior.Strict);
            formIoManager.Setup(item =>
                item.GetAllCollectionAsync(0, 1000))
               .ReturnsAsync(GetTestCollection())
               .Verifiable();

            var mandateManager = new Mock<IMandateManager>();
            mandateManager.Setup(_ => _.InsertFormIOCollectionAsync(It.IsAny<Collection>()))
                            .Throws<ApplicationException>();

            var controller = new MandateController(logger.Object, mandateManager.Object, null!, guidGenerator.Object, formIoManager.Object);

            // Act
            var result = (OkObjectResult)await controller.RecoveryFormIOAsync(0, 1000);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
            mandateManager.Verify(_ => _.InsertFormIOCollectionAsync(It.IsAny<Collection>()), Times.Exactly(100));
            ((Client.PagedRecoveryMandate)result.Value!).Imported.Should().Be(100);
            ((Client.PagedRecoveryMandate)result.Value!).Failed.Count.Should().Be(100);
        }

        [Fact]
        public async Task RecoveryFormIOAsync_ShouldThrow_CompanyNotFoundException_ApplicationException_SetupSequence()
        {
            // Arrange
            Company company = new Company(
                        1,
                        "cn",
                        "12345678910",
                        "123456789",
                        "54321",
                        null,
                        null);

            Bank bank = new Bank("30027", "bn", "bg", string.Empty, new BankAgreement(JdcPartnership.Partner));
            Bban bban1 = new Bban("30027", "00001", "12345678901", "55", "789", bank);
            Bban bban2 = new Bban("30027", "00002", "12345678901", "55", "789", bank);
            Bban bban3 = new Bban("30027", "00003", "12345678901", "55", "789", bank);

            Status status = new Status(CollectionStatus.InProgress, "InProgress");

            List<Collection> collections = new List<Collection>()
            {
                new Collection(
                    Guid.NewGuid(),
                    "54320",
                    company,
                    bban1,
                    DateTime.Now,
                    DateTime.Now,
                    status),

                new Collection(
                    Guid.NewGuid(),
                    "54321",
                    company,
                    bban2,
                    DateTime.Now,
                    DateTime.Now,
                    status),

                new Collection(
                    Guid.NewGuid(),
                    "54322",
                    company,
                    bban3,
                    DateTime.Now,
                    DateTime.Now,
                    status),
            };

            var newGuid = Guid.Parse("a0000000-0000-0000-0000-000000000000");
            var guidGenerator = new Mock<IGuidGenerator>();
            guidGenerator.Setup(g => g.NewGuid())
                .Returns(newGuid);

            var logger = new Mock<ILogger<MandateController>>(MockBehavior.Strict);
            logger.Setup(x => x.Log(
               It.IsAny<LogLevel>(),
               It.IsAny<EventId>(),
               It.IsAny<It.IsValueType>(),
               It.IsAny<Exception?>(),
               (Func<It.IsValueType, Exception?, string>)It.IsAny<object>()));

            var formIoManager = new Mock<IFormioManager>(MockBehavior.Strict);

            formIoManager.Setup(item =>
                item.GetAllCollectionAsync(0, 1000))
               .ReturnsAsync(collections)
               .Verifiable();

            var mandateManager = new Mock<IMandateManager>();
            mandateManager.SetupSequence(ite =>
                ite.InsertFormIOCollectionAsync(It.IsAny<Collection>()))
                .Returns(Task.CompletedTask)
                .Returns(Task.CompletedTask)
                .ThrowsAsync(new ApplicationException());

            var controller = new MandateController(logger.Object, mandateManager.Object, null!, guidGenerator.Object, formIoManager.Object);

            // Act
            var result = (OkObjectResult)await controller.RecoveryFormIOAsync(0, 1000);

            Client.PagedRecoveryMandate page = (Client.PagedRecoveryMandate)result.Value!;
            IReadOnlyList<Client.CollectionSummary> collectionsResult = page.Failed;

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
            mandateManager.Verify(_ => _.InsertFormIOCollectionAsync(It.IsAny<Collection>()), Times.Exactly(3));
            collectionsResult.Count.Should().Be(1);
            collectionsResult[0].ErpId.Should().Be(company.ErpId);
            collectionsResult[0].CompanyName.Should().Be(company.Name);
            collectionsResult[0].AccountNumber.Should().Be(bban2.AccountNumber);
            collectionsResult[0].BankName.Should().Be(bban2.Bank!.Name);
        }

        private static List<Collection> GetTestCollection()
        {
            var company = new Company(
                1,
                "cn",
                "12345678910",
                "123456789",
                "54321",
                null,
                null);

            Bank bank = new Bank("30027", "bn", "bg", string.Empty, new BankAgreement(JdcPartnership.Partner));

            var collections = new List<Collection>();
            for (int i = 1; i < 101; i++)
            {
                Bban bban = new Bban("30027", i.ToString("0000#"), "12345678901", "55", string.Empty, bank);

                collections.Add(new Collection(
                    Guid.NewGuid(),
                    string.Empty,
                    company,
                    bban,
                    new DateTime(2022, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    new DateTime(2022, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    new Status(CollectionStatus.InProgress, "En cours")));
            }

            return collections;
        }
    }
}