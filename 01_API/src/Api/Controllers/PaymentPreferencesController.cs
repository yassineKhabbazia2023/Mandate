// <copyright file="PaymentPreferencesController.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.AspNetCore;

using KPMG.Pulse.Back.Accounting.Mandate.Application;
using KPMG.Pulse.Back.Accounting.Mandate.Application.Interfaces;
using KPMG.Pulse.Back.Accounting.Mandate.Application.Models;
using KPMG.Pulse.Back.Accounting.Mandate.AspNetCore.Models;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Exposes prospect onboarding payment preference endpoints.
/// </summary>
[ApiController]
[Route("api/onboarding/{accountId}/payment-preferences")]
public sealed class PaymentPreferencesController(
    IPaymentPreferencesService paymentPreferencesService) : ControllerBase
{
    /// <summary>
    /// Gets the current payment preference for an account.
    /// </summary>
    /// <param name="accountId">The account identifier.</param>
    /// <returns>The current payment preference, or null when none is selected yet.</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PaymentPreferenceResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAsync(int accountId)
    {
        if (accountId <= 0)
        {
            return NotFound();
        }

        var result = await paymentPreferencesService.GetAsync(accountId);
        if (!result.AccountFound)
        {
            return NotFound();
        }

        return Ok(new PaymentPreferenceResponse
        {
            PaymentType = ToContractValue(result.PaymentType)
        });
    }

    /// <summary>
    /// Sets the account payment preference to OTHER.
    /// </summary>
    /// <param name="accountId">The account identifier.</param>
    /// <param name="contactEmail">The authenticated user email forwarded by Gateway.</param>
    /// <returns>204 when saved, 400 when the creator header is missing, or 404 when the account is not found.</returns>
    [HttpPost("other")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SetOtherAsync(
        int accountId,
        [FromHeader(Name = "ContactEmail")] string? contactEmail)
    {
        if (accountId <= 0)
        {
            return NotFound();
        }

        if (string.IsNullOrWhiteSpace(contactEmail))
        {
            ModelState.AddModelError("ContactEmail", "The ContactEmail header is required.");
            return ValidationProblem(ModelState);
        }

        var saved = await paymentPreferencesService.SetOtherAsync(accountId, contactEmail);
        return saved ? NoContent() : NotFound();
    }

    /// <summary>
    /// Generates a SEPA mandate, sends it for signature, and sets the account payment preference to MANDATE_SEPA.
    /// </summary>
    /// <param name="accountId">The account identifier.</param>
    /// <param name="request">The SEPA mandate request.</param>
    /// <returns>200 with the signature URL, 400 when the request is invalid, or 404 when the account is not found.</returns>
    [HttpPost("sepa")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SepaPaymentPreferenceResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SetSepaAsync(
        int accountId,
        [FromBody] SepaPaymentPreferenceRequest request)
    {
        if (accountId <= 0)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        try
        {
            var result = await paymentPreferencesService.SetSepaAsync(
                accountId,
                new SepaPaymentPreferenceCommand(
                    request.DocumentId,
                    request.AccountHolder,
                    request.Address,
                    request.AddressLine2,
                    request.City,
                    request.Country,
                    request.PostalCode,
                    request.Iban,
                    request.Bic,
                    new SepaRecipient(
                        request.RecipientEmail,
                        request.RecipientFirstName,
                        request.RecipientLastName)));

            return result.AccountFound
                ? Ok(new SepaPaymentPreferenceResponse { SignatureUrl = result.SignatureUrl! })
                : NotFound();
        }
        catch (ArgumentException exception)
        {
            ModelState.AddModelError("Iban", exception.Message);
            return ValidationProblem(ModelState);
        }
    }

    /// <summary>
    /// Resets the current account payment preference to an unselected state.
    /// </summary>
    /// <param name="accountId">The account identifier.</param>
    /// <returns>204 when reset, or 404 when the account or payment preference is not found.</returns>
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ResetAsync(int accountId)
    {
        if (accountId <= 0)
        {
            return NotFound();
        }

        var reset = await paymentPreferencesService.ResetAsync(accountId);
        return reset ? NoContent() : NotFound();
    }

    private static string? ToContractValue(PaymentPreferenceType? paymentType)
    {
        return paymentType switch
        {
            PaymentPreferenceType.MandateSepa => "MANDATE_SEPA",
            PaymentPreferenceType.Other => "OTHER",
            _ => null
        };
    }
}
