// <copyright file="PreloadManagerTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.AzureFunctions.Tests
{
    using KPMG.Pulse.Back.Accounting.Mandate.Client;

    public class PreloadManagerTest
    {
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

            var provider = new Mock<IMandateProvider>(MockBehavior.Strict);
            provider.Setup(p => p.GetRecoveryAsync(It.IsAny<Bban>()))
                .Callback<Bban>(b =>
                {
                    b.BankCode.Should().Be("bankCodeT");
                    b.BranchCode.Should().Be("branchCodeT");
                    b.AccountNumber.Should().Be("accountNumberT");
                    b.CheckDigits.Should().Be("checkDigitsT");
                })
                .ReturnsAsync(collectionSummary)
                .Verifiable();

            var rib = new Bban(
                bankCode: "bankCodeT",
                branchCode: "branchCodeT",
                accountNumber: "accountNumberT",
                checkDigits: "checkDigitsT");

            var manager = new PreloadManager(provider.Object);
            await manager.GetRecoveryAsync(rib);

            provider.VerifyAll();
        }
    }
}
