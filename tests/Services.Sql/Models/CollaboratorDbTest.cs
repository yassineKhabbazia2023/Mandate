// <copyright file="CollaboratorDbTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Sql.Tests
{
    public class CollaboratorDbTest
    {
        [Fact]
        public void Defaults()
        {
            // Arrange & Act
            var entity = new CollaboratorDb()
            {
                Id = Guid.Empty,
                Email = "smedini@kpmg.fr",
                FirstName = null,
                LastName = null,
            };

            // Assert
            // Make sure we don't forget propeties
            entity.GetType().GetProperties().Length.Should().Be(5);

            // Test all properties ; number of tests below should match the number of propeties above
            entity.Id.Should().Be(Guid.Empty);
            entity.Email.Should().NotBeNull();
            entity.FirstName.Should().Be(null);
            entity.LastName.Should().Be(null);
            entity.CompanyCollaborators.Should().BeNull();
        }

        [Fact]
        public void Values()
        {
            // Arrange & Act
            var entity = new CollaboratorDb()
            {
                Id = new PredictableGuid().NewGuid(),
                Email = "smedini@kpmg.fr",
                FirstName = "Seif Allah",
                LastName = "MEDINI",
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
            entity.Id.Should().Be(Guid.Parse("00000001-0000-0000-0000-000000000000"));
            entity.Email.Should().Be("smedini@kpmg.fr");
            entity.FirstName.Should().Be("Seif Allah");
            entity.LastName.Should().Be("MEDINI");
            entity.CompanyCollaborators.Count().Should().Be(1);
        }
    }
}
