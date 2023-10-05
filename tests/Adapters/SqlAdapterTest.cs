// <copyright file="SqlAdapterTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Adapters.Tests
{
    using KPMG.Pulse.Back.Accounting.Mandate.Models;
    using KPMG.Pulse.Back.Accounting.Mandate.Sql;

    public class SqlAdapterTest
    {
        [Fact]
        public void GetAllCollections()
        {
            Guid id = Guid.NewGuid();

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

            var mandateRepository = new Mock<IMandateRepository>(MockBehavior.Strict);

            SqlAdapter adapter = new SqlAdapter(mandateRepository.Object);

            var res = adapter.GetAllCollections(queryDto);

            var expected = new List<Collection>()
            {
                new Collection(id, null, null, new DateTime(2023, 10, 3), new DateTime(2023, 10, 4), null),
            };

            res.Should().BeEquivalentTo(expected);
        }
    }
}
