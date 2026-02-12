// <copyright file="IPdfHelper.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate
{
    public interface IPdfHelper
    {
        Task<byte[]> GeneratePdfFromTemplateAsync(Collection source);
    }
}
