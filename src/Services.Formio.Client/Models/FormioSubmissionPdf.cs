// <copyright file="FormioSubmissionPdf.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Formio.Client
{
    public class FormioSubmissionPdf
    {
        public string Id { get; set; } = null!;

        public string Owner { get; set; } = null!;

        public string Created { get; set; } = null!;

        public string Modified { get; set; } = null!;

        public byte[] Data { get; set; } = null!;
    }
}
