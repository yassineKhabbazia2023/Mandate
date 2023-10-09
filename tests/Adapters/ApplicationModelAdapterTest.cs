// <copyright file="ApplicationModelAdapterTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Adapters.Tests
{
    using KPMG.Pulse.Back.Accounting.Mandate.Models;
    using KPMG.Pulse.Back.Accounting.Mandate.Sql;

    public class ApplicationModelAdapterTest
    {
        [Fact]
        public void ToModelTest()
        {
            CollectionQueryDto queryDto = new CollectionQueryDto()
            {
                CreationDateEnd = new DateTime(2023, 10, 1),
                CreationDateStart = new DateTime(2023, 10, 2),
                Limit = 10,
                ModificationDateStart = new DateTime(2023, 10, 3),
                ModificationDateEnd = new DateTime(2023, 10, 4),
                SearchTerm = "companyName",
                Skip = 0,
                SortCriteria = Mandate.CollectionSortCriteria.ModificationDate,
                SortOrder = Mandate.SortOrder.Ascending,
                StatusCodes = new List<int> { 1, 2 },
            };

            var res = queryDto.ToModel();

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
