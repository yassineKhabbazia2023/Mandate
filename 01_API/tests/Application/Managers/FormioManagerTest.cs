// <copyright file="FormioManagerTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Application.Tests.Managers
{
    public class FormioManagerTest
    {
        [Fact]
        public async Task GetCollectionByBban()
        {
            Bban bban = EntityFactory.Bban;
            Status status = new Status(CollectionStatus.ToDo, "En Cours", Mandate.JdcCollectionStatus.Creation_InProgress);
            Collection collection = new Collection(
                Guid.Empty,
                "23414",
                EntityFactory.Company,
                bban,
                new DateTime(2023, 8, 12),
                new DateTime(2023, 8, 12),
                status);

            var services = new Mock<IFormioService>(MockBehavior.Strict);
            services.Setup(item =>
                item.GetSubmissionMandateAsync(
                    It.Is<Bban>(b =>
                        b.BankCode == bban.BankCode &&
                        b.BranchCode == bban.BranchCode &&
                        b.AccountNumber == bban.AccountNumber &&
                        b.CheckDigits == bban.CheckDigits)))
                .ReturnsAsync(collection)
                .Verifiable();

            var manager = new FormioManager(services.Object);

            var res = await manager.GetCollectionByBban(bban);
            res.Should().BeEquivalentTo(collection);

            services.VerifyAll();
        }

        [Fact]
        public async Task GetCollectionByBban_When_GetSubmissionMandateAsync_Exception()
        {
            Bban bban = EntityFactory.Bban;
            Status status = new Status(CollectionStatus.ToDo, "En Cours", Mandate.JdcCollectionStatus.Creation_InProgress);

            var services = new Mock<IFormioService>(MockBehavior.Strict);
            services.Setup(item =>
                item.GetSubmissionMandateAsync(
                    It.Is<Bban>(b =>
                        b.BankCode == bban.BankCode &&
                        b.BranchCode == bban.BranchCode &&
                        b.AccountNumber == bban.AccountNumber &&
                        b.CheckDigits == bban.CheckDigits)))
                .ThrowsAsync(new Exception("message"))
                .Verifiable();

            var manager = new FormioManager(services.Object);

            Func<Task> action = async () => await manager.GetCollectionByBban(bban);
            await action.Should().ThrowAsync<Exception>().WithMessage("message");

            services.VerifyAll();
        }

        [Fact]
        public async Task GetAllCollectionAsync()
        {
            Bban bban = EntityFactory.Bban;
            Status status = new Status(CollectionStatus.ToDo, "En Cours", Mandate.JdcCollectionStatus.Creation_InProgress);
            Collection collection = new Collection(
                Guid.Empty,
                "23414",
                EntityFactory.Company,
                bban,
                new DateTime(2023, 8, 12),
                new DateTime(2023, 8, 12),
                status);

            var services = new Mock<IFormioService>(MockBehavior.Strict);
            services.Setup(item =>
                item.GetAllCollectionAsync(0, 1))
                .ReturnsAsync(new List<Collection> { collection })
                .Verifiable();

            var manager = new FormioManager(services.Object);

            var res = await manager.GetAllCollectionAsync(0, 1);
            res.Should().BeEquivalentTo(new List<Collection> { collection });

            services.VerifyAll();
        }

        [Fact]
        public async Task GetAllCollectionAsync_When_GetAllCollectionAsync_Throw_Exception()
        {
            Bban bban = EntityFactory.Bban;
            Status status = new Status(CollectionStatus.ToDo, "En Cours", Mandate.JdcCollectionStatus.Creation_InProgress);
            Collection collection = new Collection(
                Guid.Empty,
                "23414",
                EntityFactory.Company,
                bban,
                new DateTime(2023, 8, 12),
                new DateTime(2023, 8, 12),
                status);

            var services = new Mock<IFormioService>(MockBehavior.Strict);
            services.Setup(item =>
                item.GetAllCollectionAsync(0, 1))
                .ThrowsAsync(new Exception("message"))
                .Verifiable();

            var manager = new FormioManager(services.Object);

            Func<Task> action = async () => await manager.GetAllCollectionAsync(0, 1);
            await action.Should().ThrowExactlyAsync<Exception>().WithMessage("message");

            services.VerifyAll();
        }
    }
}
