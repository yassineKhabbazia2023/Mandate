// <copyright file="CompanyCollaboratorTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Client.Tests;

public class CompanyCollaboratorTest
{
    [Fact]
    public void Constructor()
    {
        var entity = new CompanyCollaborator(1, 2);

        // Make sure we don't forget propeties
        entity.GetType().GetProperties().Length.Should().Be(2);

        // Test all properties ; number of tests below should match the number of propeties above
        entity.AccountId.Should().Be(1);
        entity.ContactId.Should().Be(2);
    }
}
