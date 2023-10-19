// <copyright file="SiretTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.JeDeclare.Client.Tests
{
    public class SiretTest
    {
        [Fact]
        public void Serialization()
        {
            var credentials = new Siret
            {
                Siren = "sirenT",
                Nic = "nicT",
            };

            var res = credentials.Serialize<Siret>();
            res.Should().Be(@"<?xml version=""1.0"" encoding=""utf-8""?>
<siretPrincipal xmlns=""http://jedeclare.com/gestion"">
  <siren>sirenT</siren>
  <nic>nicT</nic>
</siretPrincipal>");
        }

        [Fact]
        public void Deserialization()
        {
            var jdcReference = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes""?>
            <siretPrincipal xmlns=""http://jedeclare.com/gestion"">
              <siren>sirenT</siren>
              <nic>nicT</nic>
            </siretPrincipal>";

            var jedeclareSiret = jdcReference.Deserialize<Siret>();

            jedeclareSiret.Should().NotBeNull();
            jedeclareSiret.Siren.Should().Be("sirenT");
            jedeclareSiret.Nic.Should().Be("nicT");
        }
    }
}
