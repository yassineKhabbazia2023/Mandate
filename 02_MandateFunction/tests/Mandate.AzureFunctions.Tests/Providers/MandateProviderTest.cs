// <copyright file="MandateProviderTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.AzureFunctions.Tests
{
    using Kpmg.Constellation.IdentityService.Client;
    using KPMG.Pulse.Back.Accounting.Mandate.Client;

    public class MandateProviderTest
    {
        [Fact]
        public void Constructor()
        {
            var provider = new MandateProvider(Mock.Of<IMandateClientFactory>(), Mock.Of<ISystemAccountAuthenticationProvider>());
            provider.Should().NotBeNull();
        }

        [Fact]
        public async Task GetRecoveryAsync()
        {
            var collectionSummary = new CollectionSummary(
                Guid.Empty,
                "1234567890",
                "Weyland Corporation",
                "Crédit Agricole",
                "98765432101",
                new DateTime(2023, 10, 1, 0, 0, 0, DateTimeKind.Utc),
                new DateTime(2023, 10, 2, 0, 0, 0, DateTimeKind.Utc),
                10);

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
            factory.Setup(f => f.Create("token3"))
                .Returns(mandateClient.Object)
                .Verifiable();

            var authenticationContext = new Mock<ISystemAccountAuthenticationProvider>(MockBehavior.Strict);
            authenticationContext.Setup(a => a.GetTokenAsync())
                    .Returns(Task.FromResult("token3"))
                    .Verifiable();

            var provider = new MandateProvider(factory.Object, authenticationContext.Object);
            var res = await provider.GetRecoveryAsync(rib);

            res.Should().BeEquivalentTo(collectionSummary);

            authenticationContext.Verify();
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
            factory.Setup(f => f.Create("token3"))
                .Returns(mandateClient.Object)
                .Verifiable();

            var authenticationContext = new Mock<ISystemAccountAuthenticationProvider>(MockBehavior.Strict);
            authenticationContext.Setup(a => a.GetTokenAsync())
                    .Returns(Task.FromResult("token3"))
                    .Verifiable();

            var provider = new MandateProvider(factory.Object, authenticationContext.Object);
            var res = await provider.GetCollectionsAsync(0, 100, new List<int> { 10 });

            res.Should().BeEquivalentTo(page);

            authenticationContext.VerifyAll();
            factory.VerifyAll();
            mandateClient.VerifyAll();
        }

        [Fact]
        public async Task RecoveryAsync()
        {
            var collectionSummary = new CollectionSummary(
                Guid.Empty,
                "1234567890",
                "Weyland Corporation",
                "Crédit Agricole",
                "98765432101",
                new DateTime(2023, 10, 1, 0, 0, 0, DateTimeKind.Utc),
                new DateTime(2023, 10, 2, 0, 0, 0, DateTimeKind.Utc),
                10);

            IReadOnlyList<CollectionSummary> collectionSummaries = new List<CollectionSummary>() { collectionSummary };
            PagedRecoveryMandate page = new PagedRecoveryMandate(1, collectionSummaries);

            var mandateClient = new Mock<IMandateClient>(MockBehavior.Strict);
            mandateClient.Setup(c => c.RecoveryFormIoAsync(0, 10))
                .ReturnsAsync(page)
                .Verifiable();

            var factory = new Mock<IMandateClientFactory>(MockBehavior.Strict);
            factory.Setup(f => f.Create("token3"))
                .Returns(mandateClient.Object)
                .Verifiable();

            var authenticationContext = new Mock<ISystemAccountAuthenticationProvider>(MockBehavior.Strict);
            authenticationContext.Setup(a => a.GetTokenAsync())
                    .Returns(Task.FromResult("token3"))
                    .Verifiable();

            var provider = new MandateProvider(factory.Object, authenticationContext.Object);
            var res = await provider.RecoveryAsync(0, 10);

            res.Should().BeEquivalentTo(page);

            authenticationContext.Verify();
            factory.Verify();
            mandateClient.Verify();
        }

        [Fact]
        public async Task RecoveryAsync_When_RecoveryAsync_throwException()
        {
            var mandateClient = new Mock<IMandateClient>(MockBehavior.Strict);
            mandateClient.Setup(c => c.RecoveryFormIoAsync(0, 10))
                .ThrowsAsync(new Exception("message"))
                .Verifiable();

            var factory = new Mock<IMandateClientFactory>(MockBehavior.Strict);
            factory.Setup(f => f.Create("token3"))
                .Returns(mandateClient.Object)
                .Verifiable();

            var authenticationContext = new Mock<ISystemAccountAuthenticationProvider>(MockBehavior.Strict);
            authenticationContext.Setup(a => a.GetTokenAsync())
                    .Returns(Task.FromResult("token3"))
                    .Verifiable();

            var provider = new MandateProvider(factory.Object, authenticationContext.Object);
            Func<Task> act = async () => await provider.RecoveryAsync(0, 10);
            await act.Should().ThrowExactlyAsync<Exception>().WithMessage("message");

            authenticationContext.Verify();
            factory.Verify();
            mandateClient.Verify();
        }
    }
}
