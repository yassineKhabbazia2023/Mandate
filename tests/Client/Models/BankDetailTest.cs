// <copyright file="BankDetailTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Client.Tests
{
    public class BankDetailTest
    {
        [Fact]
        public void Constructor()
        {
            var jdc = new BankJdcDetail("NonPartner");
            var entity = new BankDetail("a", "b", jdc);

            // Make sure we don't forget propeties
            entity.GetType().GetProperties().Length.Should().Be(3);

            // Make sure propeties don't have setters
            entity.GetType().GetProperties().Should().AllSatisfy(p => p.CanWrite.Should().BeFalse());

            // Test all properties ; number of tests below should match the number of propeties above
            entity.BankCode.Should().Be("a");
            entity.BankName.Should().Be("b");
            entity.JdcDetail.Should().Be(jdc);
        }

        [Fact]
        public void Serialization()
        {
            var jdc = new BankJdcDetail("NonPartner");
            var entity = new BankDetail("a", "b", jdc);

            entity.Should().BeJsonSerializableTo(new
            {
                code = "a",
                name = "b",
                jeDeclare = new
                {
                    jdcPartnership = "NonPartner",
                },
            });
        }
    }
}
