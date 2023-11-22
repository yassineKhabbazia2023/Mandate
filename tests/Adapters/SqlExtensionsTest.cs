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
                collaboratorEmail: "collab@email.com");

            var res = queryDto.ToSql(new Guid("00000001-0000-0000-0000-000000000000"));

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
    }
}
