// <copyright file="SqlExtensionsTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Adapters.Tests
{
    public class SqlExtensionsTest
    {
        [Fact]
        public void BankToModel()
        {
            var entity = new Sql.RefBankDb() { BankCode = "c", BankName = "n", BankGroup = "g", EbicsCardId = "e", JdcPartnership = (Sql.JdcPartnership)2 };
            var result = entity.ToModel();

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(new Bank("c", "n", "g", "e", new BankAgreement(JdcPartnership.NonPartner)));
        }

        [Fact]
        public void ToSqlTest()
        {
            CollectionQueryDto queryDto = new CollectionQueryDto(
                creationDateEnd: new DateTime(2023, 10, 1, 0, 0, 0, DateTimeKind.Utc),
                creationDateStart: new DateTime(2023, 10, 2, 0, 0, 0, DateTimeKind.Utc),
                limit: 10,
                modificationDateStart: new DateTime(2023, 10, 3, 0, 0, 0, DateTimeKind.Utc),
                modificationDateEnd: new DateTime(2023, 10, 4, 0, 0, 0, DateTimeKind.Utc),
                searchTerm: "companyName",
                skip: 0,
                sortCriteria: CollectionSortCriteria.ModificationDate,
                sortOrder: SortOrder.Ascending,
                statusCodes: new List<int> { 1, 2 },
                collaboratorId: new Guid("00000001-0000-0000-0000-000000000000"));

            var res = queryDto.ToSql();

            res.Should().BeEquivalentTo(new Sql.CollectionQuery
            {
                CreationDateEnd = new DateTime(2023, 10, 1, 0, 0, 0, DateTimeKind.Utc),
                CreationDateStart = new DateTime(2023, 10, 2, 0, 0, 0, DateTimeKind.Utc),
                Limit = 10,
                ModificationDateStart = new DateTime(2023, 10, 3, 0, 0, 0, DateTimeKind.Utc),
                ModificationDateEnd = new DateTime(2023, 10, 4, 0, 0, 0, DateTimeKind.Utc),
                SearchTerm = "companyName",
                Skip = 0,
                SortCriteria = Sql.CollectionSortCriteria.ModificationDate,
                SortOrder = Sql.SortOrder.Ascending,
                StatusCodes = new List<int> { 1, 2 },
                CollaboratorId = new Guid("00000001-0000-0000-0000-000000000000"),
            });
        }

        [Fact]
        public void ToModel_ShouldConvertCompanyDbToCompanyModelCorrectly()
        {
            // Arrange
            var companyDb = new Sql.CompanyDb
            {
                Id = Guid.NewGuid(),
                Name = "Test Company",
                SiretNumber = "123456789",
                ErpId = "ERP123",
                JeDeclareFolder = new Sql.JeDeclareFolderDb { JdcDossierId = "JDC123" },
                Personal = new Sql.PersonalDb
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
                },
            };

            // Act
            var result = companyDb.ToModel();

            // Assert
            result.Id.Should().Be(companyDb.Id);
            result.Name.Should().Be("Test Company");
            result.SiretNumber.Should().Be("123456789");
            result.ErpId.Should().Be("ERP123");
            result.BankServicesProviderId.Should().Be("JDC123");
            result?.Signatory?.Title.Should().Be("Mr.");
            result?.Signatory?.FirstName.Should().Be("John");
            result?.Signatory?.LastName.Should().Be("Doe");
            result?.Signatory?.Email.Should().Be("john.doe@example.com");
            result?.Address?.Street.Should().Be("123 Main St");
            result?.Address?.ZipCode.Should().Be("12345");
            result?.Address?.City.Should().Be("Sample City");
            result?.Address?.Country.Should().Be("ExampleLand");
            result?.Address?.Complements.Should().Be("Apt 4B");
        }
    }
}
