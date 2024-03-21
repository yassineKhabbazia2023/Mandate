// <copyright file="SaveResultTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Client.Tests
{
    public class SaveResultTest
    {
        [Fact]
        public void Constructor()
        {
            var entity = new SaveResult(new PredictableGuid().NewGuid());

            // Make sure we don't forget propeties
            entity.GetType().GetProperties().Length.Should().Be(1);

            // Test all properties ; number of tests below should match the number of propeties above
            entity.Id.Should().Be(Guid.Parse("00000001-0000-0000-0000-000000000000"));
        }

        [Fact]
        public void Serialization()
        {
            var entity = new SaveResult(new PredictableGuid().NewGuid());

            entity.Should().BeJsonSerializableTo(new
            {
                id = Guid.Parse("00000001-0000-0000-0000-000000000000"),
            });
        }
    }
}
