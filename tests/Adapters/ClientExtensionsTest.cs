// <copyright file="ClientExtensionsTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Adapters.Tests
{
    public class ClientExtensionsTest
    {
        [Fact]
        public void ToBankDetail()
        {
            var entity = new Bank("a", "b", "c", "d", new BankAgreement(JdcPartnership.NonPartner));
            var result = entity.ToBankDetail();

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(new Client.BankDetail("a", "b", new Client.BankJdcDetail("NonPartner")));
        }

        [Fact]
        public void ToCollectionSummary()
        {
            Guid id = Guid.NewGuid();
            Company company = new Company(Guid.NewGuid(), "mega", "45207964300014", "1999156874", string.Empty, null, null);
            Bank? bank = new Bank("12345", "biap", "biap group", string.Empty, null!);
            Bban bban = new Bban("12345", "56789", "12345678901", "88", "6789", bank);
            Status status = new Status(CollectionStatus.ToDo, "todo");

            Collection collection = new Collection(
                id,
                "12346",
                company,
                bban,
                new DateTime(2023, 10, 1, 0, 0, 0, DateTimeKind.Utc),
                new DateTime(2023, 10, 2, 0, 0, 0, DateTimeKind.Utc),
                status);

            var model = collection.ToCollectionSummary();

            var expected = new Client.CollectionSummary(
                id,
                "1999156874",
                "mega",
                "biap",
                "12345678901",
                new DateTime(2023, 10, 1, 0, 0, 0, DateTimeKind.Utc),
                new DateTime(2023, 10, 2, 0, 0, 0, DateTimeKind.Utc),
                (int)CollectionStatus.ToDo);

            model.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void ToModel()
        {
            var entity = new Client.CollectionQuery(
                "search",
                new DateTime(2023, 10, 1, 0, 0, 0, DateTimeKind.Utc),
                new DateTime(2023, 10, 2, 0, 0, 0, DateTimeKind.Utc),
                new DateTime(2023, 10, 1, 0, 0, 0, DateTimeKind.Utc),
                new DateTime(2023, 10, 2, 0, 0, 0, DateTimeKind.Utc),
                new List<int>() { -1,3},
                10,
                0,
                "Ascending",
                "AccountNumber",
                "collab@email.com");

            var model = entity.ToModel();

            model.Should().BeEquivalentTo(new CollectionQueryDto(
                 "search",
                 new DateTime(2023, 10, 1, 0, 0, 0, DateTimeKind.Utc),
                 new DateTime(2023, 10, 2, 0, 0, 0, DateTimeKind.Utc),
                 new DateTime(2023, 10, 1, 0, 0, 0, DateTimeKind.Utc),
                 new DateTime(2023, 10, 2, 0, 0, 0, DateTimeKind.Utc),
                 new List<int>() { -1, 3 },
                 10,
                 0,
                 SortOrder.Ascending,
                 CollectionSortCriteria.AccountNumber,
                 "collab@email.com"));
        }
    }
}
