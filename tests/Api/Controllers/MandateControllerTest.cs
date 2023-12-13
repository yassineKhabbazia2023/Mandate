// <copyright file="MandateControllerTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.AspNetCore.Tests
{
    using System.Net;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

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

            var controller = new MandateController(logger.Object, manager.Object, authenticationContext.Object, guidGenerator.Object);

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

            var controller = new MandateController(logger.Object, manager.Object, authenticationContext.Object, guidGenerator.Object);

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

            var controller = new MandateController(logger.Object, mandateManager.Object, null!, guidGenerator.Object);

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
            var newGuid = Guid.Parse("a0000000-0000-0000-0000-000000000000");
            var guidGenerator = new Mock<IGuidGenerator>();
            guidGenerator.Setup(g => g.NewGuid())
                .Returns(newGuid);

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

            var controller = new MandateController(logger.Object, mandateManager.Object, null!, guidGenerator.Object);

            var result = await controller.DownloadUnsignedAsync(mandateId) as ObjectResult;

            result.Should().NotBeNull();
            result!.StatusCode.Should().Be((int)HttpStatusCode.NotFound);

            var errorType = result!.Value as Client.Error;
            errorType.Should().NotBeNull();
            errorType!.ErrorType.Should()
                               .Be("CollectionNotFound");
            errorType!.LogReference.Should()
                                  .Be(newGuid.ToString());

            mandateManager.VerifyAll();
            logger.VerifyAll();
            guidGenerator.VerifyAll();
        }

        [Fact]
        public async Task DownloadUnsignedAsync_CaseFolderIdEmptyOrNullException()
        {
            var newGuid = Guid.Parse("a0000000-0000-0000-0000-000000000000");
            var guidGenerator = new Mock<IGuidGenerator>();
            guidGenerator.Setup(g => g.NewGuid())
                .Returns(newGuid);

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

            var controller = new MandateController(logger.Object, mandateManager.Object, null!, guidGenerator.Object);

            var result = await controller.DownloadUnsignedAsync(mandateId) as ObjectResult;

            result.Should().NotBeNull();
            result!.StatusCode.Should().Be((int)HttpStatusCode.NotFound);

            var errorType = result!.Value as Client.Error;
            errorType.Should().NotBeNull();
            errorType!.ErrorType.Should()
                               .Be("FolderIdEmptyOrNull");
            errorType!.LogReference.Should()
                                  .Be(newGuid.ToString());

            mandateManager.VerifyAll();
            logger.VerifyAll();
            guidGenerator.VerifyAll();
        }

        [Fact]
        public async Task DownloadUnsignedAsync_CaseRibIdEmptyOrNullException()
        {
            var newGuid = Guid.Parse("a0000000-0000-0000-0000-000000000000");
            var guidGenerator = new Mock<IGuidGenerator>();
            guidGenerator.Setup(g => g.NewGuid())
                .Returns(newGuid);

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

            var controller = new MandateController(logger.Object, mandateManager.Object, null!, guidGenerator.Object);

            var result = await controller.DownloadUnsignedAsync(mandateId) as ObjectResult;

            result.Should().NotBeNull();
            result!.StatusCode.Should().Be((int)HttpStatusCode.NotFound);

            var errorType = result!.Value as Client.Error;
            errorType.Should().NotBeNull();
            errorType!.ErrorType.Should()
                               .Be("RibIdEmptyOrNull");
            errorType!.LogReference.Should()
                                  .Be(newGuid.ToString());

            mandateManager.VerifyAll();
            logger.VerifyAll();
            guidGenerator.VerifyAll();
        }
    }
}
