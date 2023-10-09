// <copyright file="ApplicationModelAdapter.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate
{
    using KPMG.Pulse.Back.Accounting.Mandate.Models;

    public static class ApplicationModelAdapter
    {
        public static Sql.CollectionQuery ToModel(this CollectionQueryDto source)
        {
            return new Sql.CollectionQuery
            {
                SearchTerm = source.SearchTerm,
                CreationDateStart = source.CreationDateStart,
                CreationDateEnd = source.CreationDateEnd,
                ModificationDateStart = source.ModificationDateStart,
                ModificationDateEnd = source.ModificationDateEnd,
                StatusCodes = source.StatusCodes,
                Limit = source.Limit,
                Skip = source.Skip,
                SortOrder = (Sql.SortOrder)source.SortOrder,
                SortCriteria = (Sql.CollectionSortCriteria)source.SortCriteria,
            };
        }
    }
}