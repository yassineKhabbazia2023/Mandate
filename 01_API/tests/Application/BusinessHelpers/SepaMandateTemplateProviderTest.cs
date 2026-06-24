// <copyright file="SepaMandateTemplateProviderTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Application.Tests.BusinessHelpers;

using System.Text;
using KPMG.Pulse.Back.Accounting.Mandate.Application.BusinessHelpers;
using PdfSharp.Pdf.IO;

/// <summary>
/// Unit tests for <see cref="SepaMandateTemplateProvider"/>.
/// </summary>
public sealed class SepaMandateTemplateProviderTest
{
    /// <summary>
    /// Verifies that the packaged SEPA mandate template can be loaded and opened as a PDF.
    /// </summary>
    [Fact]
    public async Task GetTemplateAsync_WhenTemplateIsPackaged_ReturnsReadablePdf()
    {
        var provider = new SepaMandateTemplateProvider();

        var template = await provider.GetTemplateAsync();

        template.Should().NotBeEmpty();
        Encoding.ASCII.GetString(template[..4]).Should().Be("%PDF");

        using var stream = new MemoryStream(template);
        using var document = PdfReader.Open(stream, PdfDocumentOpenMode.Import);
        document.PageCount.Should().BeGreaterThan(0);
    }
}
