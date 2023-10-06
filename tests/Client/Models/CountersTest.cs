// <copyright file="CountersTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Client.Tests
{
    public class CountersTest
    {
        [Fact]
        public void Constructor()
        {
            var entity = new Counters(15, 1, 2, 3, 4, 5);

            // Make sure we don't forget propeties
            entity.GetType().GetProperties().Length.Should().Be(6);

            // Make sure propeties don't have setters
            entity.GetType().GetProperties().Should().AllSatisfy(p => p.CanWrite.Should().BeFalse());

            // Test all properties ; number of tests below should match the number of propeties above
            entity.All.Should().Be(15);
            entity.Status10.Should().Be(1);
            entity.Status20.Should().Be(2);
            entity.Status30.Should().Be(3);
            entity.Status40.Should().Be(4);
            entity.Status50.Should().Be(5);
        }

        [Fact]
        public void Serialization()
        {
            var entity = new Counters(15, 1, 2, 3, 4, 5);

            entity.Should().BeJsonSerializableTo(new
            {
                all = 15,
                status10 = 1,
                status20 = 2,
                status30 = 3,
                status40 = 4,
                status50 = 5,
            });
        }
    }
}
