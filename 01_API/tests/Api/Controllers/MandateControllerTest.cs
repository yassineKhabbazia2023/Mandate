// <copyright file="MandateControllerTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.AspNetCore.Tests
{
    using System.Net;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;

    public class MandateControllerTest
    {
        [Fact]
        public async void GetCollectionsAsync_When_GetCollectionsAsync_OK()
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
                new Guid("00000001-0000-0000-0000-000000000000"),
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
                    new DateTime(2022, 1, 1),
                    new DateTime(2022, 1, 1),
                    new Status(CollectionStatus.InProgress, "En cours"));

            List<Collection> collections = new List<Collection>()
            {
                collection,
            };

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

            var expectedCollection = new Client.CollectionSummary(
                id: new Guid("00000002-0000-0000-0000-000000000000"),
                erpId: "123456789",
                companyName: "cn",
                bankName: "bn",
                accountNumber: "12345678901",
                creationDate: new DateTime(2022, 1, 1),
                modificationDate: new DateTime(2022, 1, 1),
                statusCode: 30);

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
        public async void GetCollectionsAsync_When_GetCollectionsAsync_Throw_Exception()
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

            result!.StatusCode.Should().Be((int)StatusCodes.Status500InternalServerError);

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

            var expectedResult = new FileContentResult(expectedFileData, expectedContentType)
            {
                FileDownloadName = expectedFileName,
            };

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

            var mandateManagerMock = new Mock<IMandateManager>();
            mandateManagerMock.Setup(m => m.UploadSignedMandateAsync(It.IsAny<Guid>(), It.IsAny<Stream>()))
                .ReturnsAsync("signedMandateId")
                .Verifiable();

            var logger = new Mock<ILogger<MandateController>>(MockBehavior.Strict);
            logger.Setup(x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsValueType>(),
                It.IsAny<Exception?>(),
                (Func<It.IsValueType, Exception?, string>)It.IsAny<object>())); // Ignore all logs

            var controller = new MandateController(logger.Object, mandateManagerMock.Object, null!, null!, null!);

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
            mandateManagerMock.Verify(m => m.UploadSignedMandateAsync(It.IsAny<Guid>(), It.IsAny<Stream>()), Times.Once);
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
            mandateManagerMock.Verify(m => m.UploadSignedMandateAsync(It.IsAny<Guid>(), It.IsAny<Stream>()), Times.Never);
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
            mandateManagerMock.Verify(m => m.UploadSignedMandateAsync(It.IsAny<Guid>(), It.IsAny<Stream>()), Times.Never);
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
            mandateManagerMock.Verify(m => m.UploadSignedMandateAsync(It.IsAny<Guid>(), It.IsAny<Stream>()), Times.Never);
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
            mandateManagerMock.Setup(m => m.UploadSignedMandateAsync(It.IsAny<Guid>(), It.IsAny<Stream>()))
                .ThrowsAsync(new ServicesProviderException("Test exception"))
                .Verifiable();

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
            var result = await controller.UploadSignedMandateAsync(validMandateId, fileMock.Object);

            // Assert
            var internalServerErrorResult = result as ObjectResult;
            internalServerErrorResult?.StatusCode.Should().Be(500);
            internalServerErrorResult.Should().NotBeNull();
            internalServerErrorResult.Should().BeOfType<ObjectResult>();
            mandateManagerMock.Verify(m => m.UploadSignedMandateAsync(It.IsAny<Guid>(), It.IsAny<Stream>()), Times.Once);
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
            mandateManagerMock.Setup(m => m.UploadSignedMandateAsync(It.IsAny<Guid>(), It.IsAny<Stream>()))
                .ThrowsAsync(new Exception("Test exception"))
                .Verifiable();

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
            var result = await controller.UploadSignedMandateAsync(validMandateId, fileMock.Object);

            // Assert
            var internalServerErrorResult = result as ObjectResult;
            internalServerErrorResult?.StatusCode.Should().Be(500);
            internalServerErrorResult.Should().NotBeNull();
            internalServerErrorResult.Should().BeOfType<ObjectResult>();
            mandateManagerMock.Verify(m => m.UploadSignedMandateAsync(It.IsAny<Guid>(), It.IsAny<Stream>()), Times.Once);
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

            var expectedResult = new FileContentResult(expectedFileData, expectedContentType)
            {
                FileDownloadName = expectedFileName,
            };

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

            var mandateManager = new Mock<IMandateManager>();
            mandateManager.Setup(m => m.DeactivateCollectionAsync(mandateId))
                .ReturnsAsync(true);

            var controller = new MandateController((new NullLoggerFactory() as ILoggerFactory).CreateLogger<MandateController>(), mandateManager.Object, null!, null!, null!);

            var result = await controller.DeactivateAsync(mandateId.ToString());

            result.Should().NotBeNull();
            result.Should().BeOfType<NoContentResult>();

            mandateManager.VerifyAll();
        }

        [Fact]
        public async Task DeactivateAsync_Ko()
        {
            var mandateId = new PredictableGuid().NewGuid();

            var mandateManager = new Mock<IMandateManager>();
            mandateManager.Setup(m => m.DeactivateCollectionAsync(mandateId))
                .ReturnsAsync(false);

            var controller = new MandateController((new NullLoggerFactory() as ILoggerFactory).CreateLogger<MandateController>(), mandateManager.Object, null!, null!, null!);

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
                new Guid("00000001-0000-0000-0000-000000000000"),
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
                    new DateTime(2022, 1, 1),
                    new DateTime(2022, 1, 1),
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

            var company = new Company(
                new Guid("00000001-0000-0000-0000-000000000000"),
                "cn",
                "12345678910",
                "123456789",
                string.Empty,
                null,
                null);

            Bank bank = new Bank("12345", "bn", "bg", string.Empty, new BankAgreement(JdcPartnership.NonPartner));

            Bban bban = new Bban("12345", "54321", "12345678901", "55", string.Empty, bank);

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
                new Guid("00000001-0000-0000-0000-000000000000"),
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
                    new DateTime(2022, 1, 1),
                    new DateTime(2022, 1, 1),
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
    }
}
