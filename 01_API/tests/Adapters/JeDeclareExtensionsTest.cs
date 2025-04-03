// <copyright file="JeDeclareExtensionsTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Adapters.Tests
{
    using KPMG.Pulse.Back.Accounting.Mandate;
    using KPMG.Pulse.Back.Accounting.Mandate.JeDeclare.Client;

    public class JeDeclareExtensionsTest
    {
        [Fact]
        public void ToRibClient()
        {
            Bban bban = new Bban("12345", "54321", "12345678910", "12", null, default);
            Signatory signatory = new Signatory("M", "Maroo", "Elleuch", "email@email.com");
            var entity = new CollectionCreationCommand("12345", signatory, null!, bban);

            var result = entity.ToRibClient();

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(new Rib
            {
                Etablissement = "12345",
                Guichet = "54321",
                NumCompte = "12345678910",
                Cle = "12",
                CiviliteTitulaire = "M",
                PrenomTitulaire = "Maroo",
                NomTitulaire = "Elleuch",
            });
        }

        [Theory]
        [InlineData("m", "", "Doe", "m  Doe")]
        [InlineData("m", null, "Doe", "m  Doe")]
        [InlineData("m", "Jhon", null, "m Jhon ")]
        [InlineData(null, "Jhon", "Doe", " Jhon Doe")]
        [InlineData("M", "J'hon", "Doe's", "M J hon Doe s")]
        public void ToDossierClient_(string? title, string? firstName, string? lastName, string fullName)
        {
            Signatory signatory = new Signatory(title, firstName, lastName, "email@email.com");
            Address? address = new Address("street", "cmp", "zipcode", "city", "country");
            var entity = new Company(
                101,
                "companyName",
                "79887416000046",
                "1000234",
                "12345",
                signatory,
                address);

            var result = entity.ToDossierClient(address, signatory);
            result.Should().BeEquivalentTo(new DossierClient
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
                        Name = fullName,
                        Mail = "email@email.com",
                        Adresse = new Adresse()
                        {
                            CodePostal = "zipcode",
                            Pays = "country",
                            Ville = "city",
                            Rue = "street",
                            CplRue = "cmp",
                        },
                    },
                },
            });
        }

        [Theory]
        [InlineData("m", "Maroo", "Elleuch")]
        [InlineData("MME", "MAROO", "ELLEUCH")]
        public void ToDossierClient(string title, string firstName, string lastName)
        {
            Signatory signatory = new Signatory(title, firstName, lastName, "email@email.com");
            Address? address = new Address("street", "cmp", "zipcode", "city", "country");
            var entity = new Company(
                101,
                "companyName",
                "79887416000046",
                "1000234",
                "12345",
                signatory,
                address);

            var result = entity.ToDossierClient(address, signatory);

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(new DossierClient
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
                        Name = $"{title} {firstName} {lastName}",
                        Mail = "email@email.com",
                        Adresse = new Adresse()
                        {
                            CodePostal = "zipcode",
                            Pays = "country",
                            Ville = "city",
                            Rue = "street",
                            CplRue = "cmp",
                        },
                    },
                },
            });
        }

        [Fact]
        public void ToModelCollection()
        {
            // Arrange
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
                Destinataire = destinataire,
                Rib = rib,
                Periodicite = periodicite,
                DateReprise = "2023-01-01",
            };

            Signatory signatory = new Signatory("M", "Maroo", "Elleuch", "email@email.com");
            Address? address = new Address("street", "cmp", "zipcode", "city", "country");
            var company = new Company(
                101,
                "companyName",
                "79887416000046",
                "1000234",
                "999945",
                signatory,
                address);
            Bban bban = new Bban("12345", "54321", "12345678910", "12", null, default);

            var expectedCollection = new Collection(
                new Guid("00000001-0000-0000-0000-000000000000"),
                "999945",
                company,
                bban,
                DateTime.UtcNow,
                DateTime.UtcNow,
                new Status(CollectionStatus.ToDo, "ToDo", Mandate.JdcCollectionStatus.Creation_InProgress, null),
                null);

            // Act
            var collection = newReleve.ToModel(company, bban, new Guid("00000001-0000-0000-0000-000000000000"), new Status(CollectionStatus.ToDo, "ToDo", Mandate.JdcCollectionStatus.Creation_InProgress, null), null);

            // Assert
            collection.Id.Should().Be(expectedCollection.Id);
            collection.CollectionServicesProviderId.Should().Be(expectedCollection.CollectionServicesProviderId);
            collection.Company.Should().BeEquivalentTo(expectedCollection.Company);
            collection.Bban.Should().BeEquivalentTo(expectedCollection.Bban);
            collection.Status.Should().BeEquivalentTo(expectedCollection.Status);
        }

        [Fact]
        public void ToModelTechnicalCollection()
        {
            // Arrange
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
                Destinataire = destinataire,
                Rib = rib,
                Periodicite = periodicite,
                DateReprise = "2023-01-01",
            };

            var expectedTechnicalCollection = new TechnicalCollection(
                Guid.Empty,
                "999945",
                "999945",
                new BankDetails(
                    "30003",
                    "03558",
                    "00020006536",
                    "41"),
                "2");

            // Act
            var technicalCollection = newReleve.ToModel();

            // Assert
            technicalCollection.Should().BeEquivalentTo(expectedTechnicalCollection);
        }

        [Fact]
        public void ToModelTechnicalCollection_WhenNullAttributes()
        {
            // Arrange
            var destinataire = new Destinataire()
            {
                Id = "829566",
            };

            var periodicite = new Periodicite()
            {
                Id = "1",
            };

            var newReleve = new Releve()
            {
                Id = null,
                Etat = null,
                TypeLiaison = "1",
                Destinataire = destinataire,
                Rib = null,
                Periodicite = periodicite,
                DateReprise = "2023-01-01",
            };

            var expectedTechnicalCollection = new TechnicalCollection(
                Guid.Empty,
                null!,
                null!,
                new BankDetails(
                    null!,
                    null!,
                    null!,
                    null!),
                null!);

            // Act
            var technicalCollection = newReleve.ToModel();

            // Assert
            technicalCollection.Should().BeEquivalentTo(expectedTechnicalCollection);
        }
    }
}