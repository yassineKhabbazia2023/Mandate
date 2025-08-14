// <copyright file="CollectionCreationCommandTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Tests
{
    public class CollectionCreationCommandTest
    {
        [Fact]
        public void Constructor()
        {
            var entity = new CollectionCreationCommand(
                "e",
                EntityFactory.Signatory,
                EntityFactory.Address,
                EntityFactory.Bban,
                "testDestination");

            // Make sure we don't forget propeties
            entity.GetType().GetProperties().Length.Should().Be(5);

            // Test all properties ; number of tests below should match the number of propeties above
            entity.ErpId.Should().Be("e");
            entity.Signatory.Should().NotBeNull();
            entity.Address.Should().NotBeNull();
            entity.Bban.Should().NotBeNull();
            entity.DestinationTool.Should().Be("testDestination");
        }
    }
}
