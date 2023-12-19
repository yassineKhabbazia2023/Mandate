// <copyright file="EntityFactory.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate
{
    public static class EntityFactory
    {
        public static Address Address => new ("street", "complements", "75001", "Paris", "France");

        public static Bank Bank => new ("code", "name", "group", "ebicsCardId", BankAgreement);

        public static BankAgreement BankAgreement => new (JdcPartnership.NonPartner);

        public static Bban Bban => new ("12345", "54321", "12345678901", "01", "6789", Bank);

        public static Company Company => new (new PredictableGuid().NewGuid(), "Raison Sociale", "siret", "ibsAccountNumber", "jdcDossierId", Signatory, Address);

        public static Signatory Signatory => new ("Mme", "First", "Last", "first.last@outlook.com");

        public static Status Status(CollectionStatus collectionStatus = CollectionStatus.ToDo, string? statusName = null)
        {
            return new Status(collectionStatus, string.IsNullOrEmpty(statusName) ? "En Cours" : statusName);
        }
    }
}
