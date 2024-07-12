// <copyright file="BankController.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.AspNetCore
{
    using KPMG.Pulse.Back.Accounting.Mandate.Adapters;
    using KPMG.Pulse.Back.Accounting.Mandate.Client;
    using KPMG.Pulse.Back.Accounting.Mandate.Sql;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("api/bank")]
    [AllowAnonymous]
    public class BankController : ControllerBase
    {
        private readonly ILogger<BankController> logger;
        private readonly IBbanManager bbanManager;
        private readonly IBankManager bankManager;

        public BankController(ILogger<BankController> logger, IBbanManager bbanManager, IBankManager bankManager)
        {
            this.logger = logger;
            this.bbanManager = bbanManager;
            this.bankManager = bankManager;
        }

        [HttpGet("{bankCode}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BankDetail))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBankDetailsAsync([FromRoute] string bankCode)
        {
            string correlationId = "0"; // TODO
            try
            {
                var result = await this.bankManager.GetByCodeAsync(bankCode).ConfigureAwait(false);
                return this.Ok(result.ToBankDetail());
            }
            catch (Mandate.CustomBankCodeNotFoundException ex)
            {
                return this.NotFound(new Error("BankCodeNotFound", correlationId, ex.Message));
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "MandateAPI - {correlationId} - {functionName}", correlationId, nameof(this.GetBankDetailsAsync));
                throw;
            }
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
