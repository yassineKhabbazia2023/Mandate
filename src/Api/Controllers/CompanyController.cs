// <copyright file="CompanyController.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.AspNetCore.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    [Route("api/company")]
    [ApiController]
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
            this.logger.LogInformation($"x = {erpId}");
            Client.Address address = new ("street", "complement", "zipcode", "city", "contry");
            Client.Signatory signatory = new ("M", "maroo", "elleuch", "email@emaul.com", address);
            Client.Company company = new (Guid.NewGuid(), "MK 2000", "50339868700015", "1999072765", signatory);
            return this.Ok(company);
        }
    }
}