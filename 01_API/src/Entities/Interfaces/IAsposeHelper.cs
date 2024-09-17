// <copyright file="IAsposeHelper.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate
{
    public interface IAsposeHelper
    {
        Task<byte[]> GeneratePdfFromTemplateAsync(Collection source);
    }
}
