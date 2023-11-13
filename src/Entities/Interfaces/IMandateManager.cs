// <copyright file="IMandateManager.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate
{
    public interface IMandateManager
    {
        Task<IEnumerable<Collection>> GetAllCollectionsAsync(CollectionQueryDto query);

        Task<Collaborator> GetCollaboratorByEmail(string email);
        
        Task<Guid> CreateMandate(MandateCreation mandateCreation);
    }
}
