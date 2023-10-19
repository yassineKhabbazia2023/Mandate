// <copyright file="CompanyDbTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Sql.Tests
{
    public class CompanyDbTest
    {
        [Fact]
        public void Defaults()
        {
            // Arrange & Act
            var entity = new CompanyDb();

            // Assert
            // Make sure we don't forget propeties
            entity.GetType().GetProperties().Length.Should().Be(9);

            // Test all properties ; number of tests below should match the number of propeties above
            entity.Id.Should().Be(Guid.Empty);
            entity.CompanyPersonal.Should().Be(null);
            entity.JeDeclareFolder.Should().Be(null);
            entity.Collections.Should().BeNull();
            entity.Name.Should().Be(null);
            entity.SiretNumber.Should().Be(null);
            entity.ErpId.Should().Be(null);
            entity.BankServicesProviderId.Should().Be(null);
            entity.CompanyCollaborators.Should().BeNull();
        }

        [Fact]
        public void Values()
        {
            // Arrange & Act
            var id = Guid.NewGuid();
            var entity = new CompanyDb()
            {
                Id = id,
                CompanyPersonal = new CompanyPersonalDb
                {
                    CompanyId = Guid.Parse("3f8f025b-85a4-464f-9d8a-5be9ed5d375b"),
                    Title = "Mr.",
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "john.doe@example.com",
                    Street = "123 Main St",
                    Complements = "Apt 4B",
                    ZipCode = "12345",
                    City = "Sample City",
                    Country = "ExampleLand",
                },
                Name = "JEAN LÉVAGE",
                SiretNumber = "40902900600031",
                ErpId = "1000265308",
                CompanyCollaborators = new List<CompanyCollaboratorDb>()
                {
                    new CompanyCollaboratorDb
                    {
                        CompanyId = Guid.NewGuid(),
                        Company = new CompanyDb(),
                        CollaboratorId = Guid.NewGuid(),
                        Collaborator = new CollaboratorDb(),
                    },
                },
            };

            // Assert
            entity.Id.Should().Be(id);
            entity.CompanyPersonal.CompanyId.Should().Be(Guid.Parse("3f8f025b-85a4-464f-9d8a-5be9ed5d375b"));
            entity.CompanyPersonal.Title.Should().Be("Mr.");
            entity.CompanyPersonal.FirstName.Should().Be("John");
            entity.CompanyPersonal.LastName.Should().Be("Doe");
            entity.CompanyPersonal.Email.Should().Be("john.doe@example.com");
            entity.CompanyPersonal.Street.Should().Be("123 Main St");
            entity.CompanyPersonal.Complements.Should().Be("Apt 4B");
            entity.CompanyPersonal.ZipCode.Should().Be("12345");
            entity.CompanyPersonal.City.Should().Be("Sample City");
            entity.CompanyPersonal.Country.Should().Be("ExampleLand");
            entity.Name.Should().Be("JEAN LÉVAGE");
            entity.SiretNumber.Should().Be("40902900600031");
            entity.ErpId.Should().Be("1000265308");
            entity.CompanyCollaborators.Count().Should().Be(1);

        }
    }
}
