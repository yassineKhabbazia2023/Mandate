// <copyright file="HttpFormioClientTestIntegrationTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Formio.Client.Http.Tests
{
    using System.Net;
    using System.Net.Http;
    using Kpmg.Constellation.Net.Http;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public class HttpFormioClientTestIntegrationTest
    {
        [Fact]
        public async Task GetSubmissions_IntegrationTest()
        {
            var options = Options.Create(new FormioOptions()
            {
                BaseUri = new Uri("https://cst-d2-formio-api.azurewebsites.net/"),
                FormioApiKey = "eNnPcMqUZhgY4Pz7bdiewTz8KlGQG4",
            });

            var client = new Mock<Kpmg.Constellation.Net.Http.IHttpClient>(MockBehavior.Strict);
            client.Setup(c => c.GetAsync(It.IsAny<string>()))
                .Returns((string resp) =>
                {
                    HttpClient realClient = HttpClientFactory.Create();
                    realClient.BaseAddress = new Uri($"{options.Value.BaseUri}");

                    realClient.DefaultRequestHeaders.Add("x-token", "eNnPcMqUZhgY4Pz7bdiewTz8KlGQG4");

                    return realClient.GetAsync(resp);
                });

            client.Setup(c => c.Dispose())
                .Verifiable();

            var auth = new FormioAuthToken()
            {
                Type = FormioTokenType.App,
            };
            var formioFactory = new Mock<IFormioClientFactory>(MockBehavior.Strict);
            formioFactory.Setup(f => f.Create(auth))
                .Returns(client.Object)
                .Verifiable();

            var logger = new Mock<ILogger<HttpFormIoClient>>(MockBehavior.Strict);
            logger.Setup(x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsValueType>(),
                It.IsAny<Exception>(),
                (Func<It.IsValueType, Exception?, string>)It.IsAny<object>()));

            var formioClient = new HttpFormIoClient(logger.Object, formioFactory.Object);

            var result = await formioClient.GetSubmissionsAsync("demandemandat", 0, 20, auth);

            result.Should().NotBeNull();
            result!.Submissions.Count.Should().Be(20);
        }

        [Fact(Skip = "Integration Test")]
        public async Task CheckJdcPartnerBank_IntegrationTest()
        {
            var options = Options.Create(new FormioOptions()
            {
                BaseUri = new Uri("https://cst-d2-formio-api.azurewebsites.net/"),
            });

            var client = new Mock<Kpmg.Constellation.Net.Http.IHttpClient>(MockBehavior.Strict);
            client.Setup(c => c.GetAsync(It.IsAny<string>()))
                .Returns((string resp) =>
                {
                    HttpClient realClient = HttpClientFactory.Create();
                    realClient.BaseAddress = new Uri($"{options.Value.BaseUri}");
                    realClient.DefaultRequestHeaders.Accept.Clear();

                    return realClient.GetAsync(resp);
                });

            client.Setup(c => c.Dispose())
                .Verifiable();

            var formioFactory = new Mock<IFormioClientFactory>(MockBehavior.Strict);
            formioFactory.Setup(f => f.Create())
                .Returns(client.Object)
                .Verifiable();

            var logger = new Mock<ILogger<HttpFormIoClient>>(MockBehavior.Strict);
            logger.Setup(x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsValueType>(),
                It.IsAny<Exception>(),
                (Func<It.IsValueType, Exception?, string>)It.IsAny<object>()));

            var formioClient = new HttpFormIoClient(logger.Object, formioFactory.Object);

            var result = await formioClient.CheckJdcPartnerBankAsync("jdcsupportedbanks", "30004");

            result.Should().NotBeNull();
            ((bool)result!["isPartnerBank"] !).Should().Be(false);
            ((string)result!["bankCode"] !).Should().Be("30004");
            ((string)result!["bankingGroup"] !).Should().Be("BNP");
        }

        [Fact(Skip = "Integration Test")]
        public async Task CheckMadateDematSupported_IntegrationTest()
        {
            var options = Options.Create(new FormioOptions()
            {
                BaseUri = new Uri("https://cst-d2-formio-api.azurewebsites.net/"),
                FormioApiKey = "eNnPcMqUZhgY4Pz7bdiewTz8KlGQG4",
            });

            var client = new Mock<Kpmg.Constellation.Net.Http.IHttpClient>(MockBehavior.Strict);
            client.Setup(c => c.GetAsync(It.IsAny<string>()))
                .Returns((string resp) =>
                {
                    HttpClient realClient = HttpClientFactory.Create();
                    realClient.BaseAddress = new Uri($"{options.Value.BaseUri}");
                    realClient.DefaultRequestHeaders.Accept.Clear();

                    return realClient.GetAsync(resp);
                });

            client.Setup(c => c.Dispose())
                .Verifiable();

            var formioFactory = new Mock<IFormioClientFactory>(MockBehavior.Strict);
            formioFactory.Setup(f => f.Create())
                .Returns(client.Object)
                .Verifiable();

            var logger = new Mock<ILogger<HttpFormIoClient>>(MockBehavior.Strict);
            logger.Setup(x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsValueType>(),
                It.IsAny<Exception>(),
                (Func<It.IsValueType, Exception?, string>)It.IsAny<object>()));

            var formioClient = new HttpFormIoClient(logger.Object, formioFactory.Object);

            var result = await formioClient.CheckMadateDematSupportedAsync("jdcsupportedbanks", "30004");

            result.Should().BeFalse();
        }

        [Fact(Skip = "Integration Test")]
        public async Task CheckCollecteConfigExist_IntegrationTest()
        {
            var options = Options.Create(new FormioOptions()
            {
                BaseUri = new Uri("https://cst-d2-formio-api.azurewebsites.net/"),
                FormioApiKey = "eNnPcMqUZhgY4Pz7bdiewTz8KlGQG4",
            });

            var client = new Mock<Kpmg.Constellation.Net.Http.IHttpClient>(MockBehavior.Strict);
            client.Setup(c => c.GetAsync(It.IsAny<string>()))
                .Returns((string resp) =>
                {
                    HttpClient realClient = HttpClientFactory.Create();
                    realClient.BaseAddress = new Uri($"{options.Value.BaseUri}");
                    realClient.DefaultRequestHeaders.Accept.Clear();
                    realClient.DefaultRequestHeaders.Add("x-token", "eNnPcMqUZhgY4Pz7bdiewTz8KlGQG4");

                    return realClient.GetAsync(resp);
                });

            client.Setup(c => c.Dispose())
                .Verifiable();

            var auth = new FormioAuthToken()
            {
                Type = FormioTokenType.App,
            };

            var formioFactory = new Mock<IFormioClientFactory>(MockBehavior.Strict);
            formioFactory.Setup(f => f.Create(auth))
                .Returns(client.Object)
                .Verifiable();

            var logger = new Mock<ILogger<HttpFormIoClient>>(MockBehavior.Strict);
            logger.Setup(x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsValueType>(),
                It.IsAny<Exception>(),
                (Func<It.IsValueType, Exception?, string>)It.IsAny<object>()));

            var formioClient = new HttpFormIoClient(logger.Object, formioFactory.Object);

            var result = await formioClient.CheckCollecteConfigExistAsync("demandemandat", "30004", "00000000014", "00818", auth);

            result.Should().BeTrue();
        }

        [Fact(Skip = "Integration Test")]
        public async Task GetTemplateShemaAsync_IntegrationTest()
        {
            var options = Options.Create(new FormioOptions()
            {
                BaseUri = new Uri("https://cst-d2-formio-api.azurewebsites.net/"),
                FormioApiKey = "eNnPcMqUZhgY4Pz7bdiewTz8KlGQG4",
            });

            var client = new Mock<Kpmg.Constellation.Net.Http.IHttpClient>(MockBehavior.Strict);
            client.Setup(c => c.GetAsync(It.IsAny<string>()))
                .Returns((string resp) =>
                {
                    HttpClient realClient = HttpClientFactory.Create();
                    realClient.BaseAddress = new Uri($"{options.Value.BaseUri}");
                    realClient.DefaultRequestHeaders.Accept.Clear();
                    realClient.DefaultRequestHeaders.Add("x-token", "eNnPcMqUZhgY4Pz7bdiewTz8KlGQG4");

                    return realClient.GetAsync(resp);
                });

            client.Setup(c => c.Dispose())
                .Verifiable();

            var auth = new FormioAuthToken()
            {
                Type = FormioTokenType.App,
            };

            var formioFactory = new Mock<IFormioClientFactory>(MockBehavior.Strict);
            formioFactory.Setup(f => f.Create(auth))
                .Returns(client.Object)
                .Verifiable();

            var logger = new Mock<ILogger<HttpFormIoClient>>(MockBehavior.Strict);
            logger.Setup(x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsValueType>(),
                It.IsAny<Exception>(),
                (Func<It.IsValueType, Exception?, string>)It.IsAny<object>()));

            var formioClient = new HttpFormIoClient(logger.Object, formioFactory.Object);

            var result = await formioClient.GetTemplateShemaAsync("5ce57e521c42f37327ab37ab", "30004", auth);

            result.Should().NotBeNull();
        }

        [Fact(Skip = "Integration Test")]
        public async Task GetSubmissionByIdAsync_IntegrationTest()
        {
            var options = Options.Create(new FormioOptions()
            {
                BaseUri = new Uri("https://cst-d2-formio-api.azurewebsites.net/"),
                FormioApiKey = "eNnPcMqUZhgY4Pz7bdiewTz8KlGQG4",
            });

            var client = new Mock<Kpmg.Constellation.Net.Http.IHttpClient>(MockBehavior.Strict);
            client.Setup(c => c.GetAsync(It.IsAny<string>()))
                .Returns((string resp) =>
                {
                    HttpClient realClient = HttpClientFactory.Create();
                    realClient.BaseAddress = new Uri($"{options.Value.BaseUri}");

                    realClient.DefaultRequestHeaders.Add("x-token", "eNnPcMqUZhgY4Pz7bdiewTz8KlGQG4");

                    return realClient.GetAsync(resp);
                });

            client.Setup(c => c.Dispose())
                .Verifiable();

            var auth = new FormioAuthToken()
            {
                Type = FormioTokenType.App,
            };
            var formioFactory = new Mock<IFormioClientFactory>(MockBehavior.Strict);
            formioFactory.Setup(f => f.Create(auth))
                .Returns(client.Object)
                .Verifiable();

            var logger = new Mock<ILogger<HttpFormIoClient>>(MockBehavior.Strict);
            logger.Setup(x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsValueType>(),
                It.IsAny<Exception>(),
                (Func<It.IsValueType, Exception?, string>)It.IsAny<object>()));

            var formioClient = new HttpFormIoClient(logger.Object, formioFactory.Object);

            var result = await formioClient.GetSubmissionByIdAsync("demandemandat", "5dd29cae73ff997779932250", auth);

            result.Should().NotBeNull();
            ((string)result["_id"]) !.Should().Be("5dd29cae73ff997779932250");
        }

        [Fact(Skip = "Integration Test")]
        public async Task GetProjectDefinitionAsync_IntegrationTest()
        {
            var options = Options.Create(new FormioOptions()
            {
                BaseUri = new Uri("https://cst-d2-formio-api.azurewebsites.net/"),
                FormioApiKey = "eNnPcMqUZhgY4Pz7bdiewTz8KlGQG4",
            });

            var client = new Mock<Kpmg.Constellation.Net.Http.IHttpClient>(MockBehavior.Strict);
            client.Setup(c => c.GetAsync(It.IsAny<string>()))
                .Returns((string resp) =>
                {
                    HttpClient realClient = HttpClientFactory.Create();
                    realClient.BaseAddress = new Uri($"{options.Value.BaseUri}");

                    realClient.DefaultRequestHeaders.Add("x-token", "eNnPcMqUZhgY4Pz7bdiewTz8KlGQG4");

                    return realClient.GetAsync(resp);
                });

            client.Setup(c => c.Dispose())
                .Verifiable();

            var auth = new FormioAuthToken()
            {
                Type = FormioTokenType.App,
            };
            var formioFactory = new Mock<IFormioClientFactory>(MockBehavior.Strict);
            formioFactory.Setup(f => f.Create(auth))
                .Returns(client.Object)
                .Verifiable();

            var logger = new Mock<ILogger<HttpFormIoClient>>(MockBehavior.Strict);
            logger.Setup(x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsValueType>(),
                It.IsAny<Exception>(),
                (Func<It.IsValueType, Exception?, string>)It.IsAny<object>()));

            var formioClient = new HttpFormIoClient(logger.Object, formioFactory.Object);

            var result = await formioClient.GetProjectDefinitionAsync("5ce57e521c42f37327ab37ab", auth);

            result.Should().NotBeNull();
            ((string)result["_id"]) !.Should().Be("5ce57e521c42f37327ab37ab");
        }

        [Fact(Skip = "Integration Test")]
        public async Task DownloadSubmissionAsPDFWithTemplate_IntegrationTest()
        {
            var fileToken = "aJVYt4fi8mA5p0IgvIUsXQkzKbNCql";

            var options = Options.Create(new FormioOptions()
            {
                BaseUri = new Uri("https://cst-d2-formio-api.azurewebsites.net/"),
            });

            var pdfServerUrl = "https://cst-d2-formio-pdfserver.azurewebsites.net";
            var projectId = "5ce57e521c42f37327ab37ab";
            var downloadUrl = $"{pdfServerUrl}/pdf/{projectId}/download";

            var dataString = "{\r\n  \"_id\": \"5ce57e521c42f37327ab37ab\",\r\n  \"tag\": \"0.0.0\",\r\n  \"owner\": \"631b3ed30a718a06ed019733\",\r\n  \"plan\": \"commercial\",\r\n  \"steps\": [],\r\n  \"framework\": \"vue\",\r\n  \"protect\": false,\r\n  \"title\": \"Plateforme KPMG\",\r\n  \"name\": \"constellation\",\r\n  \"access\": [\r\n    {\r\n      \"roles\": [],\r\n      \"type\": \"create_own\"\r\n    },\r\n    {\r\n      \"roles\": [\r\n        \"631b3ed30a718a06ed019733\"\r\n      ],\r\n      \"type\": \"create_all\"\r\n    },\r\n    {\r\n      \"roles\": [],\r\n      \"type\": \"read_own\"\r\n    },\r\n    {\r\n      \"roles\": [\r\n        \"5ce57e521c42f3133cab37ad\",\r\n        \"6102d5e0bb5fe8157f307716\",\r\n        \"6368ceafe017bc40e73385c0\",\r\n        \"63ea107f664a606b7bfe2106\",\r\n        \"64463a0e05c12f0615826c9b\"\r\n      ],\r\n      \"type\": \"read_all\"\r\n    },\r\n    {\r\n      \"roles\": [],\r\n      \"type\": \"update_own\"\r\n    },\r\n    {\r\n      \"roles\": [],\r\n      \"type\": \"update_all\"\r\n    },\r\n    {\r\n      \"roles\": [],\r\n      \"type\": \"delete_own\"\r\n    },\r\n    {\r\n      \"roles\": [],\r\n      \"type\": \"delete_all\"\r\n    },\r\n    {\r\n      \"roles\": [\r\n        \"5e85fdc5b093733402a5112f\"\r\n      ],\r\n      \"type\": \"team_read\"\r\n    },\r\n    {\r\n      \"roles\": [\r\n        \"631b3ed30a718a06ed019733\",\r\n        \"6363d08be017bc2ae33384fd\"\r\n      ],\r\n      \"type\": \"team_write\"\r\n    },\r\n    {\r\n      \"roles\": [\r\n        \"5ce6468b50cb83813cf0324b\"\r\n      ],\r\n      \"type\": \"team_admin\"\r\n    }\r\n  ],\r\n  \"trial\": \"2019-05-22T16:52:34.011Z\",\r\n  \"created\": \"2019-05-22T16:52:34.011Z\",\r\n  \"modified\": \"2023-07-17T14:12:59.088Z\",\r\n  \"type\": \"project\",\r\n  \"config\": {\r\n    \"ConstellationRoles\": {\r\n      \"CST-ADMIN\": \"000001\",\r\n      \"ESC-ADMIN\": \"000002\",\r\n      \"ESC-TOUS\": \"0000003\"\r\n    }\r\n  },\r\n  \"apiCalls\": {\r\n    \"used\": {\r\n      \"forms\": 0,\r\n      \"emails\": 0,\r\n      \"formRequests\": 0,\r\n      \"submissionRequests\": 0\r\n    },\r\n    \"limit\": {\r\n      \"submissionRequests\": 2000000\r\n    },\r\n    \"reset\": \"2023-11-01T00:00:00Z\"\r\n  },\r\n  \"settings\": {\r\n    \"cors\": \"*\",\r\n    \"preview\": {\r\n      \"repository\": \"https://github.com/formio/formio-app-template\",\r\n      \"url\": \"http://formio.github.io/formio-app-template/\"\r\n    },\r\n    \"filetoken\": \"aJVYt4fi8mA5p0IgvIUsXQkzKbNCql\",\r\n    \"storage\": {\r\n      \"azure\": {\r\n        \"connectionString\": \"DefaultEndpointsProtocol=https;AccountName=cstd2forms;AccountKey=apikeyForTest;EndpointSuffix=core.windows.net\",\r\n        \"container\": \"form\"\r\n      }\r\n    },\r\n    \"keys\": [\r\n      {\r\n        \"name\": \"PDF Server\",\r\n        \"key\": \"eNnPcMqUZhgY4Pz7bdiewTz8KlGQG4\"\r\n      },\r\n      {\r\n        \"name\": \"Constellation\",\r\n        \"key\": \"yVOk3cX991WqFs66qoyOdxuwmWoks4\"\r\n      },\r\n      {\r\n        \"name\": \"DevOps\",\r\n        \"key\": \"0nPi1PIbPlDYQ8xaPoHLMNbzA1WUt3\"\r\n      },\r\n      {\r\n        \"name\": \"Comptes Rendus\",\r\n        \"key\": \"2sarADUvCgrq7VM6Jec5GShADTgGwH\"\r\n      },\r\n      {\r\n        \"name\": \"Akisition\",\r\n        \"key\": \"3Q0LY6RKctF6djaIIn6liRSdLYGd2s\"\r\n      },\r\n      {\r\n        \"name\": \"SalesForce\",\r\n        \"key\": \"KLRaiXGlYr9ybkHWarUMh7dRssg6u4\"\r\n      }\r\n    ],\r\n    \"pdfserver\": \"https://cst-d2-formio-pdfserver.azurewebsites.net\",\r\n    \"allowConfig\": true,\r\n    \"tokenParse\": \"\",\r\n    \"email\": {\r\n      \"sendgrid\": {\r\n        \"auth\": {\r\n          \"api_user\": \"apikey\",\r\n          \"api_key\": \"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\"\r\n        }\r\n      }\r\n    },\r\n    \"secret\": \"bX7TOMUdD64QlBtzqc83oLDrdeucwYcB\",\r\n    \"formModule\": \"\",\r\n    \"remoteSecret\": \"Sauvage1+\"\r\n  },\r\n  \"public\": {\r\n    \"formModule\": \"\",\r\n    \"custom\": {}\r\n  }\r\n}";

            var data = JToken.Parse(dataString);

            var templateSchemaString = "{\r\n  \"_id\": \"5e2b07ba8c7108d7569c2d48\",\r\n  \"components\": [\r\n    {\r\n      \"type\": \"button\",\r\n      \"label\": \"Submit\",\r\n      \"key\": \"submit\",\r\n      \"disableOnInvalid\": true,\r\n      \"input\": true,\r\n      \"tableView\": false,\r\n      \"placeholder\": \"\",\r\n      \"prefix\": \"\",\r\n      \"customClass\": \"\",\r\n      \"suffix\": \"\",\r\n      \"multiple\": false,\r\n      \"defaultValue\": null,\r\n      \"protected\": false,\r\n      \"unique\": false,\r\n      \"persistent\": false,\r\n      \"hidden\": false,\r\n      \"clearOnHide\": true,\r\n      \"refreshOn\": \"\",\r\n      \"redrawOn\": \"\",\r\n      \"modalEdit\": false,\r\n      \"labelPosition\": \"top\",\r\n      \"description\": \"\",\r\n      \"errorLabel\": \"\",\r\n      \"tooltip\": \"\",\r\n      \"hideLabel\": false,\r\n      \"tabindex\": \"\",\r\n      \"disabled\": false,\r\n      \"autofocus\": false,\r\n      \"dbIndex\": false,\r\n      \"customDefaultValue\": \"\",\r\n      \"calculateValue\": \"\",\r\n      \"widget\": {\r\n        \"type\": \"input\"\r\n      },\r\n      \"attributes\": {},\r\n      \"validateOn\": \"change\",\r\n      \"validate\": {\r\n        \"required\": false,\r\n        \"custom\": \"\",\r\n        \"customPrivate\": false,\r\n        \"strictDateValidation\": false,\r\n        \"multiple\": false,\r\n        \"unique\": false\r\n      },\r\n      \"conditional\": {\r\n        \"show\": null,\r\n        \"when\": null,\r\n        \"eq\": \"\"\r\n      },\r\n      \"overlay\": {\r\n        \"style\": \"\",\r\n        \"left\": \"\",\r\n        \"top\": \"\",\r\n        \"width\": \"\",\r\n        \"height\": \"\"\r\n      },\r\n      \"allowCalculateOverride\": false,\r\n      \"encrypted\": false,\r\n      \"showCharCount\": false,\r\n      \"showWordCount\": false,\r\n      \"properties\": {},\r\n      \"allowMultipleMasks\": false,\r\n      \"size\": \"md\",\r\n      \"leftIcon\": \"\",\r\n      \"rightIcon\": \"\",\r\n      \"block\": false,\r\n      \"action\": \"submit\",\r\n      \"theme\": \"primary\",\r\n      \"dataGridLabel\": true,\r\n      \"id\": \"edswxyf\"\r\n    },\r\n    {\r\n      \"label\": \"signatoryFullName\",\r\n      \"hidden\": true,\r\n      \"tableView\": true,\r\n      \"key\": \"signatoryFullName\",\r\n      \"overlay\": {\r\n        \"page\": 1,\r\n        \"left\": 220.5881176470588,\r\n        \"top\": 217.11776470588237,\r\n        \"width\": 495,\r\n        \"height\": 21,\r\n        \"style\": \"\"\r\n      },\r\n      \"type\": \"textfield\",\r\n      \"input\": true,\r\n      \"placeholder\": \"\",\r\n      \"prefix\": \"\",\r\n      \"customClass\": \"\",\r\n      \"suffix\": \"\",\r\n      \"multiple\": false,\r\n      \"defaultValue\": null,\r\n      \"protected\": false,\r\n      \"unique\": false,\r\n      \"persistent\": true,\r\n      \"clearOnHide\": true,\r\n      \"refreshOn\": \"\",\r\n      \"redrawOn\": \"\",\r\n      \"modalEdit\": false,\r\n      \"labelPosition\": \"top\",\r\n      \"description\": \"\",\r\n      \"errorLabel\": \"\",\r\n      \"tooltip\": \"\",\r\n      \"hideLabel\": false,\r\n      \"tabindex\": \"\",\r\n      \"disabled\": false,\r\n      \"autofocus\": false,\r\n      \"dbIndex\": false,\r\n      \"customDefaultValue\": \"\",\r\n      \"calculateValue\": \"\",\r\n      \"widget\": {\r\n        \"type\": \"input\"\r\n      },\r\n      \"attributes\": {},\r\n      \"validateOn\": \"change\",\r\n      \"validate\": {\r\n        \"required\": false,\r\n        \"custom\": \"\",\r\n        \"customPrivate\": false,\r\n        \"strictDateValidation\": false,\r\n        \"multiple\": false,\r\n        \"unique\": false,\r\n        \"minLength\": \"\",\r\n        \"maxLength\": \"\",\r\n        \"pattern\": \"\"\r\n      },\r\n      \"conditional\": {\r\n        \"show\": null,\r\n        \"when\": null,\r\n        \"eq\": \"\"\r\n      },\r\n      \"allowCalculateOverride\": false,\r\n      \"encrypted\": false,\r\n      \"showCharCount\": false,\r\n      \"showWordCount\": false,\r\n      \"properties\": {},\r\n      \"allowMultipleMasks\": false,\r\n      \"mask\": false,\r\n      \"inputType\": \"text\",\r\n      \"inputFormat\": \"plain\",\r\n      \"inputMask\": \"\",\r\n      \"id\": \"e350qxk\"\r\n    },\r\n    {\r\n      \"label\": \"companyName\",\r\n      \"hidden\": true,\r\n      \"tableView\": true,\r\n      \"key\": \"companyName\",\r\n      \"overlay\": {\r\n        \"page\": 1,\r\n        \"left\": 207,\r\n        \"top\": 260,\r\n        \"height\": 20,\r\n        \"width\": 505,\r\n        \"style\": \"\"\r\n      },\r\n      \"type\": \"textfield\",\r\n      \"input\": true,\r\n      \"placeholder\": \"\",\r\n      \"prefix\": \"\",\r\n      \"customClass\": \"\",\r\n      \"suffix\": \"\",\r\n      \"multiple\": false,\r\n      \"defaultValue\": null,\r\n      \"protected\": false,\r\n      \"unique\": false,\r\n      \"persistent\": true,\r\n      \"clearOnHide\": true,\r\n      \"refreshOn\": \"\",\r\n      \"redrawOn\": \"\",\r\n      \"modalEdit\": false,\r\n      \"labelPosition\": \"top\",\r\n      \"description\": \"\",\r\n      \"errorLabel\": \"\",\r\n      \"tooltip\": \"\",\r\n      \"hideLabel\": false,\r\n      \"tabindex\": \"\",\r\n      \"disabled\": false,\r\n      \"autofocus\": false,\r\n      \"dbIndex\": false,\r\n      \"customDefaultValue\": \"\",\r\n      \"calculateValue\": \"\",\r\n      \"widget\": {\r\n        \"type\": \"input\"\r\n      },\r\n      \"attributes\": {},\r\n      \"validateOn\": \"change\",\r\n      \"validate\": {\r\n        \"required\": false,\r\n        \"custom\": \"\",\r\n        \"customPrivate\": false,\r\n        \"strictDateValidation\": false,\r\n        \"multiple\": false,\r\n        \"unique\": false,\r\n        \"minLength\": \"\",\r\n        \"maxLength\": \"\",\r\n        \"pattern\": \"\"\r\n      },\r\n      \"conditional\": {\r\n        \"show\": null,\r\n        \"when\": null,\r\n        \"eq\": \"\"\r\n      },\r\n      \"allowCalculateOverride\": false,\r\n      \"encrypted\": false,\r\n      \"showCharCount\": false,\r\n      \"showWordCount\": false,\r\n      \"properties\": {},\r\n      \"allowMultipleMasks\": false,\r\n      \"mask\": false,\r\n      \"inputType\": \"text\",\r\n      \"inputFormat\": \"plain\",\r\n      \"inputMask\": \"\",\r\n      \"id\": \"e4339ck\"\r\n    },\r\n    {\r\n      \"label\": \"SIRET\",\r\n      \"tableView\": true,\r\n      \"key\": \"SIRETNumber\",\r\n      \"overlay\": {\r\n        \"page\": 1,\r\n        \"left\": 743,\r\n        \"top\": 278,\r\n        \"height\": 20,\r\n        \"width\": 248,\r\n        \"style\": \"\"\r\n      },\r\n      \"type\": \"textfield\",\r\n      \"input\": true,\r\n      \"placeholder\": \"\",\r\n      \"prefix\": \"\",\r\n      \"customClass\": \"\",\r\n      \"suffix\": \"\",\r\n      \"multiple\": false,\r\n      \"defaultValue\": null,\r\n      \"protected\": false,\r\n      \"unique\": false,\r\n      \"persistent\": true,\r\n      \"hidden\": false,\r\n      \"clearOnHide\": true,\r\n      \"refreshOn\": \"\",\r\n      \"redrawOn\": \"\",\r\n      \"modalEdit\": false,\r\n      \"labelPosition\": \"top\",\r\n      \"description\": \"\",\r\n      \"errorLabel\": \"\",\r\n      \"tooltip\": \"\",\r\n      \"hideLabel\": false,\r\n      \"tabindex\": \"\",\r\n      \"disabled\": false,\r\n      \"autofocus\": false,\r\n      \"dbIndex\": false,\r\n      \"customDefaultValue\": \"\",\r\n      \"calculateValue\": \"\",\r\n      \"widget\": {\r\n        \"type\": \"input\"\r\n      },\r\n      \"attributes\": {},\r\n      \"validateOn\": \"change\",\r\n      \"validate\": {\r\n        \"required\": false,\r\n        \"custom\": \"\",\r\n        \"customPrivate\": false,\r\n        \"strictDateValidation\": false,\r\n        \"multiple\": false,\r\n        \"unique\": false,\r\n        \"minLength\": \"\",\r\n        \"maxLength\": \"\",\r\n        \"pattern\": \"\"\r\n      },\r\n      \"conditional\": {\r\n        \"show\": null,\r\n        \"when\": null,\r\n        \"eq\": \"\"\r\n      },\r\n      \"allowCalculateOverride\": false,\r\n      \"encrypted\": false,\r\n      \"showCharCount\": false,\r\n      \"showWordCount\": false,\r\n      \"properties\": {},\r\n      \"allowMultipleMasks\": false,\r\n      \"mask\": false,\r\n      \"inputType\": \"text\",\r\n      \"inputFormat\": \"plain\",\r\n      \"inputMask\": \"\",\r\n      \"id\": \"ecw26c\"\r\n    },\r\n    {\r\n      \"label\": \"address1\",\r\n      \"tableView\": true,\r\n      \"key\": \"address1\",\r\n      \"overlay\": {\r\n        \"page\": 1,\r\n        \"left\": 208,\r\n        \"top\": 299,\r\n        \"width\": 783,\r\n        \"height\": 20,\r\n        \"style\": \"\"\r\n      },\r\n      \"type\": \"textfield\",\r\n      \"input\": true,\r\n      \"placeholder\": \"\",\r\n      \"prefix\": \"\",\r\n      \"customClass\": \"\",\r\n      \"suffix\": \"\",\r\n      \"multiple\": false,\r\n      \"defaultValue\": null,\r\n      \"protected\": false,\r\n      \"unique\": false,\r\n      \"persistent\": true,\r\n      \"hidden\": false,\r\n      \"clearOnHide\": true,\r\n      \"refreshOn\": \"\",\r\n      \"redrawOn\": \"\",\r\n      \"modalEdit\": false,\r\n      \"labelPosition\": \"top\",\r\n      \"description\": \"\",\r\n      \"errorLabel\": \"\",\r\n      \"tooltip\": \"\",\r\n      \"hideLabel\": false,\r\n      \"tabindex\": \"\",\r\n      \"disabled\": false,\r\n      \"autofocus\": false,\r\n      \"dbIndex\": false,\r\n      \"customDefaultValue\": \"\",\r\n      \"calculateValue\": \"\",\r\n      \"widget\": {\r\n        \"type\": \"input\"\r\n      },\r\n      \"attributes\": {},\r\n      \"validateOn\": \"change\",\r\n      \"validate\": {\r\n        \"required\": false,\r\n        \"custom\": \"\",\r\n        \"customPrivate\": false,\r\n        \"strictDateValidation\": false,\r\n        \"multiple\": false,\r\n        \"unique\": false,\r\n        \"minLength\": \"\",\r\n        \"maxLength\": \"\",\r\n        \"pattern\": \"\"\r\n      },\r\n      \"conditional\": {\r\n        \"show\": null,\r\n        \"when\": null,\r\n        \"eq\": \"\"\r\n      },\r\n      \"allowCalculateOverride\": false,\r\n      \"encrypted\": false,\r\n      \"showCharCount\": false,\r\n      \"showWordCount\": false,\r\n      \"properties\": {},\r\n      \"allowMultipleMasks\": false,\r\n      \"mask\": false,\r\n      \"inputType\": \"text\",\r\n      \"inputFormat\": \"plain\",\r\n      \"inputMask\": \"\",\r\n      \"id\": \"e57oq0v\"\r\n    },\r\n    {\r\n      \"label\": \"address2\",\r\n      \"tableView\": true,\r\n      \"key\": \"address2\",\r\n      \"overlay\": {\r\n        \"page\": 1,\r\n        \"left\": 209,\r\n        \"top\": 322,\r\n        \"width\": 782.625,\r\n        \"height\": 18.125,\r\n        \"style\": \"\"\r\n      },\r\n      \"type\": \"textfield\",\r\n      \"input\": true,\r\n      \"placeholder\": \"\",\r\n      \"prefix\": \"\",\r\n      \"customClass\": \"\",\r\n      \"suffix\": \"\",\r\n      \"multiple\": false,\r\n      \"defaultValue\": null,\r\n      \"protected\": false,\r\n      \"unique\": false,\r\n      \"persistent\": true,\r\n      \"hidden\": false,\r\n      \"clearOnHide\": true,\r\n      \"refreshOn\": \"\",\r\n      \"redrawOn\": \"\",\r\n      \"modalEdit\": false,\r\n      \"labelPosition\": \"top\",\r\n      \"description\": \"\",\r\n      \"errorLabel\": \"\",\r\n      \"tooltip\": \"\",\r\n      \"hideLabel\": false,\r\n      \"tabindex\": \"\",\r\n      \"disabled\": false,\r\n      \"autofocus\": false,\r\n      \"dbIndex\": false,\r\n      \"customDefaultValue\": \"\",\r\n      \"calculateValue\": \"\",\r\n      \"widget\": {\r\n        \"type\": \"input\"\r\n      },\r\n      \"attributes\": {},\r\n      \"validateOn\": \"change\",\r\n      \"validate\": {\r\n        \"required\": false,\r\n        \"custom\": \"\",\r\n        \"customPrivate\": false,\r\n        \"strictDateValidation\": false,\r\n        \"multiple\": false,\r\n        \"unique\": false,\r\n        \"minLength\": \"\",\r\n        \"maxLength\": \"\",\r\n        \"pattern\": \"\"\r\n      },\r\n      \"conditional\": {\r\n        \"show\": null,\r\n        \"when\": null,\r\n        \"eq\": \"\"\r\n      },\r\n      \"allowCalculateOverride\": false,\r\n      \"encrypted\": false,\r\n      \"showCharCount\": false,\r\n      \"showWordCount\": false,\r\n      \"properties\": {},\r\n      \"allowMultipleMasks\": false,\r\n      \"mask\": false,\r\n      \"inputType\": \"text\",\r\n      \"inputFormat\": \"plain\",\r\n      \"inputMask\": \"\",\r\n      \"id\": \"e45x8ps\"\r\n    },\r\n    {\r\n      \"label\": \"bankCode\",\r\n      \"tableView\": true,\r\n      \"key\": \"bankCode\",\r\n      \"overlay\": {\r\n        \"page\": 1,\r\n        \"left\": 112,\r\n        \"top\": 521.4,\r\n        \"height\": 21,\r\n        \"width\": 215,\r\n        \"style\": \"\"\r\n      },\r\n      \"type\": \"textfield\",\r\n      \"input\": true,\r\n      \"placeholder\": \"\",\r\n      \"prefix\": \"\",\r\n      \"customClass\": \"\",\r\n      \"suffix\": \"\",\r\n      \"multiple\": false,\r\n      \"defaultValue\": null,\r\n      \"protected\": false,\r\n      \"unique\": false,\r\n      \"persistent\": true,\r\n      \"hidden\": false,\r\n      \"clearOnHide\": true,\r\n      \"refreshOn\": \"\",\r\n      \"redrawOn\": \"\",\r\n      \"modalEdit\": false,\r\n      \"labelPosition\": \"top\",\r\n      \"description\": \"\",\r\n      \"errorLabel\": \"\",\r\n      \"tooltip\": \"\",\r\n      \"hideLabel\": false,\r\n      \"tabindex\": \"\",\r\n      \"disabled\": false,\r\n      \"autofocus\": false,\r\n      \"dbIndex\": false,\r\n      \"customDefaultValue\": \"\",\r\n      \"calculateValue\": \"\",\r\n      \"widget\": {\r\n        \"type\": \"input\"\r\n      },\r\n      \"attributes\": {},\r\n      \"validateOn\": \"change\",\r\n      \"validate\": {\r\n        \"required\": false,\r\n        \"custom\": \"\",\r\n        \"customPrivate\": false,\r\n        \"strictDateValidation\": false,\r\n        \"multiple\": false,\r\n        \"unique\": false,\r\n        \"minLength\": \"\",\r\n        \"maxLength\": \"\",\r\n        \"pattern\": \"\"\r\n      },\r\n      \"conditional\": {\r\n        \"show\": null,\r\n        \"when\": null,\r\n        \"eq\": \"\"\r\n      },\r\n      \"allowCalculateOverride\": false,\r\n      \"encrypted\": false,\r\n      \"showCharCount\": false,\r\n      \"showWordCount\": false,\r\n      \"properties\": {},\r\n      \"allowMultipleMasks\": false,\r\n      \"mask\": false,\r\n      \"inputType\": \"text\",\r\n      \"inputFormat\": \"plain\",\r\n      \"inputMask\": \"\",\r\n      \"id\": \"ezzpl69\"\r\n    },\r\n    {\r\n      \"label\": \"bankSortCode\",\r\n      \"hidden\": true,\r\n      \"tableView\": true,\r\n      \"key\": \"bankSortCode\",\r\n      \"overlay\": {\r\n        \"page\": 1,\r\n        \"left\": 347,\r\n        \"top\": 522,\r\n        \"width\": 206,\r\n        \"height\": 20,\r\n        \"style\": \"\"\r\n      },\r\n      \"type\": \"textfield\",\r\n      \"input\": true,\r\n      \"placeholder\": \"\",\r\n      \"prefix\": \"\",\r\n      \"customClass\": \"\",\r\n      \"suffix\": \"\",\r\n      \"multiple\": false,\r\n      \"defaultValue\": null,\r\n      \"protected\": false,\r\n      \"unique\": false,\r\n      \"persistent\": true,\r\n      \"clearOnHide\": true,\r\n      \"refreshOn\": \"\",\r\n      \"redrawOn\": \"\",\r\n      \"modalEdit\": false,\r\n      \"labelPosition\": \"top\",\r\n      \"description\": \"\",\r\n      \"errorLabel\": \"\",\r\n      \"tooltip\": \"\",\r\n      \"hideLabel\": false,\r\n      \"tabindex\": \"\",\r\n      \"disabled\": false,\r\n      \"autofocus\": false,\r\n      \"dbIndex\": false,\r\n      \"customDefaultValue\": \"\",\r\n      \"calculateValue\": \"\",\r\n      \"widget\": {\r\n        \"type\": \"input\"\r\n      },\r\n      \"attributes\": {},\r\n      \"validateOn\": \"change\",\r\n      \"validate\": {\r\n        \"required\": false,\r\n        \"custom\": \"\",\r\n        \"customPrivate\": false,\r\n        \"strictDateValidation\": false,\r\n        \"multiple\": false,\r\n        \"unique\": false,\r\n        \"minLength\": \"\",\r\n        \"maxLength\": \"\",\r\n        \"pattern\": \"\"\r\n      },\r\n      \"conditional\": {\r\n        \"show\": null,\r\n        \"when\": null,\r\n        \"eq\": \"\"\r\n      },\r\n      \"allowCalculateOverride\": false,\r\n      \"encrypted\": false,\r\n      \"showCharCount\": false,\r\n      \"showWordCount\": false,\r\n      \"properties\": {},\r\n      \"allowMultipleMasks\": false,\r\n      \"mask\": false,\r\n      \"inputType\": \"text\",\r\n      \"inputFormat\": \"plain\",\r\n      \"inputMask\": \"\",\r\n      \"id\": \"emgo9el5\"\r\n    },\r\n    {\r\n      \"label\": \"bankAccountNumber\",\r\n      \"hideLabel\": true,\r\n      \"tableView\": true,\r\n      \"key\": \"bankAccountNumber\",\r\n      \"overlay\": {\r\n        \"page\": 1,\r\n        \"left\": 576,\r\n        \"top\": 521,\r\n        \"width\": 208,\r\n        \"height\": 21,\r\n        \"style\": \"\"\r\n      },\r\n      \"type\": \"textfield\",\r\n      \"input\": true,\r\n      \"placeholder\": \"\",\r\n      \"prefix\": \"\",\r\n      \"customClass\": \"\",\r\n      \"suffix\": \"\",\r\n      \"multiple\": false,\r\n      \"defaultValue\": null,\r\n      \"protected\": false,\r\n      \"unique\": false,\r\n      \"persistent\": true,\r\n      \"hidden\": false,\r\n      \"clearOnHide\": true,\r\n      \"refreshOn\": \"\",\r\n      \"redrawOn\": \"\",\r\n      \"modalEdit\": false,\r\n      \"labelPosition\": \"top\",\r\n      \"description\": \"\",\r\n      \"errorLabel\": \"\",\r\n      \"tooltip\": \"\",\r\n      \"tabindex\": \"\",\r\n      \"disabled\": false,\r\n      \"autofocus\": false,\r\n      \"dbIndex\": false,\r\n      \"customDefaultValue\": \"\",\r\n      \"calculateValue\": \"\",\r\n      \"widget\": {\r\n        \"type\": \"input\"\r\n      },\r\n      \"attributes\": {},\r\n      \"validateOn\": \"change\",\r\n      \"validate\": {\r\n        \"required\": false,\r\n        \"custom\": \"\",\r\n        \"customPrivate\": false,\r\n        \"strictDateValidation\": false,\r\n        \"multiple\": false,\r\n        \"unique\": false,\r\n        \"minLength\": \"\",\r\n        \"maxLength\": \"\",\r\n        \"pattern\": \"\"\r\n      },\r\n      \"conditional\": {\r\n        \"show\": null,\r\n        \"when\": null,\r\n        \"eq\": \"\"\r\n      },\r\n      \"allowCalculateOverride\": false,\r\n      \"encrypted\": false,\r\n      \"showCharCount\": false,\r\n      \"showWordCount\": false,\r\n      \"properties\": {},\r\n      \"allowMultipleMasks\": false,\r\n      \"mask\": false,\r\n      \"inputType\": \"text\",\r\n      \"inputFormat\": \"plain\",\r\n      \"inputMask\": \"\",\r\n      \"id\": \"e1t094\"\r\n    },\r\n    {\r\n      \"label\": \"bankCheckNumber\",\r\n      \"hideLabel\": true,\r\n      \"tableView\": true,\r\n      \"key\": \"bankCheckNumber\",\r\n      \"overlay\": {\r\n        \"page\": 1,\r\n        \"left\": 804,\r\n        \"top\": 521.6,\r\n        \"height\": 20,\r\n        \"width\": 201,\r\n        \"style\": \"\"\r\n      },\r\n      \"type\": \"textfield\",\r\n      \"input\": true,\r\n      \"placeholder\": \"\",\r\n      \"prefix\": \"\",\r\n      \"customClass\": \"\",\r\n      \"suffix\": \"\",\r\n      \"multiple\": false,\r\n      \"defaultValue\": null,\r\n      \"protected\": false,\r\n      \"unique\": false,\r\n      \"persistent\": true,\r\n      \"hidden\": false,\r\n      \"clearOnHide\": true,\r\n      \"refreshOn\": \"\",\r\n      \"redrawOn\": \"\",\r\n      \"modalEdit\": false,\r\n      \"labelPosition\": \"top\",\r\n      \"description\": \"\",\r\n      \"errorLabel\": \"\",\r\n      \"tooltip\": \"\",\r\n      \"tabindex\": \"\",\r\n      \"disabled\": false,\r\n      \"autofocus\": false,\r\n      \"dbIndex\": false,\r\n      \"customDefaultValue\": \"\",\r\n      \"calculateValue\": \"\",\r\n      \"widget\": {\r\n        \"type\": \"input\"\r\n      },\r\n      \"attributes\": {},\r\n      \"validateOn\": \"change\",\r\n      \"validate\": {\r\n        \"required\": false,\r\n        \"custom\": \"\",\r\n        \"customPrivate\": false,\r\n        \"strictDateValidation\": false,\r\n        \"multiple\": false,\r\n        \"unique\": false,\r\n        \"minLength\": \"\",\r\n        \"maxLength\": \"\",\r\n        \"pattern\": \"\"\r\n      },\r\n      \"conditional\": {\r\n        \"show\": null,\r\n        \"when\": null,\r\n        \"eq\": \"\"\r\n      },\r\n      \"allowCalculateOverride\": false,\r\n      \"encrypted\": false,\r\n      \"showCharCount\": false,\r\n      \"showWordCount\": false,\r\n      \"properties\": {},\r\n      \"allowMultipleMasks\": false,\r\n      \"mask\": false,\r\n      \"inputType\": \"text\",\r\n      \"inputFormat\": \"plain\",\r\n      \"inputMask\": \"\",\r\n      \"id\": \"ekbn008\"\r\n    }\r\n  ],\r\n  \"settings\": {\r\n    \"pdf\": {\r\n      \"src\": \"https://cst-d2-formio-pdfserver.azurewebsites.net/pdf/5ce57e521c42f37327ab37ab/file/43f5ca2a-3fef-5788-97e4-474555ce2916\",\r\n      \"id\": \"43f5ca2a-3fef-5788-97e4-474555ce2916\"\r\n    }\r\n  },\r\n  \"title\": \"Mandat BNP\",\r\n  \"path\": \"jdc-mandat-bnp\"\r\n}";
            var form = JToken.Parse(templateSchemaString);

            var request = JToken.FromObject(new
            {
                form,
                submission = data,
            });

            var content = new StringContent(
               JsonConvert.SerializeObject(request),
               Encoding.UTF8,
               "application/json");

            var httpContent = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

            var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.Created)
            {
                Content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json"),
            };

            var client = new Mock<Kpmg.Constellation.Net.Http.IHttpClient>(MockBehavior.Strict);

            client.Setup(c => c.PostAsync(It.IsAny<string>(), It.IsAny<HttpContent>()))
                .Callback<string, HttpContent>((url, content) =>
                {
                    url.Should().BeEquivalentTo(downloadUrl);
                    content.Should().BeEquivalentTo(httpContent);
                })
                .ReturnsAsync(httpResponseMessage);

            var headers = new HttpRequestMessage().Headers;
            headers.Add("x-file-token", fileToken);

            client.Setup(c => c.DefaultRequestHeaders)
                .Returns(headers)
                .Verifiable();

            client.Setup(c => c.Dispose())
                .Verifiable();

            var formioFactory = new Mock<IFormioClientFactory>(MockBehavior.Strict);
            formioFactory.Setup(f => f.Create())
                .Returns(client.Object)
                .Verifiable();

            var logger = new Mock<ILogger<HttpFormIoClient>>(MockBehavior.Strict);
            logger.Setup(x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsValueType>(),
                It.IsAny<Exception>(),
                (Func<It.IsValueType, Exception?, string>)It.IsAny<object>()));

            var formioClient = new HttpFormIoClient(logger.Object, formioFactory.Object);

            var result = await formioClient.DownloadSubmissionAsPDFWithTemplate(form, data, downloadUrl, fileToken);

            result.Should().NotBeNull();
            result.Data.Should().NotBeNull();
            result.Id.Should().Be("5ce57e521c42f37327ab37ab");
        }

        [Fact(Skip = "Integration Test")]
        public async Task IntegrationTest()
        {
            var auth = new FormioAuthToken()
            {
                Type = FormioTokenType.App,
            };

            var formioOptions = new FormioOptions()
            {
                BaseUri = new Uri("https://cst-d2-formio-api.azurewebsites.net/"),
                FormioApiKey = "eNnPcMqUZhgY4Pz7bdiewTz8KlGQG4",
            };

            var options = Options.Create(formioOptions);

            var client = new Mock<Kpmg.Constellation.Net.Http.IHttpClient>(MockBehavior.Strict);

            var headers = new HttpRequestMessage().Headers;
            client.Setup(c => c.DefaultRequestHeaders)
                .Returns(headers)
                .Verifiable();

            var res = new { id = "5ec874b20554e02cef26721f" };
            var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(res), Encoding.UTF8, "application/json"),
            };

            client.Setup(c => c.GetAsync(It.IsAny<string>()))
                .Callback<string>(s =>
                {
                    s.Should().Be("constellation/demandemandat/exists?data.bankCode=30004&data.bankAccountNumber=00000000014&data.bankSortCode=00818");
                })
                .ReturnsAsync(httpResponseMessage);

            client.Setup(c => c.Dispose())
               .Verifiable();

            var clientFactory = new Mock<Kpmg.Constellation.Net.Http.IHttpClientFactory>(MockBehavior.Strict);
            clientFactory.Setup(f => f.Create(It.IsAny<Uri>(), null))
                .Callback<Uri, HttpClientAuthentication>((u, h) =>
                {
                    h.Should().BeNull();
                    u.Should().Be(new Uri("https://cst-d2-formio-api.azurewebsites.net/"));
                })
                .Returns(client.Object)
                .Verifiable();

            var factory = new HttpFormioClientFactory(options, clientFactory.Object);

            var formioClient = new HttpFormIoClient((new NullLoggerFactory() as ILoggerFactory).CreateLogger<HttpFormIoClient>(), factory);

            var result = await formioClient.CheckCollecteConfigExistAsync("demandemandat", "30004", "00000000014", "00818", auth);

            result.Should().BeTrue();
        }
    }
}
