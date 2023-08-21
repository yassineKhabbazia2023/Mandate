// <copyright file="MandateController.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.AspNetCore
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("api")]
    [Authorize]
    public class MandateController : ControllerBase
    {
        private readonly ILogger<MandateController> logger;
        private readonly IMandateManager mandateManager;

        public MandateController(ILogger<MandateController> logger, IMandateManager mandateManager)
        {
            this.logger = logger;
            this.mandateManager = mandateManager;
        }

        public async Task<IActionResult> GetCollectionsAsync()
        {
            // TODO
            await Task.CompletedTask;
            this.logger.LogInformation("x");
            await this.mandateManager.GetAllCollections();
            throw new NotImplementedException();
        }
    }
}
