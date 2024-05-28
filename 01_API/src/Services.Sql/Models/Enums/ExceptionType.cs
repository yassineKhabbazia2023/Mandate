// <copyright file="SortOrder.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Sql
{
    public enum ExceptionType
    {
        /// <summary>
        /// Match AccountNumber but duplicate SIRET.
        /// </summary>
        AccountNumberMatchDoubleSiret,

        /// <summary>
        /// AccountNumber match but no SIRET match.
        /// </summary>
        AccountNumberNoMatchSiret,

        /// <summary>
        /// No AccountNumber Match and several SIRET matches.
        /// </summary>
        NoAccountNumberMatchDoubleSiret,

        /// <summary>
        /// No AccountNumber Match and no SIRET match.
        /// </summary>
        NoAccountNumberNoMatchSiret,

        /// <summary>
        /// No Bank found with code.
        /// </summary>
        BankCodeNotFound,
    }
}
