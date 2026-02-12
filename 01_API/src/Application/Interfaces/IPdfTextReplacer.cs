// <copyright file="IPdfTextReplacer.cs" company="Pulse">
// Copyright (c) Pulse. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Application
{
    public interface IPdfTextReplacer
    {
        byte[] ReplaceText(byte[] pdfTemplate, Replacement[] replacements);
    }
}
