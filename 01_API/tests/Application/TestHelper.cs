// <copyright file="TestHelper.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Application.Tests
{
    internal static class TestHelper
    {
        public static Bank GetBank(string? ebicsCardId = null, bool isJdcPartner = false)
        {
            return new Bank("code", "name", "group", ebicsCardId, GetBankAgreement(isJdcPartner));
        }

        public static BankAgreement GetBankAgreement(bool isJdcPartner = false)
        {
            return new BankAgreement(isJdcPartner ? JdcPartnership.Partner : JdcPartnership.NonPartner);
        }

        public static Company GetCompany(int? id = null, string? bankServicesProviderId = null)
        {
            return new Company(
              id ?? 0,
              "SCI IMMO JACOBINS",
              "83030022400011",
              "1000332927",
              bankServicesProviderId,
              GetSignatory(),
              GetAddress());
        }

        public static Signatory GetSignatory()
        {
            return new Signatory("M", "Ludovic", "TYREL DE POIX", "lu.de.poix@mvo-h.fr");
        }

        public static Address GetAddress()
        {
            return new Address("1 Rue du Capitaine Floch", string.Empty, "72000", "Le Mans", "FRANCE");
        }

        public static Bban GetBban(string? bbanServicesProviderId = null, bool isJdcPartner = false)
        {
            return new Bban("code", "02408", "00011269900", "58", bbanServicesProviderId, GetBank("ebicsCardId", isJdcPartner));
        }

        public static Collection GetCollection(int? companyId = null, string? collectionServicesProviderId = null)
        {
            return new Collection(
                Guid.NewGuid(),
                collectionServicesProviderId,
                GetCompany(companyId),
                GetBban(),
                new DateTime(2023, 10, 18, 0, 0, 0, DateTimeKind.Utc),
                new DateTime(2023, 10, 18, 0, 0, 0, DateTimeKind.Utc),
                GetStatus(),
                null);
        }

        public static Status GetStatus(CollectionStatus collectionStatus = CollectionStatus.ToDo, string? statusName = null)
        {
            return new Status(collectionStatus, string.IsNullOrEmpty(statusName) ? "En Cours" : statusName, JdcCollectionStatus.Creation_InProgress);
        }
    }
}
