// <copyright file="MandateController.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.AspNetCore
{
    using KPMG.Pulse.Back.Accounting.Mandate.Adapters;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("api/mandate")]
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

        [HttpGet]
        public async Task<IActionResult> GetCollectionsAsync([FromQuery] CollectionQueryDto query)
        {
            try
            {
                this.logger.LogInformation($"{query}");
                var result = await this.mandateManager.GetAllCollectionsAsync(query);
                return this.Ok(result.Select(r => r.ToMandateCollection()));
            }
            catch (Exception ex)
            {
                this.logger.LogError($"{ex.Message}");
                throw;
            }
        }
    }
}