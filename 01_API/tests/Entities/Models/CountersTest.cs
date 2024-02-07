// <copyright file="CountersTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Tests.Models
{
    public class CountersTest
    {
        [Fact]
        public void Constructor()
        {
            var entity = new Counters(10, 2, 3, 1, 2, 2);

            // Make sure we don't forget propeties
            entity.GetType().GetProperties().Length.Should().Be(6);

            // Test all properties ; number of tests below should match the number of propeties above
            entity.All.Should().Be(10);
            entity.Status10.Should().Be(2);
            entity.Status20.Should().Be(3);
            entity.Status30.Should().Be(1);
            entity.Status40.Should().Be(2);
            entity.Status50.Should().Be(2);
        }
    }
}