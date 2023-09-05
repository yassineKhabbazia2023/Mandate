// <copyright file="BankManagerTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Application.Tests
{
    public class BankManagerTest
    {
        [Fact]
        public async Task GetByCodeAsync()
        {
            var bank = TestHelper.GetBank();
            var databaseService = new Mock<IDatabaseService>(MockBehavior.Strict);
            databaseService.Setup(r => r.GetBankByCodeAsync("a"))
                .ReturnsAsync(bank)
                .Verifiable();
            var bankManager = new BankManager(databaseService.Object);

            var result = await bankManager.GetByCodeAsync("a").ConfigureAwait(false);

            result.Should().Be(bank);
            databaseService.VerifyAll();
        }
    }
}
