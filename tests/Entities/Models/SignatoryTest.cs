// <copyright file="SignatoryTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Tests
{
    public class SignatoryTest
    {
        [Fact]
        public void Constructor()
        {
            var entity = new Signatory(
                "Mme",
                "First",
                "Last",
                "first.last@outlook.com",
                EntityFactory.Address);

            // Make sure we don't forget propeties
            entity.GetType().GetProperties().Length.Should().Be(5);

            // Make sure propeties don't have setters
            entity.GetType().GetProperties().Should().AllSatisfy(p => p.CanWrite.Should().BeFalse());

            // Test all properties ; number of tests below should match the number of propeties above
            entity.Title.Should().Be("Mme");
            entity.FirstName.Should().Be("First");
            entity.LastName.Should().Be("Last");
            entity.Email.Should().Be("first.last@outlook.com");
            entity.Address.Should().NotBeNull();
        }
    }
}
