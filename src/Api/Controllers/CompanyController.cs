// <copyright file="CompanyController.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.AspNetCore.Controllers
{
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
        public async Task<IActionResult> GetCompanyByErpNumber([FromRoute] string erpId = "")
        {
            await Task.CompletedTask;
            this.logger.LogInformation($"x = {erpId}"); // TODO
            Client.Address address = new ("11 rue Street", "complement", "75014", "Paris", "France");
            Client.Signatory signatory = new ("M", "maroo", "elleuch", "email@email.com");
            Client.Company company = new (Guid.NewGuid(), "MK 2000", "50339868700015", "1999072765", signatory, address);
            return this.Ok(company);
        }
    }
}