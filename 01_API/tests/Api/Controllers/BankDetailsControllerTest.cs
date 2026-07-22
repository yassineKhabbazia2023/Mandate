// <copyright file="BankDetailsControllerTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.AspNetCore.Tests.Controllers;

using System.Reflection;
using KPMG.Pulse.Back.Accounting.Mandate.Application.Interfaces;
using KPMG.Pulse.Back.Accounting.Mandate.Application.Models;
using KPMG.Pulse.Back.Accounting.Mandate.AspNetCore.Models;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Unit tests for <see cref="BankDetailsController"/>.
/// </summary>
public sealed class BankDetailsControllerTest
{
    #region Endpoint contract

    /// <summary>
    /// Verifies the controller route, HTTP method, action route, and body binding.
    /// </summary>
    [Fact]
    public void Extract_HasExpectedEndpointContract()
    {
        var controllerRoute = typeof(BankDetailsController).GetCustomAttribute<RouteAttribute>();
        var action = typeof(BankDetailsController).GetMethod(nameof(BankDetailsController.Extract));

        controllerRoute!.Template.Should().Be("api/onboarding/bank-details");
        action.Should().NotBeNull();
        action!.GetCustomAttribute<HttpPostAttribute>()!.Template.Should().Be("extract");
        action!.GetCustomAttribute<ConsumesAttribute>()!.ContentTypes.Should().ContainSingle("application/json");
        action!.GetParameters().Single().GetCustomAttribute<FromBodyAttribute>().Should().NotBeNull();
    }

    /// <summary>
    /// Verifies that the controller depends only on its dedicated application service.
    /// </summary>
    [Fact]
    public void Constructor_DoesNotDependOnLegacyMandateComponents()
    {
        var dependencies = typeof(BankDetailsController)
            .GetConstructors()
            .Single()
            .GetParameters()
            .Select(parameter => parameter.ParameterType);

        dependencies.Should().Equal(typeof(IBankDetailsExtractionService));
    }

    #endregion

    #region Response mapping

    /// <summary>
    /// Verifies that valid identifiers are delegated and mapped to the exact response.
    /// </summary>
    [Fact]
    public void Extract_WhenServiceSucceeds_ReturnsExtractedBankDetails()
    {
        var service = new Mock<IBankDetailsExtractionService>();
        service
            .Setup(candidate => candidate.Extract("FR7630001007941234567890185", "BNPAFRPP"))
            .Returns(CreateSuccessResult());
        var controller = new BankDetailsController(service.Object);
        var request = CreateRequest();

        var result = controller.Extract(request);

        var response = result.Should().BeOfType<OkObjectResult>().Subject.Value
            .Should().BeOfType<BankDetailsExtractionResponse>().Subject;
        response.Iban.CountryCode.Should().Be("FR");
        response.Iban.CheckDigits.Should().Be("76");
        response.Iban.BankAccountPart.Should().Be("30001007941234567890185");
        response.Rib.BankCode.Should().Be("30001");
        response.Rib.BranchCode.Should().Be("00794");
        response.Rib.AccountNumber.Should().Be("12345678901");
        response.Rib.RibKey.Should().Be("85");
        response.Bic.BankCode.Should().Be("BNPA");
        response.Bic.CountryCode.Should().Be("FR");
        response.Bic.LocationCode.Should().Be("PP");
        response.Bic.BranchCode.Should().BeNull();
        response.Domiciliation.Should().Be("BNPA");
        service.Verify(candidate => candidate.Extract(request.Iban, request.Bic), Times.Once);
    }

    /// <summary>
    /// Verifies that service validation errors become a validation problem.
    /// </summary>
    [Fact]
    public void Extract_WhenServiceRejectsIdentifiers_ReturnsValidationProblem()
    {
        var service = new Mock<IBankDetailsExtractionService>();
        service
            .Setup(candidate => candidate.Extract(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(new BankDetailsExtractionResult(
                null,
                new Dictionary<string, string[]> { ["iban"] = ["A valid French IBAN is required."] }));
        var controller = new BankDetailsController(service.Object);

        var result = controller.Extract(CreateRequest());

        var problem = result.Should().BeOfType<ObjectResult>().Subject.Value
            .Should().BeOfType<ValidationProblemDetails>().Subject;
        problem.Errors.Should().ContainKey("iban");
    }

    /// <summary>
    /// Verifies that invalid model state prevents service invocation.
    /// </summary>
    [Fact]
    public void Extract_WhenModelStateIsInvalid_DoesNotCallService()
    {
        var service = new Mock<IBankDetailsExtractionService>();
        var controller = new BankDetailsController(service.Object);
        controller.ModelState.AddModelError("iban", "The IBAN is required.");

        var result = controller.Extract(CreateRequest());

        result.Should().BeOfType<ObjectResult>().Subject.Value
            .Should().BeOfType<ValidationProblemDetails>();
        service.Verify(candidate => candidate.Extract(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    #endregion

    /// <summary>
    /// Creates a valid request.
    /// </summary>
    /// <returns>The request.</returns>
    private static BankDetailsExtractionRequest CreateRequest()
    {
        return new BankDetailsExtractionRequest
        {
            Iban = "FR7630001007941234567890185",
            Bic = "BNPAFRPP"
        };
    }

    /// <summary>
    /// Creates a successful application result.
    /// </summary>
    /// <returns>The result.</returns>
    private static BankDetailsExtractionResult CreateSuccessResult()
    {
        return new BankDetailsExtractionResult(
            new ExtractedBankDetails(
                new ExtractedIbanDetails("FR", "76", "30001007941234567890185"),
                new ExtractedRibDetails("30001", "00794", "12345678901", "85"),
                new ExtractedBicDetails("BNPA", "FR", "PP", null),
                "BNPA"),
            new Dictionary<string, string[]>());
    }
}
