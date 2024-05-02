// <copyright file="StringExtensions.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Function.Helper
{
    using KPMG.Pulse.Back.Accounting.Mandate.Client;

    public static class StringExtensions
    {
        public static string Stringify(this CollectionSummary collection)
        {
            return $"{collection.Id}-{collection.AccountNumber} / {collection.ErpId}";
        }
    }
}
