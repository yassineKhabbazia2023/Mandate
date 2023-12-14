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
            var mandateFile = new byte[] { 1, 2, 3, 4, 5 };
            var jedeclareClient = new Mock<IJeDeclareClient>(MockBehavior.Strict);

            var bankServicesProviderId = collection.Company?.BankServicesProviderId;
            var ebicsCardId = collection.Bban?.Bank?.EbicsCardId;

            jedeclareClient.Setup(c => c.UploadSignedMandat(bankServicesProviderId!, ebicsCardId!, It.IsAny<byte[]>()))
                .ReturnsAsync("signedMandateId")
                .Verifiable();

            var adapter = new JeDeclareAdapter(jedeclareClient.Object);
            var result = await adapter.UploadSignedMandate(collection, mandateFile);

            result.Should().BeEquivalentTo("signedMandateId");
            jedeclareClient.Verify(client => client.UploadSignedMandat(bankServicesProviderId!, ebicsCardId!, mandateFile), Times.Once);
        }
    }
}
