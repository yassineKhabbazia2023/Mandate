// <copyright file="PaymentPreferencesControllerTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.AspNetCore.Tests.Controllers;

using KPMG.Pulse.Back.Accounting.Mandate.Application;
using KPMG.Pulse.Back.Accounting.Mandate.Application.Interfaces;
using KPMG.Pulse.Back.Accounting.Mandate.AspNetCore.Models;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Unit tests for <see cref="PaymentPreferencesController"/>.
/// </summary>
public sealed class PaymentPreferencesControllerTest
{
    /// <summary>
    /// Verifies that GET returns not found when the account identifier is invalid.
    /// </summary>
    [Fact]
    public async Task GetAsync_WhenAccountIdIsInvalid_ReturnsNotFound()
    {
        var service = new Mock<IPaymentPreferencesService>();
        var controller = new PaymentPreferencesController(service.Object);

        var result = await controller.GetAsync(0);

        result.Should().BeOfType<NotFoundResult>();
        service.Verify(candidate => candidate.GetAsync(It.IsAny<int>()), Times.Never);
    }

    /// <summary>
    /// Verifies that GET returns not found when the account does not exist.
    /// </summary>
    [Fact]
    public async Task GetAsync_WhenAccountDoesNotExist_ReturnsNotFound()
    {
        var service = new Mock<IPaymentPreferencesService>();
        service.Setup(candidate => candidate.GetAsync(42)).ReturnsAsync(new PaymentPreferenceResult(false, null));
        var controller = new PaymentPreferencesController(service.Object);

        var result = await controller.GetAsync(42);

        result.Should().BeOfType<NotFoundResult>();
    }

    /// <summary>
    /// Verifies that GET returns the OTHER contract value.
    /// </summary>
    [Fact]
    public async Task GetAsync_WhenOtherPreferenceExists_ReturnsOther()
    {
        var service = new Mock<IPaymentPreferencesService>();
        service.Setup(candidate => candidate.GetAsync(42)).ReturnsAsync(new PaymentPreferenceResult(true, PaymentPreferenceType.Other));
        var controller = new PaymentPreferencesController(service.Object);

        var result = await controller.GetAsync(42);

        var response = result.Should().BeOfType<OkObjectResult>().Subject.Value.Should().BeOfType<PaymentPreferenceResponse>().Subject;
        response.PaymentType.Should().Be("OTHER");
    }

    /// <summary>
    /// Verifies that GET returns the SEPA contract value.
    /// </summary>
    [Fact]
    public async Task GetAsync_WhenSepaPreferenceExists_ReturnsMandateSepa()
    {
        var service = new Mock<IPaymentPreferencesService>();
        service.Setup(candidate => candidate.GetAsync(42)).ReturnsAsync(new PaymentPreferenceResult(true, PaymentPreferenceType.MandateSepa));
        var controller = new PaymentPreferencesController(service.Object);

        var result = await controller.GetAsync(42);

        var response = result.Should().BeOfType<OkObjectResult>().Subject.Value.Should().BeOfType<PaymentPreferenceResponse>().Subject;
        response.PaymentType.Should().Be("MANDATE_SEPA");
    }

    /// <summary>
    /// Verifies that GET returns null when no payment preference is selected.
    /// </summary>
    [Fact]
    public async Task GetAsync_WhenPreferenceIsNotSelected_ReturnsNullPaymentType()
    {
        var service = new Mock<IPaymentPreferencesService>();
        service.Setup(candidate => candidate.GetAsync(42)).ReturnsAsync(new PaymentPreferenceResult(true, null));
        var controller = new PaymentPreferencesController(service.Object);

        var result = await controller.GetAsync(42);

        var response = result.Should().BeOfType<OkObjectResult>().Subject.Value.Should().BeOfType<PaymentPreferenceResponse>().Subject;
        response.PaymentType.Should().BeNull();
    }

    /// <summary>
    /// Verifies that POST OTHER returns validation problem when the contact email header is missing.
    /// </summary>
    [Fact]
    public async Task SetOtherAsync_WhenContactEmailIsMissing_ReturnsValidationProblem()
    {
        var service = new Mock<IPaymentPreferencesService>();
        var controller = new PaymentPreferencesController(service.Object);

        var result = await controller.SetOtherAsync(42, " ");

        result.Should().BeOfType<ObjectResult>().Which.Value.Should().BeOfType<ValidationProblemDetails>();
        service.Verify(candidate => candidate.SetOtherAsync(It.IsAny<int>(), It.IsAny<string>()), Times.Never);
    }

    /// <summary>
    /// Verifies that POST OTHER returns no content when the preference is saved.
    /// </summary>
    [Fact]
    public async Task SetOtherAsync_WhenSaved_ReturnsNoContent()
    {
        var service = new Mock<IPaymentPreferencesService>();
        service.Setup(candidate => candidate.SetOtherAsync(42, "user@test.fr")).ReturnsAsync(true);
        var controller = new PaymentPreferencesController(service.Object);

        var result = await controller.SetOtherAsync(42, "user@test.fr");

        result.Should().BeOfType<NoContentResult>();
    }

    /// <summary>
    /// Verifies that POST OTHER returns not found when the account is missing.
    /// </summary>
    [Fact]
    public async Task SetOtherAsync_WhenNotSaved_ReturnsNotFound()
    {
        var service = new Mock<IPaymentPreferencesService>();
        service.Setup(candidate => candidate.SetOtherAsync(42, "user@test.fr")).ReturnsAsync(false);
        var controller = new PaymentPreferencesController(service.Object);

        var result = await controller.SetOtherAsync(42, "user@test.fr");

        result.Should().BeOfType<NotFoundResult>();
    }

    /// <summary>
    /// Verifies that DELETE returns no content when reset succeeds.
    /// </summary>
    [Fact]
    public async Task ResetAsync_WhenReset_ReturnsNoContent()
    {
        var service = new Mock<IPaymentPreferencesService>();
        service.Setup(candidate => candidate.ResetAsync(42)).ReturnsAsync(true);
        var controller = new PaymentPreferencesController(service.Object);

        var result = await controller.ResetAsync(42);

        result.Should().BeOfType<NoContentResult>();
    }

    /// <summary>
    /// Verifies that DELETE returns not found when reset fails.
    /// </summary>
    [Fact]
    public async Task ResetAsync_WhenResetFails_ReturnsNotFound()
    {
        var service = new Mock<IPaymentPreferencesService>();
        service.Setup(candidate => candidate.ResetAsync(42)).ReturnsAsync(false);
        var controller = new PaymentPreferencesController(service.Object);

        var result = await controller.ResetAsync(42);

        result.Should().BeOfType<NotFoundResult>();
    }
}
