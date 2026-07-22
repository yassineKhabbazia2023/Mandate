// <copyright file="BankDetailsExtractionMapperTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.AspNetCore.Tests.Mappers;

using KPMG.Pulse.Back.Accounting.Mandate.AspNetCore.Mappers;

/// <summary>
/// Unit tests for <see cref="BankDetailsExtractionMapper"/>.
/// </summary>
public sealed class BankDetailsExtractionMapperTest
{
    /// <summary>
    /// Verifies that missing application details are rejected.
    /// </summary>
    [Fact]
    public void ToResponse_WhenDetailsAreNull_ThrowsArgumentNullException()
    {
        var act = () => BankDetailsExtractionMapper.ToResponse(null!);

        act.Should().Throw<ArgumentNullException>();
    }
}
