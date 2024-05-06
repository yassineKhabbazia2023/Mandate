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
            var entity = new PagedMandate(
                new Counters(15, 1, 2, 3, 4, 5),
                new List<CollectionSummary>()
                {
                    new CollectionSummary(
                    new PredictableGuid().NewGuid(),
                    "1234567890",
                    "Weyland Corporation",
                    "Crédit Agricole",
                    "98765432101",
                    2,
                    new DateTime(2023, 10, 1, 0, 0, 0, DateTimeKind.Utc),
                    new DateTime(2023, 10, 2, 0, 0, 0, DateTimeKind.Utc),
                    10),
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
