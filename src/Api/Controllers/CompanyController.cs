// <copyright file="CompanyController.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.AspNetCore
{
    using KPMG.Pulse.Back.Accounting.Mandate.Client;
    using KPMG.Pulse.Back.Accounting.Mandate.Sql;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("api/company")]
    [Authorize]
    public class CompanyController : ControllerBase
    {
        private readonly ILogger<CompanyController> logger;
        private readonly ICompanyManager companyManager;

        public CompanyController(ILogger<CompanyController> logger, ICompanyManager companyManager)
        {
            this.logger = logger;
            this.companyManager = companyManager;
        }

        [HttpGet("{erpId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Company))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCompanyByErpIdAsync([FromRoute] string erpId)
        {
            string correlationId = Guid.NewGuid().ToString();

            try
            {
                this.logger.LogInformation($"Get company by erpId : {erpId}");
                var company = await this.companyManager.GetCompanyByErpIdAsync(erpId);
                return this.Ok(company);
            }
            catch (CompanyNotFoundException ex)
            {
                this.logger.LogError(ex, "[{CorrelationId}] - the company: {ErpId} has not been found.", correlationId, erpId);
                return this.NotFound(new Error("CompanyNotFound", correlationId.ToString(), ex.Message));
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "MandateAPI - {correlationId} - {functionName}", correlationId, nameof(this.GetCompanyByErpIdAsync));
                return this.StatusCode(StatusCodes.Status500InternalServerError, new Error("TechnicalError", correlationId, ex.Message));
            }
        }
    }
}