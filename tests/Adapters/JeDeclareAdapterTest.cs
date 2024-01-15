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
            var signatory = new Signatory(title, firstName, lastName, "john.doe@example.com");
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
                            CplRue = "Apt 4B",
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
                    dc.Client.Responsable.Adresse.CplRue.Should().Be("Apt 4B");
                    dc.Client.Responsable.Adresse.Pays.Should().Be("USA");
                    dc.Client.Responsable.Adresse.Rue.Should().Be("123 Main St");
                    dc.Client.Responsable.Adresse.Ville.Should().Be("New York");
                    dc.Client.Responsable.Mail.Should().Be("john.doe@example.com");
                    dc.Client.Responsable.Name.Should().Be(nom);
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
                complements: "Apt 4B",
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
        public async Task GetMandatPdfAsync_when_GetMandatPdfAsync_Throw_JeDeclareApiException()
        {
            var jedeclareClient = new Mock<IJeDeclareClient>(MockBehavior.Strict);
            jedeclareClient.Setup(c => c.GetMandatPdfAsync("jdcFolderIdT", "jdcRibIdT"))
                .ThrowsAsync(new JeDeclareApiException("message"))
                .Verifiable();

            var adapter = new JeDeclareAdapter(jedeclareClient.Object);
            Func<Task> action = async () => await adapter.GetMandatPdfAsync("jdcFolderIdT", "jdcRibIdT");
            await action.Should().ThrowAsync<ServicesProviderException>().WithMessage("message");

            jedeclareClient.VerifyAll();
        }

        [Fact]
        public async Task CreateCollecteConfigurationAsync()
        {
            var bankServicesProviderId = "bankServicesProviderIdT";
            var signatory = new Signatory("Mr.", "John", "Doe", "john.doe@example.com");

            var rib = new Rib()
            {
                Id = "6789",
                Etablissement = "12345",
                Guichet = "56789",
                NumCompte = "12345678901",
                Cle = "88",
                CiviliteTitulaire = "Mr.",
                PrenomTitulaire = "John",
                NomTitulaire = "Doe",
            };

            var releve = new Releve()
            {
                Id = "releveId",
                Rib = rib,
            };

            var jedeclareClient = new Mock<IJeDeclareClient>(MockBehavior.Strict);
            jedeclareClient.Setup(dc => dc.CreateCollecteConfigurationAsync(
                "bankServicesProviderIdT",
                It.Is<Releve>(item => CompareRib(item.Rib!, rib)),
                "12345",
                "ebicsCardIdT"))
                .ReturnsAsync(releve)
                .Verifiable();

            Company company = new Company(Guid.NewGuid(), "mega", "45207964300014", "1999156874", bankServicesProviderId, signatory, null);
            Bank? bank = new Bank("12345", "biap", "biap group", "ebicsCardIdT", new BankAgreement(JdcPartnership.NonPartner));
            Bban bban = new Bban("12345", "56789", "12345678901", "88", "6789", bank);

            var adapter = new JeDeclareAdapter(jedeclareClient.Object);
            var result = await adapter.CreateCollecteConfigurationAsync(company, bban);

            result.Should().Be("releveId");

            jedeclareClient.VerifyAll();
        }

        [Fact]
        public async Task GetMandatPdfAsync_when_GetMandatPdfAsync_Throw_Exception()
        {
            var jedeclareClient = new Mock<IJeDeclareClient>(MockBehavior.Strict);
            jedeclareClient.Setup(c => c.GetMandatPdfAsync("jdcFolderIdT", "jdcRibIdT"))
                .ThrowsAsync(new Exception("message"))
                .Verifiable();

            var adapter = new JeDeclareAdapter(jedeclareClient.Object);
            Func<Task> action = async () => await adapter.GetMandatPdfAsync("jdcFolderIdT", "jdcRibIdT");
            await action.Should().NotThrowAsync<ServicesProviderException>();
            await action.Should().ThrowAsync<Exception>().WithMessage("message");

            jedeclareClient.VerifyAll();
        }

        [Fact]
        public async Task UploadSignedMandate_WithValidInputs_CallsJedeclareClient()
        {
            var collectionId = new PredictableGuid().NewGuid();
            var companyId = new PredictableGuid().NewGuid();

            var company = EntityFactory.Company;
            Bban bban = EntityFactory.Bban;
            Status status = EntityFactory.Status();

            var collection = new Collection(
                collectionId,
                "yourServiceProviderId",
                company,
                bban,
                DateTime.Now,
                DateTime.Now,
                status);
            var mandateFile = new byte[] { 1, 2, 3, 4, 5 };
            var jedeclareClient = new Mock<IJeDeclareClient>(MockBehavior.Strict);

            var bankServicesProviderId = collection.Company?.BankServicesProviderId;
            var bbanServicesProviderId = collection.Bban?.BbanServicesProviderId;

            jedeclareClient.Setup(c => c.UploadSignedMandat(bankServicesProviderId!, bbanServicesProviderId!, It.IsAny<byte[]>()))
                .ReturnsAsync("signedMandateId")
                .Verifiable();

            var adapter = new JeDeclareAdapter(jedeclareClient.Object);
            var result = await adapter.UploadSignedMandate(collection, mandateFile);

            result.Should().BeEquivalentTo("signedMandateId");
            jedeclareClient.Verify(client => client.UploadSignedMandat(bankServicesProviderId!, bbanServicesProviderId!, mandateFile), Times.Once);
        }

        [Fact]
        public async Task UploadSignedMandate_WithValidInputs_ThrowJeDeclareApiException()
        {
            var collectionId = new PredictableGuid().NewGuid();
            var companyId = new PredictableGuid().NewGuid();

            var company = EntityFactory.Company;
            Bban bban = EntityFactory.Bban;
            Status status = EntityFactory.Status();

            var collection = new Collection(
                collectionId,
                "yourServiceProviderId",
                company,
                bban,
                DateTime.Now,
                DateTime.Now,
                status);
            var mandateFile = new byte[] { 1, 2, 3, 4, 5 };
            var jedeclareClient = new Mock<IJeDeclareClient>(MockBehavior.Strict);

            var bankServicesProviderId = collection.Company?.BankServicesProviderId;
            var bbanServicesProviderId = collection.Bban?.BbanServicesProviderId;

            jedeclareClient.Setup(c => c.UploadSignedMandat(bankServicesProviderId!, bbanServicesProviderId!, It.IsAny<byte[]>()))
                  .ThrowsAsync(new JeDeclareApiException("message"))
                  .Verifiable();

            var adapter = new JeDeclareAdapter(jedeclareClient.Object);
            Func<Task> action = async () => await adapter.UploadSignedMandate(collection, mandateFile);
            await action.Should().ThrowAsync<ServicesProviderException>().WithMessage("message");

            jedeclareClient.Verify(client => client.UploadSignedMandat(bankServicesProviderId!, bbanServicesProviderId!, mandateFile), Times.Once);
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
        public async Task GetSignedMandatPdfAsync_ReturnsPdf_WhenSuccessful()
        {
            // Arrange
            var expectedPdf = new byte[] { 1, 2, 3, 4, 5 };

            var jedeclareClient = new Mock<IJeDeclareClient>(MockBehavior.Strict);
            jedeclareClient.Setup(client => client.GetSignedMandatPdfAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(expectedPdf);

            var adapter = new JeDeclareAdapter(jedeclareClient.Object);

            // Act
            var result = await adapter.GetSignedMandatPdfAsync("jdcFolderId", "jdcRibId");

            // Assert
            result.Should().BeEquivalentTo(expectedPdf);
        }

        [Fact]
        public async Task GetSignedMandatPdfAsync_ThrowsServicesProviderException_WhenJeDeclareApiExceptionThrown()
        {
            // Arrange
            var jedeclareClient = new Mock<IJeDeclareClient>(MockBehavior.Strict);
            jedeclareClient.Setup(client => client.GetSignedMandatPdfAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new JeDeclareApiException("Error message"));

            var adapter = new JeDeclareAdapter(jedeclareClient.Object);

            // Act & Assert
            await Assert.ThrowsAsync<ServicesProviderException>(() => adapter.GetSignedMandatPdfAsync("jdcFolderId", "jdcRibId"));
        }

        private static bool CompareRib(Rib rib1, Rib rib2)
        {
            return rib1.Etablissement == rib2.Etablissement &&
                rib1.NumCompte == rib2.NumCompte &&
                rib1.Guichet == rib2.Guichet &&
                rib1.Cle == rib2.Cle &&
                rib1.CiviliteTitulaire == rib2.CiviliteTitulaire &&
                rib1.NomTitulaire == rib2.NomTitulaire &&
                rib1.PrenomTitulaire == rib2.PrenomTitulaire &&
                rib1.Id == rib2.Id &&
                rib1.Libelle == rib2.Libelle;
        }
    }
}
