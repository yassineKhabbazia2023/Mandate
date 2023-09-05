// <copyright file="CompanyController.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.AspNetCore.Controllers
{
    using Address = KPMG.Pulse.Back.Accounting.Mandate.Client.Address;
    using Company = KPMG.Pulse.Back.Accounting.Mandate.Client.Company;
    using Signatory = KPMG.Pulse.Back.Accounting.Mandate.Client.Signatory;
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
            Address address = new ("street", "complement", "zipcode", "city", "contry");
            Signatory signatory = new ("M", "maroo", "elleuch", "email@emaul.com", address);
            Company company = new (Guid.NewGuid(), "MK 2000", "50339868700015", "1999072765", signatory);
            return this.Ok(company);
        }
    }
}