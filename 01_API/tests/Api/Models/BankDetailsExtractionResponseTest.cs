// <copyright file="BankDetailsExtractionResponseTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.AspNetCore.Tests.Models;

using KPMG.Pulse.Back.Accounting.Mandate.AspNetCore.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

/// <summary>
/// Unit tests for <see cref="BankDetailsExtractionResponse"/>.
/// </summary>
public sealed class BankDetailsExtractionResponseTest
{
    /// <summary>
    /// Verifies exact response property names and preservation of a null BIC branch.
    /// </summary>
    [Fact]
    public void Serialization_UsesExpectedContractAndPreservesNullBranch()
    {
        var response = new BankDetailsExtractionResponse
        {
            Iban = new ExtractedIbanResponse
            {
                CountryCode = "FR",
                CheckDigits = "76",
                BankAccountPart = "30001007941234567890185"
            },
            Rib = new ExtractedRibResponse
            {
                BankCode = "30001",
                BranchCode = "00794",
                AccountNumber = "12345678901",
                RibKey = "85"
            },
            Bic = new ExtractedBicResponse
            {
                BankCode = "BNPA",
                CountryCode = "FR",
                LocationCode = "PP",
                BranchCode = null
            },
            Domiciliation = "BNPA"
        };

        var json = JObject.Parse(JsonConvert.SerializeObject(response));

        json.Properties().Select(property => property.Name).Should().Equal("iban", "rib", "bic", "domiciliation");
        json["iban"]!["countryCode"]!.Value<string>().Should().Be("FR");
        json["iban"]!["checkDigits"]!.Value<string>().Should().Be("76");
        json["iban"]!["bankAccountPart"]!.Value<string>().Should().Be("30001007941234567890185");
        json["rib"]!["bankCode"]!.Value<string>().Should().Be("30001");
        json["rib"]!["branchCode"]!.Value<string>().Should().Be("00794");
        json["rib"]!["accountNumber"]!.Value<string>().Should().Be("12345678901");
        json["rib"]!["ribKey"]!.Value<string>().Should().Be("85");
        json["bic"]!["bankCode"]!.Value<string>().Should().Be("BNPA");
        json["bic"]!["countryCode"]!.Value<string>().Should().Be("FR");
        json["bic"]!["locationCode"]!.Value<string>().Should().Be("PP");
        json["bic"]!["branchCode"]!.Type.Should().Be(JTokenType.Null);
        json["domiciliation"]!.Value<string>().Should().Be("BNPA");
    }
}
