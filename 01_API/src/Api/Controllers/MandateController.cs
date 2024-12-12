// <copyright file="MandateController.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.AspNetCore
{
    using KPMG.Pulse.Back.Accounting.Mandate.Adapters;
    using KPMG.Pulse.Back.Accounting.Mandate.Client;
    using KPMG.Pulse.Back.Accounting.Mandate.Sql;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;
    using CollectionQuery = KPMG.Pulse.Back.Accounting.Mandate.Client.CollectionQuery;

    [ApiController]
    [Route("api/mandate")]
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
        public async Task<IActionResult> GetCollectionsAsync([FromQuery] string? searchTerm, [FromQuery] DateTime? creationDateStart, [FromQuery] DateTime? creationDateEnd, [FromQuery] DateTime? modificationDateStart, [FromQuery] DateTime? modificationDateEnd, [FromQuery] List<int>? statusCodes, [FromQuery] int? limit, [FromQuery] int? skip, [FromQuery] string? sortOrder, [FromQuery] string? sortCriteria, [FromQuery] string? contactEmail)
        {
            var correlationId = "0"; // TODO

            try
            {
                sortOrder ??= "Ascending";
                sortCriteria ??= "Name";
                contactEmail ??= this.Request.Headers["ContactEmail"].ToString();

                if (string.IsNullOrWhiteSpace(contactEmail))
                {
                    this.logger.LogError("Forbidden access due to missing contactEmail. CorrelationId: {CorrelationId}, FunctionName : {FunctionName}", correlationId, nameof(this.GetCollectionsAsync));
                    return this.StatusCode(StatusCodes.Status403Forbidden, new Error("Forbidden", correlationId.ToString(), "Forbidden access due to missing contactEmail."));
                }

                var collectionQuery = new CollectionQuery(searchTerm, creationDateStart, creationDateEnd, modificationDateStart, modificationDateEnd, statusCodes, limit, skip, sortOrder, sortCriteria, contactEmail!);
                this.logger.LogInformation("{collectionQuery}", JsonConvert.SerializeObject(collectionQuery));
                var result = await this.mandateManager.GetAllCollectionsAsync(collectionQuery.ToModel());
                return this.Ok(result.ToPageMandateDetails());
            }
            catch (UnauthorizedAccessException)
            {
                this.logger.LogError("The user is not authorized - MandateAPI - {correlationId} - {functionName}", correlationId, nameof(this.GetCollectionsAsync));
                return this.StatusCode(StatusCodes.Status401Unauthorized, new Error("Unauthorized", correlationId, "You are not authorized to access this resource."));
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "MandateAPI - {correlationId} - {functionName}", correlationId, nameof(this.GetCollectionsAsync));
                return this.StatusCode(StatusCodes.Status500InternalServerError, new Error("TechnicalError", correlationId, ex.Message));
            }
        }

        /// <summary>
        /// we add temporary this decorator of request timeout until we have a better performant solution.
        /// </summary>
        /// <param name="collectionCreationCommand">Collection command.</param>
        /// <param name="contactId">Collaborator id.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [HttpPost]
        public async Task<IActionResult> CreateMandateAsync(
            [FromBody] CollectionCreationCommand collectionCreationCommand,
            [FromQuery] int contactId)
        {
            var correlationId = "0";

            if (contactId == default)
            {
                this.logger.LogError("contactId is required - {correlationId} - {functionName}", correlationId, nameof(this.CreateMandateAsync));
                return this.StatusCode(StatusCodes.Status400BadRequest, new Error("ParamRequired", correlationId, "contactId is required."));
            }

            try
            {
                var collectionId = await this.mandateManager.CreateMandateAsync(collectionCreationCommand.ToModel(), contactId);
                return this.Ok(new SaveResult(collectionId));
            }
            catch (CompanyNotFoundException ex)
            {
                this.logger.LogError(ex, "Company was not found - {correlationId} - {contactId} - {functionName}", correlationId, contactId, nameof(this.CreateMandateAsync));
                return this.StatusCode(StatusCodes.Status400BadRequest, new Error("CompanyNotFound", correlationId, ex.Message));
            }
            catch (InactiveCompanyException ex)
            {
                this.logger.LogError(ex, "Company is inactive - {correlationId} - {contactId} - {functionName}", correlationId, contactId, nameof(this.CreateMandateAsync));
                return this.StatusCode(StatusCodes.Status400BadRequest, new Error("CompanyInactive", correlationId, ex.Message));
            }
            catch (InaccessibleCompanyException ex)
            {
                this.logger.LogError(ex, "Company is inaccessible - {correlationId} - {contactId} - {functionName}", correlationId, contactId, nameof(this.CreateMandateAsync));
                return this.StatusCode(StatusCodes.Status400BadRequest, new Error("CompanyInaccessible", correlationId, ex.Message));
            }
            catch (CompanyHasNoSiretException ex)
            {
                this.logger.LogError(ex, "Company has no SIRET - {correlationId} - {functionName}", correlationId, nameof(this.CreateMandateAsync));
                return this.StatusCode(StatusCodes.Status400BadRequest, new Error("CompanyHasNoSiret", correlationId, ex.Message));
            }
            catch (CustomBankCodeNotFoundException ex)
            {
                this.logger.LogError(ex, "Bank was not found - {correlationId} - {functionName}", correlationId, nameof(this.CreateMandateAsync));
                return this.StatusCode(StatusCodes.Status400BadRequest, new Error("BankNotFound", correlationId, ex.Message));
            }
            catch (JdcCollecteConfigExistException ex)
            {
                this.logger.LogError(ex, "JDC collecte config exists - {correlationId} - {functionName}", correlationId, nameof(this.CreateMandateAsync));
                return this.StatusCode(StatusCodes.Status400BadRequest, new Error("JdcCollecteConfigExists", correlationId, ex.Message));
            }
            catch (BankHasNoJdcPartnershipException ex)
            {
                this.logger.LogError(ex, "Bank has no JDC partnership - {correlationId} - {functionName}", correlationId, nameof(this.CreateMandateAsync));
                return this.StatusCode(StatusCodes.Status400BadRequest, new Error("BankHasNoJdcPartnership", correlationId, ex.Message));
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
        public async Task<IActionResult> UploadSignedMandateAsync([FromRoute] string mandateId, [FromForm] IFormFile file, [FromQuery] string? contactEmail)
        {
            var correlationId = "0"; // TODO
            if (!Guid.TryParse(mandateId, out var parsedMandateId))
            {
                return this.BadRequest(new Error("InvalidMandateId", correlationId, "MandateId should be an UUID"));
            }

            try
            {
                contactEmail ??= this.Request.Headers["ContactEmail"].ToString();
                if (string.IsNullOrWhiteSpace(contactEmail))
                {
                    this.logger.LogError("Forbidden access due to missing contactEmail. mandateId : {mandateId}, CorrelationId: {CorrelationId}, FunctionName : {FunctionName}", mandateId, correlationId, nameof(this.UploadSignedMandateAsync));
                    return this.StatusCode(StatusCodes.Status403Forbidden, new Error("Forbidden", correlationId.ToString(), "Forbidden access due to missing contactEmail."));
                }

                if (file == null || file.ContentType != "application/pdf")
                {
                    throw new InvalidFileTypeException("The file must be a PDF.");
                }

                var result = await this.mandateManager.UploadSignedMandateAsync(parsedMandateId, file.OpenReadStream(), contactEmail!);
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
        public async Task<IActionResult> DeactivateAsync([FromRoute] string mandateId, [FromQuery] string? contactEmail)
        {
            var correlationId = "0"; // TODO
            if (!Guid.TryParse(mandateId, out var parsedMandateId))
            {
                return this.BadRequest(new Error("InvalidMandateId", correlationId, "MandateId should be an UUID"));
            }

            contactEmail ??= this.Request.Headers["ContactEmail"].ToString();
            if (string.IsNullOrWhiteSpace(contactEmail))
            {
                this.logger.LogError("Forbidden access due to missing contactEmail. mandateId : {mandateId}, CorrelationId: {CorrelationId}, FunctionName : {FunctionName}", mandateId, correlationId, nameof(this.DeactivateAsync));
                return this.StatusCode(StatusCodes.Status403Forbidden, new Error("Forbidden", correlationId.ToString(), "Forbidden access due to missing contactEmail."));
            }

            if (await this.mandateManager.DeactivateCollectionAsync(parsedMandateId, contactEmail!))
            {
                return this.NoContent();
            }
            else
            {
                return this.NotFound();
            }
        }

        [HttpGet("{mandateId}/check/status")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> CheckMandateCreationStatus([FromRoute] string mandateId)
        {
            try
            {
                var mandateStatus = await this.mandateManager.GetMandateStatusAsync(Guid.Parse(mandateId));
                return mandateStatus switch
                {
                    CollectionStatus.Creation_Inprogress => this.NoContent(),
                    CollectionStatus.Incident => this.Ok(new { created = false }),
                    _ => this.Ok(new { created = true }),
                };
            }
            catch (CollectionNotFoundException ex)
            {
                return this.NotFound(new Error("CollectionNotFound", mandateId, ex.Message));
            }
        }
    }
}