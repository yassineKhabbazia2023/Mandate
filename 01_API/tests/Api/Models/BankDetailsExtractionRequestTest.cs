// <copyright file="BankDetailsExtractionRequestTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.AspNetCore.Tests.Models;

using System.ComponentModel.DataAnnotations;
using KPMG.Pulse.Back.Accounting.Mandate.AspNetCore.Models;
using Newtonsoft.Json;

/// <summary>
/// Unit tests for <see cref="BankDetailsExtractionRequest"/>.
/// </summary>
public sealed class BankDetailsExtractionRequestTest
{
    #region Validation

    /// <summary>
    /// Verifies that both banking identifiers are mandatory.
    /// </summary>
    [Fact]
    public void Validation_WhenIdentifiersAreMissing_ReturnsBothErrors()
    {
        var request = new BankDetailsExtractionRequest();
        var results = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(
            request,
            new ValidationContext(request),
            results,
            validateAllProperties: true);

        isValid.Should().BeFalse();
        results.SelectMany(result => result.MemberNames).Should().BeEquivalentTo(
            nameof(BankDetailsExtractionRequest.Iban),
            nameof(BankDetailsExtractionRequest.Bic));
    }

    #endregion

    #region Serialization

    /// <summary>
    /// Verifies the exact request property names.
    /// </summary>
    [Fact]
    public void Serialization_UsesExpectedJsonPropertyNames()
    {
        var request = new BankDetailsExtractionRequest
        {
            Iban = "FR7630001007941234567890185",
            Bic = "BNPAFRPP"
        };

        var json = JsonConvert.SerializeObject(request);

        json.Should().Be("{\"iban\":\"FR7630001007941234567890185\",\"bic\":\"BNPAFRPP\"}");
    }

    #endregion
}
