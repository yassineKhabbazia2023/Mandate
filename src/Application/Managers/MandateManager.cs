// <copyright file="MandateManager.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Application
{
    public class MandateManager : IMandateManager
    {
        public Task<IEnumerable<Collection>> GetAllCollections()
        {
            throw new NotImplementedException();
        }
    }
}
