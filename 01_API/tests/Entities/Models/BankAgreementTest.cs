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
            var entity = new BankAgreement(JdcPartnership.NonPartner);

            // Make sure we don't forget propeties
            entity.GetType().GetProperties().Length.Should().Be(1);

            // Test all properties ; number of tests below should match the number of propeties above
            entity.JdcPartnership.Should().Be(JdcPartnership.NonPartner);
        }
    }
}
