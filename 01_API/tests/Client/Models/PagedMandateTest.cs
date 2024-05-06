// <copyright file="PagedMandateTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Client.Tests
{
    public class PagedMandateTest
    {
        [Fact]
        public void Constructor()
        {
            var collectionBankInfo = new CollectionBankInfo(
                bankName: "Crédit Agricole",
                accountNumber: "98765432101",
                jdcPartnership: 2);

            var entity = new PagedMandate(
                new Counters(15, 1, 2, 3, 4, 5),
                new List<CollectionSummary>()
                {
                    new CollectionSummary(
                        id:new PredictableGuid().NewGuid(),
                        erpId: "1234567890",
                        companyName: "Weyland Corporation",
                        collectionBankInfo: collectionBankInfo,
                        creationDate: new DateTime(2023, 10, 1, 0, 0, 0, DateTimeKind.Utc),
                        modificationDate: new DateTime(2023, 10, 2, 0, 0, 0, DateTimeKind.Utc),
                        statusCode: 10),
                });

            // Make sure we don't forget propeties
            entity.GetType().GetProperties().Length.Should().Be(2);

            // Test all properties ; number of tests below should match the number of propeties above
            entity.Counters.Should().NotBeNull();
            entity.Data.Should().NotBeNull();
            entity.Data.Count.Should().Be(1);
        }
    }
}
