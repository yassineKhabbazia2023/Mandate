// <copyright file="BankDetailsExtractionServiceTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Application.Tests.Managers;

using KPMG.Pulse.Back.Accounting.Mandate.Application.Managers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

/// <summary>
/// Unit tests for <see cref="BankDetailsExtractionService"/>.
/// </summary>
public sealed class BankDetailsExtractionServiceTest
{
    private const string ValidIban = "FR7630001007941234567890185";
    private const string ValidBic = "BNPAFRPP";

    #region Architecture

    /// <summary>
    /// Verifies that the service has no legacy provider or repository dependency.
    /// </summary>
    [Fact]
    public void Constructor_DependsOnlyOnLoggingInfrastructure()
    {
        var dependencies = typeof(BankDetailsExtractionService)
            .GetConstructors()
            .Single()
            .GetParameters()
            .Select(parameter => parameter.ParameterType);

        dependencies.Should().Equal(typeof(ILogger<BankDetailsExtractionService>));
    }

    #endregion

    #region Successful extraction

    /// <summary>
    /// Verifies that valid identifiers produce every expected field.
    /// </summary>
    [Fact]
    public void Extract_WhenBankingIdentifiersAreValid_ReturnsAllDetails()
    {
        var result = CreateService().Extract(ValidIban, ValidBic);

        result.IsSuccess.Should().BeTrue();
        result.ValidationErrors.Should().BeEmpty();
        result.BankDetails.Should().NotBeNull();
        result.BankDetails!.Iban.CountryCode.Should().Be("FR");
        result.BankDetails.Iban.CheckDigits.Should().Be("76");
        result.BankDetails.Iban.BankAccountPart.Should().Be("30001007941234567890185");
        result.BankDetails.Rib.BankCode.Should().Be("30001");
        result.BankDetails.Rib.BranchCode.Should().Be("00794");
        result.BankDetails.Rib.AccountNumber.Should().Be("12345678901");
        result.BankDetails.Rib.RibKey.Should().Be("85");
        result.BankDetails.Bic.BankCode.Should().Be("BNPA");
        result.BankDetails.Bic.CountryCode.Should().Be("FR");
        result.BankDetails.Bic.LocationCode.Should().Be("PP");
        result.BankDetails.Bic.BranchCode.Should().BeNull();
        result.BankDetails.Domiciliation.Should().Be("BNPA");
    }

    /// <summary>
    /// Verifies that lowercase identifiers are normalized and a long BIC exposes its branch.
    /// </summary>
    [Fact]
    public void Extract_WhenIdentifiersAreLowercaseAndBicHasBranch_NormalizesOutput()
    {
        var result = CreateService().Extract(ValidIban.ToLowerInvariant(), "bnpafrppxxx");

        result.IsSuccess.Should().BeTrue();
        result.BankDetails!.Iban.CountryCode.Should().Be("FR");
        result.BankDetails.Bic.BankCode.Should().Be("BNPA");
        result.BankDetails.Bic.BranchCode.Should().Be("XXX");
    }

    #endregion

    #region IBAN validation

    /// <summary>
    /// Verifies that missing IBAN values are rejected.
    /// </summary>
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Extract_WhenIbanIsMissing_ReturnsIbanValidationError(string? iban)
    {
        var result = CreateService().Extract(iban!, ValidBic);

        result.IsSuccess.Should().BeFalse();
        result.BankDetails.Should().BeNull();
        result.ValidationErrors.Should().ContainKey("iban");
    }

    /// <summary>
    /// Verifies that whitespace in an IBAN is rejected.
    /// </summary>
    [Fact]
    public void Extract_WhenIbanContainsWhitespace_ReturnsIbanValidationError()
    {
        var result = CreateService().Extract("FR76 30001007941234567890185", ValidBic);

        result.ValidationErrors.Should().ContainKey("iban");
    }

    /// <summary>
    /// Verifies malformed, non-French, and checksum-invalid IBAN values are rejected.
    /// </summary>
    [Theory]
    [InlineData("DE89370400440532013000")]
    [InlineData("FRXX30001007941234567890185")]
    [InlineData("FR7630001007941234567890186")]
    [InlineData("FR763000100794123456789@185")]
    public void Extract_WhenIbanIsInvalid_ReturnsIbanValidationError(string iban)
    {
        var result = CreateService().Extract(iban, ValidBic);

        result.ValidationErrors.Should().ContainKey("iban");
    }

    #endregion

    #region BIC validation

    /// <summary>
    /// Verifies that missing BIC values are rejected.
    /// </summary>
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Extract_WhenBicIsMissing_ReturnsBicValidationError(string? bic)
    {
        var result = CreateService().Extract(ValidIban, bic!);

        result.IsSuccess.Should().BeFalse();
        result.ValidationErrors.Should().ContainKey("bic");
    }

    /// <summary>
    /// Verifies that whitespace in a BIC is rejected.
    /// </summary>
    [Fact]
    public void Extract_WhenBicContainsWhitespace_ReturnsBicValidationError()
    {
        var result = CreateService().Extract(ValidIban, "BNPA FRPP");

        result.ValidationErrors.Should().ContainKey("bic");
    }

    /// <summary>
    /// Verifies malformed BIC values are rejected.
    /// </summary>
    [Theory]
    [InlineData("BNPAFRP")]
    [InlineData("12PAFRPP")]
    [InlineData("BNPAFRP!")]
    [InlineData("BNPAFRPPXX")]
    public void Extract_WhenBicIsMalformed_ReturnsBicValidationError(string bic)
    {
        var result = CreateService().Extract(ValidIban, bic);

        result.ValidationErrors.Should().ContainKey("bic");
    }

    /// <summary>
    /// Verifies that the BIC and IBAN country codes must match.
    /// </summary>
    [Fact]
    public void Extract_WhenBicCountryDiffersFromIban_ReturnsBicValidationError()
    {
        var result = CreateService().Extract(ValidIban, "DEUTDEFF");

        result.ValidationErrors.Should().ContainKey("bic")
            .WhoseValue.Should().Contain("The BIC country code must match the IBAN country code.");
    }

    #endregion

    /// <summary>
    /// Creates the service under test.
    /// </summary>
    /// <returns>The service.</returns>
    private static BankDetailsExtractionService CreateService()
    {
        return new BankDetailsExtractionService(NullLogger<BankDetailsExtractionService>.Instance);
    }
}
