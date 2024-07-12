// <copyright file="RefStatusCodeDb.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Sql
{
    public class RefStatusCodeDb
    {
        public int StatusCode { get; set; }

        public int? CollectionStatusCode { get; set; } = null!;

        public int PulseCode { get; set; }

        public string StatusNameFr { get; set; } = null!;

        public string StatusNameEn { get; set; } = null!;
    }
}
