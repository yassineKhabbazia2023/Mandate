// <copyright file="CompanyCollaboratorDbTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Sql.Tests
{
    public class CompanyCollaboratorDbTest
    {
        [Fact]
        public void Defaults()
        {
            // Arrange & Act
            var entity = new CompanyCollaboratorDb();

            // Assert
            // Make sure we don't forget propeties
            entity.GetType().GetProperties().Length.Should().Be(4);

            // Test all properties ; number of tests below should match the number of propeties above
            entity.CompanyId.Should().Be(Guid.Empty);
            entity.Company.Should().Be(null);
            entity.CollaboratorId.Should().Be(Guid.Empty);
            entity.Collaborator.Should().Be(null);
        }

        [Fact]
        public void Values()
        {
            // Arrange & Act
            var companyId = Guid.NewGuid();
            var collaboratorId = Guid.NewGuid();
            var entity = new CompanyCollaboratorDb()
            {
                CompanyId = companyId,
                Company = new CompanyDb
                {
                    Id = companyId,
                    Name = "JEAN LÉVAGE",
                    SiretNumber = "40902900600031",
                    ErpId = "1000265308",
                },
                CollaboratorId = collaboratorId,
                Collaborator = new CollaboratorDb
                {
                    Id = collaboratorId,
                    Email = "john.doe@example.com",
                    FirstName = "John",
                    LastName = "Doe",
                },
            };

            // Assert
            entity.CompanyId.Should().Be(companyId);
            entity.Company.Name.Should().Be("JEAN LÉVAGE");
            entity.Company.SiretNumber.Should().Be("40902900600031");
            entity.Company.ErpId.Should().Be("1000265308");
            entity.CollaboratorId.Should().Be(collaboratorId);
            entity.Collaborator.FirstName.Should().Be("John");
            entity.Collaborator.LastName.Should().Be("Doe");
            entity.Collaborator.Email.Should().Be("john.doe@example.com");
        }
    }
}
