// <copyright file="CollectionQuery.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Client
{
    public class CollectionQuery
    {
        public CollectionQuery(string? searchTerm, DateTime? creationDateStart, DateTime? creationDateEnd, DateTime? modificationDateStart, DateTime? modificationDateEnd, List<int>? statusCodes, int? limit, int? skip, string? sortOrder, string? sortCriteria)
        {
            this.SearchTerm = searchTerm;
            this.CreationDateStart = creationDateStart;
            this.CreationDateEnd = creationDateEnd;
            this.ModificationDateStart = modificationDateStart;
            this.ModificationDateEnd = modificationDateEnd;
            this.StatusCodes = statusCodes;
            this.Limit = limit;
            this.Skip = skip;
            this.SortOrder = sortOrder;
            this.SortCriteria = sortCriteria;
        }

        public string? SearchTerm { get; }

        public DateTime? CreationDateStart { get; }

        public DateTime? CreationDateEnd { get; }

        public DateTime? ModificationDateStart { get; }

        public DateTime? ModificationDateEnd { get; }

        public List<int>? StatusCodes { get; }

        public int? Limit { get; }

        public int? Skip { get; }

        public string? SortOrder { get; }

        public string? SortCriteria { get; }
    }
}
