// <copyright file="SqlExtensionsTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Adapters.Tests
{
    using KPMG.Pulse.Back.Accounting.Mandate.Sql;

    public class SqlExtensionsTest
    {
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
                sortCriteria: Mandate.CollectionSortCriteria.ModificationDate,
                sortOrder: Mandate.SortOrder.Ascending,
                statusCodes: new List<int> { 1, 2 },
                collaboratorId: new Guid("00000001-0000-0000-0000-000000000000"));

            var res = queryDto.ToSql();

            res.Should().BeEquivalentTo(new CollectionQuery
            {
                CreationDateEnd = new DateTime(2023, 10, 1, 0, 0, 0, DateTimeKind.Utc),
                CreationDateStart = new DateTime(2023, 10, 2, 0, 0, 0, DateTimeKind.Utc),
                Limit = 10,
                ModificationDateStart = new DateTime(2023, 10, 3, 0, 0, 0, DateTimeKind.Utc),
                ModificationDateEnd = new DateTime(2023, 10, 4, 0, 0, 0, DateTimeKind.Utc),
                SearchTerm = "companyName",
                Skip = 0,
                SortCriteria = CollectionSortCriteria.ModificationDate,
                SortOrder = SortOrder.Ascending,
                StatusCodes = new List<int> { 1, 2 },
                CollaboratorId = new Guid("00000001-0000-0000-0000-000000000000"),
            });
        }
    }
}
