// <copyright file="MandateController.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.AspNetCore
{
    using KPMG.Pulse.Back.Accounting.Mandate.Adapters;
    using KPMG.Pulse.Back.Accounting.Mandate.Client;
    using KPMG.Pulse.Back.Accounting.Mandate.JeDeclare.Client;
    using KPMG.Pulse.Back.Accounting.Mandate.Portal;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("api/mandate")]
    [Authorize]
    public class MandateController : ControllerBase
    {
        private readonly ILogger<MandateController> logger;
        private readonly IMandateManager mandateManager;
        private readonly IAuthenticationContext authenticationContext;

        public MandateController(ILogger<MandateController> logger, IMandateManager mandateManager, IAuthenticationContext authenticationContext)
        {
            this.logger = logger;
            this.mandateManager = mandateManager;
            this.authenticationContext = authenticationContext;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PagedMandate))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCollectionsAsync([FromQuery] string? searchTerm, [FromQuery] DateTime? creationDateStart, [FromQuery] DateTime? creationDateEnd, [FromQuery] DateTime? modificationDateStart, [FromQuery] DateTime? modificationDateEnd, [FromQuery] List<int>? statusCodes, [FromQuery] int? limit, [FromQuery] int? skip, [FromQuery] string? sortOrder, [FromQuery] string? sortCriteria)
        {
            try
            {
                string email = this.authenticationContext.Email!;
                var collectionQuery = new CollectionQuery(searchTerm, creationDateStart, creationDateEnd, modificationDateStart, modificationDateEnd, statusCodes, limit, skip, sortOrder, sortCriteria, email);
                this.logger.LogInformation($"{collectionQuery}");
                var result = await this.mandateManager.GetAllCollectionsAsync(collectionQuery.ToModel());
                return this.Ok(result.Select(r => r.ToCollectionSummary()));
            }
            catch (Exception ex)
            {
                this.logger.LogError($"{ex.Message}");
                throw;
            }
        }

        [HttpPost]
        public async Task<IActionResult> PostCollectionAsync([FromBody] CollectionCreationCommand collectionCreationCommand)
        {
            try
            {
                await Task.CompletedTask;
                return this.Ok(); // TODO
            }
            catch (JeDeclareApiException e)
            {

                throw;
            }
        }

        [HttpGet("{mandateId}/unsigned")]
        public async Task<IActionResult> DownloadUnsignedAsync([FromRoute] string mandateId)
        {
            await Task.CompletedTask;
            return this.Ok(); // TODO
        }

        [HttpGet("{mandateId}/signed")]
        public async Task<IActionResult> DownloadSignedAsync([FromRoute] string mandateId)
        {
            await Task.CompletedTask;
            return this.Ok(); // TODO
        }

        [HttpPost("{mandateId}/signed")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadSignedAsync([FromRoute] string mandateId, [FromForm] IFormFile file)
        {
            await Task.CompletedTask;
            return this.NoContent(); // TODO
        }

        [HttpPost("{mandateId}/deactivate")]
        public async Task<IActionResult> DeactivateAsync([FromRoute] string mandateId)
        {
            await Task.CompletedTask;
            return this.NoContent(); // TODO
        }
    }
}