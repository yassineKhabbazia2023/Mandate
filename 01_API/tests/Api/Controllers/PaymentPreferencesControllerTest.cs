// <copyright file="PaymentPreferencesControllerTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.AspNetCore.Tests.Controllers;

using KPMG.Pulse.Back.Accounting.Mandate.Application;
using KPMG.Pulse.Back.Accounting.Mandate.Application.Interfaces;
using KPMG.Pulse.Back.Accounting.Mandate.Application.Models;
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
    /// Verifies that GET carries signed mandate metadata for Gateway internal orchestration.
    /// </summary>
    [Fact]
    public async Task GetAsync_WhenSignedMandateContentExists_ReturnsInternalSignedMandateFields()
    {
        var service = new Mock<IPaymentPreferencesService>();
        service
            .Setup(candidate => candidate.GetAsync(42))
            .ReturnsAsync(new PaymentPreferenceResult(
                true,
                PaymentPreferenceType.MandateSepa,
                42,
                123,
                 [1, 2, 3],
                 "application/pdf",
                 "signed.pdf",
                 null,
                 "FR7630006000011234567890189",
                 "AGRIFRPP"));
        var controller = new PaymentPreferencesController(service.Object);

        var result = await controller.GetAsync(42);

        var response = result.Should().BeOfType<OkObjectResult>().Subject.Value.Should().BeOfType<PaymentPreferenceResponse>().Subject;
        response.PaymentType.Should().Be("MANDATE_SEPA");
        response.AccountId.Should().Be(42);
        response.RibDocumentId.Should().Be(123);
        response.SignedMandatePdfBase64.Should().Be(Convert.ToBase64String([1, 2, 3]));
        response.SignedMandateContentType.Should().Be("application/pdf");
        response.SignedMandateFileName.Should().Be("signed.pdf");
        response.Iban.Should().Be("FR7630006000011234567890189");
        response.Bic.Should().Be("AGRIFRPP");
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
    /// Verifies that POST SEPA returns the generated signature URL.
    /// </summary>
    [Fact]
    public async Task SetSepaAsync_WhenGenerated_ReturnsSignatureUrl()
    {
        var request = CreateSepaRequest();
        var service = new Mock<IPaymentPreferencesService>();
        service
            .Setup(candidate => candidate.SetSepaAsync(
                42,
                It.Is<SepaPaymentPreferenceCommand>(command =>
                    command.DocumentId == request.DocumentId
                    && command.AccountHolder == request.AccountHolder
                    && command.Address == request.Address
                    && command.AddressLine2 == request.AddressLine2
                    && command.City == request.City
                    && command.Country == request.Country
                    && command.PostalCode == request.PostalCode
                    && command.Iban == request.Iban
                    && command.Bic == request.Bic
                    && command.Recipient.Email == request.RecipientEmail
                    && command.Recipient.FirstName == request.RecipientFirstName
                    && command.Recipient.LastName == request.RecipientLastName)))
            .ReturnsAsync(new SepaPaymentPreferenceResult(true, "https://signature.test"));
        var controller = new PaymentPreferencesController(service.Object);

        var result = await controller.SetSepaAsync(42, request);

        var ok = result.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().BeOfType<SepaPaymentPreferenceResponse>()
            .Which.SignatureUrl.Should().Be("https://signature.test");
    }

    /// <summary>
    /// Verifies that POST SEPA returns not found when the account is missing.
    /// </summary>
    [Fact]
    public async Task SetSepaAsync_WhenAccountIsMissing_ReturnsNotFound()
    {
        var service = new Mock<IPaymentPreferencesService>();
        service
            .Setup(candidate => candidate.SetSepaAsync(42, It.IsAny<SepaPaymentPreferenceCommand>()))
            .ReturnsAsync(new SepaPaymentPreferenceResult(false, null));
        var controller = new PaymentPreferencesController(service.Object);

        var result = await controller.SetSepaAsync(42, CreateSepaRequest());

        result.Should().BeOfType<NotFoundResult>();
    }

    /// <summary>
    /// Verifies that POST SEPA rejects invalid account identifiers before calling the service.
    /// </summary>
    [Fact]
    public async Task SetSepaAsync_WhenAccountIdIsInvalid_ReturnsNotFound()
    {
        var service = new Mock<IPaymentPreferencesService>();
        var controller = new PaymentPreferencesController(service.Object);

        var result = await controller.SetSepaAsync(0, CreateSepaRequest());

        result.Should().BeOfType<NotFoundResult>();
        service.Verify(
            candidate => candidate.SetSepaAsync(
                It.IsAny<int>(),
                It.IsAny<SepaPaymentPreferenceCommand>()),
            Times.Never);
    }

    /// <summary>
    /// Verifies that POST SEPA exposes IBAN validation failures as validation problems.
    /// </summary>
    [Fact]
    public async Task SetSepaAsync_WhenServiceRejectsIban_ReturnsValidationProblem()
    {
        var service = new Mock<IPaymentPreferencesService>();
        service
            .Setup(candidate => candidate.SetSepaAsync(42, It.IsAny<SepaPaymentPreferenceCommand>()))
            .ThrowsAsync(new ArgumentException("A valid French IBAN is required.", "iban"));
        var controller = new PaymentPreferencesController(service.Object);

        var result = await controller.SetSepaAsync(42, CreateSepaRequest());

        result.Should().BeOfType<ObjectResult>().Which.Value.Should().BeOfType<ValidationProblemDetails>();
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

    /// <summary>
    /// Verifies that mark-sent-to-akuiteo returns no content when the mandate is updated.
    /// </summary>
    [Fact]
    public async Task MarkSentToAkuiteoAsync_WhenMarked_ReturnsNoContent()
    {
        var service = new Mock<IPaymentPreferencesService>();
        service.Setup(candidate => candidate.MarkSentToAkuiteoAsync(42)).ReturnsAsync(true);
        var controller = new PaymentPreferencesController(service.Object);

        var result = await controller.MarkSentToAkuiteoAsync(42);

        result.Should().BeOfType<NoContentResult>();
    }

    /// <summary>
    /// Verifies that mark-sent-to-akuiteo returns not found when no mandate is updated.
    /// </summary>
    [Fact]
    public async Task MarkSentToAkuiteoAsync_WhenNotMarked_ReturnsNotFound()
    {
        var service = new Mock<IPaymentPreferencesService>();
        service.Setup(candidate => candidate.MarkSentToAkuiteoAsync(42)).ReturnsAsync(false);
        var controller = new PaymentPreferencesController(service.Object);

        var result = await controller.MarkSentToAkuiteoAsync(42);

        result.Should().BeOfType<NotFoundResult>();
    }

    /// <summary>
    /// Verifies that saving the signed mandate document identifier returns no content when the mandate is updated.
    /// </summary>
    [Fact]
    public async Task SaveSignedMandateDocumentIdAsync_WhenSaved_ReturnsNoContent()
    {
        var service = new Mock<IPaymentPreferencesService>();
        service.Setup(candidate => candidate.SaveSignedMandateDocumentIdAsync(42, "456")).ReturnsAsync(true);
        var controller = new PaymentPreferencesController(service.Object);

        var result = await controller.SaveSignedMandateDocumentIdAsync(42, "456");

        result.Should().BeOfType<NoContentResult>();
    }

    /// <summary>
    /// Verifies that saving the signed mandate document identifier returns not found when no mandate is updated.
    /// </summary>
    [Fact]
    public async Task SaveSignedMandateDocumentIdAsync_WhenNotSaved_ReturnsNotFound()
    {
        var service = new Mock<IPaymentPreferencesService>();
        service.Setup(candidate => candidate.SaveSignedMandateDocumentIdAsync(42, "456")).ReturnsAsync(false);
        var controller = new PaymentPreferencesController(service.Object);

        var result = await controller.SaveSignedMandateDocumentIdAsync(42, "456");

        result.Should().BeOfType<NotFoundResult>();
    }

    /// <summary>
    /// Verifies that the signed mandate document identifier is returned when available.
    /// </summary>
    [Fact]
    public async Task GetSignedMandateDocumentIdAsync_WhenAvailable_ReturnsDocumentId()
    {
        var service = new Mock<IPaymentPreferencesService>();
        service.Setup(candidate => candidate.GetSignedMandateDocumentIdAsync(42)).ReturnsAsync("456");
        var controller = new PaymentPreferencesController(service.Object);

        var result = await controller.GetSignedMandateDocumentIdAsync(42);

        var ok = result.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().Be("456");
    }

    /// <summary>
    /// Verifies that the signed mandate document identifier endpoint returns not found when unavailable.
    /// </summary>
    [Fact]
    public async Task GetSignedMandateDocumentIdAsync_WhenUnavailable_ReturnsNotFound()
    {
        var service = new Mock<IPaymentPreferencesService>();
        service.Setup(candidate => candidate.GetSignedMandateDocumentIdAsync(42)).ReturnsAsync((string?)null);
        var controller = new PaymentPreferencesController(service.Object);

        var result = await controller.GetSignedMandateDocumentIdAsync(42);

        result.Should().BeOfType<NotFoundResult>();
    }

    #region CleanupAsync

    /// <summary>
    /// Verifies that cleanup returns not found when the account identifier is invalid.
    /// </summary>
    [Fact]
    public async Task CleanupAsync_WhenAccountIdIsInvalid_ReturnsNotFound()
    {
        var service = new Mock<IPaymentPreferencesService>();
        var controller = new PaymentPreferencesController(service.Object);

        var result = await controller.CleanupAsync(0);

        result.Should().BeOfType<NotFoundResult>();
        service.Verify(candidate => candidate.CleanupAsync(It.IsAny<int>()), Times.Never);
    }

    /// <summary>
    /// Verifies that cleanup returns no content when Mandate cleanup succeeds.
    /// </summary>
    [Fact]
    public async Task CleanupAsync_WhenCleaned_ReturnsNoContent()
    {
        var service = new Mock<IPaymentPreferencesService>();
        service.Setup(candidate => candidate.CleanupAsync(42)).ReturnsAsync(true);
        var controller = new PaymentPreferencesController(service.Object);

        var result = await controller.CleanupAsync(42);

        result.Should().BeOfType<NoContentResult>();
    }

    /// <summary>
    /// Verifies that cleanup returns not found when the account is unknown.
    /// </summary>
    [Fact]
    public async Task CleanupAsync_WhenAccountIsUnknown_ReturnsNotFound()
    {
        var service = new Mock<IPaymentPreferencesService>();
        service.Setup(candidate => candidate.CleanupAsync(42)).ReturnsAsync(false);
        var controller = new PaymentPreferencesController(service.Object);

        var result = await controller.CleanupAsync(42);

        result.Should().BeOfType<NotFoundResult>();
    }

    #endregion

    /// <summary>
    /// Creates a valid SEPA request.
    /// </summary>
    /// <returns>The request.</returns>
    private static SepaPaymentPreferenceRequest CreateSepaRequest()
    {
        return new SepaPaymentPreferenceRequest
        {
            DocumentId = 123,
            AccountHolder = "Jean Dupont",
            Address = "10 rue de Paris",
            AddressLine2 = "Batiment A",
            City = "Paris",
            Country = "France",
            PostalCode = "75008",
            Iban = "FR7630006000011234567890189",
            Bic = "AGRIFRPP",
            RecipientEmail = "jean.dupont@test.fr",
            RecipientFirstName = "Jean",
            RecipientLastName = "Dupont"
        };
    }
}
