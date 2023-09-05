// <copyright file="BbanTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Tests
{
    public class BbanTest
    {
        [Fact]
        public void Constructor()
        {
            var entity = new Bban(
                "12345",
                "54321",
                "12345678901",
                "01");

            // Make sure we don't forget propeties
            entity.GetType().GetProperties().Length.Should().Be(4);

            // Make sure propeties don't have setters
            entity.GetType().GetProperties().Should().AllSatisfy(p => p.CanWrite.Should().BeFalse());

            // Test all properties ; number of tests below should match the number of propeties above
            entity.BankCode.Should().Be("12345");
            entity.BranchCode.Should().Be("54321");
            entity.AccountNumber.Should().Be("12345678901");
            entity.CheckDigits.Should().Be("01");
        }
    }
}
