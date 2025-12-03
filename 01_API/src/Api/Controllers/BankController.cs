// <copyright file="BankController.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>
using Pulse.ExceptionMiddleware.Exceptions;
using Pulse.ExceptionMiddleware.Model;
namespace KPMG.Pulse.Back.Accounting.Mandate.AspNetCore
{

    using KPMG.Pulse.Back.Accounting.Mandate.Adapters;
    using KPMG.Pulse.Back.Accounting.Mandate.Client;
    using KPMG.Pulse.Back.Accounting.Mandate.Sql;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// Controller providing bank-related operations.
    /// </summary>
    [ApiController]
    [Route("api/bank")]
    [AllowAnonymous]
    public class BankController : ControllerBase
    {
        private readonly IBbanManager bbanManager;
        private readonly IBankManager bankManager;


        /// <summary>
        /// Initializes a new instance of the <see cref="BankController"/> class.
        /// </summary>
        /// <param name="bbanManager">Service used to manage BBAN operations.</param>
        /// <param name="bankManager">Service used to retrieve bank information.</param>
        public BankController( IBbanManager bbanManager, IBankManager bankManager)
        {
            this.bbanManager = bbanManager;
            this.bankManager = bankManager;
        }

        [HttpGet("{bankCode}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BankDetail))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
        public async Task<IActionResult> GetBankDetailsAsync([FromRoute] string bankCode)
        {
     
            var result = await this.bankManager.GetByCodeAsync(bankCode).ConfigureAwait(false);
            return this.Ok(result.ToBankDetail());
        }

        [HttpGet("check-bban-validity")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ValidationResult))]
        public IActionResult ValidateBban([FromQuery] string bankCode, [FromQuery] string branchCode, [FromQuery] string accountNumber, [FromQuery] string checkDigits)
        {
            var bban = new Bban(bankCode, branchCode, accountNumber, checkDigits);
            var result = this.bbanManager.IsValid(bban.ToModel());

            var validationResult = new ValidationResult(result);

            return this.Ok(validationResult);
        }
    }
}
