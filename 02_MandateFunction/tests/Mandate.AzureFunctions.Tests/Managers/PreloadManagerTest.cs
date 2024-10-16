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
                statusInfo: statusSummary,
                new List<string>());

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

        [Fact]
        public async Task RecoveryAsync()
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
                new List<string>());

            IReadOnlyList<CollectionSummary> collectionSummaries = new List<CollectionSummary>() { collectionSummary };
            PagedRecoveryMandate page = new PagedRecoveryMandate(1, collectionSummaries);

            var provider = new Mock<IMandateProvider>(MockBehavior.Strict);
            provider.Setup(p => p.RecoveryAsync(0, 10))
                .ReturnsAsync(page)
                .Verifiable();

            var manager = new PreloadManager(provider.Object);
            var result = await manager.RecoveryAsync(0, 10);

            result.Should().BeEquivalentTo(page);
            provider.VerifyAll();
        }

        [Fact]
        public async Task RecoveryAsync_When_RecoveryAsync_throwException()
        {
            var provider = new Mock<IMandateProvider>(MockBehavior.Strict);
            provider.Setup(p => p.RecoveryAsync(0, 10))
                .ThrowsAsync(new Exception("message"))
                .Verifiable();

            var manager = new PreloadManager(provider.Object);
            Func<Task> act = async () => await manager.RecoveryAsync(0, 10);
            await act.Should().ThrowExactlyAsync<Exception>().WithMessage("message");

            provider.VerifyAll();
        }
    }
}
