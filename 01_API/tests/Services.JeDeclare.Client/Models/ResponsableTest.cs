// <copyright file="ResponsableTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.JeDeclare.Client.Tests
{
    public class ResponsableTest
    {
        [Fact]
        public void Serialization()
        {
            var jeDeclareResponsable = new Responsable
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
            };

            var res = jeDeclareResponsable.Serialize<Responsable>();

            res.Should().Be(@"<?xml version=""1.0"" encoding=""utf-8""?>
<responsable xmlns=""http://jedeclare.com/gestion"">
  <nom>nameT</nom>
  <mel>mail.toto@gmail.com</mel>
  <adresse>
    <rue>rueT</rue>
    <cplRue>cplRueT</cplRue>
    <codePostal>codePostalT</codePostal>
    <ville>villeT</ville>
    <pays>paysT</pays>
  </adresse>
</responsable>");
        }

        [Fact]
        public void Deserialization()
        {
            var jdcReference = @"<?xml version=""1.0"" encoding=""utf-8""?>
<responsable xmlns=""http://jedeclare.com/gestion"">
  <nom>nameT</nom>
  <mel>mail.toto@gmail.com</mel>
  <adresse>
    <rue>rueT</rue>
    <cplRue>cplRueT</cplRue>
    <codePostal>codePostalT</codePostal>
    <ville>villeT</ville>
    <pays>paysT</pays>
  </adresse>
</responsable>";

            var jedeclareResponsable = jdcReference.Deserialize<Responsable>();

            jedeclareResponsable.Should().NotBeNull();
            jedeclareResponsable.Name.Should().Be("nameT");
            jedeclareResponsable.Mail.Should().Be("mail.toto@gmail.com");
            jedeclareResponsable.Adresse.Rue.Should().Be("rueT");
            jedeclareResponsable.Adresse.CplRue.Should().Be("cplRueT");
            jedeclareResponsable.Adresse.CodePostal.Should().Be("codePostalT");
            jedeclareResponsable.Adresse.Ville.Should().Be("villeT");
            jedeclareResponsable.Adresse.Pays.Should().Be("paysT");
        }
    }
}
