// <copyright file="SepaMandatePdfGeneratorTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Application.Tests.BusinessHelpers;

using KPMG.Pulse.Back.Accounting.Mandate.Application.BusinessHelpers;
using KPMG.Pulse.Back.Accounting.Mandate.Application.Interfaces;
using KPMG.Pulse.Back.Accounting.Mandate.Application.Models;

/// <summary>
/// Unit tests for <see cref="SepaMandatePdfGenerator"/>.
/// </summary>
public sealed class SepaMandatePdfGeneratorTest
{
    /// <summary>
    /// Verifies that French IBAN components and SEPA fields are applied to the existing PDF replacement pipeline.
    /// </summary>
    [Fact]
    public async Task GenerateAsync_WhenFrenchIbanIsValid_ReplacesTemplateFields()
    {
        Replacement[]? replacements = null;
        var templateProvider = new Mock<ISepaMandateTemplateProvider>();
        var pdfFormFieldFiller = new Mock<IPdfFormFieldFiller>();
        templateProvider
            .Setup(provider => provider.GetTemplateAsync())
            .ReturnsAsync([4, 5, 6]);
        pdfFormFieldFiller
            .Setup(filler => filler.FillFields(
                It.Is<byte[]>(template => template.SequenceEqual(new byte[] { 4, 5, 6 })),
                It.IsAny<Replacement[]>()))
            .Callback<byte[], Replacement[]>((_, captured) => replacements = captured)
            .Returns([1, 2, 3]);
        var generator = new SepaMandatePdfGenerator(templateProvider.Object, pdfFormFieldFiller.Object);

        var result = await generator.GenerateAsync(new SepaMandatePdfData(
            "Cabinet Étoile",
            "1001102412",
            "10 rue de l'Église",
            "Bâtiment A",
            "Lyon",
            "France",
            "fr-75008",
            "FR76 3000 6000 0112 3456 7890 189",
            "AGRIFRPP"));

        result.Should().Equal(1, 2, 3);
        replacements.Should().NotBeNull();
        var capturedReplacements = replacements!;
        capturedReplacements.Should().HaveCount(12);
        capturedReplacements.Single(replacement => replacement.WildCard == "{ACCOUNTNUMBER}").Value.Should().Be("1001102412");
        capturedReplacements.Single(replacement => replacement.WildCard == "{RAISON_SOCIALE}").Value.Should().Be("CABINET ÉTOILE");
        capturedReplacements.Single(replacement => replacement.WildCard == "{ADRESSE}").Value.Should().Be("10 RUE DE L'ÉGLISE");
        capturedReplacements.Single(replacement => replacement.WildCard == "{ADRESSE2}").Value.Should().Be("Bâtiment A");
        capturedReplacements.Single(replacement => replacement.WildCard == "{CP}").Value.Should().Be("FR-75008");
        capturedReplacements.Single(replacement => replacement.WildCard == "{VILLE}").Value.Should().Be("LYON");
        capturedReplacements.Single(replacement => replacement.WildCard == "{PAYS}").Value.Should().Be("FRANCE");
        capturedReplacements.Single(replacement => replacement.WildCard == "{DATE1}").Value.Should().Be(DateTime.Today.ToString("dd/MM/yyyy"));
        capturedReplacements.Single(replacement => replacement.WildCard == "{CODE_BANK}").Value.Should().Be("30006");
        capturedReplacements.Single(replacement => replacement.WildCard == "{COMPTE}").Value.Should().Be("FR76 3000 6000 0112 3456 7890 189");
        capturedReplacements.Single(replacement => replacement.WildCard == "{BIC}").Value.Should().Be("AGRIFRPP");
        capturedReplacements.Single(replacement => replacement.WildCard == "{CLRB}").Value.Should().Be("89");
    }

    /// <summary>
    /// Verifies that non-French IBAN values are rejected before template lookup.
    /// </summary>
    [Fact]
    public async Task GenerateAsync_WhenIbanIsNotFrench_ThrowsAndDoesNotLoadTemplate()
    {
        var templateProvider = new Mock<ISepaMandateTemplateProvider>();
        var pdfFormFieldFiller = new Mock<IPdfFormFieldFiller>();
        var generator = new SepaMandatePdfGenerator(templateProvider.Object, pdfFormFieldFiller.Object);

        var act = () => generator.GenerateAsync(new SepaMandatePdfData(
            "Jean Dupont",
            "1001102412",
            "10 rue de Paris",
            "Batiment A",
            "Paris",
            "France",
            "75008",
            "BE68539007547034",
            "AGRIFRPP"));

        await act.Should().ThrowAsync<ArgumentException>();
        templateProvider.Verify(
            provider => provider.GetTemplateAsync(),
            Times.Never);
        pdfFormFieldFiller.Verify(
            filler => filler.FillFields(It.IsAny<byte[]>(), It.IsAny<Replacement[]>()),
            Times.Never);
    }
}
