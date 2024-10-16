// <copyright file="PagedRecoveryMandateTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Client.Tests.Models
{
    public class PagedRecoveryMandateTest
    {
        [Fact]
        public void Constructor()
        {
            var collectionBankInfo = new CollectionBankInfo(
                bankName: "bank",
                accountNumber: "12345678910",
                jdcPartnership: 2);

            var statusSummary = new Client.StatusInfo(
                statusCode: 30,
                jdcStatusDescription: string.Empty,
                null);

            var entity = new PagedRecoveryMandate(
                1,
                new List<CollectionSummary>()
                {
                    new CollectionSummary(
                        id: new PredictableGuid().NewGuid(),
                        erpId: "12345",
                        companyName: "name",
                        collectionBankInfo: collectionBankInfo,
                        creationDate: DateTime.UtcNow,
                        modificationDate: DateTime.UtcNow,
                        statusSummary,
                        new List<string>()),
                });

            // Make sure we don't forget propeties
            entity.GetType().GetProperties().Length.Should().Be(2);

            // Test all properties ; number of tests below should match the number of propeties above
            entity.Imported.Should().Be(1);
            entity.Failed.Should().NotBeNull();
            entity.Failed.Count.Should().Be(1);
        }
    }
}