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
                EntityFactory.Company,
                EntityFactory.Bban,
                new DateTime(2023, 8, 30, 0, 0, 0, DateTimeKind.Utc),
                new DateTime(2023, 8, 30, 0, 0, 0, DateTimeKind.Utc),
                new Status(CollectionStatus.Active, "active"));

            // Make sure we don't forget propeties
            entity.GetType().GetProperties().Length.Should().Be(6);

            // Make sure propeties don't have setters
            entity.GetType().GetProperties().Should().AllSatisfy(p => p.CanWrite.Should().BeFalse());

            // Test all properties ; number of tests below should match the number of propeties above
            entity.Id.Should().Be(Guid.Parse("00000001-0000-0000-0000-000000000000"));
            entity.Company.Should().NotBeNull();
            entity.Bban.Should().NotBeNull();
            entity.CreationDate.Should().Be(new DateTime(2023, 8, 30, 0, 0, 0, DateTimeKind.Utc));
            entity.Status.Should().BeEquivalentTo(new Status(CollectionStatus.Active, "active"));
        }
    }
}
