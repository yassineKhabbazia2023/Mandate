// <copyright file="CollectionQueryDto.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Models
{
    public class CollectionQueryDto
    {
        public CollectionQueryDto()
        {
        }

        public CollectionQueryDto(
            string? searchTerm,
            DateTime? creationDateStart,
            DateTime? creationDateEnd,
            DateTime? modificationDateStart,
            DateTime? modificationDateEnd,
            List<int>? statusCodes,
            int? limit,
            int? skip,
            SortOrder sortOrder,
            CollectionSortCriteria sortCriteria)
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

        public string? SearchTerm { get; set; }

        public DateTime? CreationDateStart { get; set; }

        public DateTime? CreationDateEnd { get; set; }

        public DateTime? ModificationDateStart { get; set; }

        public DateTime? ModificationDateEnd { get; set; }

        public List<int>? StatusCodes { get; set; }

        public int? Limit { get; set; }

        public int? Skip { get; set; }

        public SortOrder SortOrder { get; set; }

        public CollectionSortCriteria SortCriteria { get; set; }
    }
}