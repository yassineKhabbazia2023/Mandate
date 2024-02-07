// <copyright file="DestinataireTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.JeDeclare.Client.Tests
{
    public class DestinataireTest
    {
        [Fact]
        public void Serialization()
        {
            var destinataire = new Destinataire()
            {
                Id = "123456",
            };

            var res = destinataire.Serialize<Destinataire>();

            res.Should().Be(@"<?xml version=""1.0"" encoding=""utf-8""?>
<destinataire xmlns=""http://jedeclare.com/gestion"">
  <id>123456</id>
</destinataire>");
        }

        [Fact]
        public void Deserialization()
        {
            var jdcReference = @"<?xml version=""1.0"" encoding=""utf-8""?>
<destinataire xmlns=""http://jedeclare.com/gestion"">
  <id>123456</id>
</destinataire>";

            var jedeclaredestinataire = jdcReference.Deserialize<Destinataire>();

            jedeclaredestinataire.Should().NotBeNull();
            jedeclaredestinataire.Id.Should().Be("123456");
        }
    }
}
