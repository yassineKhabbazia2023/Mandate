// <copyright file="MandateController.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.AspNetCore
{
    using KPMG.Pulse.Back.Accounting.Mandate.Sql;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("api/[Controller]")]
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

        [HttpGet("collection")]
        public async Task<IActionResult> GetCollectionsAsync([FromQuery] CollectionQuery query)
        {
            await Task.CompletedTask;
            this.logger.LogInformation($"{query}");
            await this.mandateManager.GetAllCollections();
            return this.Ok();
        }
    }
}