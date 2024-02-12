// <copyright file="AdresseTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.JeDeclare.Client.Tests
{
    public class AdresseTest
    {
        [Fact]
        public void Serialization()
        {
            var credentials = new Adresse
            {
                Rue = "rueT",
                CplRue = "cplRueT",
                CodePostal = "codePostalT",
                Ville = "villeT",
                Pays = "paysT",
            };

            var res = credentials.Serialize<Adresse>();
            res.Should().Be(@"<?xml version=""1.0"" encoding=""utf-8""?>
<adresse xmlns=""http://jedeclare.com/gestion"">
  <rue>rueT</rue>
  <cplRue>cplRueT</cplRue>
  <codePostal>codePostalT</codePostal>
  <ville>villeT</ville>
  <pays>paysT</pays>
</adresse>");
        }

        [Fact]
        public void Serialization_CaseNull()
        {
            Adresse? credentials = null;

            var res = credentials!.Serialize<Adresse>();
            res.Should().BeEmpty();
        }

        [Fact]
        public void Deserialization()
        {
            var jdcReference = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes""?>
            <adresse xmlns=""http://jedeclare.com/gestion"">
                <rue>rueT</rue>
                <cplRue>cplRueT</cplRue>
                <codePostal>codePostalT</codePostal>
                <ville>villeT</ville>
                <pays>paysT</pays>
            </adresse>";

            var jedeclareAdresse = jdcReference.Deserialize<Adresse>();

            jedeclareAdresse.Should().NotBeNull();
            jedeclareAdresse.Rue.Should().Be("rueT");
            jedeclareAdresse.CplRue.Should().Be("cplRueT");
            jedeclareAdresse.CodePostal.Should().Be("codePostalT");
            jedeclareAdresse.Ville.Should().Be("villeT");
            jedeclareAdresse.Pays.Should().Be("paysT");
        }
    }
}
