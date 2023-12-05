// <copyright file="StatusDb.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Sql
{
    public class StatusDb
    {
        public Guid Id { get; set; }

        public Guid CollectionId { get; set; }

        public CollectionDb? Collection { get; set; } = null!;

        public int StatusCode { get; set; }

        public int? CollectionStatusCode { get; set; } = null!;

        public RefStatusCodeDb? RefStatusCode { get; set; } = null!;

        public bool IsCurrent { get; set; }

        public DateTime? StatusDate { get; set; } = null!;

        public byte[]? MandateFile { get; set; } = null!;

        public string? CreatedBy { get; set; } = null!;
    }
}
