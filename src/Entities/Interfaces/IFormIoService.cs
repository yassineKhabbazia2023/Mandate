// <copyright file="IFormIoService.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate
{
    public interface IFormIoService
    {
        Task<Collection?> GetSubmissionMandateAsync(Bban bban);
    }
}
