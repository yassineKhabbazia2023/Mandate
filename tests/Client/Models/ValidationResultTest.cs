// <copyright file="ValidationResultTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Client.Tests
{
    public class ValidationResultTest
    {
        [Fact]
        public void Constructor()
        {
            var entity = new ValidationResult(true);

            // Make sure we don't forget propeties
            entity.GetType().GetProperties().Length.Should().Be(1);

            // Test all properties ; number of tests below should match the number of propeties above
            entity.Valid.Should().BeTrue();
        }

        [Fact]
        public void Serialization()
        {
            var entity = new ValidationResult(true);

            entity.Should().BeJsonSerializableTo(new
            {
                valid = true,
            });
        }
    }
}
