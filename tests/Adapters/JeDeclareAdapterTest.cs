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

        [Theory]
        [InlineData("m Maroo ELLEUCH", "m", "Maroo", "ELLEUCH")]
        [InlineData("mme Anne DE OLIVEIRA LAPIZE DE SALLEE", "mme", "Anne", "DE OLIVEIRA LAPIZE DE SALLEE")]
        [InlineData("m Jean Claude GARAUDET", "m", "Jean Claude", "GARAUDET")]
        public async Task CreateFolderAsync(string nom, string title, string firstName, string lastName)
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
                        Name = nom,
                        Mail = "email@email.com",
                        Adresse = new Adresse()
                        {
                            CodePostal = "zipcode",
                            Pays = "country",
                            Ville = "city",
                            Rue = "street",
                            CplRue = "streetT",
                        },
                    },
                },
            };

            var jedeclareClient = new Mock<IJeDeclareClient>(MockBehavior.Strict);
            jedeclareClient.Setup(c => c.CreateFolderAsync(It.IsAny<DossierClient>()))
                .Callback<DossierClient>(dc =>
                {
                    dc.Client.Id.Should().Be("BSP1234");
                    dc.Client.RaisonSociale.Should().Be("Example Company");
                    dc.Client.Siret.Siren.Should().Be("123456789");
                    dc.Client.Siret.Nic.Should().Be("01234");
                    dc.Client.Responsable.Adresse.CodePostal.Should().Be("12345");
                    dc.Client.Responsable.Adresse.CplRue.Should().Be("123 Main St");
                    dc.Client.Responsable.Adresse.Pays.Should().Be("USA");
                    dc.Client.Responsable.Adresse.Rue.Should().Be("123 Main St");
                    dc.Client.Responsable.Adresse.Ville.Should().Be("New York");
                    dc.Client.Responsable.Mail.Should().Be("john.doe@example.com");
                    dc.Client.Responsable.Name.Should().Be("John Doe");
                })
                .ReturnsAsync(dossierClient)
                .Verifiable();

            var adapter = new JeDeclareAdapter(jedeclareClient.Object);

            var createdFolder = await adapter.CreateFolderAsync(company);

            var expectedSignatory = new Signatory(
                title: title,
                firstName: firstName,
                lastName: lastName,
                email: "email@email.com");

            var expectedAdress = new Address(
                street: "street",
                complements: "streetT",
                zipCode: "zipcode",
                city: "city",
                country: "country");

            var expectedCompany = new Company(
                id: Guid.Empty,
                name: "companyName",
                siretNumber: "79887416000046",
                erpId: null,
                bankServicesProviderId: "12345",
                signatory: expectedSignatory,
                address: expectedAdress);

            createdFolder.Should().BeEquivalentTo(expectedCompany);
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

        [Fact]
        public async Task CreateCollecteConfigurationAsync()
        {
            var bankServicesProviderId = "bankServicesProviderIdT";
            var collectionId = Guid.Empty;
            var signatory = new Signatory("Mr.", "John", "Doe", "john.doe@example.com");

            var rib = new Rib()
            {
                Etablissement = "12345",
                Guichet = "56789",
                Cle = "88",
                CiviliteTitulaire = "Mr.",
                PrenomTitulaire = "John",
                NomTitulaire = "Doe",
            };

            var releve = new Releve()
            {
                Rib = rib,
            };

            var jedeclareClient = new Mock<IJeDeclareClient>(MockBehavior.Strict);
            jedeclareClient.Setup(dc => dc.CreateCollecteConfigurationAsync(It.IsAny<string>(), It.IsAny<Releve>(), It.IsAny<string>(), It.IsAny<string>()))
                .Callback<string, Releve, string, string>((f, r, b, e) =>
                {
                    f.Should().Be(bankServicesProviderId);
                    r.Should().BeEquivalentTo(releve);
                    b.Should().Be("12345");
                    e.Should().Be("ebicsCardIdT");
                })
                .ReturnsAsync(releve)
                .Verifiable();

            Guid id = Guid.NewGuid();
            Company company = new Company(Guid.NewGuid(), "mega", "45207964300014", "1999156874", string.Empty, signatory, null);
            Bank? bank = new Bank("12345", "biap", "biap group", "ebicsCardIdT", null!);
            Bban bban = new Bban("12345", "56789", "12345678901", "88", "6789", bank);
            Status status = new Status(CollectionStatus.ToDo, "todo");

            var expecedCollection = new Collection(
                Guid.Empty,
                releve.Id,
                company,
                bban,
                DateTime.UtcNow,
                DateTime.UtcNow,
                status);

            var adapter = new JeDeclareAdapter(jedeclareClient.Object);
            var result = await adapter.CreateCollecteConfigurationAsync(bankServicesProviderId, bban, company, collectionId, status);

            result.Id.Should().Be(Guid.Empty);
            result.CollectionServicesProviderId.Should().BeEquivalentTo(releve.Id);
            result.Company.Should().BeEquivalentTo(company);
            result.Bban.Should().BeEquivalentTo(bban);
            result.Status.Should().BeEquivalentTo(status);
            result.CreationDate.Year.Should().Be(DateTime.UtcNow.Year);
            result.CreationDate.Month.Should().Be(DateTime.UtcNow.Month);
            result.CreationDate.Day.Should().Be(DateTime.UtcNow.Day);
            result.Status.Should().BeEquivalentTo(status);

            jedeclareClient.VerifyAll();
        }
    }
}
