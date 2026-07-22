// <copyright file="BankDetailsController.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.AspNetCore;

using KPMG.Pulse.Back.Accounting.Mandate.Application.Interfaces;
using KPMG.Pulse.Back.Accounting.Mandate.AspNetCore.Mappers;
using KPMG.Pulse.Back.Accounting.Mandate.AspNetCore.Models;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Exposes stateless onboarding banking-information operations.
/// </summary>
/// <param name="bankDetailsExtractionService">The dedicated banking-details extraction service.</param>
[ApiController]
[Route("api/onboarding/bank-details")]
public sealed class BankDetailsController(
    IBankDetailsExtractionService bankDetailsExtractionService) : ControllerBase
{
    /// <summary>
    /// Extracts IBAN, RIB, and BIC information from valid French banking identifiers.
    /// </summary>
    /// <param name="request">The IBAN and BIC to validate and extract.</param>
    /// <returns>Extracted banking details, or a validation problem when the identifiers are invalid.</returns>
    [HttpPost("extract")]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BankDetailsExtractionResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    public IActionResult Extract([FromBody] BankDetailsExtractionRequest request)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var result = bankDetailsExtractionService.Extract(request.Iban, request.Bic);
        if (!result.IsSuccess)
        {
            foreach (var validationError in result.ValidationErrors)
            {
                foreach (var message in validationError.Value)
                {
                    ModelState.AddModelError(validationError.Key, message);
                }
            }

            return ValidationProblem(ModelState);
        }

        return Ok(BankDetailsExtractionMapper.ToResponse(result.BankDetails!));
    }
}
