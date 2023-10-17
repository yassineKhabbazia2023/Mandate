// <copyright file="BankTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Tests
{
    public class BankTest
    {
        [Fact]
        public void Constructor()
        {
            var entity = new Bank(
                "code",
                "name",
                "group",
                string.Empty,
                EntityFactory.BankAgreement);

            // Make sure we don't forget propeties
            entity.GetType().GetProperties().Length.Should().Be(4);

            // Make sure propeties don't have setters
            entity.GetType().GetProperties().Should().AllSatisfy(p => p.CanWrite.Should().BeFalse());

            // Test all properties ; number of tests below should match the number of propeties above
            entity.Code.Should().Be("code");
            entity.Name.Should().Be("name");
            entity.Group.Should().Be("group");
            entity.JdcAgreement.Should().NotBeNull();
        }
    }
}
