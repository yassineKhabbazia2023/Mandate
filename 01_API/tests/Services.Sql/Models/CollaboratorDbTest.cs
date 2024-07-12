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
                Id = 1,
                Email = "smedini@kpmg.fr",
                FirstName = null,
                LastName = null,
                IsActive = false,
            };

            // Assert
            // Make sure we don't forget propeties
            entity.GetType().GetProperties().Length.Should().Be(6);

            // Test all properties ; number of tests below should match the number of propeties above
            entity.Id.Should().Be(1);
            entity.Email.Should().NotBeNull();
            entity.FirstName.Should().Be(null);
            entity.LastName.Should().Be(null);
            entity.IsActive.Should().Be(false);
            entity.CompanyCollaborators.Should().BeNull();
        }

        [Fact]
        public void Values()
        {
            // Arrange & Act
            var entity = new CollaboratorDb()
            {
                Id = 1,
                Email = "smedini@kpmg.fr",
                FirstName = "Seif Allah",
                LastName = "MEDINI",
                CompanyCollaborators = new List<CompanyCollaboratorDb>()
                {
                    new CompanyCollaboratorDb
                    {
                        CompanyId = 1,
                        Company = new CompanyDb(),
                        CollaboratorId = 1,
                        Collaborator = new CollaboratorDb(),
                    },
                },
            };

            // Assert
            entity.Id.Should().Be(1);
            entity.Email.Should().Be("smedini@kpmg.fr");
            entity.FirstName.Should().Be("Seif Allah");
            entity.LastName.Should().Be("MEDINI");
            entity.CompanyCollaborators.Count.Should().Be(1);
        }
    }
}
