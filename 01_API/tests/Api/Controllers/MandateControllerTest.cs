// <copyright file="MandateControllerTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.AspNetCore.Tests
{
    using Aspose.Pdf.Operators;
    using KPMG.Pulse.Back.Accounting.Mandate.Adapters;
    using KPMG.Pulse.Back.Accounting.Mandate.Application;
    using KPMG.Pulse.Back.Accounting.Mandate.Sql.Implementation;
    using KPMG.Pulse.Back.Accounting.Mandate.Sql.Implementation.Tests;
    using KPMG.Pulse.Back.Accounting.Mandate.Sql.Implementation.Tests.Tools;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;
    using Microsoft.Extensions.Options;
    using System.Net;

    public class MandateControllerTest
        : SqlServerTestBase
    {
        private readonly Mock<IJeDeclareService> _mockJeDeclareService;
        private readonly Mock<IAsposeHelper> _mockAsposeHelper;
        private readonly IOptions<MandateEmailOptions> _emailOptions;
        private readonly Mock<ILogger<MandateManager>> _mockMandateLogger;

        public MandateControllerTest(SqlServerFixture sqlServerFixture)
            : base(sqlServerFixture)
        {
            _mockJeDeclareService = new Mock<IJeDeclareService>(MockBehavior.Strict);
            _mockAsposeHelper = new Mock<IAsposeHelper>(MockBehavior.Strict);
            _emailOptions = Options.Create(new MandateEmailOptions
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
            _mockMandateLogger = new Mock<ILogger<MandateManager>>(MockBehavior.Loose);
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
                    new Status(CollectionStatus.InProgress, "En cours", JdcCollectionStatus.Creation_InProgress));

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

            var guidGenerator = new Mock<IGuidGenerator>();

            var controller = new MandateController(logger.Object, manager.Object, guidGenerator.Object);

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
                "Name",
                "collab@email.com");

            var expectedCounters = new Client.Counters(1, 1, 0, 0, 0, 0);

            var collectionBankInfo = new Client.CollectionBankInfo(
                bankName: "bn",
                accountNumber: "12345678901",
                jdcPartnership: 2);

            var statusSummary = new Client.StatusInfo(
                statusCode: (int)CollectionStatus.InProgress,
                jdcStatusDescription: "En cours",
                (int)JdcCollectionStatus.Creation_InProgress);

            var expectedCollection = new Client.CollectionSummary(
                id: new Guid("00000002-0000-0000-0000-000000000000"),
                erpId: "123456789",
                companyName: "cn",
                collectionBankInfo: collectionBankInfo,
                creationDate: new DateTime(2022, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                modificationDate: new DateTime(2022, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                statusSummary,
                ["CAN_DOWNLOAD_PREFILLED_MANDATE", "CAN_UPLOAD_SIGNED_MANDATE", "CAN_DOWNLOAD_SIGNED_MANDATE"]);

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

            var guidGenerator = new Mock<IGuidGenerator>();

            var controller = new MandateController(logger.Object, manager.Object, guidGenerator.Object);

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
                "Name",
                "collab@email.com") as ObjectResult;

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
                    new Status(CollectionStatus.InProgress, "En cours", JdcCollectionStatus.Creation_InProgress));

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

            var controller = new MandateController(logger.Object, manager.Object, guidGenerator.Object);

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
        public async Task GetCollectionsAsync_ReturnsForbidden_WhenContactEmailIsMissing()
        {
            // Arrange
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
                null!);

            var logger = new Mock<ILogger<MandateController>>(MockBehavior.Loose);
            var manager = new Mock<IMandateManager>(MockBehavior.Strict);
            var guidGenerator = new Mock<IGuidGenerator>();

            var controller = new MandateController(logger.Object, manager.Object, guidGenerator.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext(),
            };

            // Act
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
                "Name",
                null) as ObjectResult;

            // Assert
            result.Should().NotBeNull();
            result!.StatusCode.Should().Be(StatusCodes.Status403Forbidden);

            var error = result.Value as Client.Error;
            error.Should().NotBeNull();
            error!.Message.Should().Be("Forbidden access due to missing contactEmail.");

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

            var controller = new MandateController(logger.Object, manager.Object, guidGenerator.Object);

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

            var controller = new MandateController(logger.Object, mandateManager.Object, guidGenerator.Object);

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

            var controller = new MandateController(logger.Object, mandateManager.Object, null!);

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

            var controller = new MandateController(logger.Object, mandateManager.Object, null!);

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

            var controller = new MandateController(logger.Object, mandateManager.Object, null!);

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

            var controller = new MandateController(logger.Object, mandateManagerMock.Object, null!);

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
            var result = await controller.UploadSignedMandateAsync(validMandateId, fileMock.Object, "collab@email.com");

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

            var controller = new MandateController(logger.Object, mandateManagerMock.Object, guidGenerator.Object);

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
            var result = await controller.UploadSignedMandateAsync(validMandateId, fileMock.Object, "collab@email.com");

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

            var controller = new MandateController(logger.Object, mandateManagerMock.Object, guidGenerator.Object);

            // Act
            var result = await controller.UploadSignedMandateAsync(validMandateId, null!, "collab@email.com");

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

            var controller = new MandateController(logger.Object, mandateManagerMock.Object, guidGenerator.Object);

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
            var result = await controller.UploadSignedMandateAsync(inValidMandateId, fileMock.Object, "collab@email.com");

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

            var controller = new MandateController(logger.Object, mandateManagerMock.Object, guidGenerator.Object);

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
            var result = await controller.UploadSignedMandateAsync(validMandateId, fileMock.Object, "collab@email.com");

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

            var controller = new MandateController(logger.Object, mandateManagerMock.Object, guidGenerator.Object);

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
            var result = await controller.UploadSignedMandateAsync(validMandateId, fileMock.Object, "collab@email.com");

            // Assert
            var internalServerErrorResult = result as ObjectResult;
            internalServerErrorResult?.StatusCode.Should().Be(500);
            internalServerErrorResult.Should().NotBeNull();
            internalServerErrorResult.Should().BeOfType<ObjectResult>();
            mandateManagerMock.Verify(m => m.UploadSignedMandateAsync(It.IsAny<Guid>(), It.IsAny<Stream>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task UploadSignedMandateAsync_ReturnsForbidden_WhenContactEmailIsMissing()
        {
            // Arrange
            var mandateId = Guid.NewGuid().ToString();
            var mockMandateManager = new Mock<IMandateManager>();
            var controller = new MandateController((new NullLoggerFactory() as ILoggerFactory).CreateLogger<MandateController>(), mockMandateManager.Object, null!);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            // Act
            var result = await controller.UploadSignedMandateAsync(mandateId, null, null);

            // Assert
            var forbiddenResult = result.Should().BeOfType<ObjectResult>().Subject;
            forbiddenResult.StatusCode.Should().Be(403);
            
            var error = forbiddenResult.Value.Should().BeOfType<KPMG.Pulse.Back.Accounting.Mandate.Client.Error>().Subject;

            error.Message.Should().Be("Forbidden access due to missing contactEmail.");
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

            var controller = new MandateController(logger.Object, mandateManager.Object, guidGenerator.Object);

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

            var controller = new MandateController(logger.Object, mandateManager.Object, null!);

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

            var controller = new MandateController(logger.Object, mandateManager.Object, null!);

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

            var controller = new MandateController(logger.Object, mandateManager.Object, null!);

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

            var controller = new MandateController(logger.Object, mandateManager.Object, null!);

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

            var controller = new MandateController(logger.Object, mandateManager.Object, null!);

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

            var mandateManager = new Mock<IMandateManager>();
            mandateManager.Setup(m => m.DeactivateCollectionAsync(mandateId, It.IsAny<string>()))
                .ReturnsAsync(true);

            var controller = new MandateController((new NullLoggerFactory() as ILoggerFactory).CreateLogger<MandateController>(), mandateManager.Object, null!);

            var result = await controller.DeactivateAsync(mandateId.ToString(), "collab@email.com");

            result.Should().NotBeNull();
            result.Should().BeOfType<NoContentResult>();

            mandateManager.VerifyAll();
        }

        [Fact]
        public async Task DeactivateAsync_Ko()
        {
            var mandateId = new PredictableGuid().NewGuid();

            var mandateManager = new Mock<IMandateManager>();
            mandateManager.Setup(m => m.DeactivateCollectionAsync(mandateId, It.IsAny<string>()))
                .ReturnsAsync(false);

            var controller = new MandateController((new NullLoggerFactory() as ILoggerFactory).CreateLogger<MandateController>(), mandateManager.Object, null!);

            var result = await controller.DeactivateAsync(mandateId.ToString(), "collab@email.com");

            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundResult>();

            mandateManager.VerifyAll();
        }

        [Fact]
        public async Task DeactivateAsync_Throws()
        {
            var controller = new MandateController((new NullLoggerFactory() as ILoggerFactory).CreateLogger<MandateController>(), null!, null!);

            var result = await controller.DeactivateAsync("x", "collab@email.com");

            result.Should().NotBeNull();
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task DeactivateAsync_ReturnsForbidden_WhenContactEmailIsMissing()
        {
            // Arrange
            var mandateId = Guid.NewGuid().ToString();
            var mockMandateManager = new Mock<IMandateManager>();
            var controller = new MandateController((new NullLoggerFactory() as ILoggerFactory).CreateLogger<MandateController>(), mockMandateManager.Object, null!);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            // Act
            var result = await controller.DeactivateAsync(mandateId, null);

            // Assert
            var forbiddenResult = result.Should().BeOfType<ObjectResult>().Subject;
            forbiddenResult.StatusCode.Should().Be(403);

            var error = forbiddenResult.Value.Should().BeOfType<KPMG.Pulse.Back.Accounting.Mandate.Client.Error>().Subject;

            error.Message.Should().Be("Forbidden access due to missing contactEmail.");
        }

        [InlineData(CollectionStatus.Incident, false)]
        [InlineData(CollectionStatus.Inactive, true)]
        [InlineData(CollectionStatus.Active, true)]
        [InlineData(CollectionStatus.ToDo, true)]
        [InlineData(CollectionStatus.InProgress, true)]
        [Theory]

        public async Task VerifyMandateCreation_Given_MandateWithOtherStatusThanCreationInprogress_ShouldReturn200(CollectionStatus status, bool created)
        {
            // Arrange
            var logger = new Mock<ILogger<MandateController>>();
            var mandateId = Guid.NewGuid();
            var mandateManager = new Mock<IMandateManager>();
            mandateManager.Setup(m => m.GetMandateStatusAsync(mandateId))
                .ReturnsAsync(status);
            var controller = new MandateController(logger.Object, mandateManager.Object, null!);

            // Act
            var result = await controller.CheckMandateCreationStatus(mandateId.ToString());

            // Assert
            var expected = new OkObjectResult(new { created = created });
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]

        public async Task VerifyMandateCreation_Given_MandateWithStatusCreationInProgress_ShouldReturn204()
        {
            // Arrange
            var mandateId = Guid.NewGuid();
            var logger = new Mock<ILogger<MandateController>>();
            var mandateManager = new Mock<IMandateManager>();
            mandateManager.Setup(m => m.GetMandateStatusAsync(mandateId))
                .ReturnsAsync(CollectionStatus.Creation_Inprogress);
            var controller = new MandateController(logger.Object, mandateManager.Object, null!);

            // Act
            var expected = new NoContentResult();
            var result = await controller.CheckMandateCreationStatus(mandateId.ToString());

            // Assert
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]

        public async Task VerifyMandateCreation_Given_NotFoundMandateId_ShouldReturnNotFound()
        {
            // Arrange
            var mandateId = Guid.NewGuid();
            var expectedError = new Client.Error("CollectionNotFound", mandateId.ToString(), "error");
            var logger = new Mock<ILogger<MandateController>>();

            var mandateManager = new Mock<IMandateManager>();
            mandateManager.Setup(m => m.GetMandateStatusAsync(mandateId))
                .ThrowsAsync(new Sql.CollectionNotFoundException("error"));
            var controller = new MandateController(logger.Object, mandateManager.Object, null!);

            // Act
            var result = (NotFoundObjectResult)await controller.CheckMandateCreationStatus(mandateId.ToString());

            // Assert
            result.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
            result.Value.Should().BeEquivalentTo(expectedError);
        }

        [Fact]
        public async Task CreateMandateAsync_ReturnOK()
        {
            // Arrange
            var rib = new Client.Bban("CodeB", "54321", "12345678901", "01");
            var signature = new Client.Signatory("M", "marwen", "elleuch", "maroo@email.com");
            var adresse = new Client.Address("LE ROUSSEL", "complements", "63520", "DOMAIZE", "France");
            var mandateCreation = new Client.CollectionCreationCommand(
                "1234567890",
                signature,
                adresse,
                rib);

            var logger = new Mock<ILogger<MandateController>>(MockBehavior.Strict);
            var collectionId = Guid.NewGuid();
            var mandateManager = new Mock<IMandateManager>();
            mandateManager.Setup(_ => _.CreateMandateAsync(It.IsAny<CollectionCreationCommand>(), It.IsAny<int>()))
                .Callback<CollectionCreationCommand, int>((cmd, id) =>
                {
                    cmd.Should().BeEquivalentTo(mandateCreation.ToModel());
                    id.Should().Be(2);
                })
                .ReturnsAsync(collectionId)
                .Verifiable();

            var controller = new MandateController(logger.Object, mandateManager.Object, null!);

            // Act
            var result = await controller.CreateMandateAsync(mandateCreation, 2);

            // Assert
            result.Should().NotBeNull();
            Assert.IsType<OkObjectResult>(result);
            mandateManager.Verify(x => x.CreateMandateAsync(It.IsAny<CollectionCreationCommand>(), It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task CreateMandateAsync_CompanyNotFoundException()
        {
            // Arrange
            var rib = new Client.Bban("CodeB", "54321", "12345678901", "01");
            var signature = new Client.Signatory("M", "marwen", "elleuch", "maroo@email.com");
            var adresse = new Client.Address("LE ROUSSEL", "complements", "63520", "DOMAIZE", "France");
            var mandateCreation = new Client.CollectionCreationCommand(
                "1234567890",
                signature,
                adresse,
                rib);

            var logger = new Mock<ILogger<MandateController>>(MockBehavior.Loose);
            var mandateManager = new Mock<IMandateManager>();
            mandateManager.Setup(_ => _.CreateMandateAsync(It.IsAny<CollectionCreationCommand>(), It.IsAny<int>()))
                .Callback<CollectionCreationCommand, int>((cmd, id) =>
                {
                    cmd.Should().BeEquivalentTo(mandateCreation.ToModel());
                    id.Should().Be(2);
                })
                .ThrowsAsync(new Sql.CompanyNotFoundException())
                .Verifiable();

            var controller = new MandateController(logger.Object, mandateManager.Object, null!);

            // Act
            var result = await controller.CreateMandateAsync(mandateCreation, 2) as ObjectResult;

            // Assert
            result?.StatusCode.Should().Be(400);
            mandateManager.Verify(x => x.CreateMandateAsync(It.IsAny<CollectionCreationCommand>(), It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task CreateMandateAsync_InactiveCompanyException()
        {
            // Arrange
            var rib = new Client.Bban("CodeB", "54321", "12345678901", "01");
            var signature = new Client.Signatory("M", "marwen", "elleuch", "maroo@email.com");
            var adresse = new Client.Address("LE ROUSSEL", "complements", "63520", "DOMAIZE", "France");
            var mandateCreation = new Client.CollectionCreationCommand(
                "1234567890",
                signature,
                adresse,
                rib);

            var logger = new Mock<ILogger<MandateController>>(MockBehavior.Loose);

            var mandateManager = new Mock<IMandateManager>();
            mandateManager.Setup(_ => _.CreateMandateAsync(It.IsAny<CollectionCreationCommand>(), It.IsAny<int>()))
                .Callback<CollectionCreationCommand, int>((cmd, id) =>
                {
                    cmd.Should().BeEquivalentTo(mandateCreation.ToModel());
                    id.Should().Be(2);
                })
                .ThrowsAsync(new Sql.InactiveCompanyException())
                .Verifiable();

            var controller = new MandateController(logger.Object, mandateManager.Object, null!);

            // Act
            var result = await controller.CreateMandateAsync(mandateCreation, 2) as ObjectResult;

            // Assert
            result?.StatusCode.Should().Be(400);
            mandateManager.Verify(x => x.CreateMandateAsync(It.IsAny<CollectionCreationCommand>(), It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task CreateMandateAsync_InaccessibleCompanyException()
        {
            // Arrange
            var rib = new Client.Bban("CodeB", "54321", "12345678901", "01");
            var signature = new Client.Signatory("M", "marwen", "elleuch", "maroo@email.com");
            var adresse = new Client.Address("LE ROUSSEL", "complements", "63520", "DOMAIZE", "France");
            var mandateCreation = new Client.CollectionCreationCommand(
                "1234567890",
                signature,
                adresse,
                rib);

            var logger = new Mock<ILogger<MandateController>>(MockBehavior.Loose);
            var mandateManager = new Mock<IMandateManager>();
            mandateManager.Setup(_ => _.CreateMandateAsync(It.IsAny<CollectionCreationCommand>(), It.IsAny<int>()))
                .Callback<CollectionCreationCommand, int>((cmd, id) =>
                {
                    cmd.Should().BeEquivalentTo(mandateCreation.ToModel());
                    id.Should().Be(2);
                })
                .ThrowsAsync(new Sql.InaccessibleCompanyException())
                .Verifiable();

            var controller = new MandateController(logger.Object, mandateManager.Object, null!);

            // Act
            var result = await controller.CreateMandateAsync(mandateCreation, 2) as ObjectResult;

            // Assert
            result?.StatusCode.Should().Be(400);
            mandateManager.Verify(x => x.CreateMandateAsync(It.IsAny<CollectionCreationCommand>(), It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task CreateMandateAsync_CustomBankCodeNotFoundException()
        {
            // Arrange
            var rib = new Client.Bban("CodeB", "54321", "12345678901", "01");
            var signature = new Client.Signatory("M", "marwen", "elleuch", "maroo@email.com");
            var adresse = new Client.Address("LE ROUSSEL", "complements", "63520", "DOMAIZE", "France");
            var mandateCreation = new Client.CollectionCreationCommand(
                "1234567890",
                signature,
                adresse,
                rib);

            var logger = new Mock<ILogger<MandateController>>(MockBehavior.Loose);
            var mandateManager = new Mock<IMandateManager>();
            mandateManager.Setup(_ => _.CreateMandateAsync(It.IsAny<CollectionCreationCommand>(), It.IsAny<int>()))
                .Callback<CollectionCreationCommand, int>((cmd, id) =>
                {
                    cmd.Should().BeEquivalentTo(mandateCreation.ToModel());
                    id.Should().Be(2);
                })
                .ThrowsAsync(new CustomBankCodeNotFoundException())
                .Verifiable();

            var controller = new MandateController(logger.Object, mandateManager.Object, null!);

            // Act
            var result = await controller.CreateMandateAsync(mandateCreation, 2) as ObjectResult;

            // Assert
            result?.StatusCode.Should().Be(400);
            mandateManager.Verify(x => x.CreateMandateAsync(It.IsAny<CollectionCreationCommand>(), It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task CreateMandateAsync_CompanyHasNoSiretException_ReturnsBadRequest()
        {
            // Arrange
            var rib = new Client.Bban("CodeB", "54321", "12345678901", "01");
            var signature = new Client.Signatory("M", "marwen", "elleuch", "maroo@email.com");
            var adresse = new Client.Address("LE ROUSSEL", "complements", "63520", "DOMAIZE", "France");
            var mandateCreation = new Client.CollectionCreationCommand(
                "1234567890",
                signature,
                adresse,
                rib);

            var logger = new Mock<ILogger<MandateController>>(MockBehavior.Loose);
            var mandateManager = new Mock<IMandateManager>();
            mandateManager.Setup(_ => _.CreateMandateAsync(It.IsAny<CollectionCreationCommand>(), It.IsAny<int>()))
                .ThrowsAsync(new CompanyHasNoSiretException("La Compagnie n'a pas de SIRET"))
                .Verifiable();

            var controller = new MandateController(logger.Object, mandateManager.Object, null!);

            // Act
            var result = await controller.CreateMandateAsync(mandateCreation, 2) as ObjectResult;

            // Assert
            result?.StatusCode.Should().Be(400);
            mandateManager.Verify(x => x.CreateMandateAsync(It.IsAny<CollectionCreationCommand>(), It.IsAny<int>()), Times.Once);
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
                    new Status(CollectionStatus.InProgress, "En cours", JdcCollectionStatus.Creation_InProgress)));
            }

            return collections;
        }
    }
}