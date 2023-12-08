// <copyright file="FormIoManager.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Application
{
    public class FormIoManager : IFormIoManager
    {
        private readonly IFormIoService formIoService;

        public FormIoManager(IFormIoService formIoService)
        {
            this.formIoService = formIoService;
        }

        public async Task<Collection?> GetCollectionByBban(Bban bban)
        {
            return await this.formIoService.GetSubmissionMandateAsync(bban);
        }
    }
}
