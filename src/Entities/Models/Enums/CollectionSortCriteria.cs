// <copyright file="CollectionSortCriteria.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate
{
    public enum CollectionSortCriteria
    {
        /// <summary>
        /// Sort by ErpId (AccountNumber from IBS or other)
        /// </summary>
        ErpId,

        /// <summary>
        /// Sort by company name
        /// </summary>
        Name,

        /// <summary>
        /// Sort by bank account (third BBAN field)
        /// </summary>
        AccountNumber,

        /// <summary>
        /// Sort by bank name
        /// </summary>
        BankName,

        /// <summary>
        /// Sort by first status date
        /// </summary>
        CreationDate,

        /// <summary>
        /// Sort by last status date
        /// </summary>
        ModificationDate,

        /// <summary>
        /// Sort by Pulse status (KPMG.Pulse.Back.Accounting.Mandate.CollectionStatus)
        /// </summary>
        Status,
    }
}