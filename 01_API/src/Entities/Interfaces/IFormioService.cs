// <copyright file="IFormioService.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate
{
    public interface IFormioService
    {
        Task<List<Collection>> GetAllCollectionAsync(int skip, int limit);

        Task<Collection?> GetSubmissionMandateAsync(Bban bban);
    }
}
