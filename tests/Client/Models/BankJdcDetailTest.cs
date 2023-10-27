// <copyright file="BankJdcDetailTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Client.Tests
{
    public class BankJdcDetailTest
    {
        [Fact]
        public void Constructor()
        {
            var entity = new BankJdcDetail("NonPartner");

            // Make sure we don't forget propeties
            entity.GetType().GetProperties().Length.Should().Be(1);

            // Make sure propeties don't have setters
            entity.GetType().GetProperties().Should().AllSatisfy(p => p.CanWrite.Should().BeFalse());

            // Test all properties ; number of tests below should match the number of propeties above
            entity.JdcPartnership.Should().Be("NonPartner");
        }

        [Fact]
        public void Serialization()
        {
            var entity = new BankJdcDetail("NonPartner");

            entity.Should().BeJsonSerializableTo(new
            {
                jdcPartnership = "NonPartner",
            });
        }
    }
}
