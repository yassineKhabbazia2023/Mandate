// <copyright file="AddressTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Tests
{
    public class AddressTest
    {
        [Fact]
        public void Constructor()
        {
            var entity = new Address(
                "street",
                "complements",
                "75001",
                "Paris",
                "France");

            // Make sure we don't forget propeties
            entity.GetType().GetProperties().Length.Should().Be(5);

            // Test all properties ; number of tests below should match the number of propeties above
            entity.Street.Should().Be("street");
            entity.Complements.Should().Be("complements");
            entity.ZipCode.Should().Be("75001");
            entity.City.Should().Be("Paris");
            entity.Country.Should().Be("France");
        }
    }
}
