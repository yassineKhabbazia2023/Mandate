// <copyright file="HttpJeDeclareClientIntegrationTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.JeDeclare.Client.Http.Tests
{
    using System.Diagnostics.CodeAnalysis;
    using Kpmg.Constellation.Net.Http;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    [ExcludeFromCodeCoverage]
    public class HttpJeDeclareClientIntegrationTest
    {
        [Fact(Skip = "test jedeclare api")]
        public async Task GetAllConfigurationFromFolderAsync_integrationTest()
        {
            var options = Options.Create(new JeDeclareOptions()
            {
                BaseUri = new Uri("https://recette.jedeclare.com/webservice/gestion/"),
                Login = "kpmg-test@jedeclare.com",
                Password = "****",
                JdcCompteId = "****",
            });

            var credentials = $"{options.Value.Login}" + ":" + $"{options.Value.Password}";
            var encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(credentials));

            var client = new Mock<Kpmg.Constellation.Net.Http.IHttpClient>(MockBehavior.Strict);
            client.Setup(c => c.GetAsync(It.IsAny<string>()))
                .Returns((string resp) =>
                {
                    HttpClient realClient = HttpClientFactory.Create();
                    realClient.BaseAddress = new Uri($"{options.Value.BaseUri}");

                    realClient.DefaultRequestHeaders.Add("Authorization", "Basic " + encoded);

                    return realClient.GetAsync(resp);
                });

            client.Setup(c => c.Dispose())
                .Verifiable();

            var jedeclareFactory = new Mock<IJeDeclareClientFactory>(MockBehavior.Strict);
            jedeclareFactory.Setup(f => f.Create())
                .Returns(client.Object)
                .Verifiable();

            var logger = new Mock<ILogger<HttpJeDeclareClient>>(MockBehavior.Strict);
            logger.Setup(x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsValueType>(),
                It.IsAny<Exception>(),
                (Func<It.IsValueType, Exception?, string>)It.IsAny<object>()));

            var jeDeclareClient = new HttpJeDeclareClient(logger.Object, jedeclareFactory.Object, options);

            var result = await jeDeclareClient.GetAllConfigurationFromFolderAsync("1670097");

            result.Releve!.Length.Should().Be(1);

            client.VerifyAll();
            jedeclareFactory.VerifyAll();
        }

        [Fact(Skip = "test jedeclare api")]
        public async Task GetSignedMandatPdfAsync_integrationTest()
        {
            var options = Options.Create(new JeDeclareOptions()
            {
                BaseUri = new Uri("https://jedeclare.com/webservice/gestion/"),
                Login = "kpmgfr@jedeclare.com",
                Password = "****",
                JdcCompteId = "****",
            });

            var credentials = $"{options.Value.Login}" + ":" + $"{options.Value.Password}";
            var encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(credentials));

            var client = new Mock<Kpmg.Constellation.Net.Http.IHttpClient>(MockBehavior.Strict);
            client.Setup(c => c.GetAsync(It.IsAny<string>()))
                .Returns((string resp) =>
                {
                    HttpClient realClient = HttpClientFactory.Create();
                    realClient.BaseAddress = new Uri($"{options.Value.BaseUri}");

                    realClient.DefaultRequestHeaders.Add("Authorization", "Basic " + encoded);

                    return realClient.GetAsync(resp);
                });

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

            var jeDeclareClient = new HttpJeDeclareClient(logger.Object, factory.Object, options);

            var result = await jeDeclareClient.GetSignedMandatPdfAsync("21570139", "10379399");
            result.Should().NotBeNull();

            client.VerifyAll();
            factory.VerifyAll();
        }

        [Fact(Skip = "test jedeclare api")]
        public async Task CreateFolderAsync_integrationTest()
        {
            var options = Options.Create(new JeDeclareOptions()
            {
                BaseUri = new Uri("https://recette.jedeclare.com/webservice/gestion/"),
                Login = "kpmg-test@jedeclare.com",
                Password = "******",
                JdcCompteId = "****",
            });

            var credentials = $"{options.Value.Login}" + ":" + $"{options.Value.Password}";
            var encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(credentials));

            var client = new Mock<Kpmg.Constellation.Net.Http.IHttpClient>(MockBehavior.Strict);
            client.Setup(c => c.PostAsync(It.IsAny<string>(), It.IsAny<HttpContent>()))
                .Returns<string, HttpContent>((resp, content) =>
                {
                    HttpClient realClient = HttpClientFactory.Create();
                    realClient.BaseAddress = new Uri($"{options.Value.BaseUri}");

                    realClient.DefaultRequestHeaders.Add("Authorization", "Basic " + encoded);

                    return realClient.PostAsync(resp, content);
                });

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

            var jeDeclareClient = new HttpJeDeclareClient(logger.Object, factory.Object, options);

            var newFolder = new DossierClient
            {
                Client = new Client
                {
                    Id = "idT",
                    RaisonSociale = "raisonSocialeT",
                    Siret = new Siret
                    {
                        Siren = "804327252",
                        Nic = "00016",
                    },
                    Responsable = new Responsable
                    {
                        Name = "nameT",
                        Mail = "mailT.ooo@gmail.com",
                        Adresse = new Adresse
                        {
                            Rue = "rueT",
                            CplRue = "CplRueT",
                            CodePostal = "94800",
                            Ville = "VilleT",
                            Pays = "PaysT",
                        },
                    },
                },
                ExploitationDonnees = true,
            };

            var result = await jeDeclareClient.CreateFolderAsync(newFolder);

            result.ExploitationDonnees.Should().BeTrue();
            result.Client.Should().NotBeNull();
            result.Client.Id.Should().NotBeEmpty();
            result.Client.RaisonSociale.Should().NotBeEmpty();
            result.Client.Responsable.Name.Should().NotBeEmpty();
            result.Client.Responsable.Mail.Should().NotBeEmpty();
            result.Client.Responsable.Adresse.Rue.Should().NotBeEmpty();
            result.Client.Responsable.Adresse.CplRue.Should().NotBeEmpty();
            result.Client.Responsable.Adresse.CodePostal.Should().NotBeEmpty();
            result.Client.Responsable.Adresse.Ville.Should().NotBeEmpty();
            result.Client.Responsable.Adresse.Pays.Should().NotBeEmpty();
            result.Client.Siret.Siren.Should().NotBeEmpty();
            result.Client.Siret.Nic.Should().NotBeEmpty();

            client.VerifyAll();
            factory.VerifyAll();
        }

        [Fact(Skip = "test jedeclare api")]
        public async Task AddRibToFolderAsync_integrationTest()
        {
            var options = Options.Create(new JeDeclareOptions()
            {
                BaseUri = new Uri("https://recette.jedeclare.com/webservice/gestion/"),
                Login = "kpmg-test@jedeclare.com",
                Password = "****",
                JdcCompteId = "****",
            });

            var credentials = $"{options.Value.Login}" + ":" + $"{options.Value.Password}";
            var encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(credentials));

            var client = new Mock<Kpmg.Constellation.Net.Http.IHttpClient>(MockBehavior.Strict);
            client.Setup(c => c.PostAsync(It.IsAny<string>(), It.IsAny<HttpContent>()))
                .Returns<string, HttpContent>((resp, content) =>
                {
                    HttpClient realClient = HttpClientFactory.Create();
                    realClient.BaseAddress = options.Value.BaseUri;

                    realClient.DefaultRequestHeaders.Add("Authorization", "Basic " + encoded);

                    return realClient.PostAsync(resp, content);
                });

            client.Setup(c => c.Dispose())
                .Verifiable();

            var authent = new BasicHttpClientAuthentication("kpmg-test@jedeclare.com", "****");
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

            var jeDeclareClient = new HttpJeDeclareClient(logger.Object, factory.Object, options);

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

            var result = await jeDeclareClient.AddRibToFolderAsync("1670097", newRib);
            result.Should().NotBeNull();
            result.Id.Should().NotBeEmpty();
            result.Libelle.Should().NotBeEmpty();
            result.CiviliteTitulaire.Should().NotBeEmpty();
            result.NomTitulaire.Should().NotBeEmpty();
            result.PrenomTitulaire.Should().NotBeEmpty();
            result.Etablissement.Should().NotBeEmpty();
            result.Guichet.Should().NotBeEmpty();
            result.NumCompte.Should().NotBeEmpty();
            result.Cle.Should().NotBeEmpty();

            client.VerifyAll();
            factory.VerifyAll();
        }

        [Fact(Skip = "test jedeclare api")]
        public async Task CreateCollecteConfigurationAsync_integrationTest()
        {
            var bankCode = "****";
            var ebicsCarteId = "*****";

            var options = Options.Create(new JeDeclareOptions()
            {
                BaseUri = new Uri("https://recette.jedeclare.com/webservice/gestion/"),
                Login = "kpmg-test@jedeclare.com",
                Password = "****",
                JdcCompteId = "****",
            });

            var credentials = $"{options.Value.Login}" + ":" + $"{options.Value.Password}";

            var encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(credentials));

            var client = new Mock<Kpmg.Constellation.Net.Http.IHttpClient>(MockBehavior.Strict);
            client.Setup(c => c.PostAsync(It.IsAny<string>(), It.IsAny<HttpContent>()))
                .Returns<string, HttpContent>((resp, content) =>
                {
                    HttpClient realClient = HttpClientFactory.Create();
                    realClient.BaseAddress = options.Value.BaseUri;

                    realClient.DefaultRequestHeaders.Add("Authorization", "Basic " + encoded);

                    return realClient.PostAsync(resp, content);
                });

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

            var jeDeclareClient = new HttpJeDeclareClient(logger.Object, factory.Object, options);

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

            var result = await jeDeclareClient.CreateCollecteConfigurationAsync("1670097", newReleve, bankCode, ebicsCarteId);
            result.Should().NotBeNull();

            client.VerifyAll();
            factory.VerifyAll();
        }

        [Fact(Skip = "test jedeclare api")]
        public async Task UpdateCollecteConfigurationAsync_integrationTest()
        {
            var options = Options.Create(new JeDeclareOptions()
            {
                BaseUri = new Uri("https://recette.jedeclare.com/webservice/gestion/"),
                Login = "kpmg-test@jedeclare.com",
                Password = "****",
                JdcCompteId = "****",
            });

            var credentials = $"{options.Value.Login}" + ":" + $"{options.Value.Password}";

            var encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(credentials));

            var client = new Mock<Kpmg.Constellation.Net.Http.IHttpClient>(MockBehavior.Strict);
            client.Setup(c => c.PutAsync(It.IsAny<string>(), It.IsAny<HttpContent>()))
                .Returns<string, HttpContent>((resp, content) =>
                {
                    HttpClient realClient = HttpClientFactory.Create();
                    realClient.BaseAddress = options.Value.BaseUri;

                    realClient.DefaultRequestHeaders.Add("Authorization", "Basic " + encoded);

                    return realClient.PutAsync(resp, content);
                });

            client.Setup(c => c.Dispose())
                .Verifiable();

            var authent = new BasicHttpClientAuthentication("kpmg-test@jedeclare.com", "****");
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

            var jeDeclareClient = new HttpJeDeclareClient(logger.Object, factory.Object, options);

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
                CauseRejet = "causeRejetT",
                Destinataire = destinataire,
                Rib = rib,
                Periodicite = periodicite,
                DateReprise = "2023-01-01",
            };

            var result = await jeDeclareClient.UpdateCollecteConfigurationAsync("1670097", newReleve);

            client.VerifyAll();
            factory.VerifyAll();
        }

        [Fact(Skip = "test jedeclare api")]
        public async Task CheckSignedMandatExists_integrationTest()
        {
            var options = Options.Create(new JeDeclareOptions()
            {
                BaseUri = new Uri("https://recette.jedeclare.com/webservice/gestion/"),
                Login = "kpmg-test@jedeclare.com",
                Password = "****",
                JdcCompteId = "****",
            });

            var credentials = $"{options.Value.Login}" + ":" + $"{options.Value.Password}";

            var encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(credentials));

            var client = new Mock<Kpmg.Constellation.Net.Http.IHttpClient>(MockBehavior.Strict);
            client.Setup(c => c.SendAsync(It.IsAny<HttpRequestMessage>()))
                .Returns((HttpRequestMessage request) =>
                {
                    HttpClient realClient = HttpClientFactory.Create();
                    realClient.BaseAddress = options.Value.BaseUri;

                    realClient.DefaultRequestHeaders.Add("Authorization", "Basic " + encoded);

                    return realClient.SendAsync(request);
                });

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

            var jeDeclareClient = new HttpJeDeclareClient(logger.Object, factory.Object, options);

            var result = await jeDeclareClient.CheckSignedMandatExists("1670097", "999945");

            result.Should().BeTrue();

            client.VerifyAll();
            factory.VerifyAll();
        }
    }
}
