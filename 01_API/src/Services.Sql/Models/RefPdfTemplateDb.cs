// <copyright file="RefPdfTemplateDb.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Sql
{
    public class RefPdfTemplateDb
    {
        public string BankCode { get; set; } = string.Empty;

        public byte[] PdfFile { get; set; } = null!;
    }
}
