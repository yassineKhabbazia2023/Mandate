// <copyright file="MandateProviderTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.AzureFunctions.Tests
{
    using KPMG.Pulse.Back.Accounting.Mandate.Client;
    using Moq;

    public class MandateProviderTest
    {
        [Fact]
        public void Constructor()
        {
            var provider = new MandateProvider(Mock.Of<IMandateClientFactory>());
            provider.Should().NotBeNull();
        }

        [Fact]
        public async Task GetRecoveryAsync()
        {
            var statusSummary = new Client.StatusInfo(
                statusCode: 10,
                jdcStatusDescription: string.Empty,
                null);

            var collectionSummary = new CollectionSummary(
                Guid.Empty,
                "1234567890",
                "Weyland Corporation",
                new CollectionBankInfo("bankName", "accountNumber", 1),
                new DateTime(2023, 10, 1, 0, 0, 0, DateTimeKind.Utc),
                new DateTime(2023, 10, 2, 0, 0, 0, DateTimeKind.Utc),
                statusSummary,
                new List<string>(), destinationTool: "testdestination");

            var rib = new Bban(
                bankCode: "bankCodeM",
                branchCode: "branchCodeM",
                accountNumber: "accountNumberM",
                checkDigits: "checkDigitsM");

            var mandateClient = new Mock<IMandateClient>(MockBehavior.Strict);
            mandateClient.Setup(c => c.GetRecoveryAsync(It.IsAny<Bban>()))
                .Callback<Bban>(b =>
                {
                    b.BankCode.Should().Be("bankCodeM");
                    b.BranchCode.Should().Be("branchCodeM");
                    b.AccountNumber.Should().Be("accountNumberM");
                    b.CheckDigits.Should().Be("checkDigitsM");
                })
                .ReturnsAsync(collectionSummary)
                .Verifiable();

            var factory = new Mock<IMandateClientFactory>(MockBehavior.Strict);
            factory.Setup(f => f.Create())
                .Returns(mandateClient.Object)
                .Verifiable();


            var provider = new MandateProvider(factory.Object);
            var res = await provider.GetRecoveryAsync(rib);

            res.Should().BeEquivalentTo(collectionSummary);

            factory.Verify();
            mandateClient.Verify();
        }

        [Fact]
        public async Task GetCollectionsAsync()
        {
            BankDetails bank = new BankDetails("bcode", "bname", "accountNumber", "cle");
            List<TechnicalCollectionSummary> technicalCollections = new List<TechnicalCollectionSummary>()
            {
                new TechnicalCollectionSummary(
                    new Guid("00000001-0000-0000-0000-000000000000"),
                    "folderId1",
                    "ribId1",
                    bank,
                    10),
            };

            PagedTechnicalMandate page = new PagedTechnicalMandate(technicalCollections);
            var mandateClient = new Mock<IMandateClient>(MockBehavior.Strict);
            mandateClient.Setup(item => item.GetTechnicalCollectionSummaryAsync(0, 100, new List<int> { 10 }))
                .ReturnsAsync(page)
                .Verifiable();

            var factory = new Mock<IMandateClientFactory>(MockBehavior.Strict);
            factory.Setup(f => f.Create())
                .Returns(mandateClient.Object)
                .Verifiable();

         

            var provider = new MandateProvider(factory.Object);
            var res = await provider.GetCollectionsAsync(0, 100, new List<int> { 10 });

            res.Should().BeEquivalentTo(page);

            factory.VerifyAll();
            mandateClient.VerifyAll();
        }
        
        [Fact]
        public async Task RefreshCollectionsStatuses()
        {
            var collectionSummary = new TechnicalCollectionSummary(
                Guid.Empty,
                "fId",
                "ribId",
                default!,
                10);

            var list = new List<TechnicalCollectionSummary>() { collectionSummary };

            var mandateClient = new Mock<IMandateClient>(MockBehavior.Strict);
            mandateClient.Setup(c => c.RefreshMandatsStatusesAsync(list))
                .ReturnsAsync(It.IsAny<bool>())
                .Verifiable();

            var factory = new Mock<IMandateClientFactory>(MockBehavior.Strict);
            factory.Setup(f => f.Create())
                .Returns(mandateClient.Object)
                .Verifiable();

            var provider = new MandateProvider(factory.Object);
            await provider.RefreshCollectionsStatuses(list);

            factory.Verify();
            mandateClient.Verify();
        }
    }
}
