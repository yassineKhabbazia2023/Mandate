// <copyright file="JeDeclareExtensions.cs" company="KPMG">
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
            var entity = new MandateCreation("12345", signatory, null!, bban);

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

        [Fact]
        public void ToDossierClient()
        {
            Signatory signatory = new Signatory("M", "Maroo", "Elleuch", "email@email.com");
            Address? address = new Address("street", null, "zipcode", "city", "country");
            var entity = new Company(
                new Guid("00000001-0000-0000-0000-000000000000"),
                "companyName",
                "79887416000046",
                "1000234",
                "12345",
                signatory,
                address);

            var result = entity.ToDossierClient();

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
            });
        }

        [Fact]
        public void ConstructReleve()
        {
            var entity = new Rib()
            {
                Etablissement = "12345",
                Guichet = "54321",
                NumCompte = "12345678910",
                Cle = "12",
                CiviliteTitulaire = "M",
                PrenomTitulaire = "Maroo",
                NomTitulaire = "Elleuch",
            };

            var result = entity.ConstructReleve("12345", null, "1;2");

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(new Releve
            {
                Rib = new Rib()
                {
                    Etablissement = "12345",
                    Guichet = "54321",
                    NumCompte = "12345678910",
                    Cle = "12",
                    CiviliteTitulaire = "M",
                    PrenomTitulaire = "Maroo",
                    NomTitulaire = "Elleuch",
                },
                Etat = "2",
                Periodicite = new Periodicite()
                {
                    Id = "1",
                },
            });
        }

        [Fact]
        public void ConstructReleve_WhenCardId()
        {
            var entity = new Rib()
            {
                Etablissement = "12345",
                Guichet = "54321",
                NumCompte = "12345678910",
                Cle = "12",
                CiviliteTitulaire = "M",
                PrenomTitulaire = "Maroo",
                NomTitulaire = "Elleuch",
            };

            var result = entity.ConstructReleve("12345", "cardId", "1;2");

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(new Releve
            {
                Rib = new Rib()
                {
                    Etablissement = "12345",
                    Guichet = "54321",
                    NumCompte = "12345678910",
                    Cle = "12",
                    CiviliteTitulaire = "M",
                    PrenomTitulaire = "Maroo",
                    NomTitulaire = "Elleuch",
                },
                Etat = "2",
                Periodicite = new Periodicite()
                {
                    Id = "1",
                },
                TypeLiaison = "2",
                Card = new Carte()
                {
                    Id = "cardId",
                },
            });
        }

        [Theory]
        [InlineData("12345")]
        [InlineData("12346")]
        public void ConstructReleve_WhenHistoryDateEnabledBanks(string bankCode)
        {
            var entity = new Rib()
            {
                Etablissement = bankCode,
                Guichet = "54321",
                NumCompte = "12345678910",
                Cle = "12",
                CiviliteTitulaire = "M",
                PrenomTitulaire = "Maroo",
                NomTitulaire = "Elleuch",
            };

            var result = entity.ConstructReleve(bankCode, string.Empty, "12345;12346");

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(new Releve
            {
                Rib = new Rib()
                {
                    Etablissement = bankCode,
                    Guichet = "54321",
                    NumCompte = "12345678910",
                    Cle = "12",
                    CiviliteTitulaire = "M",
                    PrenomTitulaire = "Maroo",
                    NomTitulaire = "Elleuch",
                },
                Etat = "2",
                Periodicite = new Periodicite()
                {
                    Id = "1",
                },
                DateReprise = "2023-01-01",
            });
        }
    }
}