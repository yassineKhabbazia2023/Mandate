// <copyright file="PeriodiciteTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.JeDeclare.Client.Tests
{
    public class PeriodiciteTest
    {
        [Fact]
        public void Serialization()
        {
            var periodicite = new Periodicite()
            {
                Id = "123456",
            };

            var res = periodicite.Serialize<Periodicite>();

            res.Should().Be(@"<?xml version=""1.0"" encoding=""utf-8""?>
<periodicite xmlns=""http://jedeclare.com/gestion"">
  <id>123456</id>
</periodicite>");
        }

        [Fact]
        public void Deserialization()
        {
            var jdcReference = @"<?xml version=""1.0"" encoding=""utf-8""?>
<periodicite xmlns=""http://jedeclare.com/gestion"">
  <id>123456</id>
</periodicite>";

            var jedeclarePeriodicite = jdcReference.Deserialize<Periodicite>();

            jedeclarePeriodicite.Should().NotBeNull();
            jedeclarePeriodicite.Id.Should().Be("123456");
        }
    }
}
