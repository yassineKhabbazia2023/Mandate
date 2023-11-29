// <copyright file="MandateController.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.AspNetCore
{
    using KPMG.Pulse.Back.Accounting.Mandate.Adapters;
    using KPMG.Pulse.Back.Accounting.Mandate.Client;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("api/mandate")]
    [Authorize]
    public class MandateController : ControllerBase
    {
        private readonly ILogger<MandateController> logger;
        private readonly IMandateManager mandateManager;
        private readonly IGuidGenerator guidGenerator;

        public MandateController(ILogger<MandateController> logger, IMandateManager mandateManager, IGuidGenerator guidGenerator)
        {
            this.logger = logger;
            this.mandateManager = mandateManager;
            this.guidGenerator = guidGenerator;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PagedMandate))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCollectionsAsync([FromQuery] string? searchTerm, [FromQuery] DateTime? creationDateStart, [FromQuery] DateTime? creationDateEnd, [FromQuery] DateTime? modificationDateStart, [FromQuery] DateTime? modificationDateEnd, [FromQuery] List<int>? statusCodes, [FromQuery] int? limit, [FromQuery] int? skip, [FromQuery] string? sortOrder, [FromQuery] string? sortCriteria)
        {
            string correlationId = Guid.NewGuid().ToString();

            try
            {
                var collectionQuery = new CollectionQuery(searchTerm, creationDateStart, creationDateEnd, modificationDateStart, modificationDateEnd, statusCodes, limit, skip, sortOrder, sortCriteria, string.Empty);
                this.logger.LogInformation($"{collectionQuery}");
                var result = await this.mandateManager.GetAllCollectionsAsync(collectionQuery.ToModel());
                return this.Ok(result.ToPageMandateDetails());
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "MandateAPI - {correlationId} - {functionName}", correlationId, nameof(this.GetCollectionsAsync));
                return this.StatusCode(StatusCodes.Status500InternalServerError, new Error("TechnicalError", correlationId, ex.Message));
            }
        }

        [HttpPost]
        public async Task<IActionResult> PostCollectionAsync([FromBody] CollectionCreationCommand collectionCreationCommand)
        {
            await Task.CompletedTask;
            return this.Ok(); // TODO
        }

        [HttpGet("{mandateId}/unsigned")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FileContentResult))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DownloadUnsignedAsync([FromRoute] string mandateId)
        {
            var correlationId = this.guidGenerator.NewGuid();
            try
            {
                // Assuming you have your file data as byte[]
                byte[] fileData = await this.mandateManager.DownloadUnsignedAsync(Guid.Parse(mandateId));

                // Get the file type (MIME type)
                var contentType = "application/pdf";

                // Set a file download name (optional)
                var fileName = $"unsigned-mandate-{mandateId}.pdf";

                // Return the file
                return this.File(fileData, contentType, fileName);
            }
            catch (Sql.CollectionNotFoundException ex)
            {
                this.logger.LogError("[{correlationId}] - There is no mandate with this [{mandateId}]", correlationId.ToString(), nameof(mandateId));
                return this.NotFound(new Error("CollectionNotFound", correlationId.ToString(), ex.Message));
            }
            catch (FolderIdEmptyOrNullException ex)
            {
                this.logger.LogError("[{correlationId}] - There is no folderId in the mandate with this [{mandateId}]", correlationId.ToString(), nameof(mandateId));
                return this.NotFound(new Error("FolderIdEmptyOrNull", correlationId.ToString(), ex.Message));
            }
            catch (RibIdEmptyOrNullException ex)
            {
                this.logger.LogError("[{correlationId}] - There is no ridId in the mandate with this [{mandateId}]", correlationId.ToString(), nameof(mandateId));
                return this.NotFound(new Error("RibIdEmptyOrNull", correlationId.ToString(), ex.Message));
            }
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