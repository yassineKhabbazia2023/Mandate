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
        public void ToCountersDetail()
        {
            Counters counters = new Counters(10, 2, 3, 1, 2, 2);

            var model = counters.ToCountersDetail();

            model.Should().BeEquivalentTo(new Client.Counters(10, 2, 3, 1, 2, 2));
        }

        [Fact]
        public void ToPageMandateDetails()
        {
            Counters counters = new Counters(1, 1, 0, 0, 0, 0);
            Company company = new Company(
                new Guid("00000001-0000-0000-0000-000000000000"),
                "cn",
                "12345678910",
                "123456789",
                string.Empty,
                null,
                null);

            Bban bban = new Bban("12345", "54321", "12345678901", "55", string.Empty, null);

            Collection collection = new Collection(
                    new Guid("00000002-0000-0000-0000-000000000000"),
                    string.Empty,
                    company,
                    bban,
                    new DateTime(2022, 1, 1),
                    new DateTime(2022, 1, 1),
                    new Status(default, string.Empty));

            List<Collection> collections = new List<Collection>()
            {
                collection,
            };

            PagedMandate page1 = new PagedMandate(counters, collections);

            var model = page1.ToPageMandateDetails();
            var expectedCounters = new Client.Counters(1, 1, 0, 0, 0, 0);
            var expectedCollections = new List<Client.CollectionSummary>()
            {
               collection.ToCollectionSummary(),
            };

            model.Should().BeEquivalentTo(new Client.PagedMandate(expectedCounters, expectedCollections));
        }
    }
}
