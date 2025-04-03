// <copyright file="PagedMandateTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Tests
{
    public class PagedMandateTest
    {
        [Fact]
        public void Constructor()
        {
            Signatory signatory = new Signatory("M", "maroo", "elleuch", "maroo@email.com");
            Address address = new Address("street", "comlements", "zip", "city", "country");

            Company company = new Company(
               1,
               "cn",
               "12345678910",
               "123456789",
               "123",
               signatory,
               address);

            Bank bank = new Bank("12345", "bn", "bg", "cartId", new BankAgreement(JdcPartnership.NonPartner));
            Bban bban = new Bban("12345", "54321", "12345678901", "55", "321", bank);

            Collection collection = new Collection(
                    new Guid("00000002-0000-0000-0000-000000000000"),
                    "1234",
                    company,
                    bban,
                    new DateTime(2022, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    new DateTime(2022, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    new Status(CollectionStatus.InProgress, "En cours", JdcCollectionStatus.Creation_InProgress, null), null);

            var counters = new Counters(1, 1, 0, 0, 0, 0);
            var entity = new PagedMandate(counters, new List<Collection> { collection });

            entity.GetType().GetProperties().Length.Should().Be(2);

            entity.Counters.Should().BeEquivalentTo(new Counters(1, 1, 0, 0, 0, 0));
            entity.Data.Should().BeEquivalentTo(new List<Collection>() { collection });
        }
    }
}
