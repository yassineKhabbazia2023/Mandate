// <copyright file="PagedTechnicalMandateTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Client.Tests
{
    public class PagedTechnicalMandateTest
    {
        [Fact]
        public void Constructor()
        {
            var entity = new PagedTechnicalMandate(
                new List<TechnicalCollectionSummary>()
                {
                    new TechnicalCollectionSummary(
                    new PredictableGuid().NewGuid(),
                    "12345",
                    "12347",
                    new BankDetails(
                        "99999",
                        "00000",
                        "77340082511",
                        "99"),
                    20),
                });

            // Make sure we don't forget propeties
            entity.GetType().GetProperties().Length.Should().Be(1);

            // Test all properties ; number of tests below should match the number of propeties above
            entity.Data.Should().NotBeNull();
            entity.Data.Count.Should().Be(1);
        }
    }
}
