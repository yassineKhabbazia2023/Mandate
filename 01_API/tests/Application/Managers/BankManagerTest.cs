// <copyright file="BankManagerTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

using Pulse.Back.Accounting.Mandate.Application.Exceptions;
using Pulse.ExceptionMiddleware.Exceptions;

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

            var result = await bankManager.GetByCodeAsync("a");

            result.Should().Be(bank);
            databaseService.VerifyAll();
        }

        [Fact]
        public async Task GetByCodeAsync_ShouldThrowBadRequestException_WhenCodeIs30003()
        {
            // Arrange
            var databaseService = new Mock<IDatabaseService>(MockBehavior.Strict);
            var bankManager = new BankManager(databaseService.Object);

            // Act
            Func<Task> act = async () => await bankManager.GetByCodeAsync("30003");

            // Assert
            await act.Should()
                .ThrowAsync<BadRequestException>()
                .WithMessage(string.Format(Errors.NotAuthorizedBankCodeMessage, "30003"));

            databaseService.VerifyNoOtherCalls();
        }
        [Fact]
        public async Task GetByCodeAsync_ShouldThrowBadRequestException_WhenCodeIs30004()
        {
            // Arrange
            var databaseService = new Mock<IDatabaseService>(MockBehavior.Strict);
            var bankManager = new BankManager(databaseService.Object);

            // Act
            Func<Task> act = async () => await bankManager.GetByCodeAsync("30004");

            // Assert
            await act.Should()
                .ThrowAsync<BadRequestException>()
                .WithMessage(string.Format(Errors.NotAuthorizedBankCodeMessage, "30004"));

            databaseService.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task GetByCodeAsync_ShouldThrowBadRequestException_WhenCodeIs41919()
        {
            // Arrange
            var databaseService = new Mock<IDatabaseService>(MockBehavior.Strict);
            var bankManager = new BankManager(databaseService.Object);

            // Act
            Func<Task> act = async () => await bankManager.GetByCodeAsync("41919");

            // Assert
            await act.Should()
                .ThrowAsync<BadRequestException>()
                .WithMessage(string.Format(Errors.NotAuthorizedBankCodeMessage, "41919"));

            databaseService.VerifyNoOtherCalls();
        }


    }
}
