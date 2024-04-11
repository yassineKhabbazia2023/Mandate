// <copyright file="CollectionQuery.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Sql
{
    public class CollectionQuery
    {
        public int CollaboratorId { get; set; }

        public string? SearchTerm { get; set; }

        public DateTime? CreationDateStart { get; set; }

        public DateTime? CreationDateEnd { get; set; }

        public DateTime? ModificationDateStart { get; set; }

        public DateTime? ModificationDateEnd { get; set; }

        public List<int>? StatusCodes { get; set; } = null!;

        public int? Limit { get; set; }

        public int? Skip { get; set; }

        public SortOrder SortOrder { get; set; }

        public CollectionSortCriteria SortCriteria { get; set; }
    }
}
