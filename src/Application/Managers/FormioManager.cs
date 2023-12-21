// <copyright file="FormioManager.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Application
{
    public class FormioManager : IFormioManager
    {
        private readonly IFormioService formIoService;

        public FormioManager(IFormioService formIoService)
        {
            this.formIoService = formIoService;
        }

        public async Task<Collection?> GetCollectionByBban(Bban bban)
        {
            return await this.formIoService.GetSubmissionMandateAsync(bban);
        }
    }
}
