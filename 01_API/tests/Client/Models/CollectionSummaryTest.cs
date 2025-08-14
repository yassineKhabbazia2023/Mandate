// <copyright file="CollectionSummaryTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Client.Tests
{
    public class CollectionSummaryTest
    {
        [Fact]
        public void Constructor()
        {
            var collectionBankInfo = new CollectionBankInfo(
                bankName: "Crédit Agricole",
                accountNumber: "98765432101",
                jdcPartnership: 2);

            var statusSummary = new Client.StatusInfo(
                statusCode: 10,
                jdcStatusDescription: string.Empty,
                null);

            var entity = new CollectionSummary(
                id: new PredictableGuid().NewGuid(),
                erpId: "1234567890",
                companyName: "Weyland Corporation",
                collectionBankInfo: collectionBankInfo,
                creationDate: new DateTime(2023, 10, 1, 0, 0, 0, DateTimeKind.Utc),
                modificationDate: new DateTime(2023, 10, 2, 0, 0, 0, DateTimeKind.Utc),
                statusInfo: statusSummary,
                new List<string>(),
                destinationTool: "testdestination");

            // Make sure we don't forget propeties
            entity.GetType().GetProperties().Length.Should().Be(13);

            // Test all properties ; number of tests below should match the number of propeties above
            entity.Id.Should().Be(Guid.Parse("00000001-0000-0000-0000-000000000000"));
            entity.ErpId.Should().Be("1234567890");
            entity.CompanyName.Should().Be("Weyland Corporation");
            entity.BankName.Should().Be("Crédit Agricole");
            entity.AccountNumber.Should().Be("98765432101");
            entity.JdcPartnership.Should().Be(2);
            entity.CreationDate.Should().Be(new DateTime(2023, 10, 1, 0, 0, 0, DateTimeKind.Utc));
            entity.ModificationDate.Should().Be(new DateTime(2023, 10, 2, 0, 0, 0, DateTimeKind.Utc));
            entity.StatusCode.Should().Be(10);
        }
    }
}
