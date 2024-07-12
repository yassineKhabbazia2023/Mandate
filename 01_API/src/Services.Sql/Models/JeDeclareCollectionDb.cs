// <copyright file="JeDeclareCollectionDb.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Sql
{
    public class JeDeclareCollectionDb
    {
        public Guid Id { get; set; }

        public Guid CollectionId { get; set; }

        public CollectionDb? Collection { get; set; } = null!;

        public string? JdcReleveId { get; set; } = null!;

        public string? JdcRibId { get; set; } = null!;
    }
}
