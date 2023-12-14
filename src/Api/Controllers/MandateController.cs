// <copyright file="MandateController.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.AspNetCore
{
    using Aspose.Pdf.Operators;
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
                sortOrder ??= "Ascending";
                sortCriteria ??= "Name";

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
            string correlationId = Guid.NewGuid().ToString();

            try
            {
                var result = await this.mandateManager.CreateMandate(collectionCreationCommand.ToModel());
                return this.Ok(new SaveResult(result));
            }
            catch (Exception ex)
            {
                // TODO
                this.logger.LogError(ex, "MandateAPI - {correlationId} - {functionName}", correlationId, nameof(this.PostCollectionAsync));
                return this.StatusCode(StatusCodes.Status500InternalServerError, new Error("TechnicalError", correlationId, ex.Message));
            }
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
                this.logger.LogError(ex, "[{correlationId}] - There is no mandate with this [{mandateId}]", correlationId.ToString(), nameof(mandateId));
                return this.NotFound(new Error("CollectionNotFound", correlationId.ToString(), ex.Message));
            }
            catch (FolderIdEmptyOrNullException ex)
            {
                this.logger.LogError(ex, "[{correlationId}] - There is no folderId in the mandate with this [{mandateId}]", correlationId.ToString(), nameof(mandateId));
                return this.NotFound(new Error("FolderIdEmptyOrNull", correlationId.ToString(), ex.Message));
            }
            catch (RibIdEmptyOrNullException ex)
            {
                this.logger.LogError(ex, "[{correlationId}] - There is no ridId in the mandate with this [{mandateId}]", correlationId.ToString(), nameof(mandateId));
                return this.NotFound(new Error("RibIdEmptyOrNull", correlationId.ToString(), ex.Message));
            }
            catch (ServicesProviderException ex)
            {
                this.logger.LogError(ex, "[{correlationId}] - There is error when trying to download unsigned mandate [{mandateId}]", correlationId.ToString(), nameof(mandateId));
                return this.StatusCode(StatusCodes.Status500InternalServerError, new Error("ServicesProviderError", correlationId.ToString(), ex.Message));
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "[{correlationId}] - [{mandateId}]", correlationId.ToString(), nameof(mandateId));
                return this.StatusCode(StatusCodes.Status500InternalServerError, new Error("Exception", correlationId.ToString(), ex.Message));
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
        public async Task<IActionResult> UploadSignedMandateAsync([FromRoute] string mandateId, [FromForm] IFormFile file)
        {
            var correlationId = "0"; // TODO
            if (!Guid.TryParse(mandateId, out var parsedMandateId))
            {
                return this.BadRequest(new Error("InvalidMandateId", correlationId, "MandateId should be an UUID"));
            }

            try
            {
                if (file == null || file.ContentType != "application/pdf")
                {
                    throw new InvalidFileTypeException("The file must be a PDF.");
                }

                var result = await this.mandateManager.UploadSignedMandateAsync(parsedMandateId, file.OpenReadStream());
                return this.Ok(result);
            }
            catch (InvalidFileTypeException ex)
            {
                this.logger.LogError(ex, "MandateAPI - {correlationId} - Invalid file type", correlationId);
                return this.BadRequest(new Error("InvalidFileType", correlationId, ex.Message));
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "MandateAPI - {correlationId} - UploadSignedAsync", correlationId);
                return this.StatusCode(StatusCodes.Status500InternalServerError, new Error("TechnicalError", correlationId, ex.Message));
            }
        }

        [HttpPost("{mandateId}/deactivate")]
        public async Task<IActionResult> DeactivateAsync([FromRoute] string mandateId)
        {
            await Task.CompletedTask;
            return this.NoContent(); // TODO
        }
    }
}