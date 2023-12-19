// <copyright file="JeDeclareAdapterTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Adapters.Tests
{
    using KPMG.Pulse.Back.Accounting.Mandate.JeDeclare.Client;

    public class JeDeclareAdapterTest
    {
        [Fact]
        public async Task GetMandatPdfAsync()
        {
            byte[] data = { 0, 16, 104, 213 };

            var jedeclareClient = new Mock<IJeDeclareClient>(MockBehavior.Strict);
            jedeclareClient.Setup(c => c.GetMandatPdfAsync("jdcFolderIdT", "jdcRibIdT"))
                .ReturnsAsync(data)
                .Verifiable();

            var adapter = new JeDeclareAdapter(jedeclareClient.Object);
            var result = await adapter.GetMandatPdfAsync("jdcFolderIdT", "jdcRibIdT");

            result.Should().BeEquivalentTo(data);
            jedeclareClient.VerifyAll();
        }

        [Fact]
        public async Task GetMandatPdfAsync_when_GetMandatPdfAsync_Throw_JeDeclareApiException()
        {
            var jedeclareClient = new Mock<IJeDeclareClient>(MockBehavior.Strict);
            jedeclareClient.Setup(c => c.GetMandatPdfAsync("jdcFolderIdT", "jdcRibIdT"))
                .ThrowsAsync(new JeDeclareApiException("message"))
                .Verifiable();

            var adapter = new JeDeclareAdapter(jedeclareClient.Object);
            Func<Task> action = async () => await adapter.GetMandatPdfAsync("jdcFolderIdT", "jdcRibIdT");
            await action.Should().ThrowAsync<ServicesProviderException>().WithMessage("message");

            jedeclareClient.VerifyAll();
        }

        [Fact]
        public async Task GetMandatPdfAsync_when_GetMandatPdfAsync_Throw_Exception()
        {
            var jedeclareClient = new Mock<IJeDeclareClient>(MockBehavior.Strict);
            jedeclareClient.Setup(c => c.GetMandatPdfAsync("jdcFolderIdT", "jdcRibIdT"))
                .ThrowsAsync(new Exception("message"))
                .Verifiable();

            var adapter = new JeDeclareAdapter(jedeclareClient.Object);
            Func<Task> action = async () => await adapter.GetMandatPdfAsync("jdcFolderIdT", "jdcRibIdT");
            await action.Should().NotThrowAsync<ServicesProviderException>();
            await action.Should().ThrowAsync<Exception>().WithMessage("message");

            jedeclareClient.VerifyAll();
        }

        [Fact]
        public async Task UploadSignedMandate_WithValidInputs_CallsJedeclareClient()
        {
            var collectionId = new PredictableGuid().NewGuid();
            var companyId = new PredictableGuid().NewGuid();

            var company = EntityFactory.Company;
            Bban bban = EntityFactory.Bban;
            Status status = EntityFactory.Status();

            var collection = new Collection(
                collectionId,
                "yourServiceProviderId",
                company,
                bban,
                DateTime.Now,
                DateTime.Now,
                status);
            var mandateFile = new byte[] { 1, 2, 3, 4, 5 };
            var jedeclareClient = new Mock<IJeDeclareClient>(MockBehavior.Strict);

            var bankServicesProviderId = collection.Company?.BankServicesProviderId;
            var bbanServicesProviderId = collection.Bban?.BbanServicesProviderId;

            jedeclareClient.Setup(c => c.UploadSignedMandat(bankServicesProviderId!, bbanServicesProviderId!, It.IsAny<byte[]>()))
                .ReturnsAsync("signedMandateId")
                .Verifiable();

            var adapter = new JeDeclareAdapter(jedeclareClient.Object);
            var result = await adapter.UploadSignedMandate(collection, mandateFile);

            result.Should().BeEquivalentTo("signedMandateId");
            jedeclareClient.Verify(client => client.UploadSignedMandat(bankServicesProviderId!, bbanServicesProviderId!, mandateFile), Times.Once);
        }

        [Fact]
        public async Task UploadSignedMandate_WithValidInputs_ThrowJeDeclareApiException()
        {
            var collectionId = new PredictableGuid().NewGuid();
            var companyId = new PredictableGuid().NewGuid();

            var company = EntityFactory.Company;
            Bban bban = EntityFactory.Bban;
            Status status = EntityFactory.Status();

            var collection = new Collection(
                collectionId,
                "yourServiceProviderId",
                company,
                bban,
                DateTime.Now,
                DateTime.Now,
                status);
            var mandateFile = new byte[] { 1, 2, 3, 4, 5 };
            var jedeclareClient = new Mock<IJeDeclareClient>(MockBehavior.Strict);

            var bankServicesProviderId = collection.Company?.BankServicesProviderId;
            var bbanServicesProviderId = collection.Bban?.BbanServicesProviderId;

            jedeclareClient.Setup(c => c.UploadSignedMandat(bankServicesProviderId!, bbanServicesProviderId!, It.IsAny<byte[]>()))
                  .ThrowsAsync(new JeDeclareApiException("message"))
                  .Verifiable();

            var adapter = new JeDeclareAdapter(jedeclareClient.Object);
            Func<Task> action = async () => await adapter.UploadSignedMandate(collection, mandateFile);
            await action.Should().ThrowAsync<ServicesProviderException>().WithMessage("message");

            jedeclareClient.Verify(client => client.UploadSignedMandat(bankServicesProviderId!, bbanServicesProviderId!, mandateFile), Times.Once);
        }

        [Fact]
        public async Task GetSignedMandatPdfAsync_ReturnsPdf_WhenSuccessful()
        {
            // Arrange
            var expectedPdf = new byte[] { 1, 2, 3, 4, 5 };

            var jedeclareClient = new Mock<IJeDeclareClient>(MockBehavior.Strict);
            jedeclareClient.Setup(client => client.GetSignedMandatPdfAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(expectedPdf);

            var adapter = new JeDeclareAdapter(jedeclareClient.Object);

            // Act
            var result = await adapter.GetSignedMandatPdfAsync("jdcFolderId", "jdcRibId");

            // Assert
            result.Should().BeEquivalentTo(expectedPdf);
        }

        [Fact]
        public async Task GetSignedMandatPdfAsync_ThrowsServicesProviderException_WhenJeDeclareApiExceptionThrown()
        {
            // Arrange
            var jedeclareClient = new Mock<IJeDeclareClient>(MockBehavior.Strict);
            jedeclareClient.Setup(client => client.GetSignedMandatPdfAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new JeDeclareApiException("Error message"));

            var adapter = new JeDeclareAdapter(jedeclareClient.Object);

            // Act & Assert
            await Assert.ThrowsAsync<ServicesProviderException>(() => adapter.GetSignedMandatPdfAsync("jdcFolderId", "jdcRibId"));
        }
    }
}
