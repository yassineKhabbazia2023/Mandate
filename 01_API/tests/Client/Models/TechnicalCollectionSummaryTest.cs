// <copyright file="TechnicalCollectionSummaryTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Client.Tests
{
    public class TechnicalCollectionSummaryTest
    {
        [Fact]
        public void Constructor()
        {
            var entity = new TechnicalCollectionSummary(
                new PredictableGuid().NewGuid(),
                "12345",
                "12347",
                new BankDetails(
                    "99999",
                    "00000",
                    "77340082511",
                    "99"),
                20);

            // Make sure we don't forget propeties
            entity.GetType().GetProperties().Length.Should().Be(5);

            // Test all properties ; number of tests below should match the number of propeties above
            entity.Id.Should().Be(Guid.Parse("00000001-0000-0000-0000-000000000000"));
            entity.FolderId.Should().Be("12345");
            entity.RibId.Should().Be("12347");
            entity.BankDetails.BankCode.Should().Be("99999");
            entity.BankDetails.BranchCode.Should().Be("00000");
            entity.BankDetails.AccountNumber.Should().Be("77340082511");
            entity.BankDetails.CheckDigits.Should().Be("99");
            entity.StatusCode.Should().Be(20);
        }
    }
}
