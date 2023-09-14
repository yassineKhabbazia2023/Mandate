// <copyright file="BankAgreementTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Tests
{
    public class BankAgreementTest
    {
        [Fact]
        public void Constructor()
        {
            var entity = new BankAgreement(
                true,
                false,
                true);

            // Make sure we don't forget propeties
            entity.GetType().GetProperties().Length.Should().Be(3);

            // Make sure propeties don't have setters
            entity.GetType().GetProperties().Should().AllSatisfy(p => p.CanWrite.Should().BeFalse());

            // Test all properties ; number of tests below should match the number of propeties above
            entity.IsJdcPartner.Should().BeTrue();
            entity.IsJdcScrapable.Should().BeFalse();
            entity.HasJdcReleve.Should().BeTrue();
        }
    }
}
