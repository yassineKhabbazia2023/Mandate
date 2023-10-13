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
                creationDateEnd: new DateTime(2023, 10, 1),
                creationDateStart: new DateTime(2023, 10, 2),
                limit: 10,
                modificationDateStart: new DateTime(2023, 10, 3),
                modificationDateEnd: new DateTime(2023, 10, 4),
                searchTerm: "companyName",
                skip: 0,
                sortCriteria: Mandate.CollectionSortCriteria.ModificationDate,
                sortOrder: Mandate.SortOrder.Ascending,
                statusCodes: new List<int> { 1, 2 });

            var res = queryDto.ToSql();

            res.Should().BeEquivalentTo(new CollectionQuery
            {
                CreationDateEnd = new DateTime(2023, 10, 1),
                CreationDateStart = new DateTime(2023, 10, 2),
                Limit = 10,
                ModificationDateStart = new DateTime(2023, 10, 3),
                ModificationDateEnd = new DateTime(2023, 10, 4),
                SearchTerm = "companyName",
                Skip = 0,
                SortCriteria = CollectionSortCriteria.ModificationDate,
                SortOrder = SortOrder.Ascending,
                StatusCodes = new List<int> { 1, 2 },
            });
        }
    }
}
