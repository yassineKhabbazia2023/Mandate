// <copyright file="DossierClientTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.JeDeclare.Client.Tests
{
    public class DossierClientTest
    {
        [Fact]
        public void Serialization()
        {
            var jeDeclareClient = new Client
            {
                Id = "idT",
                RaisonSociale = "raisonSocialeT",
                Siret = new Siret
                {
                    Siren = "sirenT",
                    Nic = "nicT",
                },
                Responsable = new Responsable
                {
                    Name = "nameT",
                    Adresse = new Adresse
                    {
                        Rue = "rueT",
                        CplRue = "cplRueT",
                        CodePostal = "codePostalT",
                        Ville = "villeT",
                        Pays = "paysT",
                    },
                    Mail = "mail.toto@gmail.com",
                },
            };

            var jedeclareFolder = new DossierClient()
            {
                Client = jeDeclareClient,
                ExploitationDonnees = true,
            };

            var res = jedeclareFolder.Serialize<DossierClient>();

            res.Should().Be(@"<?xml version=""1.0"" encoding=""utf-8""?>
<dossierClient xmlns=""http://jedeclare.com/gestion"">
  <client>
    <id>idT</id>
    <siretPrincipal>
      <siren>sirenT</siren>
      <nic>nicT</nic>
    </siretPrincipal>
    <rs>raisonSocialeT</rs>
    <responsable>
      <nom>nameT</nom>
      <mel>mail.toto@gmail.com</mel>
      <adresse>
        <rue>rueT</rue>
        <cplRue>cplRueT</cplRue>
        <codePostal>codePostalT</codePostal>
        <ville>villeT</ville>
        <pays>paysT</pays>
      </adresse>
    </responsable>
  </client>
  <exploitationDonnees>true</exploitationDonnees>
</dossierClient>");
        }

        [Fact]
        public void Deserialization()
        {
            var jdcReference = @"<?xml version=""1.0"" encoding=""utf-8""?>
<dossierClient xmlns=""http://jedeclare.com/gestion"">
  <client>
    <id>idT</id>
    <siretPrincipal>
      <siren>sirenT</siren>
      <nic>nicT</nic>
    </siretPrincipal>
    <rs>raisonSocialeT</rs>
    <responsable>
      <nom>nameT</nom>
      <mel>mail.toto@gmail.com</mel>
      <adresse>
        <rue>rueT</rue>
        <cplRue>cplRueT</cplRue>
        <codePostal>codePostalT</codePostal>
        <ville>villeT</ville>
        <pays>paysT</pays>
      </adresse>
    </responsable>
  </client>
  <exploitationDonnees>true</exploitationDonnees>
</dossierClient>";

            var jedeclareFolder = jdcReference.Deserialize<DossierClient>();

            jedeclareFolder.Should().NotBeNull();
            jedeclareFolder.Client.Id.Should().Be("idT");
            jedeclareFolder.Client.Siret.Siren.Should().Be("sirenT");
            jedeclareFolder.Client.Siret.Nic.Should().Be("nicT");
            jedeclareFolder.Client.RaisonSociale.Should().Be("raisonSocialeT");
            jedeclareFolder.Client.Responsable.Name.Should().Be("nameT");
            jedeclareFolder.Client.Responsable.Mail.Should().Be("mail.toto@gmail.com");
            jedeclareFolder.Client.Responsable.Adresse.Rue.Should().Be("rueT");
            jedeclareFolder.Client.Responsable.Adresse.CplRue.Should().Be("cplRueT");
            jedeclareFolder.Client.Responsable.Adresse.CodePostal.Should().Be("codePostalT");
            jedeclareFolder.Client.Responsable.Adresse.Ville.Should().Be("villeT");
            jedeclareFolder.Client.Responsable.Adresse.Pays.Should().Be("paysT");
            jedeclareFolder.ExploitationDonnees.Should().BeTrue();
        }
    }
}
