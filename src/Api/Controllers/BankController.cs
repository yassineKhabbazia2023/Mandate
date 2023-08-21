// <copyright file="BankController.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.AspNetCore
{
    using KPMG.Pulse.Back.Accounting.Mandate.Client;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("api/bank")]
    [AllowAnonymous]
    public class BankController : ControllerBase
    {
        private readonly IBbanManager bbanManager;

        public BankController(IBbanManager bbanManager)
        {
            this.bbanManager = bbanManager;
        }

        [HttpGet("checkBbanValidity")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ValidationResult))]
        public IActionResult ValidateBban([FromQuery] string bban)
        {
            var result = this.bbanManager.IsValid(bban);

            var validationResult = new ValidationResult(result);

            return this.Ok(validationResult);
        }

        [HttpGet("{bankCode}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BankDetail))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetBankDetails([FromRoute] string bankCode)
        {
            return this.Ok();
        }
    }
}
