// <copyright file="CompanyPersonalDbTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Sql.Tests
{
    public class CompanyPersonalDbTest
    {
        [Fact]
        public void Defaults()
        {
            // Arrange
            var entity = new CompanyPersonalDb();

            // Assert
            // Make sure we don't forget propeties
            entity.GetType().GetProperties().Length.Should().Be(12);

            // Test all properties ; number of tests below should match the number of propeties above
            entity.Company.Should().BeNull("because it is initialized as null");
            entity.Title.Should().BeNull();
            entity.FirstName.Should().BeNull();
            entity.LastName.Should().BeNull();
            entity.Email.Should().BeNull();
            entity.Street.Should().BeNull();
            entity.Complements.Should().BeNull();
            entity.ZipCode.Should().BeNull();
            entity.City.Should().BeNull();
            entity.Country.Should().BeNull();
        }

        [Fact]
        public void Values()
        {
            // Arrange
            var entity = new CompanyPersonalDb
            {
                Title = "Mr.",
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                Street = "123 Main St",
                Complements = "Apt 4B",
                ZipCode = "12345",
                City = "Sample City",
                Country = "ExampleLand",
            };

            // Assert
            entity.Title.Should().Be("Mr.");
            entity.FirstName.Should().Be("John");
            entity.LastName.Should().Be("Doe");
            entity.Email.Should().Be("john.doe@example.com");
            entity.Street.Should().Be("123 Main St");
            entity.Complements.Should().Be("Apt 4B");
            entity.ZipCode.Should().Be("12345");
            entity.City.Should().Be("Sample City");
            entity.Country.Should().Be("ExampleLand");
        }
    }
}
