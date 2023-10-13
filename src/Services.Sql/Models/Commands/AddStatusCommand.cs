// <copyright file="AddStatusCommand.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Sql
{
    public class AddStatusCommand
    {
        public Guid CollectionId { get; set; }

        public int StatusCode { get; set; }

        public byte[]? MandateFile { get; set; } = null!;

        public string? CreatedBy { get; set; } = null!;
    }
}
