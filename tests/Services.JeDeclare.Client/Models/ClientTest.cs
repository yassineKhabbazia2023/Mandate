// <copyright file="ClientTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.JeDeclare.Client.Tests
{
    public class ClientTest
    {
        [Fact]
        public void Serialization()
        {
            var jeDeclareClient = new Client
            {
                Id = "idT",
                RaisonSocial = "raisonSocialeT",
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

            var res = jeDeclareClient.Serialize<Client>();

            res.Should().Be(@"<?xml version=""1.0"" encoding=""utf-8""?>
<client xmlns=""http://jedeclare.com/gestion"">
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
</client>");
        }

        [Fact]
        public void Deserialization()
        {
            var jdcReference = @"<?xml version=""1.0"" encoding=""utf-8""?>
<client xmlns=""http://jedeclare.com/gestion"">
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
</client>";

            var jedeclareClient = jdcReference.Deserialize<Client>();

            jedeclareClient.Should().NotBeNull();
            jedeclareClient.Id.Should().Be("idT");
            jedeclareClient.Siret.Siren.Should().Be("sirenT");
            jedeclareClient.Siret.Nic.Should().Be("nicT");
            jedeclareClient.RaisonSocial.Should().Be("raisonSocialeT");
            jedeclareClient.Responsable.Name.Should().Be("nameT");
            jedeclareClient.Responsable.Mail.Should().Be("mail.toto@gmail.com");
            jedeclareClient.Responsable.Adresse.Rue.Should().Be("rueT");
            jedeclareClient.Responsable.Adresse.CplRue.Should().Be("cplRueT");
            jedeclareClient.Responsable.Adresse.CodePostal.Should().Be("codePostalT");
            jedeclareClient.Responsable.Adresse.Ville.Should().Be("villeT");
            jedeclareClient.Responsable.Adresse.Pays.Should().Be("paysT");
        }
    }
}
