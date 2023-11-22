// <copyright file="EntityDbFactory.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Application.Tests
{
    public static class EntityFactory
    {
        public static Signatory Signatory => new Signatory(
            "Mme",
            "Jane",
            "Doe",
            "sign@email.com");

        public static Address Address => new Address(
            "36 rue de Liège",
            "étage 5",
            "75008",
            "Paris",
            "France");

        public static Company Company => new Company(
            new Guid("00000001-0000-0000-0000-000000000000"),
            "cn",
            "12345678910",
            "123456789",
            "10987",
            Signatory,
            Address);

        public static BankAgreement BankAgreement => new BankAgreement(JdcPartnership.NonPartner);

        public static Bank Bank => new Bank(
              "12345",
              "bn",
              "bg",
              "cartId",
              BankAgreement);

        public static Bban Bban => new Bban(
            "12345",
            "54321",
            "12345678901",
            "55",
            "98765",
            Bank);

        public static Collection Collection => new Collection(
                new Guid("00000002-0000-0000-0000-000000000000"),
                string.Empty,
                Company,
                Bban,
                new DateTime(2022, 1, 1),
                new DateTime(2022, 1, 1),
                new Status(CollectionStatus.InProgress, "En cours"));

        public static Counters Counters => new Counters(1, 1, 0, 0, 0, 0);

        public static Collaborator Collaborator => new Collaborator(
            new Guid("00000001-0000-0000-0000-000000000000"),
            "collab@email.com",
            "fname",
            "lname");

        public static PagedMandate Page(List<Collection> collections) => new PagedMandate(Counters, collections);
    }
}
