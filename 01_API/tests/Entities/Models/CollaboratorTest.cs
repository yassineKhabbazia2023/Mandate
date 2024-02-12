// <copyright file="CollaboratorTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Tests
{
    public class CollaboratorTest
    {
        [Fact]
        public void Constructor()
        {
            var entity = new Collaborator(new Guid("00000001-0000-0000-0000-000000000000"), "maroo@email.com", "maroo", "ell");

            // Make sure we don't forget propeties
            entity.GetType().GetProperties().Length.Should().Be(4);

            // Test all properties ; number of tests below should match the number of propeties above
            entity.Id.Should().Be(new Guid("00000001-0000-0000-0000-000000000000"));
            entity.Email.Should().Be("maroo@email.com");
            entity.FirstName.Should().Be("maroo");
            entity.LastName.Should().Be("ell");
        }
    }
}
