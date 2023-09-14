// <copyright file="ReleveTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.JeDeclare.Client.Tests
{
    public class ReleveTest
    {
        [Fact]
        public void Serialization()
        {
            var destinataire = new Destinataire()
            {
                Id = "829566",
            };

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

            var periodicite = new Periodicite()
            {
                Id = "1",
            };

            var newReleve = new Releve()
            {
                Id = "999945",
                Etat = "2",
                TypeLiaison = "1",
                CauseRejet = "causeRejetT",
                Destinataire = destinataire,
                Rib = rib,
                Periodicite = periodicite,
                DateReprise = "2023-01-01",
            };

            var res = newReleve.Serialize<Releve>();

            res.Should().Be(@"<?xml version=""1.0"" encoding=""utf-8""?>
<releve xmlns=""http://jedeclare.com/gestion"">
  <id>999945</id>
  <etat>2</etat>
  <typeLiaison>1</typeLiaison>
  <causeRejet>causeRejetT</causeRejet>
  <destinataire>
    <id>829566</id>
  </destinataire>
  <rib>
    <id>999945</id>
    <libelle>libelleM</libelle>
    <civiliteTitulaire>Mme</civiliteTitulaire>
    <nomTitulaire>nomTitulaireT</nomTitulaire>
    <prenomTitulaire>prenomTitulaireM</prenomTitulaire>
    <etablissement>30003</etablissement>
    <guichet>03558</guichet>
    <numCompte>00020006536</numCompte>
    <cle>41</cle>
  </rib>
  <periodicite>
    <id>1</id>
  </periodicite>
  <dateReprise>2023-01-01</dateReprise>
</releve>");
        }

        [Fact]
        public void Deserialization()
        {
            var jdcReference = @"<?xml version=""1.0"" encoding=""utf-8""?>
<releve xmlns=""http://jedeclare.com/gestion"">
  <id>999945</id>
  <etat>2</etat>
  <typeLiaison>1</typeLiaison>
  <causeRejet>causeRejetT</causeRejet>
  <destinataire>
    <id>829566</id>
  </destinataire>
  <rib>
    <id>999945</id>
    <libelle>libelleM</libelle>
    <civiliteTitulaire>Mme</civiliteTitulaire>
    <nomTitulaire>nomTitulaireT</nomTitulaire>
    <prenomTitulaire>prenomTitulaireM</prenomTitulaire>
    <etablissement>30003</etablissement>
    <guichet>03558</guichet>
    <numCompte>00020006536</numCompte>
    <cle>41</cle>
  </rib>
  <periodicite>
    <id>1</id>
  </periodicite>
  <dateReprise>2023-01-01</dateReprise>
</releve>";

            var releve = jdcReference.Deserialize<Releve>();

            releve.Should().NotBeNull();
            releve.Id.Should().Be("999945");
            releve.Etat.Should().Be("2");
            releve.TypeLiaison.Should().Be("1");
            releve.CauseRejet.Should().Be("causeRejetT");
            releve.Destinataire!.Id.Should().Be("829566");
            releve.Rib!.Id.Should().Be("999945");
            releve.Rib!.Libelle.Should().Be("libelleM");
            releve.Rib!.CiviliteTitulaire.Should().Be("Mme");
            releve.Rib!.NomTitulaire.Should().Be("nomTitulaireT");
            releve.Rib!.PrenomTitulaire.Should().Be("prenomTitulaireM");
            releve.Rib!.Etablissement.Should().Be("30003");
            releve.Rib!.Guichet.Should().Be("03558");
            releve.Rib!.NumCompte.Should().Be("00020006536");
            releve.Rib!.Cle.Should().Be("41");
            releve.Periodicite!.Id.Should().Be("1");
            releve.DateReprise!.Should().Be("2023-01-01");
        }
    }
}
