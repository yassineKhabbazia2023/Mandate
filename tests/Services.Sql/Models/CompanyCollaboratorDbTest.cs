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
            PredictableGuid generator = new PredictableGuid();
            Guid comapanyId = generator.NewGuid();
            Guid collaboratorId = generator.NewGuid();
            var entity = new CompanyCollaboratorDb()
            {
                CompanyId = comapanyId,
                Company = new CompanyDb
                {
                    Id = comapanyId,
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
            entity.CompanyId.Should().Be(Guid.Parse("00000001-0000-0000-0000-000000000000"));
            entity.Company.Name.Should().Be("JEAN LÉVAGE");
            entity.Company.SiretNumber.Should().Be("40902900600031");
            entity.Company.ErpId.Should().Be("1000265308");
            entity.CollaboratorId.Should().Be(Guid.Parse("00000002-0000-0000-0000-000000000000"));
            entity.Collaborator.FirstName.Should().Be("John");
            entity.Collaborator.LastName.Should().Be("Doe");
            entity.Collaborator.Email.Should().Be("john.doe@example.com");
        }
    }
}
