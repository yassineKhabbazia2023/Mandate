// <copyright file="CarteTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.JeDeclare.Client.Tests
{
    public class CarteTest
    {
        [Fact]
        public void Serialization()
        {
            var carte = new Carte()
            {
                Id = "123456",
                Statut = "2",
                CodeBanque = "codeBanqueT",
                NomConfig = "nomconfigT",
                UserId = "userIdT",
                PartnerId = "partnerIdT",
                EmailResponsable = "email.toto@gmail.com",
                FileFormat = "pdf",
                CarteEBICs = "carteT",
            };

            var res = carte.Serialize<Carte>();

            res.Should().Be(@"<?xml version=""1.0"" encoding=""utf-8""?>
<carte xmlns=""http://jedeclare.com/gestion"">
  <id>123456</id>
  <statut>2</statut>
  <codeBanque>codeBanqueT</codeBanque>
  <nomConfig>nomconfigT</nomConfig>
  <userId>userIdT</userId>
  <partnerId>partnerIdT</partnerId>
  <emailResponsable>email.toto@gmail.com</emailResponsable>
  <fileFormat>pdf</fileFormat>
  <carteEBICs>carteT</carteEBICs>
</carte>");
        }

        [Fact]
        public void Deserialization()
        {
            var jdcReference = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes""?>
            <carte xmlns=""http://jedeclare.com/gestion"">
              <id>123456</id>
              <statut>2</statut>
              <codeBanque>codeBanqueT</codeBanque>
              <nomConfig>nomconfigT</nomConfig>
              <userId>userIdT</userId>
              <partnerId>partnerIdT</partnerId>
              <emailResponsable>email.toto@gmail.com</emailResponsable>
              <fileFormat>pdf</fileFormat>
              <carteEBICs>carteT</carteEBICs>
            </carte>";

            var jedeclareAdresse = jdcReference.Deserialize<Carte>();

            jedeclareAdresse.Should().NotBeNull();
            jedeclareAdresse.Id.Should().Be("123456");
            jedeclareAdresse.Statut.Should().Be("2");
            jedeclareAdresse.CodeBanque.Should().Be("codeBanqueT");
            jedeclareAdresse.NomConfig.Should().Be("nomconfigT");
            jedeclareAdresse.UserId.Should().Be("userIdT");
            jedeclareAdresse.PartnerId.Should().Be("partnerIdT");
            jedeclareAdresse.EmailResponsable.Should().Be("email.toto@gmail.com");
            jedeclareAdresse.FileFormat.Should().Be("pdf");
            jedeclareAdresse.CarteEBICs.Should().Be("carteT");
        }
    }
}
