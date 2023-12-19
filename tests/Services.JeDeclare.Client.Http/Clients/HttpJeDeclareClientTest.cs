// <copyright file="HttpJeDeclareClientTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.JeDeclare.Client.Http.Tests
{
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using Kpmg.Constellation.Net.Http;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Moq.Protected;

    public class HttpJeDeclareClientTest
    {
        [Fact]
        public async Task GetAllConfigurationFromFolderAsync_CaseOK()
        {
            var destinataire = new Destinataire()
            {
                Id = "destinataireIdT",
            };

            var rib = new Rib()
            {
                Id = "1234",
                Libelle = "libelleM",
                CiviliteTitulaire = "Mme",
                NomTitulaire = "nomTitulaireT",
                PrenomTitulaire = "prenomTitulaireM",
                Etablissement = "30003",
                Guichet = "03558",
                NumCompte = "00020006536",
                Cle = "41",
            };

            var card = new Carte()
            {
                Id = "carteIdT",
                Statut = "statutT",
                CodeBanque = "codeBanqueT",
                NomConfig = "nomConfigT",
                UserId = "userIdT",
                PartnerId = "partnerIdT",
                EmailResponsable = "emailResponsableT",
                FileFormat = "formatT",
                CarteEBICs = "carteEbicsT",
            };

            var periodicite = new Periodicite()
            {
                Id = "periodiciteIdT",
            };

            var releve = new Releve()
            {
                Id = "idT",
                Etat = "etatT",
                TypeLiaison = "typeLiaisonT",
                CauseRejet = "causeRejetT",
                Destinataire = destinataire,
                Rib = rib,
                Card = card,
                Periodicite = periodicite,
                DateReprise = "dateT",
            };

            var listeReleves = new ListeReleves()
            {
                Releve = new Releve[] { releve },
            };

            var serializedlisteReleves = listeReleves.Serialize();

            var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(serializedlisteReleves, Encoding.UTF8, "text/xml"),
            };

            var client = new Mock<IHttpClient>(MockBehavior.Strict);
            client.Setup(c => c.GetAsync(It.IsAny<string>()))
                .Callback<string>(url =>
                {
                    url.Should().Be($"compte/19581575/dossierClient/21570139/releve");
                })
                .ReturnsAsync(httpResponseMessage)
                .Verifiable();
            client.Setup(c => c.Dispose())
                .Verifiable();

            var factory = new Mock<IJeDeclareClientFactory>(MockBehavior.Strict);
            factory.Setup(f => f.Create())
                .Returns(client.Object)
                .Verifiable();

            var logger = new Mock<ILogger<HttpJeDeclareClient>>(MockBehavior.Strict);
            logger.Setup(x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsValueType>(),
                It.IsAny<Exception>(),
                (Func<It.IsValueType, Exception?, string>)It.IsAny<object>()));

            var options = Options.Create(new JeDeclareOptions()
            {
                JdcCompteId = "19581575",
                BaseUri = new Uri("http://example.com"),
                Login = "yourLogin",
                Password = "yourPassword",
            });

            var jeDeclareClient = new HttpJeDeclareClient(logger.Object, factory.Object, options);

            var result = await jeDeclareClient.GetAllConfigurationFromFolderAsync("21570139");

            result.Releve!.Count().Should().Be(1);
            result.Releve![0].Id.Should().Be("idT");
            result.Releve![0].Etat.Should().Be("etatT");
            result.Releve![0].TypeLiaison.Should().Be("typeLiaisonT");
            result.Releve![0].CauseRejet.Should().Be("causeRejetT");
            result.Releve![0].DateReprise.Should().Be("dateT");
            result.Releve![0].Destinataire!.Id.Should().Be("destinataireIdT");
            result.Releve![0].Rib!.Id.Should().Be("1234");
            result.Releve![0].Rib!.Libelle.Should().Be("libelleM");
            result.Releve![0].Rib!.CiviliteTitulaire.Should().Be("Mme");
            result.Releve![0].Rib!.NomTitulaire.Should().Be("nomTitulaireT");
            result.Releve![0].Rib!.PrenomTitulaire.Should().Be("prenomTitulaireM");
            result.Releve![0].Rib!.Etablissement.Should().Be("30003");
            result.Releve![0].Rib!.Guichet.Should().Be("03558");
            result.Releve![0].Rib!.NumCompte.Should().Be("00020006536");
            result.Releve![0].Rib!.Cle.Should().Be("41");

            result.Releve![0].Card!.Id.Should().Be("carteIdT");
            result.Releve![0].Card!.Statut.Should().Be("statutT");
            result.Releve![0].Card!.CodeBanque.Should().Be("codeBanqueT");
            result.Releve![0].Card!.NomConfig.Should().Be("nomConfigT");
            result.Releve![0].Card!.UserId.Should().Be("userIdT");
            result.Releve![0].Card!.PartnerId.Should().Be("partnerIdT");
            result.Releve![0].Card!.EmailResponsable.Should().Be("emailResponsableT");
            result.Releve![0].Card!.FileFormat.Should().Be("formatT");
            result.Releve![0].Card!.CarteEBICs.Should().Be("carteEbicsT");

            result.Releve![0].Periodicite!.Id.Should().Be("periodiciteIdT");

            client.VerifyAll();
            factory.VerifyAll();
        }

        [Fact]
        public async Task GetAllConfigurationFromFolderAsync_CaseThrowApiException()
        {
            var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent("error message returned by jeDeclareApi"),
            };

            var client = new Mock<IHttpClient>(MockBehavior.Strict);
            client.Setup(c => c.GetAsync(It.IsAny<string>()))
                .Callback<string>(url =>
                {
                    url.Should().Be($"compte/19581575/dossierClient/21570139/releve");
                })
                .ReturnsAsync(httpResponseMessage)
                .Verifiable();

            client.Setup(c => c.Dispose())
                .Verifiable();

            var factory = new Mock<IJeDeclareClientFactory>(MockBehavior.Strict);
            factory.Setup(f => f.Create())
                .Returns(client.Object)
                .Verifiable();

            var logger = new Mock<ILogger<HttpJeDeclareClient>>(MockBehavior.Strict);
            logger.Setup(x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsValueType>(),
                It.IsAny<Exception>(),
                (Func<It.IsValueType, Exception?, string>)It.IsAny<object>()));

            var options = Options.Create(new JeDeclareOptions()
            {
                JdcCompteId = "19581575",
                BaseUri = new Uri("http://example.com"),
                Login = "yourLogin",
                Password = "yourPassword",
            });

            var jeDeclareClient = new HttpJeDeclareClient(logger.Object, factory.Object, options);

            Func<Task> act = async () => await jeDeclareClient.GetAllConfigurationFromFolderAsync("21570139");

            await act.Should().ThrowExactlyAsync<JeDeclareApiException>()
                .WithMessage("Exception was thrown : status code : BadRequest - Message : 'error message returned by jeDeclareApi'");

            client.VerifyAll();
            factory.VerifyAll();
        }

        [Fact]
        public async Task GetSignedMandatPdfAsync_CaseOK()
        {
            var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);

            var client = new Mock<IHttpClient>(MockBehavior.Strict);
            client.Setup(c => c.GetAsync(It.IsAny<string>()))
                .Callback<string>(url =>
                {
                    url.Should().Be($"compte/19581575/dossierClient/21570139/rib/1234/mandatSigne");
                })
                .ReturnsAsync(httpResponseMessage)
                .Verifiable();
            client.Setup(c => c.Dispose())
                .Verifiable();

            var factory = new Mock<IJeDeclareClientFactory>(MockBehavior.Strict);
            factory.Setup(f => f.Create())
                .Returns(client.Object)
                .Verifiable();

            var logger = new Mock<ILogger<HttpJeDeclareClient>>(MockBehavior.Strict);
            logger.Setup(x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsValueType>(),
                It.IsAny<Exception>(),
                (Func<It.IsValueType, Exception?, string>)It.IsAny<object>()));

            var options = Options.Create(new JeDeclareOptions()
            {
                JdcCompteId = "19581575",
                BaseUri = new Uri("http://example.com"),
                Login = "yourLogin",
                Password = "yourPassword",
            });

            var jeDeclareClient = new HttpJeDeclareClient(logger.Object, factory.Object, options);

            var result = await jeDeclareClient.GetSignedMandatPdfAsync("21570139", "1234");
            result.Should().NotBeNull();

            client.VerifyAll();
            factory.VerifyAll();
        }

        [Fact]
        public async Task GetSignedMandatPdfAsync_CaseThrowApiException()
        {
            var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent("Error message"),
            };

            var client = new Mock<IHttpClient>(MockBehavior.Strict);
            client.Setup(c => c.GetAsync(It.IsAny<string>()))
                .Callback<string>(url =>
                {
                    url.Should().Be($"compte/19581575/dossierClient/21570139/rib/1234/mandatSigne");
                })
                .ReturnsAsync(httpResponseMessage)
                .Verifiable();
            client.Setup(c => c.Dispose())
                .Verifiable();

            var factory = new Mock<IJeDeclareClientFactory>(MockBehavior.Strict);
            factory.Setup(f => f.Create())
                .Returns(client.Object)
                .Verifiable();

            var logger = new Mock<ILogger<HttpJeDeclareClient>>(MockBehavior.Strict);
            logger.Setup(x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsValueType>(),
                It.IsAny<Exception>(),
                (Func<It.IsValueType, Exception?, string>)It.IsAny<object>()));

            var options = Options.Create(new JeDeclareOptions()
            {
                JdcCompteId = "19581575",
                BaseUri = new Uri("http://example.com"),
                Login = "yourLogin",
                Password = "yourPassword",
            });

            var jeDeclareClient = new HttpJeDeclareClient(logger.Object, factory.Object, options);

            Func<Task> act = async () => await jeDeclareClient.GetSignedMandatPdfAsync("21570139", "1234");

            await act.Should().ThrowExactlyAsync<JeDeclareApiException>()
                .WithMessage("Exception was thrown : status code : BadRequest - Message : 'Error message'");

            client.VerifyAll();
            factory.VerifyAll();
        }

        [Fact]
        public async Task CreateFolderAsync_CaseOK()
        {
            var newFolder = new DossierClient
            {
                Client = new Client
                {
                    Id = "idT",
                    RaisonSociale = "raisonSocialeT",
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
                },
                ExploitationDonnees = true,
            };

            var serializedfoder = newFolder.Serialize();

            var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.Created)
            {
                Content = new StringContent(serializedfoder, Encoding.UTF8, "text/xml"),
            };

            var httpContent = new StringContent(serializedfoder, Encoding.UTF8, "text/xml");

            var client = new Mock<IHttpClient>(MockBehavior.Strict);
            client.Setup(c => c.PostAsync(It.IsAny<string>(), It.IsAny<HttpContent>()))
                .Callback<string, HttpContent>((url, content) =>
                {
                    url.Should().Be($"compte/19581575/dossierClient");
                    content.Should().BeEquivalentTo(httpContent);
                })
                .ReturnsAsync(httpResponseMessage)
                .Verifiable();
            client.Setup(c => c.Dispose())
                .Verifiable();

            var factory = new Mock<IJeDeclareClientFactory>(MockBehavior.Strict);
            factory.Setup(f => f.Create())
                .Returns(client.Object)
                .Verifiable();

            var logger = new Mock<ILogger<HttpJeDeclareClient>>(MockBehavior.Strict);
            logger.Setup(x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsValueType>(),
                It.IsAny<Exception>(),
                (Func<It.IsValueType, Exception?, string>)It.IsAny<object>()));

            var options = Options.Create(new JeDeclareOptions()
            {
                JdcCompteId = "19581575",
                BaseUri = new Uri("http://example.com"),
                Login = "yourLogin",
                Password = "yourPassword",
            });

            var jeDeclareClient = new HttpJeDeclareClient(logger.Object, factory.Object, options);

            var result = await jeDeclareClient.CreateFolderAsync(newFolder);

            result.ExploitationDonnees.Should().BeTrue();
            result.Client.Id.Should().Be("idT");
            result.Client.RaisonSociale.Should().Be("raisonSocialeT");
            result.Client.Responsable.Name.Should().Be("nameT");
            result.Client.Responsable.Mail.Should().Be("mail.toto@gmail.com");
            result.Client.Responsable.Adresse.Pays.Should().Be("paysT");
            result.Client.Responsable.Adresse.Rue.Should().Be("rueT");
            result.Client.Responsable.Adresse.CplRue.Should().Be("cplRueT");
            result.Client.Responsable.Adresse.Ville.Should().Be("villeT");
            result.Client.Responsable.Adresse.CodePostal.Should().Be("codePostalT");
            result.Client.Siret.Siren.Should().Be("sirenT");
            result.Client.Siret.Nic.Should().Be("nicT");

            client.VerifyAll();
            factory.VerifyAll();
        }

        [Fact]
        public async Task CreateFolderAsync_CaseThrowJeDeclareApiException()
        {
            var newFolder = new DossierClient
            {
                Client = new Client
                {
                    Id = "idT",
                    RaisonSociale = "raisonSocialeT",
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
                },
                ExploitationDonnees = true,
            };

            var serializedfoder = newFolder.Serialize();

            var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent("error message returned by jeDeclareApi"),
            };

            var httpContent = new StringContent(serializedfoder, Encoding.UTF8, "text/xml");

            var client = new Mock<IHttpClient>(MockBehavior.Strict);
            client.Setup(c => c.PostAsync(It.IsAny<string>(), It.IsAny<HttpContent>()))
                .Callback<string, HttpContent>((url, content) =>
                {
                    url.Should().Be($"compte/19581575/dossierClient");
                    content.Should().BeEquivalentTo(httpContent);
                })
                .ReturnsAsync(httpResponseMessage)
                .Verifiable();
            client.Setup(c => c.Dispose())
                .Verifiable();

            var factory = new Mock<IJeDeclareClientFactory>(MockBehavior.Strict);
            factory.Setup(f => f.Create())
                .Returns(client.Object)
                .Verifiable();

            var logger = new Mock<ILogger<HttpJeDeclareClient>>(MockBehavior.Strict);
            logger.Setup(x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsValueType>(),
                It.IsAny<Exception>(),
                (Func<It.IsValueType, Exception?, string>)It.IsAny<object>()));

            var options = Options.Create(new JeDeclareOptions()
            {
                JdcCompteId = "19581575",
                BaseUri = new Uri("http://example.com"),
                Login = "yourLogin",
                Password = "yourPassword",
            });

            var jeDeclareClient = new HttpJeDeclareClient(logger.Object, factory.Object, options);

            Func<Task> act = async () => await jeDeclareClient.CreateFolderAsync(newFolder);

            await act.Should().ThrowExactlyAsync<JeDeclareApiException>()
                .WithMessage("Exception was thrown : status code : BadRequest - Message : 'error message returned by jeDeclareApi'");

            client.VerifyAll();
            factory.VerifyAll();
        }

        [Fact]
        public async Task AddRibToFolderAsync_CaseOK()
        {
            var newRib = new Rib()
            {
                Id = "1234",
                Libelle = "libelleM",
                CiviliteTitulaire = "Mme",
                NomTitulaire = "nomTitulaireT",
                PrenomTitulaire = "prenomTitulaireM",
                Etablissement = "30003",
                Guichet = "03558",
                NumCompte = "00020006536",
                Cle = "41",
            };

            var serializedRib = newRib.Serialize();

            var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.Created)
            {
                Content = new StringContent(serializedRib, Encoding.UTF8, "text/xml"),
            };

            var httpContent = new StringContent(serializedRib, Encoding.UTF8, "text/xml");

            var client = new Mock<IHttpClient>(MockBehavior.Strict);
            client.Setup(c => c.PostAsync(It.IsAny<string>(), It.IsAny<HttpContent>()))
                .Callback<string, HttpContent>((url, content) =>
                {
                    url.Should().Be($"compte/19581575/dossierClient/98765/rib");
                    content.Should().BeEquivalentTo(httpContent);
                })
                .ReturnsAsync(httpResponseMessage)
                .Verifiable();
            client.Setup(c => c.Dispose())
                .Verifiable();

            var factory = new Mock<IJeDeclareClientFactory>(MockBehavior.Strict);
            factory.Setup(f => f.Create())
                .Returns(client.Object)
                .Verifiable();

            var logger = new Mock<ILogger<HttpJeDeclareClient>>(MockBehavior.Strict);
            logger.Setup(x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsValueType>(),
                It.IsAny<Exception>(),
                (Func<It.IsValueType, Exception?, string>)It.IsAny<object>()));

            var options = Options.Create(new JeDeclareOptions()
            {
                JdcCompteId = "19581575",
                BaseUri = new Uri("http://example.com"),
                Login = "yourLogin",
                Password = "yourPassword",
            });

            var jeDeclareClient = new HttpJeDeclareClient(logger.Object, factory.Object, options);

            var result = await jeDeclareClient.AddRibToFolderAsync("98765", newRib);

            result.Id.Should().Be("1234");
            result.Libelle.Should().Be("libelleM");
            result.CiviliteTitulaire.Should().Be("Mme");
            result.NomTitulaire.Should().Be("nomTitulaireT");
            result.PrenomTitulaire.Should().Be("prenomTitulaireM");
            result.Etablissement.Should().Be("30003");
            result.Guichet.Should().Be("03558");
            result.NumCompte.Should().Be("00020006536");
            result.Cle.Should().Be("41");

            client.VerifyAll();
            factory.VerifyAll();
        }

        [Fact]
        public async Task AddRibToFolderAsync_CaseThrowJeDeclareApiException()
        {
            var newRib = new Rib()
            {
                Id = "1234",
                Libelle = "libelleM",
                CiviliteTitulaire = "Mme",
                NomTitulaire = "nomTitulaireT",
                PrenomTitulaire = "prenomTitulaireM",
                Etablissement = "30003",
                Guichet = "03558",
                NumCompte = "00020006536",
                Cle = "41",
            };

            var serializedRib = newRib.Serialize();

            var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent("Error Message"),
            };

            var httpContent = new StringContent(serializedRib, Encoding.UTF8, "text/xml");

            var client = new Mock<IHttpClient>(MockBehavior.Strict);
            client.Setup(c => c.PostAsync(It.IsAny<string>(), It.IsAny<HttpContent>()))
                .Callback<string, HttpContent>((url, content) =>
                {
                    url.Should().Be($"compte/19581575/dossierClient/98765/rib");
                    content.Should().BeEquivalentTo(httpContent);
                })
                .ReturnsAsync(httpResponseMessage)
                .Verifiable();
            client.Setup(c => c.Dispose())
                .Verifiable();

            var factory = new Mock<IJeDeclareClientFactory>(MockBehavior.Strict);
            factory.Setup(f => f.Create())
                .Returns(client.Object)
                .Verifiable();

            var logger = new Mock<ILogger<HttpJeDeclareClient>>(MockBehavior.Strict);
            logger.Setup(x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsValueType>(),
                It.IsAny<Exception>(),
                (Func<It.IsValueType, Exception?, string>)It.IsAny<object>()));

            var options = Options.Create(new JeDeclareOptions()
            {
                JdcCompteId = "19581575",
                BaseUri = new Uri("http://example.com"),
                Login = "yourLogin",
                Password = "yourPassword",
            });

            var jeDeclareClient = new HttpJeDeclareClient(logger.Object, factory.Object, options);

            Func<Task> act = async () => await jeDeclareClient.AddRibToFolderAsync("98765", newRib);

            await act.Should().ThrowExactlyAsync<JeDeclareApiException>()
                .WithMessage("Exception was thrown : status code : BadRequest - Message : 'Error Message'");

            client.VerifyAll();
            factory.VerifyAll();
        }

        [Fact]
        public async Task CreateCollecteConfigurationAsync_CaseOK()
        {
            var bankCode = "bankCodeT";
            var ebicsCarteId = "ebicsCarteIdT";

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

            var card = new Carte()
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

            var periodicite = new Periodicite()
            {
                Id = "1",
            };

            var newReleve = new Releve()
            {
                Id = "999945",
                Etat = "2",
                TypeLiaison = "1",
                CauseRejet = "causeRejetT",
                Destinataire = destinataire,
                Rib = rib,
                Card = card,
                Periodicite = periodicite,
                DateReprise = "2023-01-01",
            };

            var serializedReleve = newReleve.Serialize();

            var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.Created)
            {
                Content = new StringContent(serializedReleve, Encoding.UTF8, "text/xml"),
            };

            var httpContent = new StringContent(serializedReleve, Encoding.UTF8, "text/xml");

            var client = new Mock<IHttpClient>(MockBehavior.Strict);
            client.Setup(c => c.PostAsync(It.IsAny<string>(), It.IsAny<HttpContent>()))
                .Callback<string, HttpContent>((url, content) =>
                {
                    url.Should().Be($"compte/19581575/dossierClient/98765/releve");
                    content.Should().BeEquivalentTo(httpContent);
                })
                .ReturnsAsync(httpResponseMessage)
                .Verifiable();
            client.Setup(c => c.Dispose())
                .Verifiable();

            var factory = new Mock<IJeDeclareClientFactory>(MockBehavior.Strict);
            factory.Setup(f => f.Create())
                .Returns(client.Object)
                .Verifiable();

            var logger = new Mock<ILogger<HttpJeDeclareClient>>(MockBehavior.Strict);
            logger.Setup(x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsValueType>(),
                It.IsAny<Exception>(),
                (Func<It.IsValueType, Exception?, string>)It.IsAny<object>()));

            var options = Options.Create(new JeDeclareOptions()
            {
                JdcCompteId = "19581575",
                BaseUri = new Uri("http://example.com"),
                Login = "yourLogin",
                Password = "yourPassword",
                HistoryDateEnabledBanks = "bankCodeT;",
            });

            var jeDeclareClient = new HttpJeDeclareClient(logger.Object, factory.Object, options);

            var result = await jeDeclareClient.CreateCollecteConfigurationAsync("98765", newReleve, bankCode, ebicsCarteId);

            result.Id.Should().Be("999945");
            result.Etat.Should().Be("2");
            result.TypeLiaison.Should().Be("1");
            result.CauseRejet.Should().Be("causeRejetT");

            result.Destinataire!.Id.Should().Be("829566");

            result.Card!.Id.Should().Be("123456");
            result.Card!.Statut.Should().Be("2");
            result.Card!.CodeBanque.Should().Be("codeBanqueT");
            result.Card!.NomConfig.Should().Be("nomconfigT");
            result.Card!.UserId.Should().Be("userIdT");
            result.Card!.PartnerId.Should().Be("partnerIdT");
            result.Card!.EmailResponsable.Should().Be("email.toto@gmail.com");
            result.Card!.FileFormat.Should().Be("pdf");
            result.Card!.CarteEBICs.Should().Be("carteT");

            result.Rib!.Id.Should().Be("999945");
            result.Rib!.Libelle.Should().Be("libelleM");
            result.Rib!.CiviliteTitulaire.Should().Be("Mme");
            result.Rib.NomTitulaire.Should().Be("nomTitulaireT");
            result.Rib!.PrenomTitulaire.Should().Be("prenomTitulaireM");
            result.Rib!.Etablissement.Should().Be("30003");
            result.Rib!.Guichet.Should().Be("03558");
            result.Rib!.NumCompte.Should().Be("00020006536");
            result.Rib!.Cle.Should().Be("41");

            client.VerifyAll();
            factory.VerifyAll();
        }

        [Fact]
        public async Task CreateCollecteConfigurationAsync_CaseThrowJeDeclareApiException()
        {
            var bankCode = "bankCodeT";
            var ebicsCarteId = "ebicsCarteIdT";

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

            var card = new Carte()
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

            var periodicite = new Periodicite()
            {
                Id = "1",
            };

            var newReleve = new Releve()
            {
                Id = "999945",
                Etat = "2",
                TypeLiaison = "1",
                CauseRejet = "causeRejetT",
                Destinataire = destinataire,
                Rib = rib,
                Card = card,
                Periodicite = periodicite,
                DateReprise = "2023-01-01",
            };

            var serializedReleve = newReleve.Serialize();

            var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent("error message"),
            };

            var httpContent = new StringContent(serializedReleve, Encoding.UTF8, "text/xml");

            var client = new Mock<IHttpClient>(MockBehavior.Strict);
            client.Setup(c => c.PostAsync(It.IsAny<string>(), It.IsAny<HttpContent>()))
                .Callback<string, HttpContent>((url, content) =>
                {
                    url.Should().Be($"compte/19581575/dossierClient/98765/releve");
                    content.Should().BeEquivalentTo(httpContent);
                })
                .ReturnsAsync(httpResponseMessage)
                .Verifiable();
            client.Setup(c => c.Dispose())
                .Verifiable();

            var factory = new Mock<IJeDeclareClientFactory>(MockBehavior.Strict);
            factory.Setup(f => f.Create())
                .Returns(client.Object)
                .Verifiable();

            var logger = new Mock<ILogger<HttpJeDeclareClient>>(MockBehavior.Strict);
            logger.Setup(x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsValueType>(),
                It.IsAny<Exception>(),
                (Func<It.IsValueType, Exception?, string>)It.IsAny<object>()));

            var options = Options.Create(new JeDeclareOptions()
            {
                JdcCompteId = "19581575",
                BaseUri = new Uri("http://example.com"),
                Login = "yourLogin",
                Password = "yourPassword",
                HistoryDateEnabledBanks = "bankCodeT",
            });

            var jeDeclareClient = new HttpJeDeclareClient(logger.Object, factory.Object, options);

            Func<Task> act = async () => await jeDeclareClient.CreateCollecteConfigurationAsync("98765", newReleve, bankCode, ebicsCarteId);

            await act.Should().ThrowExactlyAsync<JeDeclareApiException>()
                .WithMessage("Exception was thrown : status code : BadRequest - Message : 'error message'");

            client.VerifyAll();
            factory.VerifyAll();
        }

        [Fact]
        public async Task UpdateCollecteConfigurationAsync_CaseOK()
        {
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

            var card = new Carte()
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

            var periodicite = new Periodicite()
            {
                Id = "1",
            };

            var newReleve = new Releve()
            {
                Id = "999945",
                Etat = "2",
                TypeLiaison = "1",
                CauseRejet = "causeRejetT",
                Destinataire = destinataire,
                Rib = rib,
                Card = card,
                Periodicite = periodicite,
                DateReprise = "2023-01-01",
            };

            var serializedReleve = newReleve.Serialize();

            var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.Created)
            {
                Content = new StringContent(serializedReleve, Encoding.UTF8, "text/xml"),
            };

            var httpContent = new StringContent(serializedReleve, Encoding.UTF8, "text/xml");

            var client = new Mock<IHttpClient>(MockBehavior.Strict);
            client.Setup(c => c.PutAsync(It.IsAny<string>(), It.IsAny<HttpContent>()))
                .Callback<string, HttpContent>((url, content) =>
                {
                    url.Should().Be($"compte/19581575/dossierClient/98765/releve/999945");
                    content.Should().BeEquivalentTo(httpContent);
                })
                .ReturnsAsync(httpResponseMessage)
                .Verifiable();
            client.Setup(c => c.Dispose())
                .Verifiable();

            var factory = new Mock<IJeDeclareClientFactory>(MockBehavior.Strict);
            factory.Setup(f => f.Create())
                .Returns(client.Object)
                .Verifiable();

            var logger = new Mock<ILogger<HttpJeDeclareClient>>(MockBehavior.Strict);
            logger.Setup(x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsValueType>(),
                It.IsAny<Exception>(),
                (Func<It.IsValueType, Exception?, string>)It.IsAny<object>()));

            var options = Options.Create(new JeDeclareOptions()
            {
                JdcCompteId = "19581575",
                BaseUri = new Uri("http://example.com"),
                Login = "yourLogin",
                Password = "yourPassword",
            });

            var jeDeclareClient = new HttpJeDeclareClient(logger.Object, factory.Object, options);

            var result = await jeDeclareClient.UpdateCollecteConfigurationAsync("98765", newReleve);

            result.Should().BeTrue();

            client.VerifyAll();
            factory.VerifyAll();
        }

        [Fact]
        public async Task UpdateCollecteConfigurationAsync_CaseThrowJeDeclareApiException()
        {
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

            var card = new Carte()
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

            var periodicite = new Periodicite()
            {
                Id = "1",
            };

            var newReleve = new Releve()
            {
                Id = "999945",
                Etat = "2",
                TypeLiaison = "1",
                CauseRejet = "causeRejetT",
                Destinataire = destinataire,
                Rib = rib,
                Card = card,
                Periodicite = periodicite,
                DateReprise = "2023-01-01",
            };

            var serializedReleve = newReleve.Serialize();

            var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent("error message"),
            };

            var httpContent = new StringContent(serializedReleve, Encoding.UTF8, "text/xml");

            var client = new Mock<IHttpClient>(MockBehavior.Strict);
            client.Setup(c => c.PutAsync(It.IsAny<string>(), It.IsAny<HttpContent>()))
                .Callback<string, HttpContent>((url, content) =>
                {
                    url.Should().Be($"compte/19581575/dossierClient/98765/releve/999945");
                    content.Should().BeEquivalentTo(httpContent);
                })
                .ReturnsAsync(httpResponseMessage)
                .Verifiable();
            client.Setup(c => c.Dispose())
                .Verifiable();

            var factory = new Mock<IJeDeclareClientFactory>(MockBehavior.Strict);
            factory.Setup(f => f.Create())
                .Returns(client.Object)
                .Verifiable();

            var logger = new Mock<ILogger<HttpJeDeclareClient>>(MockBehavior.Strict);
            logger.Setup(x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsValueType>(),
                It.IsAny<Exception>(),
                (Func<It.IsValueType, Exception?, string>)It.IsAny<object>()));

            var options = Options.Create(new JeDeclareOptions()
            {
                JdcCompteId = "19581575",
                BaseUri = new Uri("http://example.com"),
                Login = "yourLogin",
                Password = "yourPassword",
            });

            var jeDeclareClient = new HttpJeDeclareClient(logger.Object, factory.Object, options);

            Func<Task> act = async () => await jeDeclareClient.UpdateCollecteConfigurationAsync("98765", newReleve);

            await act.Should().ThrowExactlyAsync<JeDeclareApiException>()
                .WithMessage("Exception was thrown : status code : BadRequest - Message : 'error message'");

            client.VerifyAll();
            factory.VerifyAll();
        }

        [Fact]
        public async Task UploadSignedMandat_CaseOK()
        {
            var mandatString = "test mandat string";
            var mandat = Encoding.UTF8.GetBytes(mandatString);

            var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("response body test"),
            };

            var httpContent = new StringContent("dGVzdCBtYW5kYXQgc3RyaW5n", Encoding.UTF8, "text/plain");

            var client = new Mock<IHttpClient>(MockBehavior.Strict);
            client.Setup(c => c.PostAsync(It.IsAny<string>(), It.IsAny<HttpContent>()))
                .Callback<string, HttpContent>((url, content) =>
                {
                    url.Should().Be($"compte/19581575/dossierClient/98765/rib/999945/mandatSigne");
                    content.Should().BeEquivalentTo(httpContent);
                })
                .ReturnsAsync(httpResponseMessage)
                .Verifiable();
            client.Setup(c => c.Dispose())
                .Verifiable();

            var factory = new Mock<IJeDeclareClientFactory>(MockBehavior.Strict);
            factory.Setup(f => f.Create())
                .Returns(client.Object)
                .Verifiable();

            var logger = new Mock<ILogger<HttpJeDeclareClient>>(MockBehavior.Strict);
            logger.Setup(x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsValueType>(),
                It.IsAny<Exception>(),
                (Func<It.IsValueType, Exception?, string>)It.IsAny<object>()));

            var options = Options.Create(new JeDeclareOptions()
            {
                JdcCompteId = "19581575",
                BaseUri = new Uri("http://example.com"),
                Login = "yourLogin",
                Password = "yourPassword",
            });

            var jeDeclareClient = new HttpJeDeclareClient(logger.Object, factory.Object, options);

            var result = await jeDeclareClient.UploadSignedMandat("98765", "999945", mandat);

            result.Should().Be("response body test");

            client.VerifyAll();
            factory.VerifyAll();
        }

        [Fact]
        public async Task UploadSignedMandat_CaseThrowJeDeclareApiException()
        {
            var mandatString = "test mandat string";
            var mandat = Encoding.UTF8.GetBytes(mandatString);

            var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent("error message"),
            };

            var httpContent = new StringContent("dGVzdCBtYW5kYXQgc3RyaW5n", Encoding.UTF8);

            var client = new Mock<IHttpClient>(MockBehavior.Strict);
            client.Setup(c => c.PostAsync(It.IsAny<string>(), It.IsAny<HttpContent>()))
                .Callback<string, HttpContent>((url, content) =>
                {
                    url.Should().Be($"compte/19581575/dossierClient/98765/rib/999945/mandatSigne");
                    content.Should().BeEquivalentTo(httpContent);
                })
                .ReturnsAsync(httpResponseMessage)
                .Verifiable();
            client.Setup(c => c.Dispose())
                .Verifiable();

            var factory = new Mock<IJeDeclareClientFactory>(MockBehavior.Strict);
            factory.Setup(f => f.Create())
                .Returns(client.Object)
                .Verifiable();

            var logger = new Mock<ILogger<HttpJeDeclareClient>>(MockBehavior.Strict);
            logger.Setup(x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsValueType>(),
                It.IsAny<Exception>(),
                (Func<It.IsValueType, Exception?, string>)It.IsAny<object>()));

            var options = Options.Create(new JeDeclareOptions()
            {
                JdcCompteId = "19581575",
                BaseUri = new Uri("http://example.com"),
                Login = "yourLogin",
                Password = "yourPassword",
            });

            var jeDeclareClient = new HttpJeDeclareClient(logger.Object, factory.Object, options);

            Func<Task> act = async () => await jeDeclareClient.UploadSignedMandat("98765", "999945", mandat);

            await act.Should().ThrowExactlyAsync<JeDeclareApiException>()
                .WithMessage("Exception was thrown : status code : BadRequest - Message : 'error message'");

            client.VerifyAll();
            factory.VerifyAll();
        }

        [Fact]
        public async Task CheckSignedMandatExists_CaseOK()
        {
            var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);

            var client = new Mock<IHttpClient>(MockBehavior.Strict);
            client.Setup(c => c.SendAsync(It.IsAny<HttpRequestMessage>()))
                .Callback<HttpRequestMessage>(req =>
                {
                    req.Method.Should().Be(HttpMethod.Head);
                    req.RequestUri.Should().Be("compte/19581575/dossierClient/98765/rib/999945/mandatSigne");
                })
                .ReturnsAsync(httpResponseMessage)
                .Verifiable();
            client.Setup(c => c.Dispose())
                .Verifiable();

            var factory = new Mock<IJeDeclareClientFactory>(MockBehavior.Strict);
            factory.Setup(f => f.Create())
                .Returns(client.Object)
                .Verifiable();

            var logger = new Mock<ILogger<HttpJeDeclareClient>>(MockBehavior.Strict);
            logger.Setup(x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsValueType>(),
                It.IsAny<Exception>(),
                (Func<It.IsValueType, Exception?, string>)It.IsAny<object>()));

            var options = Options.Create(new JeDeclareOptions()
            {
                JdcCompteId = "19581575",
                BaseUri = new Uri("http://example.com"),
                Login = "yourLogin",
                Password = "yourPassword",
            });

            var jeDeclareClient = new HttpJeDeclareClient(logger.Object, factory.Object, options);

            var result = await jeDeclareClient.CheckSignedMandatExists("98765", "999945");

            result.Should().BeTrue();

            client.VerifyAll();
            factory.VerifyAll();
        }

        [Fact]
        public async Task CheckSignedMandatExists_CaseThrowJeDeclareApiException()
        {
            var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent("error message"),
            };

            var client = new Mock<IHttpClient>(MockBehavior.Strict);
            client.Setup(c => c.SendAsync(It.IsAny<HttpRequestMessage>()))
                .Callback<HttpRequestMessage>(req =>
                {
                    req.Method.Should().Be(HttpMethod.Head);
                    req.RequestUri.Should().Be("compte/19581575/dossierClient/98765/rib/999945/mandatSigne");
                })
                .ReturnsAsync(httpResponseMessage)
                .Verifiable();
            client.Setup(c => c.Dispose())
                .Verifiable();

            var factory = new Mock<IJeDeclareClientFactory>(MockBehavior.Strict);
            factory.Setup(f => f.Create())
                .Returns(client.Object)
                .Verifiable();

            var logger = new Mock<ILogger<HttpJeDeclareClient>>(MockBehavior.Strict);
            logger.Setup(x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsValueType>(),
                It.IsAny<Exception>(),
                (Func<It.IsValueType, Exception?, string>)It.IsAny<object>()));

            var options = Options.Create(new JeDeclareOptions()
            {
                JdcCompteId = "19581575",
                BaseUri = new Uri("http://example.com"),
                Login = "yourLogin",
                Password = "yourPassword",
            });

            var jeDeclareClient = new HttpJeDeclareClient(logger.Object, factory.Object, options);

            Func<Task> act = async () => await jeDeclareClient.CheckSignedMandatExists("98765", "999945");

            await act.Should().ThrowExactlyAsync<JeDeclareApiException>()
                .WithMessage("Exception was thrown : status code : BadRequest - Message : 'error message'");

            client.VerifyAll();
            factory.VerifyAll();
        }

        [Fact]
        public async Task GetMandatPdfAsync_CaseOK()
        {
            string content = Convert.ToBase64String(new byte[] { 0x20, 0x20, });
            var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(content),
            };

            var client = new Mock<IHttpClient>(MockBehavior.Strict);
            client.Setup(c => c.GetAsync("compte/19581575/dossierClient/jdcFolderId/rib/jdcRibId/mandat"))
                .ReturnsAsync(httpResponseMessage)
                .Verifiable();

            client.Setup(c => c.Dispose())
                .Verifiable();

            client.Setup(c => c.DefaultRequestHeaders)
                 .Returns(() =>
                 {
                     var httpClient = new HttpClient();
                     httpClient.DefaultRequestHeaders.Accept.Clear();
                     httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/*"));
                     return httpClient.DefaultRequestHeaders;
                 })
                 .Verifiable();

            var factory = new Mock<IJeDeclareClientFactory>(MockBehavior.Strict);
            factory.Setup(f => f.Create())
                .Returns(client.Object)
                .Verifiable();

            var options = Options.Create(new JeDeclareOptions()
            {
                JdcCompteId = "19581575",
                BaseUri = new Uri("http://example.com"),
                Login = "Login",
                Password = "Password",
            });

            var logger = new Mock<ILogger<HttpJeDeclareClient>>(MockBehavior.Strict);
            var jeDeclareClient = new HttpJeDeclareClient(logger.Object, factory.Object, options);

            var restlt = await jeDeclareClient.GetMandatPdfAsync("jdcFolderId", "jdcRibId");

            var convertedResult = Convert.ToBase64String(restlt);
            restlt.Should().BeOfType<byte[]>();
            convertedResult.Should().Be(content);

            client.VerifyAll();
            factory.VerifyAll();
            logger.VerifyAll();
        }

        [Fact]
        public async Task GetMandatPdfAsync_CaseThrowJeDeclareApiException()
        {
            var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent("error message"),
            };

            var client = new Mock<IHttpClient>(MockBehavior.Strict);
            client.Setup(c => c.GetAsync("compte/19581575/dossierClient/jdcFolderId/rib/jdcRibId/mandat"))
                .ReturnsAsync(httpResponseMessage)
                .Verifiable();

            client.Setup(c => c.Dispose())
                .Verifiable();

            client.Setup(c => c.DefaultRequestHeaders)
                 .Returns(() =>
                 {
                     var httpClient = new HttpClient();
                     httpClient.DefaultRequestHeaders.Accept.Clear();
                     httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/*"));
                     return httpClient.DefaultRequestHeaders;
                 })
                 .Verifiable();

            var factory = new Mock<IJeDeclareClientFactory>(MockBehavior.Strict);
            factory.Setup(f => f.Create())
                .Returns(client.Object)
                .Verifiable();

            var options = Options.Create(new JeDeclareOptions()
            {
                JdcCompteId = "19581575",
                BaseUri = new Uri("http://example.com"),
                Login = "Login",
                Password = "Password",
            });

            var logger = new Mock<ILogger<HttpJeDeclareClient>>(MockBehavior.Strict);
            logger.Setup(x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsValueType>(),
                It.IsAny<Exception>(),
                (Func<It.IsValueType, Exception?, string>)It.IsAny<object>()));

            var jeDeclareClient = new HttpJeDeclareClient(logger.Object, factory.Object, options);

            Func<Task> action = async () => await jeDeclareClient.GetMandatPdfAsync("jdcFolderId", "jdcRibId");
            await action.Should().ThrowExactlyAsync<JeDeclareApiException>()
                .WithMessage("Exception was thrown : status code : BadRequest - Message : 'error message'");

            client.VerifyAll();
            factory.VerifyAll();
            logger.VerifyAll();
        }
    }
}
