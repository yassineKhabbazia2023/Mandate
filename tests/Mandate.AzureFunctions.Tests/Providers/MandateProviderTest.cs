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
    }
}
