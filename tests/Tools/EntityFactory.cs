// <copyright file="EntityFactory.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate
{
    public static class EntityFactory
    {
        public static Address Address => new ("street", "complements", "75001", "Paris", "France");

        public static BankAgreement BankAgreement => new (true, false, true);

        public static Bban Bban => new ("12345", "54321", "12345678901", "01");

        public static Company Company => new (new PredictableGuid().NewGuid(), "Raison Sociale", "siret", "ibsAccountNumber", "jdcDossierId", Signatory);

        public static Signatory Signatory => new ("Mme", "First", "Last", "first.last@outlook.com", Address);
    }
}
