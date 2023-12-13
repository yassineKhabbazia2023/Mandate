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

        [Fact]
        public async Task AddRibToFolderAsync()
        {
            var signatory = new Signatory("M", "Ludovic", "TYREL DE POIX", "toto@gmail.com.fr");
            var adress = new Address("1 Rue du Capitaine Floch", string.Empty, "72000", "Le Mans", "FRANCE");
            var bankAgrement = new BankAgreement(JdcPartnership.Partner);

            var bank = new Bank("code", "name", "group", "ebicsCardId", bankAgrement);
            Bban bban = new Bban("code", "02408", "00011269900", "58", "bbanServicesProviderId", bank);

            var bankServicesProviderId = "bankServicesProviderIdT";
            var mandateCreation = new CollectionCreationCommand("1000332927", signatory, adress, bban);

            var ribSaved = new Rib()
            {
                Id = "idT",
                Etablissement = "12345",
                Guichet = "54321",
                NumCompte = "12345678910",
                Cle = "12",
                CiviliteTitulaire = "M",
                PrenomTitulaire = "Maroo",
                NomTitulaire = "Elleuch",
            };

            var jedeclareClient = new Mock<IJeDeclareClient>(MockBehavior.Strict);
            jedeclareClient.Setup(dc => dc.AddRibToFolderAsync(It.IsAny<string>(), It.IsAny<Rib>()))
                .Callback<string, Rib>((s, r) =>
                {
                    s.Should().Be(bankServicesProviderId);
                    r.Etablissement.Should().Be("code");
                    r.Guichet.Should().Be("02408");
                    r.NumCompte.Should().Be("00011269900");
                    r.Cle.Should().Be("58");
                    r.CiviliteTitulaire.Should().Be("M");
                    r.NomTitulaire.Should().Be("TYREL DE POIX");
                    r.PrenomTitulaire.Should().Be("Ludovic");
                })
                .ReturnsAsync(ribSaved)
                .Verifiable();

            var expextedBban = new Bban(
                bankCode: "12345",
                branchCode: "54321",
                accountNumber: "12345678910",
                checkDigits: "12",
                bbanServicesProviderId: "idT",
                bank: bank);

            var adapter = new JeDeclareAdapter(jedeclareClient.Object);
            var result = await adapter.AddRibToFolderAsync(bankServicesProviderId, mandateCreation, bank);

            result.Should().BeEquivalentTo(expextedBban);
            jedeclareClient.VerifyAll();
        }
    }
}
