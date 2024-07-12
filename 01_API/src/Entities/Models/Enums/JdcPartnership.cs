// <copyright file="JdcPartnership.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate
{
    public enum JdcPartnership : byte
    {
        NonScrappable = 0,
        Scrappable = 1,
        NonPartner = 2,
        Partner = 3,
    }
}
