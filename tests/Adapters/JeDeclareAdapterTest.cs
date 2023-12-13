// <copyright file="JeDeclareAdapterTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Adapters.Tests
{
    using KPMG.Pulse.Back.Accounting.Mandate.JeDeclare.Client;

    public class JeDeclareAdapterTest
    {
        [Fact]
        public async Task GetMandatPdfAsync()
        {
            byte[] data = { 0, 16, 104, 213 };

            var jedeclareClient = new Mock<IJeDeclareClient>(MockBehavior.Strict);
            jedeclareClient.Setup(c => c.GetMandatPdfAsync("jdcFolderIdT", "jdcRibIdT"))
                .ReturnsAsync(data)
                .Verifiable();

            var adapter = new JeDeclareAdapter(jedeclareClient.Object);
            var result = await adapter.GetMandatPdfAsync("jdcFolderIdT", "jdcRibIdT");

            result.Should().BeEquivalentTo(data);
            jedeclareClient.VerifyAll();
        }

        [Fact]
        public async Task CreateFolderAsync()
        {
            var signatory = new Signatory("Mr.", "John", "Doe", "john.doe@example.com");
            var address = new Address("123 Main St", "Apt 4B", "12345", "New York", "USA");

            var company = new Company(Guid.NewGuid(), "Example Company", "12345678901234", "testErpId", "BSP1234", signatory, address);

            var dossierClient = new DossierClient()
            {
                Client = new Client()
                {
                    Id = "12345",
                    RaisonSociale = "companyName",
                    Siret = new Siret()
                    {
                        Siren = "798874160",
                        Nic = "00046",
                    },
                    Responsable = new Responsable()
                    {
                        Name = $"Maroo Elleuch",
                        Mail = "email@email.com",
                        Adresse = new Adresse()
                        {
                            CodePostal = "zipcode",
                            Pays = "country",
                            Ville = "city",
                            Rue = "street",
                            CplRue = "street",
                        },
                    },
                },
            };

            var jedeclareClient = new Mock<IJeDeclareClient>(MockBehavior.Strict);
            jedeclareClient.Setup(c => c.CreateFolderAsync(It.IsAny<DossierClient>()))
                .Callback<DossierClient>(dc =>
                {
                    var t = 2;
                })
                .ReturnsAsync(dossierClient)
                .Verifiable();

            var adapter = new JeDeclareAdapter(jedeclareClient.Object);

            var companyR = await adapter.CreateFolderAsync(company);
        }
    }
}
