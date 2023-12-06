// <copyright file="FakeDataController.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.AspNetCore
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("api/fake")]
    [Authorize]
    public class FakeDataController : ControllerBase
    {
        private readonly ILogger<FakeDataController> logger;
        private readonly IFakeDataManager fakeDataManager;

        public FakeDataController(ILogger<FakeDataController> logger, IFakeDataManager fakeDataManager)
        {
            this.logger = logger;
            this.fakeDataManager = fakeDataManager;
        }

        [HttpPut("referential")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> CreateFakeRef()
        {
            string correlationId = "0"; // TODO
            try
            {
                await this.fakeDataManager.CreateFakeRefAsync().ConfigureAwait(false);
                return this.NoContent();
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "MandateAPI - {correlationId} - {functionName}", correlationId, nameof(this.CreateFakeRef));
                throw;
            }
        }

        [HttpDelete("referential")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> DeleteFakeRef()
        {
            string correlationId = "0"; // TODO
            try
            {
                await this.fakeDataManager.DeleteFakeRefAsync().ConfigureAwait(false);
                return this.NoContent();
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "MandateAPI - {correlationId} - {functionName}", correlationId, nameof(this.DeleteFakeRef));
                throw;
            }
        }

        [HttpPut("auth")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> CreateFakeAuth()
        {
            string correlationId = "0"; // TODO
            try
            {
                await this.fakeDataManager.CreateFakeAuthAsync().ConfigureAwait(false);
                return this.NoContent();
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "MandateAPI - {correlationId} - {functionName}", correlationId, nameof(this.CreateFakeAuth));
                throw;
            }
        }

        [HttpDelete("auth")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> DeleteFakeAuth()
        {
            string correlationId = "0"; // TODO
            try
            {
                await this.fakeDataManager.DeleteFakeAuthAsync().ConfigureAwait(false);
                return this.NoContent();
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "MandateAPI - {correlationId} - {functionName}", correlationId, nameof(this.DeleteFakeAuth));
                throw;
            }
        }

        [HttpPost("data")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> AddFakeData()
        {
            string correlationId = "0"; // TODO
            try
            {
                await this.fakeDataManager.AddFakeDataAsync().ConfigureAwait(false);
                return this.NoContent();
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "MandateAPI - {correlationId} - {functionName}", correlationId, nameof(this.AddFakeData));
                throw;
            }
        }

        [HttpDelete("data")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> DeleteFakeData()
        {
            string correlationId = "0"; // TODO
            try
            {
                await this.fakeDataManager.DeleteFakeDataAsync().ConfigureAwait(false);
                return this.NoContent();
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "MandateAPI - {correlationId} - {functionName}", correlationId, nameof(this.DeleteFakeData));
                throw;
            }
        }
    }
}
