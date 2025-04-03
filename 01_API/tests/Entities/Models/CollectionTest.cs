// <copyright file="CollectionTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Tests
{
    public class CollectionTest
    {
        [Fact]
        public void Constructor()
        {
            var entity = new Collection(
                new PredictableGuid().NewGuid(),
                "12345",
                EntityFactory.Company,
                EntityFactory.Bban,
                new DateTime(2023, 8, 30, 0, 0, 0, DateTimeKind.Utc),
                new DateTime(2023, 8, 30, 0, 0, 0, DateTimeKind.Utc),
                new Status(CollectionStatus.Active, "active", JdcCollectionStatus.Creation_InProgress, null), null);

            // Make sure we don't forget propeties
            entity.GetType().GetProperties().Length.Should().Be(8);

            // Test all properties ; number of tests below should match the number of propeties above
            entity.Id.Should().Be(Guid.Parse("00000001-0000-0000-0000-000000000000"));
            entity.CollectionServicesProviderId.Should().Be("12345");
            entity.Company.Should().NotBeNull();
            entity.Bban.Should().NotBeNull();
            entity.CreationDate.Should().Be(new DateTime(2023, 8, 30, 0, 0, 0, DateTimeKind.Utc));
            entity.Status.Should().BeEquivalentTo(new Status(CollectionStatus.Active, "active", JdcCollectionStatus.Creation_InProgress, null));
        }
    }
}
