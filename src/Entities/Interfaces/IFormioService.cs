// <copyright file="IFormioService.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate
{
    public interface IFormioService
    {
        Task<Collection?> GetSubmissionMandateAsync(Bban bban);
    }
}
