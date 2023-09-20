// <copyright file="JdcPartnership.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Sql
{
    public enum JdcPartnership : short
    {
        NoNScrappable = 0,
        Scrappable = 1,
        NonPartner = 2,
        Partner = 3,
    }
}
