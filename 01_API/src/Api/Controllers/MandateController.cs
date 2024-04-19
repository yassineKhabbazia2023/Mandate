// <copyright file="MandateController.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.AspNetCore
{
    using KPMG.Pulse.Back.Accounting.Mandate.Adapters;
    using KPMG.Pulse.Back.Accounting.Mandate.Client;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;

    [ApiController]
    [Route("api/mandate")]
    [Authorize]
    [ServiceFilter(typeof(MandateAuthorizationFilterAttribute))]
    public class MandateController : ControllerBase
    {
        private readonly ILogger<MandateController> logger;
        private readonly IMandateManager mandateManager;
        private readonly IAuthenticationServices authenticationContext;
        private readonly IGuidGenerator guidGenerator;
        private readonly IFormioManager formIoManager;

        public MandateController(ILogger<MandateController> logger, IMandateManager mandateManager, IAuthenticationServices authenticationContext, IGuidGenerator guidGenerator, IFormioManager formIoManager)
        {
            this.logger = logger;
            this.mandateManager = mandateManager;
            this.formIoManager = formIoManager;
            this.guidGenerator = guidGenerator;
            this.authenticationContext = authenticationContext;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PagedMandate))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCollectionsAsync([FromQuery] string? searchTerm, [FromQuery] DateTime? creationDateStart, [FromQuery] DateTime? creationDateEnd, [FromQuery] DateTime? modificationDateStart, [FromQuery] DateTime? modificationDateEnd, [FromQuery] List<int>? statusCodes, [FromQuery] int? limit, [FromQuery] int? skip, [FromQuery] string? sortOrder, [FromQuery] string? sortCriteria)
        {
            var correlationId = "0"; // TODO

            try
            {
                string email = this.authenticationContext.Email!;
                sortOrder ??= "Ascending";
                sortCriteria ??= "Name";

                var collectionQuery = new CollectionQuery(searchTerm, creationDateStart, creationDateEnd, modificationDateStart, modificationDateEnd, statusCodes, limit, skip, sortOrder, sortCriteria, email);
                this.logger.LogInformation("{collectionQuery}", JsonConvert.SerializeObject(collectionQuery));
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
            var correlationId = "0"; // TODO

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

        [HttpGet("technical")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetTechnicalCollectionsAsync([FromQuery] string? searchTerm, [FromQuery] DateTime? creationDateStart, [FromQuery] DateTime? creationDateEnd, [FromQuery] DateTime? modificationDateStart, [FromQuery] DateTime? modificationDateEnd, [FromQuery] List<int>? statusCodes, [FromQuery] int? limit, [FromQuery] int? skip, [FromQuery] string? sortOrder, [FromQuery] string? sortCriteria)
        {
            string correlationId = Guid.NewGuid().ToString();

            try
            {
                sortOrder ??= "Ascending";
                sortCriteria ??= "Name";

                var collectionQuery = new CollectionQuery(searchTerm, creationDateStart, creationDateEnd, modificationDateStart, modificationDateEnd, statusCodes, limit, skip, sortOrder, sortCriteria, null!);
                this.logger.LogInformation("Processing collection query: {collectionQuery}", collectionQuery);
                var result = await this.mandateManager.GetAllTechnicalCollectionsAsync(collectionQuery.ToModel());
                return this.Ok(result.ToPageTechnicalMandateDetails());
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "MandateAPI - {correlationId} - {functionName}", correlationId, nameof(this.GetCollectionsAsync));
                return this.StatusCode(StatusCodes.Status500InternalServerError, new Error("TechnicalError", correlationId, ex.Message));
            }
        }

        [HttpPost("refresh-mandates-statuses")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RefreshMandatsStatusesAsync([FromBody] List<TechnicalCollectionSummary> mandates)
        {
            string correlationId = Guid.NewGuid().ToString();

            try
            {
                await this.mandateManager.RefreshMandatsStatusesAsync(mandates.Select(m => m.ToModel()).ToList());
                return this.Ok();
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "MandateAPI - {correlationId} - {functionName}", correlationId, nameof(this.GetCollectionsAsync));
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
            var correlationId = "0"; // TODO
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
            var correlationId = "0"; // TODO
            try
            {
                // Assuming you have your file data as byte[]
                byte[] fileData = await this.mandateManager.DownloadSignedAsync(Guid.Parse(mandateId));

                // Get the file type (MIME type)
                var contentType = "application/pdf";

                // Set a file download name (optional)
                var fileName = $"signed-mandate-{mandateId}.pdf";

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
            catch (ServicesProviderException ex)
            {
                this.logger.LogError(ex, "[{correlationId}] - There is error when trying to download signed mandate [{mandateId}]", correlationId.ToString(), nameof(mandateId));
                return this.StatusCode(StatusCodes.Status500InternalServerError, new Error("ServicesProviderError", correlationId.ToString(), ex.Message));
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "[{correlationId}] - [{mandateId}]", correlationId.ToString(), nameof(mandateId));
                return this.StatusCode(StatusCodes.Status500InternalServerError, new Error("Exception", correlationId.ToString(), ex.Message));
            }
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
            catch (ServicesProviderException ex)
            {
                this.logger.LogError(ex, "[{correlationId}] - There is error when trying to upload signed mandate [{mandateId}]", correlationId.ToString(), nameof(mandateId));
                return this.StatusCode(StatusCodes.Status500InternalServerError, new Error("ServicesProviderError", correlationId.ToString(), ex.Message));
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
            var correlationId = "0"; // TODO
            if (!Guid.TryParse(mandateId, out var parsedMandateId))
            {
                return this.BadRequest(new Error("InvalidMandateId", correlationId, "MandateId should be an UUID"));
            }

            if (await this.mandateManager.DeactivateCollectionAsync(parsedMandateId))
            {
                return this.NoContent();
            }
            else
            {
                return this.NotFound();
            }
        }

        [HttpPost("recovery")]
        public async Task<IActionResult> Recovery([FromBody] Bban rib)
        {
            var correlationId = "0"; // TODO
            try
            {
                Collection? collection = await this.formIoManager.GetCollectionByBban(rib.ToModel());
                if (collection != null)
                {
                    await this.mandateManager.InsertFormIOCollectionAsync(collection);
                    return this.Ok();
                }

                return this.NoContent();
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "MandateAPI - {correlationId} - {functionName}", correlationId, nameof(this.Recovery));
                return this.StatusCode(StatusCodes.Status500InternalServerError, new Error("TechnicalError", correlationId, ex.Message));
            }
        }

        [HttpPost("recovery-form-io")]
        public async Task<IActionResult> RecoveryFormIOAsync([FromQuery] int skip, [FromQuery] int limit)
        {
            var correlationId = "0";
            try
            {
                List<Collection> failed = new List<Collection>();
                List<Collection> collections = await this.formIoManager.GetAllCollectionAsync(skip, limit);

                foreach (var collection in collections)
                {
                    try
                    {
                        await this.mandateManager.InsertFormIOCollectionAsync(collection);
                    }
                    catch (ApplicationException)
                    {
                        this.logger.LogInformation("MandateAPI - {correlationId} - {functionName} : mandat trouvé {rib}", correlationId, nameof(this.RecoveryFormIOAsync), collection.Bban?.ToRibString());
                        failed.Add(collection);
                    }
                    catch (Sql.CompanyNotFoundException ex)
                    {
                        this.logger.LogError(ex, "MandateAPI - {correlationId} - {functionName} : {message}", correlationId, nameof(this.RecoveryFormIOAsync), ex.Message);
                        failed.Add(collection);
                    }
                }

                return this.Ok(failed);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "MandateAPI - {correlationId} - {functionName}", correlationId, nameof(this.RecoveryFormIOAsync));
                return this.StatusCode(StatusCodes.Status500InternalServerError, new Error("TechnicalError", correlationId, ex.Message));
            }
        }
    }
}