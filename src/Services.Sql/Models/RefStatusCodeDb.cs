// <copyright file="RefStatusCodeDb.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Sql
{
    public class RefStatusCodeDb
    {
        public int StatusCode { get; set; }

        public string StatusName { get; set; } = null!;
    }
}
