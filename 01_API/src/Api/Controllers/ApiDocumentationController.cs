// <copyright file="ApiDocumentationController.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.AspNetCore
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("api")]
    [AllowAnonymous]
    public class ApiDocumentationController : ControllerBase
    {
        public ApiDocumentationController()
        {
        }

        [HttpGet("api.json")]
        public IActionResult Get()
        {
            return this.File(this.GetType().Assembly.GetManifestResourceStream("KPMG.Pulse.Back.Accounting.Mandate.AspNetCore.API.Designer.json")!, "application/json");
        }
    }
}
