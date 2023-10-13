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
        public async Task<IActionResult> GetCollectionsAsync([FromQuery] string? searchTerm, [FromQuery] DateTime? creationDateStart, [FromQuery] DateTime? creationDateEnd, [FromQuery] DateTime? modificationDateStart, [FromQuery] DateTime? modificationDateEnd, [FromQuery] List<int>? statusCodes, [FromQuery] int? limit, [FromQuery] int? skip, [FromQuery] string? sortOrder, [FromQuery] string? sortCriteria)
        {
            try
            {
                var collectionQuery = new Client.CollectionQuery(searchTerm, creationDateStart, creationDateEnd, modificationDateStart, modificationDateEnd, statusCodes, limit, skip, sortOrder, sortCriteria);
                this.logger.LogInformation($"{collectionQuery}");
                var result = await this.mandateManager.GetAllCollectionsAsync(collectionQuery.ToModel());
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