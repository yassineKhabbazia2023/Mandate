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
            var entity = new PagedRecoveryMandate(
                1,
                new List<CollectionSummary>()
                {
                    new CollectionSummary(
                    new PredictableGuid().NewGuid(),
                    "12345",
                    "name",
                    "bank",
                    "12345678910",
                    2,
                    DateTime.UtcNow,
                    DateTime.UtcNow,
                    30),
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