// <copyright file="SepaPaymentPreferenceCommandTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Application.Tests.Models;

using KPMG.Pulse.Back.Accounting.Mandate.Application.Models;

/// <summary>
/// Unit tests for <see cref="SepaPaymentPreferenceCommand"/>.
/// </summary>
public sealed class SepaPaymentPreferenceCommandTest
{
    /// <summary>
    /// Verifies the command stores all constructor values.
    /// </summary>
    [Fact]
    public void Constructor_StoresValues()
    {
        var recipient = new SepaRecipient("john.doe@test.fr", "John", "Doe");

        var command = new SepaPaymentPreferenceCommand(
            12,
            "Jean Dupont",
            "10 rue de Paris",
            "Batiment A",
            "Paris",
            "France",
            "75008",
            "FR7630006000011234567890189",
            "AGRIFRPP",
            recipient);

        command.DocumentId.Should().Be(12);
        command.AccountHolder.Should().Be("Jean Dupont");
        command.Address.Should().Be("10 rue de Paris");
        command.AddressLine2.Should().Be("Batiment A");
        command.City.Should().Be("Paris");
        command.Country.Should().Be("France");
        command.PostalCode.Should().Be("75008");
        command.Iban.Should().Be("FR7630006000011234567890189");
        command.Bic.Should().Be("AGRIFRPP");
        command.Recipient.Should().BeSameAs(recipient);
    }
}
