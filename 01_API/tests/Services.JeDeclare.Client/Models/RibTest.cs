// <copyright file="RibTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.JeDeclare.Client.Tests
{
    public class RibTest
    {
        [Fact]
        public void Serialization()
        {
            var rib = new Rib()
            {
                Id = "999945",
                Libelle = "libelleM",
                CiviliteTitulaire = "Mme",
                NomTitulaire = "nomTitulaireT",
                PrenomTitulaire = "prenomTitulaireM",
                Etablissement = "30003",
                Guichet = "03558",
                NumCompte = "00020006536",
                Cle = "41",
            };

            var res = rib.Serialize<Rib>();

            res.Should().Be(@"<?xml version=""1.0"" encoding=""utf-8""?>
<rib xmlns=""http://jedeclare.com/gestion"">
  <id>999945</id>
  <libelle>libelleM</libelle>
  <civiliteTitulaire>Mme</civiliteTitulaire>
  <nomTitulaire>nomTitulaireT</nomTitulaire>
  <prenomTitulaire>prenomTitulaireM</prenomTitulaire>
  <etablissement>30003</etablissement>
  <guichet>03558</guichet>
  <numCompte>00020006536</numCompte>
  <cle>41</cle>
</rib>");
        }

        [Fact]
        public void Deserialization()
        {
            var jdcReference = @"<?xml version=""1.0"" encoding=""utf-8""?>
<rib xmlns=""http://jedeclare.com/gestion"">
  <id>999945</id>
  <libelle>libelleM</libelle>
  <civiliteTitulaire>Mme</civiliteTitulaire>
  <nomTitulaire>nomTitulaireT</nomTitulaire>
  <prenomTitulaire>prenomTitulaireM</prenomTitulaire>
  <etablissement>30003</etablissement>
  <guichet>03558</guichet>
  <numCompte>00020006536</numCompte>
  <cle>41</cle>
</rib>";

            var rib = jdcReference.Deserialize<Rib>();

            rib.Should().NotBeNull();
            rib!.Id.Should().Be("999945");
            rib!.Libelle.Should().Be("libelleM");
            rib!.CiviliteTitulaire.Should().Be("Mme");
            rib!.NomTitulaire.Should().Be("nomTitulaireT");
            rib!.PrenomTitulaire.Should().Be("prenomTitulaireM");
            rib!.Etablissement.Should().Be("30003");
            rib!.Guichet.Should().Be("03558");
            rib!.NumCompte.Should().Be("00020006536");
            rib!.Cle.Should().Be("41");
        }
    }
}
