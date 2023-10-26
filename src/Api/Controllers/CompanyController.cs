// <copyright file="CompanyController.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.AspNetCore
{
    using KPMG.Pulse.Back.Accounting.Mandate.Client;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("api/company")]
    [Authorize]
    public class CompanyController : ControllerBase
    {
        private readonly ILogger<CompanyController> logger;

        public CompanyController(ILogger<CompanyController> logger)
        {
            this.logger = logger;
        }

        [HttpGet("{erpId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Company))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCompanyByErpIdAsync([FromRoute] string erpId)
        {
            string correlationId = "0"; // TODO

            await Task.CompletedTask;
            this.logger.LogInformation($"x = {erpId}"); // TODO
            Address address = new ("11 rue Street", "complement", "75014", "Paris", "France");
            Signatory signatory = new ("M", "maroo", "elleuch", "email@email.com");
            Company company = new (Guid.NewGuid(), "MK 2000", "50339868700015", "1999072765", signatory, address);
            return this.Ok(company);
        }
    }
}