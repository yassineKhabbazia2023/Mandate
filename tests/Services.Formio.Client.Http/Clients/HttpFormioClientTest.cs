// <copyright file="HttpFormIoClientTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Formio.Client.Http.Tests
{
    using System.Net;
    using System.Text.Json.Nodes;
    using Kpmg.Constellation.Net.Http;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public class HttpFormIoClientTest
    {
        [Fact]
        public async Task GetSubmissionsAsync_CaseThrowExeption()
        {
            var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent("error message returned by formioApi"),
            };

            var client = new Mock<IHttpClient>(MockBehavior.Strict);
            client.Setup(c => c.GetAsync(It.IsAny<string>()))
                .Callback<string>(url =>
                {
                    url.Should().Be($"constellation/demandemandat/submission?skip=0&limit=20");
                })
                .ReturnsAsync(httpResponseMessage)
                .Verifiable();

            client.Setup(c => c.Dispose())
                .Verifiable();

            var auth = new FormioAuthToken();

            var factory = new Mock<IFormioClientFactory>(MockBehavior.Strict);
            factory.Setup(f => f.Create(auth))
                .Returns(client.Object)
                .Verifiable();

            var logger = new Mock<ILogger<HttpFormIoClient>>(MockBehavior.Strict);
            logger.Setup(x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsValueType>(),
                It.IsAny<Exception>(),
                (Func<It.IsValueType, Exception?, string>)It.IsAny<object>()));

            var jeDeclareClient = new HttpFormIoClient(logger.Object, factory.Object);

            Func<Task> act = async () => await jeDeclareClient.GetSubmissionsAsync("demandemandat", 0, 20, auth);

            await act.Should().ThrowExactlyAsync<FormioApiException>()
                .WithMessage("Exception was thrown : status code : BadRequest - Message : 'error message returned by formioApi'");

            client.VerifyAll();
            factory.VerifyAll();
        }

        [Fact]
        public async Task CheckJdcPartnerBankAsync_CaseThrowExeption()
        {
            var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent("error message returned by formioApi"),
            };

            var client = new Mock<IHttpClient>(MockBehavior.Strict);
            client.Setup(c => c.GetAsync(It.IsAny<string>()))
                .Callback<string>(url =>
                {
                    url.Should().Be($"constellation/demandemandat/submission?data.bankCode=codeBankT");
                })
                .ReturnsAsync(httpResponseMessage)
                .Verifiable();

            client.Setup(c => c.Dispose())
                .Verifiable();

            var factory = new Mock<IFormioClientFactory>(MockBehavior.Strict);
            factory.Setup(f => f.Create())
                .Returns(client.Object)
                .Verifiable();

            var logger = new Mock<ILogger<HttpFormIoClient>>(MockBehavior.Strict);
            logger.Setup(x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsValueType>(),
                It.IsAny<Exception>(),
                (Func<It.IsValueType, Exception?, string>)It.IsAny<object>()));

            var jeDeclareClient = new HttpFormIoClient(logger.Object, factory.Object);

            Func<Task> act = async () => await jeDeclareClient.CheckJdcPartnerBankAsync("demandemandat", "codeBankT");

            await act.Should().ThrowExactlyAsync<FormioApiException>()
                .WithMessage("Exception was thrown : status code : BadRequest - Message : 'error message returned by formioApi'");

            client.VerifyAll();
            factory.VerifyAll();
        }

        [Fact]
        public async Task CheckMadateDematSupportedAsync_CaseThrowExeption()
        {
            var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent("error message returned by formioApi"),
            };

            var client = new Mock<IHttpClient>(MockBehavior.Strict);
            client.Setup(c => c.GetAsync(It.IsAny<string>()))
                .Callback<string>(url =>
                {
                    url.Should().Be($"constellation/demandemandat/submission?data.bankCode=codeBankT&data.supportDigitalMandate=true");
                })
                .ReturnsAsync(httpResponseMessage)
                .Verifiable();

            client.Setup(c => c.Dispose())
                .Verifiable();

            var factory = new Mock<IFormioClientFactory>(MockBehavior.Strict);
            factory.Setup(f => f.Create())
                .Returns(client.Object)
                .Verifiable();

            var logger = new Mock<ILogger<HttpFormIoClient>>(MockBehavior.Strict);
            logger.Setup(x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsValueType>(),
                It.IsAny<Exception>(),
                (Func<It.IsValueType, Exception?, string>)It.IsAny<object>()));

            var jeDeclareClient = new HttpFormIoClient(logger.Object, factory.Object);

            Func<Task> act = async () => await jeDeclareClient.CheckMadateDematSupportedAsync("demandemandat", "codeBankT");

            await act.Should().ThrowExactlyAsync<FormioApiException>()
                .WithMessage("Exception was thrown : status code : BadRequest - Message : 'error message returned by formioApi'");

            client.VerifyAll();
            factory.VerifyAll();
        }

        [Fact]
        public async Task CheckCollecteConfigExistAsync_CaseThrowExeption()
        {
            var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent("error message returned by formioApi"),
            };

            var client = new Mock<IHttpClient>(MockBehavior.Strict);
            client.Setup(c => c.GetAsync(It.IsAny<string>()))
                .Callback<string>(url =>
                {
                    url.Should().Be($"constellation/demandemandat/exists?data.bankCode=bankCodeT&data.bankAccountNumber=bankAccountNumberT&data.bankSortCode=bankSortT");
                })
                .ReturnsAsync(httpResponseMessage)
                .Verifiable();

            client.Setup(c => c.Dispose())
                .Verifiable();

            var auth = new FormioAuthToken();

            var factory = new Mock<IFormioClientFactory>(MockBehavior.Strict);
            factory.Setup(f => f.Create(auth))
                .Returns(client.Object)
                .Verifiable();

            var logger = new Mock<ILogger<HttpFormIoClient>>(MockBehavior.Strict);
            logger.Setup(x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsValueType>(),
                It.IsAny<Exception>(),
                (Func<It.IsValueType, Exception?, string>)It.IsAny<object>()));

            var jeDeclareClient = new HttpFormIoClient(logger.Object, factory.Object);

            Func<Task> act = async () => await jeDeclareClient.CheckCollecteConfigExistAsync("demandemandat", "bankCodeT", "bankAccountNumberT", "bankSortT", auth);

            await act.Should().ThrowExactlyAsync<FormioApiException>()
                .WithMessage("Exception was thrown : status code : BadRequest - Message : 'error message returned by formioApi'");

            client.VerifyAll();
            factory.VerifyAll();
        }

        [Fact]
        public async Task GetTemplateShemaAsync_CaseThrowExeption()
        {
            var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent("error message returned by formioApi"),
            };

            var client = new Mock<IHttpClient>(MockBehavior.Strict);
            client.Setup(c => c.GetAsync(It.IsAny<string>()))
                .Callback<string>(url =>
                {
                    url.Should().Be($"project/projectIdT/form?limit=100&properties.bankCode__regex=codeBankT&select=components%2csettings%2ctitle%2cpath");
                })
                .ReturnsAsync(httpResponseMessage)
                .Verifiable();

            client.Setup(c => c.Dispose())
                .Verifiable();

            var auth = new FormioAuthToken();

            var factory = new Mock<IFormioClientFactory>(MockBehavior.Strict);
            factory.Setup(f => f.Create(auth))
                .Returns(client.Object)
                .Verifiable();

            var logger = new Mock<ILogger<HttpFormIoClient>>(MockBehavior.Strict);
            logger.Setup(x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsValueType>(),
                It.IsAny<Exception>(),
                (Func<It.IsValueType, Exception?, string>)It.IsAny<object>()));

            var jeDeclareClient = new HttpFormIoClient(logger.Object, factory.Object);

            Func<Task> act = async () => await jeDeclareClient.GetTemplateShemaAsync("projectIdT", "codeBankT", auth);

            await act.Should().ThrowExactlyAsync<FormioApiException>()
                .WithMessage("Exception was thrown : status code : BadRequest - Message : 'error message returned by formioApi'");

            client.VerifyAll();
            factory.VerifyAll();
        }

        [Fact]
        public async Task GetSubmissionByIdAsync_CaseThrowExeption()
        {
            var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent("error message returned by formioApi"),
            };

            var client = new Mock<IHttpClient>(MockBehavior.Strict);
            client.Setup(c => c.GetAsync(It.IsAny<string>()))
                .Callback<string>(url =>
                {
                    url.Should().Be($"constellation/formIdT/submission/submissionIdT");
                })
                .ReturnsAsync(httpResponseMessage)
                .Verifiable();

            client.Setup(c => c.Dispose())
                .Verifiable();

            var auth = new FormioAuthToken();

            var factory = new Mock<IFormioClientFactory>(MockBehavior.Strict);
            factory.Setup(f => f.Create(auth))
                .Returns(client.Object)
                .Verifiable();

            var logger = new Mock<ILogger<HttpFormIoClient>>(MockBehavior.Strict);
            logger.Setup(x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsValueType>(),
                It.IsAny<Exception>(),
                (Func<It.IsValueType, Exception?, string>)It.IsAny<object>()));

            var jeDeclareClient = new HttpFormIoClient(logger.Object, factory.Object);

            Func<Task> act = async () => await jeDeclareClient.GetSubmissionByIdAsync("formIdT", "submissionIdT", auth);

            await act.Should().ThrowExactlyAsync<FormioApiException>()
                .WithMessage("Exception was thrown : status code : BadRequest - Message : 'error message returned by formioApi'");

            client.VerifyAll();
            factory.VerifyAll();
        }

        [Fact]
        public async Task DownloadSubmissionAsPDFWithTemplate_CaseThrowExeption()
        {
            var fileToken = "fileTokenT";

            var pdfServerUrl = "https://toto.com";
            var projectId = "projectIdT";

            var dataString = "{\r\n  \"_id\": \"5ce57e521c42f37327ab37ab\",\r\n  \"tag\": \"0.0.0\",\r\n  \"owner\": \"631b3ed30a718a06ed019733\",\r\n  \"plan\": \"commercial\",\r\n  \"steps\": [],\r\n  \"framework\": \"vue\",\r\n  \"protect\": false,\r\n  \"title\": \"Plateforme KPMG\",\r\n  \"name\": \"constellation\",\r\n  \"access\": [\r\n    {\r\n      \"roles\": [],\r\n      \"type\": \"create_own\"\r\n    },\r\n    {\r\n      \"roles\": [\r\n        \"631b3ed30a718a06ed019733\"\r\n      ],\r\n      \"type\": \"create_all\"\r\n    },\r\n    {\r\n      \"roles\": [],\r\n      \"type\": \"read_own\"\r\n    },\r\n    {\r\n      \"roles\": [\r\n        \"5ce57e521c42f3133cab37ad\",\r\n        \"6102d5e0bb5fe8157f307716\",\r\n        \"6368ceafe017bc40e73385c0\",\r\n        \"63ea107f664a606b7bfe2106\",\r\n        \"64463a0e05c12f0615826c9b\"\r\n      ],\r\n      \"type\": \"read_all\"\r\n    },\r\n    {\r\n      \"roles\": [],\r\n      \"type\": \"update_own\"\r\n    },\r\n    {\r\n      \"roles\": [],\r\n      \"type\": \"update_all\"\r\n    },\r\n    {\r\n      \"roles\": [],\r\n      \"type\": \"delete_own\"\r\n    },\r\n    {\r\n      \"roles\": [],\r\n      \"type\": \"delete_all\"\r\n    },\r\n    {\r\n      \"roles\": [\r\n        \"5e85fdc5b093733402a5112f\"\r\n      ],\r\n      \"type\": \"team_read\"\r\n    },\r\n    {\r\n      \"roles\": [\r\n        \"631b3ed30a718a06ed019733\",\r\n        \"6363d08be017bc2ae33384fd\"\r\n      ],\r\n      \"type\": \"team_write\"\r\n    },\r\n    {\r\n      \"roles\": [\r\n        \"5ce6468b50cb83813cf0324b\"\r\n      ],\r\n      \"type\": \"team_admin\"\r\n    }\r\n  ],\r\n  \"trial\": \"2019-05-22T16:52:34.011Z\",\r\n  \"created\": \"2019-05-22T16:52:34.011Z\",\r\n  \"modified\": \"2023-07-17T14:12:59.088Z\",\r\n  \"type\": \"project\",\r\n  \"config\": {\r\n    \"ConstellationRoles\": {\r\n      \"CST-ADMIN\": \"000001\",\r\n      \"ESC-ADMIN\": \"000002\",\r\n      \"ESC-TOUS\": \"0000003\"\r\n    }\r\n  },\r\n  \"apiCalls\": {\r\n    \"used\": {\r\n      \"forms\": 0,\r\n      \"emails\": 0,\r\n      \"formRequests\": 0,\r\n      \"submissionRequests\": 0\r\n    },\r\n    \"limit\": {\r\n      \"submissionRequests\": 2000000\r\n    },\r\n    \"reset\": \"2023-11-01T00:00:00Z\"\r\n  },\r\n  \"settings\": {\r\n    \"cors\": \"*\",\r\n    \"preview\": {\r\n      \"repository\": \"https://github.com/formio/formio-app-template\",\r\n      \"url\": \"http://formio.github.io/formio-app-template/\"\r\n    },\r\n    \"filetoken\": \"aJVYt4fi8mA5p0IgvIUsXQkzKbNCql\",\r\n    \"storage\": {\r\n      \"azure\": {\r\n        \"connectionString\": \"DefaultEndpointsProtocol=https;AccountName=cstd2forms;AccountKey=acountKeyT;EndpointSuffix=core.windows.net\",\r\n        \"container\": \"form\"\r\n      }\r\n    },\r\n    \"keys\": [\r\n      {\r\n        \"name\": \"PDF Server\",\r\n        \"key\": \"eNnPcMqUZhgY4Pz7bdiewTz8KlGQG4\"\r\n      },\r\n      {\r\n        \"name\": \"Constellation\",\r\n        \"key\": \"yVOk3cX991WqFs66qoyOdxuwmWoks4\"\r\n      },\r\n      {\r\n        \"name\": \"DevOps\",\r\n        \"key\": \"0nPi1PIbPlDYQ8xaPoHLMNbzA1WUt3\"\r\n      },\r\n      {\r\n        \"name\": \"Comptes Rendus\",\r\n        \"key\": \"2sarADUvCgrq7VM6Jec5GShADTgGwH\"\r\n      },\r\n      {\r\n        \"name\": \"Akisition\",\r\n        \"key\": \"3Q0LY6RKctF6djaIIn6liRSdLYGd2s\"\r\n      },\r\n      {\r\n        \"name\": \"SalesForce\",\r\n        \"key\": \"KLRaiXGlYr9ybkHWarUMh7dRssg6u4\"\r\n      }\r\n    ],\r\n    \"pdfserver\": \"https://cst-d2-formio-pdfserver.azurewebsites.net\",\r\n    \"allowConfig\": true,\r\n    \"tokenParse\": \"\",\r\n    \"email\": {\r\n      \"sendgrid\": {\r\n        \"auth\": {\r\n          \"api_user\": \"apikey\",\r\n          \"api_key\": \"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\"\r\n        }\r\n      }\r\n    },\r\n    \"secret\": \"bX7TOMUdD64QlBtzqc83oLDrdeucwYcB\",\r\n    \"formModule\": \"\",\r\n    \"remoteSecret\": \"Sauvage1+\"\r\n  },\r\n  \"public\": {\r\n    \"formModule\": \"\",\r\n    \"custom\": {}\r\n  }\r\n}";
            var data = JToken.Parse(dataString);

            var templateSchemaString = "{\r\n  \"_id\": \"5e2b07ba8c7108d7569c2d48\",\r\n  \"components\": [\r\n    {\r\n      \"type\": \"button\",\r\n      \"label\": \"Submit\",\r\n      \"key\": \"submit\",\r\n      \"disableOnInvalid\": true,\r\n      \"input\": true,\r\n      \"tableView\": false,\r\n      \"placeholder\": \"\",\r\n      \"prefix\": \"\",\r\n      \"customClass\": \"\",\r\n      \"suffix\": \"\",\r\n      \"multiple\": false,\r\n      \"defaultValue\": null,\r\n      \"protected\": false,\r\n      \"unique\": false,\r\n      \"persistent\": false,\r\n      \"hidden\": false,\r\n      \"clearOnHide\": true,\r\n      \"refreshOn\": \"\",\r\n      \"redrawOn\": \"\",\r\n      \"modalEdit\": false,\r\n      \"labelPosition\": \"top\",\r\n      \"description\": \"\",\r\n      \"errorLabel\": \"\",\r\n      \"tooltip\": \"\",\r\n      \"hideLabel\": false,\r\n      \"tabindex\": \"\",\r\n      \"disabled\": false,\r\n      \"autofocus\": false,\r\n      \"dbIndex\": false,\r\n      \"customDefaultValue\": \"\",\r\n      \"calculateValue\": \"\",\r\n      \"widget\": {\r\n        \"type\": \"input\"\r\n      },\r\n      \"attributes\": {},\r\n      \"validateOn\": \"change\",\r\n      \"validate\": {\r\n        \"required\": false,\r\n        \"custom\": \"\",\r\n        \"customPrivate\": false,\r\n        \"strictDateValidation\": false,\r\n        \"multiple\": false,\r\n        \"unique\": false\r\n      },\r\n      \"conditional\": {\r\n        \"show\": null,\r\n        \"when\": null,\r\n        \"eq\": \"\"\r\n      },\r\n      \"overlay\": {\r\n        \"style\": \"\",\r\n        \"left\": \"\",\r\n        \"top\": \"\",\r\n        \"width\": \"\",\r\n        \"height\": \"\"\r\n      },\r\n      \"allowCalculateOverride\": false,\r\n      \"encrypted\": false,\r\n      \"showCharCount\": false,\r\n      \"showWordCount\": false,\r\n      \"properties\": {},\r\n      \"allowMultipleMasks\": false,\r\n      \"size\": \"md\",\r\n      \"leftIcon\": \"\",\r\n      \"rightIcon\": \"\",\r\n      \"block\": false,\r\n      \"action\": \"submit\",\r\n      \"theme\": \"primary\",\r\n      \"dataGridLabel\": true,\r\n      \"id\": \"edswxyf\"\r\n    },\r\n    {\r\n      \"label\": \"signatoryFullName\",\r\n      \"hidden\": true,\r\n      \"tableView\": true,\r\n      \"key\": \"signatoryFullName\",\r\n      \"overlay\": {\r\n        \"page\": 1,\r\n        \"left\": 220.5881176470588,\r\n        \"top\": 217.11776470588237,\r\n        \"width\": 495,\r\n        \"height\": 21,\r\n        \"style\": \"\"\r\n      },\r\n      \"type\": \"textfield\",\r\n      \"input\": true,\r\n      \"placeholder\": \"\",\r\n      \"prefix\": \"\",\r\n      \"customClass\": \"\",\r\n      \"suffix\": \"\",\r\n      \"multiple\": false,\r\n      \"defaultValue\": null,\r\n      \"protected\": false,\r\n      \"unique\": false,\r\n      \"persistent\": true,\r\n      \"clearOnHide\": true,\r\n      \"refreshOn\": \"\",\r\n      \"redrawOn\": \"\",\r\n      \"modalEdit\": false,\r\n      \"labelPosition\": \"top\",\r\n      \"description\": \"\",\r\n      \"errorLabel\": \"\",\r\n      \"tooltip\": \"\",\r\n      \"hideLabel\": false,\r\n      \"tabindex\": \"\",\r\n      \"disabled\": false,\r\n      \"autofocus\": false,\r\n      \"dbIndex\": false,\r\n      \"customDefaultValue\": \"\",\r\n      \"calculateValue\": \"\",\r\n      \"widget\": {\r\n        \"type\": \"input\"\r\n      },\r\n      \"attributes\": {},\r\n      \"validateOn\": \"change\",\r\n      \"validate\": {\r\n        \"required\": false,\r\n        \"custom\": \"\",\r\n        \"customPrivate\": false,\r\n        \"strictDateValidation\": false,\r\n        \"multiple\": false,\r\n        \"unique\": false,\r\n        \"minLength\": \"\",\r\n        \"maxLength\": \"\",\r\n        \"pattern\": \"\"\r\n      },\r\n      \"conditional\": {\r\n        \"show\": null,\r\n        \"when\": null,\r\n        \"eq\": \"\"\r\n      },\r\n      \"allowCalculateOverride\": false,\r\n      \"encrypted\": false,\r\n      \"showCharCount\": false,\r\n      \"showWordCount\": false,\r\n      \"properties\": {},\r\n      \"allowMultipleMasks\": false,\r\n      \"mask\": false,\r\n      \"inputType\": \"text\",\r\n      \"inputFormat\": \"plain\",\r\n      \"inputMask\": \"\",\r\n      \"id\": \"e350qxk\"\r\n    },\r\n    {\r\n      \"label\": \"companyName\",\r\n      \"hidden\": true,\r\n      \"tableView\": true,\r\n      \"key\": \"companyName\",\r\n      \"overlay\": {\r\n        \"page\": 1,\r\n        \"left\": 207,\r\n        \"top\": 260,\r\n        \"height\": 20,\r\n        \"width\": 505,\r\n        \"style\": \"\"\r\n      },\r\n      \"type\": \"textfield\",\r\n      \"input\": true,\r\n      \"placeholder\": \"\",\r\n      \"prefix\": \"\",\r\n      \"customClass\": \"\",\r\n      \"suffix\": \"\",\r\n      \"multiple\": false,\r\n      \"defaultValue\": null,\r\n      \"protected\": false,\r\n      \"unique\": false,\r\n      \"persistent\": true,\r\n      \"clearOnHide\": true,\r\n      \"refreshOn\": \"\",\r\n      \"redrawOn\": \"\",\r\n      \"modalEdit\": false,\r\n      \"labelPosition\": \"top\",\r\n      \"description\": \"\",\r\n      \"errorLabel\": \"\",\r\n      \"tooltip\": \"\",\r\n      \"hideLabel\": false,\r\n      \"tabindex\": \"\",\r\n      \"disabled\": false,\r\n      \"autofocus\": false,\r\n      \"dbIndex\": false,\r\n      \"customDefaultValue\": \"\",\r\n      \"calculateValue\": \"\",\r\n      \"widget\": {\r\n        \"type\": \"input\"\r\n      },\r\n      \"attributes\": {},\r\n      \"validateOn\": \"change\",\r\n      \"validate\": {\r\n        \"required\": false,\r\n        \"custom\": \"\",\r\n        \"customPrivate\": false,\r\n        \"strictDateValidation\": false,\r\n        \"multiple\": false,\r\n        \"unique\": false,\r\n        \"minLength\": \"\",\r\n        \"maxLength\": \"\",\r\n        \"pattern\": \"\"\r\n      },\r\n      \"conditional\": {\r\n        \"show\": null,\r\n        \"when\": null,\r\n        \"eq\": \"\"\r\n      },\r\n      \"allowCalculateOverride\": false,\r\n      \"encrypted\": false,\r\n      \"showCharCount\": false,\r\n      \"showWordCount\": false,\r\n      \"properties\": {},\r\n      \"allowMultipleMasks\": false,\r\n      \"mask\": false,\r\n      \"inputType\": \"text\",\r\n      \"inputFormat\": \"plain\",\r\n      \"inputMask\": \"\",\r\n      \"id\": \"e4339ck\"\r\n    },\r\n    {\r\n      \"label\": \"SIRET\",\r\n      \"tableView\": true,\r\n      \"key\": \"SIRETNumber\",\r\n      \"overlay\": {\r\n        \"page\": 1,\r\n        \"left\": 743,\r\n        \"top\": 278,\r\n        \"height\": 20,\r\n        \"width\": 248,\r\n        \"style\": \"\"\r\n      },\r\n      \"type\": \"textfield\",\r\n      \"input\": true,\r\n      \"placeholder\": \"\",\r\n      \"prefix\": \"\",\r\n      \"customClass\": \"\",\r\n      \"suffix\": \"\",\r\n      \"multiple\": false,\r\n      \"defaultValue\": null,\r\n      \"protected\": false,\r\n      \"unique\": false,\r\n      \"persistent\": true,\r\n      \"hidden\": false,\r\n      \"clearOnHide\": true,\r\n      \"refreshOn\": \"\",\r\n      \"redrawOn\": \"\",\r\n      \"modalEdit\": false,\r\n      \"labelPosition\": \"top\",\r\n      \"description\": \"\",\r\n      \"errorLabel\": \"\",\r\n      \"tooltip\": \"\",\r\n      \"hideLabel\": false,\r\n      \"tabindex\": \"\",\r\n      \"disabled\": false,\r\n      \"autofocus\": false,\r\n      \"dbIndex\": false,\r\n      \"customDefaultValue\": \"\",\r\n      \"calculateValue\": \"\",\r\n      \"widget\": {\r\n        \"type\": \"input\"\r\n      },\r\n      \"attributes\": {},\r\n      \"validateOn\": \"change\",\r\n      \"validate\": {\r\n        \"required\": false,\r\n        \"custom\": \"\",\r\n        \"customPrivate\": false,\r\n        \"strictDateValidation\": false,\r\n        \"multiple\": false,\r\n        \"unique\": false,\r\n        \"minLength\": \"\",\r\n        \"maxLength\": \"\",\r\n        \"pattern\": \"\"\r\n      },\r\n      \"conditional\": {\r\n        \"show\": null,\r\n        \"when\": null,\r\n        \"eq\": \"\"\r\n      },\r\n      \"allowCalculateOverride\": false,\r\n      \"encrypted\": false,\r\n      \"showCharCount\": false,\r\n      \"showWordCount\": false,\r\n      \"properties\": {},\r\n      \"allowMultipleMasks\": false,\r\n      \"mask\": false,\r\n      \"inputType\": \"text\",\r\n      \"inputFormat\": \"plain\",\r\n      \"inputMask\": \"\",\r\n      \"id\": \"ecw26c\"\r\n    },\r\n    {\r\n      \"label\": \"address1\",\r\n      \"tableView\": true,\r\n      \"key\": \"address1\",\r\n      \"overlay\": {\r\n        \"page\": 1,\r\n        \"left\": 208,\r\n        \"top\": 299,\r\n        \"width\": 783,\r\n        \"height\": 20,\r\n        \"style\": \"\"\r\n      },\r\n      \"type\": \"textfield\",\r\n      \"input\": true,\r\n      \"placeholder\": \"\",\r\n      \"prefix\": \"\",\r\n      \"customClass\": \"\",\r\n      \"suffix\": \"\",\r\n      \"multiple\": false,\r\n      \"defaultValue\": null,\r\n      \"protected\": false,\r\n      \"unique\": false,\r\n      \"persistent\": true,\r\n      \"hidden\": false,\r\n      \"clearOnHide\": true,\r\n      \"refreshOn\": \"\",\r\n      \"redrawOn\": \"\",\r\n      \"modalEdit\": false,\r\n      \"labelPosition\": \"top\",\r\n      \"description\": \"\",\r\n      \"errorLabel\": \"\",\r\n      \"tooltip\": \"\",\r\n      \"hideLabel\": false,\r\n      \"tabindex\": \"\",\r\n      \"disabled\": false,\r\n      \"autofocus\": false,\r\n      \"dbIndex\": false,\r\n      \"customDefaultValue\": \"\",\r\n      \"calculateValue\": \"\",\r\n      \"widget\": {\r\n        \"type\": \"input\"\r\n      },\r\n      \"attributes\": {},\r\n      \"validateOn\": \"change\",\r\n      \"validate\": {\r\n        \"required\": false,\r\n        \"custom\": \"\",\r\n        \"customPrivate\": false,\r\n        \"strictDateValidation\": false,\r\n        \"multiple\": false,\r\n        \"unique\": false,\r\n        \"minLength\": \"\",\r\n        \"maxLength\": \"\",\r\n        \"pattern\": \"\"\r\n      },\r\n      \"conditional\": {\r\n        \"show\": null,\r\n        \"when\": null,\r\n        \"eq\": \"\"\r\n      },\r\n      \"allowCalculateOverride\": false,\r\n      \"encrypted\": false,\r\n      \"showCharCount\": false,\r\n      \"showWordCount\": false,\r\n      \"properties\": {},\r\n      \"allowMultipleMasks\": false,\r\n      \"mask\": false,\r\n      \"inputType\": \"text\",\r\n      \"inputFormat\": \"plain\",\r\n      \"inputMask\": \"\",\r\n      \"id\": \"e57oq0v\"\r\n    },\r\n    {\r\n      \"label\": \"address2\",\r\n      \"tableView\": true,\r\n      \"key\": \"address2\",\r\n      \"overlay\": {\r\n        \"page\": 1,\r\n        \"left\": 209,\r\n        \"top\": 322,\r\n        \"width\": 782.625,\r\n        \"height\": 18.125,\r\n        \"style\": \"\"\r\n      },\r\n      \"type\": \"textfield\",\r\n      \"input\": true,\r\n      \"placeholder\": \"\",\r\n      \"prefix\": \"\",\r\n      \"customClass\": \"\",\r\n      \"suffix\": \"\",\r\n      \"multiple\": false,\r\n      \"defaultValue\": null,\r\n      \"protected\": false,\r\n      \"unique\": false,\r\n      \"persistent\": true,\r\n      \"hidden\": false,\r\n      \"clearOnHide\": true,\r\n      \"refreshOn\": \"\",\r\n      \"redrawOn\": \"\",\r\n      \"modalEdit\": false,\r\n      \"labelPosition\": \"top\",\r\n      \"description\": \"\",\r\n      \"errorLabel\": \"\",\r\n      \"tooltip\": \"\",\r\n      \"hideLabel\": false,\r\n      \"tabindex\": \"\",\r\n      \"disabled\": false,\r\n      \"autofocus\": false,\r\n      \"dbIndex\": false,\r\n      \"customDefaultValue\": \"\",\r\n      \"calculateValue\": \"\",\r\n      \"widget\": {\r\n        \"type\": \"input\"\r\n      },\r\n      \"attributes\": {},\r\n      \"validateOn\": \"change\",\r\n      \"validate\": {\r\n        \"required\": false,\r\n        \"custom\": \"\",\r\n        \"customPrivate\": false,\r\n        \"strictDateValidation\": false,\r\n        \"multiple\": false,\r\n        \"unique\": false,\r\n        \"minLength\": \"\",\r\n        \"maxLength\": \"\",\r\n        \"pattern\": \"\"\r\n      },\r\n      \"conditional\": {\r\n        \"show\": null,\r\n        \"when\": null,\r\n        \"eq\": \"\"\r\n      },\r\n      \"allowCalculateOverride\": false,\r\n      \"encrypted\": false,\r\n      \"showCharCount\": false,\r\n      \"showWordCount\": false,\r\n      \"properties\": {},\r\n      \"allowMultipleMasks\": false,\r\n      \"mask\": false,\r\n      \"inputType\": \"text\",\r\n      \"inputFormat\": \"plain\",\r\n      \"inputMask\": \"\",\r\n      \"id\": \"e45x8ps\"\r\n    },\r\n    {\r\n      \"label\": \"bankCode\",\r\n      \"tableView\": true,\r\n      \"key\": \"bankCode\",\r\n      \"overlay\": {\r\n        \"page\": 1,\r\n        \"left\": 112,\r\n        \"top\": 521.4,\r\n        \"height\": 21,\r\n        \"width\": 215,\r\n        \"style\": \"\"\r\n      },\r\n      \"type\": \"textfield\",\r\n      \"input\": true,\r\n      \"placeholder\": \"\",\r\n      \"prefix\": \"\",\r\n      \"customClass\": \"\",\r\n      \"suffix\": \"\",\r\n      \"multiple\": false,\r\n      \"defaultValue\": null,\r\n      \"protected\": false,\r\n      \"unique\": false,\r\n      \"persistent\": true,\r\n      \"hidden\": false,\r\n      \"clearOnHide\": true,\r\n      \"refreshOn\": \"\",\r\n      \"redrawOn\": \"\",\r\n      \"modalEdit\": false,\r\n      \"labelPosition\": \"top\",\r\n      \"description\": \"\",\r\n      \"errorLabel\": \"\",\r\n      \"tooltip\": \"\",\r\n      \"hideLabel\": false,\r\n      \"tabindex\": \"\",\r\n      \"disabled\": false,\r\n      \"autofocus\": false,\r\n      \"dbIndex\": false,\r\n      \"customDefaultValue\": \"\",\r\n      \"calculateValue\": \"\",\r\n      \"widget\": {\r\n        \"type\": \"input\"\r\n      },\r\n      \"attributes\": {},\r\n      \"validateOn\": \"change\",\r\n      \"validate\": {\r\n        \"required\": false,\r\n        \"custom\": \"\",\r\n        \"customPrivate\": false,\r\n        \"strictDateValidation\": false,\r\n        \"multiple\": false,\r\n        \"unique\": false,\r\n        \"minLength\": \"\",\r\n        \"maxLength\": \"\",\r\n        \"pattern\": \"\"\r\n      },\r\n      \"conditional\": {\r\n        \"show\": null,\r\n        \"when\": null,\r\n        \"eq\": \"\"\r\n      },\r\n      \"allowCalculateOverride\": false,\r\n      \"encrypted\": false,\r\n      \"showCharCount\": false,\r\n      \"showWordCount\": false,\r\n      \"properties\": {},\r\n      \"allowMultipleMasks\": false,\r\n      \"mask\": false,\r\n      \"inputType\": \"text\",\r\n      \"inputFormat\": \"plain\",\r\n      \"inputMask\": \"\",\r\n      \"id\": \"ezzpl69\"\r\n    },\r\n    {\r\n      \"label\": \"bankSortCode\",\r\n      \"hidden\": true,\r\n      \"tableView\": true,\r\n      \"key\": \"bankSortCode\",\r\n      \"overlay\": {\r\n        \"page\": 1,\r\n        \"left\": 347,\r\n        \"top\": 522,\r\n        \"width\": 206,\r\n        \"height\": 20,\r\n        \"style\": \"\"\r\n      },\r\n      \"type\": \"textfield\",\r\n      \"input\": true,\r\n      \"placeholder\": \"\",\r\n      \"prefix\": \"\",\r\n      \"customClass\": \"\",\r\n      \"suffix\": \"\",\r\n      \"multiple\": false,\r\n      \"defaultValue\": null,\r\n      \"protected\": false,\r\n      \"unique\": false,\r\n      \"persistent\": true,\r\n      \"clearOnHide\": true,\r\n      \"refreshOn\": \"\",\r\n      \"redrawOn\": \"\",\r\n      \"modalEdit\": false,\r\n      \"labelPosition\": \"top\",\r\n      \"description\": \"\",\r\n      \"errorLabel\": \"\",\r\n      \"tooltip\": \"\",\r\n      \"hideLabel\": false,\r\n      \"tabindex\": \"\",\r\n      \"disabled\": false,\r\n      \"autofocus\": false,\r\n      \"dbIndex\": false,\r\n      \"customDefaultValue\": \"\",\r\n      \"calculateValue\": \"\",\r\n      \"widget\": {\r\n        \"type\": \"input\"\r\n      },\r\n      \"attributes\": {},\r\n      \"validateOn\": \"change\",\r\n      \"validate\": {\r\n        \"required\": false,\r\n        \"custom\": \"\",\r\n        \"customPrivate\": false,\r\n        \"strictDateValidation\": false,\r\n        \"multiple\": false,\r\n        \"unique\": false,\r\n        \"minLength\": \"\",\r\n        \"maxLength\": \"\",\r\n        \"pattern\": \"\"\r\n      },\r\n      \"conditional\": {\r\n        \"show\": null,\r\n        \"when\": null,\r\n        \"eq\": \"\"\r\n      },\r\n      \"allowCalculateOverride\": false,\r\n      \"encrypted\": false,\r\n      \"showCharCount\": false,\r\n      \"showWordCount\": false,\r\n      \"properties\": {},\r\n      \"allowMultipleMasks\": false,\r\n      \"mask\": false,\r\n      \"inputType\": \"text\",\r\n      \"inputFormat\": \"plain\",\r\n      \"inputMask\": \"\",\r\n      \"id\": \"emgo9el5\"\r\n    },\r\n    {\r\n      \"label\": \"bankAccountNumber\",\r\n      \"hideLabel\": true,\r\n      \"tableView\": true,\r\n      \"key\": \"bankAccountNumber\",\r\n      \"overlay\": {\r\n        \"page\": 1,\r\n        \"left\": 576,\r\n        \"top\": 521,\r\n        \"width\": 208,\r\n        \"height\": 21,\r\n        \"style\": \"\"\r\n      },\r\n      \"type\": \"textfield\",\r\n      \"input\": true,\r\n      \"placeholder\": \"\",\r\n      \"prefix\": \"\",\r\n      \"customClass\": \"\",\r\n      \"suffix\": \"\",\r\n      \"multiple\": false,\r\n      \"defaultValue\": null,\r\n      \"protected\": false,\r\n      \"unique\": false,\r\n      \"persistent\": true,\r\n      \"hidden\": false,\r\n      \"clearOnHide\": true,\r\n      \"refreshOn\": \"\",\r\n      \"redrawOn\": \"\",\r\n      \"modalEdit\": false,\r\n      \"labelPosition\": \"top\",\r\n      \"description\": \"\",\r\n      \"errorLabel\": \"\",\r\n      \"tooltip\": \"\",\r\n      \"tabindex\": \"\",\r\n      \"disabled\": false,\r\n      \"autofocus\": false,\r\n      \"dbIndex\": false,\r\n      \"customDefaultValue\": \"\",\r\n      \"calculateValue\": \"\",\r\n      \"widget\": {\r\n        \"type\": \"input\"\r\n      },\r\n      \"attributes\": {},\r\n      \"validateOn\": \"change\",\r\n      \"validate\": {\r\n        \"required\": false,\r\n        \"custom\": \"\",\r\n        \"customPrivate\": false,\r\n        \"strictDateValidation\": false,\r\n        \"multiple\": false,\r\n        \"unique\": false,\r\n        \"minLength\": \"\",\r\n        \"maxLength\": \"\",\r\n        \"pattern\": \"\"\r\n      },\r\n      \"conditional\": {\r\n        \"show\": null,\r\n        \"when\": null,\r\n        \"eq\": \"\"\r\n      },\r\n      \"allowCalculateOverride\": false,\r\n      \"encrypted\": false,\r\n      \"showCharCount\": false,\r\n      \"showWordCount\": false,\r\n      \"properties\": {},\r\n      \"allowMultipleMasks\": false,\r\n      \"mask\": false,\r\n      \"inputType\": \"text\",\r\n      \"inputFormat\": \"plain\",\r\n      \"inputMask\": \"\",\r\n      \"id\": \"e1t094\"\r\n    },\r\n    {\r\n      \"label\": \"bankCheckNumber\",\r\n      \"hideLabel\": true,\r\n      \"tableView\": true,\r\n      \"key\": \"bankCheckNumber\",\r\n      \"overlay\": {\r\n        \"page\": 1,\r\n        \"left\": 804,\r\n        \"top\": 521.6,\r\n        \"height\": 20,\r\n        \"width\": 201,\r\n        \"style\": \"\"\r\n      },\r\n      \"type\": \"textfield\",\r\n      \"input\": true,\r\n      \"placeholder\": \"\",\r\n      \"prefix\": \"\",\r\n      \"customClass\": \"\",\r\n      \"suffix\": \"\",\r\n      \"multiple\": false,\r\n      \"defaultValue\": null,\r\n      \"protected\": false,\r\n      \"unique\": false,\r\n      \"persistent\": true,\r\n      \"hidden\": false,\r\n      \"clearOnHide\": true,\r\n      \"refreshOn\": \"\",\r\n      \"redrawOn\": \"\",\r\n      \"modalEdit\": false,\r\n      \"labelPosition\": \"top\",\r\n      \"description\": \"\",\r\n      \"errorLabel\": \"\",\r\n      \"tooltip\": \"\",\r\n      \"tabindex\": \"\",\r\n      \"disabled\": false,\r\n      \"autofocus\": false,\r\n      \"dbIndex\": false,\r\n      \"customDefaultValue\": \"\",\r\n      \"calculateValue\": \"\",\r\n      \"widget\": {\r\n        \"type\": \"input\"\r\n      },\r\n      \"attributes\": {},\r\n      \"validateOn\": \"change\",\r\n      \"validate\": {\r\n        \"required\": false,\r\n        \"custom\": \"\",\r\n        \"customPrivate\": false,\r\n        \"strictDateValidation\": false,\r\n        \"multiple\": false,\r\n        \"unique\": false,\r\n        \"minLength\": \"\",\r\n        \"maxLength\": \"\",\r\n        \"pattern\": \"\"\r\n      },\r\n      \"conditional\": {\r\n        \"show\": null,\r\n        \"when\": null,\r\n        \"eq\": \"\"\r\n      },\r\n      \"allowCalculateOverride\": false,\r\n      \"encrypted\": false,\r\n      \"showCharCount\": false,\r\n      \"showWordCount\": false,\r\n      \"properties\": {},\r\n      \"allowMultipleMasks\": false,\r\n      \"mask\": false,\r\n      \"inputType\": \"text\",\r\n      \"inputFormat\": \"plain\",\r\n      \"inputMask\": \"\",\r\n      \"id\": \"ekbn008\"\r\n    }\r\n  ],\r\n  \"settings\": {\r\n    \"pdf\": {\r\n      \"src\": \"https://cst-d2-formio-pdfserver.azurewebsites.net/pdf/5ce57e521c42f37327ab37ab/file/43f5ca2a-3fef-5788-97e4-474555ce2916\",\r\n      \"id\": \"43f5ca2a-3fef-5788-97e4-474555ce2916\"\r\n    }\r\n  },\r\n  \"title\": \"Mandat BNP\",\r\n  \"path\": \"jdc-mandat-bnp\"\r\n}";
            var form = JToken.Parse(templateSchemaString);

            var downloadUrl = $"{pdfServerUrl}/pdf/{projectId}/download";

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

            var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent("error message returned by formioApi"),
            };

            var client = new Mock<IHttpClient>(MockBehavior.Strict);
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

            var factory = new Mock<IFormioClientFactory>(MockBehavior.Strict);
            factory.Setup(f => f.Create())
                .Returns(client.Object)
                .Verifiable();

            var logger = new Mock<ILogger<HttpFormIoClient>>(MockBehavior.Strict);
            logger.Setup(x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsValueType>(),
                It.IsAny<Exception>(),
                (Func<It.IsValueType, Exception?, string>)It.IsAny<object>()));

            var jeDeclareClient = new HttpFormIoClient(logger.Object, factory.Object);

            Func<Task> act = async () => await jeDeclareClient.DownloadSubmissionAsPDFWithTemplate(form, data, downloadUrl, fileToken);

            await act.Should().ThrowExactlyAsync<FormioApiException>()
                .WithMessage("Exception was thrown : status code : BadRequest - Message : 'error message returned by formioApi'");

            client.VerifyAll();
            factory.VerifyAll();
        }

        [Fact]
        public async Task DownloadSubmissionAsPDFWithTemplate_CaseOK()
        {
            var fileToken = "fileTokenT";

            var pdfServerUrl = "https://toto.com";
            var projectId = "projectIdT";

            var dataString = "{\r\n  \"_id\": \"5ce57e521c42f37327ab37ab\",\r\n  \"tag\": \"0.0.0\",\r\n  \"owner\": \"631b3ed30a718a06ed019733\",\r\n  \"plan\": \"commercial\",\r\n  \"steps\": [],\r\n  \"framework\": \"vue\",\r\n  \"protect\": false,\r\n  \"title\": \"Plateforme KPMG\",\r\n  \"name\": \"constellation\",\r\n  \"access\": [\r\n    {\r\n      \"roles\": [],\r\n      \"type\": \"create_own\"\r\n    },\r\n    {\r\n      \"roles\": [\r\n        \"631b3ed30a718a06ed019733\"\r\n      ],\r\n      \"type\": \"create_all\"\r\n    },\r\n    {\r\n      \"roles\": [],\r\n      \"type\": \"read_own\"\r\n    },\r\n    {\r\n      \"roles\": [\r\n        \"5ce57e521c42f3133cab37ad\",\r\n        \"6102d5e0bb5fe8157f307716\",\r\n        \"6368ceafe017bc40e73385c0\",\r\n        \"63ea107f664a606b7bfe2106\",\r\n        \"64463a0e05c12f0615826c9b\"\r\n      ],\r\n      \"type\": \"read_all\"\r\n    },\r\n    {\r\n      \"roles\": [],\r\n      \"type\": \"update_own\"\r\n    },\r\n    {\r\n      \"roles\": [],\r\n      \"type\": \"update_all\"\r\n    },\r\n    {\r\n      \"roles\": [],\r\n      \"type\": \"delete_own\"\r\n    },\r\n    {\r\n      \"roles\": [],\r\n      \"type\": \"delete_all\"\r\n    },\r\n    {\r\n      \"roles\": [\r\n        \"5e85fdc5b093733402a5112f\"\r\n      ],\r\n      \"type\": \"team_read\"\r\n    },\r\n    {\r\n      \"roles\": [\r\n        \"631b3ed30a718a06ed019733\",\r\n        \"6363d08be017bc2ae33384fd\"\r\n      ],\r\n      \"type\": \"team_write\"\r\n    },\r\n    {\r\n      \"roles\": [\r\n        \"5ce6468b50cb83813cf0324b\"\r\n      ],\r\n      \"type\": \"team_admin\"\r\n    }\r\n  ],\r\n  \"trial\": \"2019-05-22T16:52:34.011Z\",\r\n  \"created\": \"2019-05-22T16:52:34.011Z\",\r\n  \"modified\": \"2023-07-17T14:12:59.088Z\",\r\n  \"type\": \"project\",\r\n  \"config\": {\r\n    \"ConstellationRoles\": {\r\n      \"CST-ADMIN\": \"000001\",\r\n      \"ESC-ADMIN\": \"000002\",\r\n      \"ESC-TOUS\": \"0000003\"\r\n    }\r\n  },\r\n  \"apiCalls\": {\r\n    \"used\": {\r\n      \"forms\": 0,\r\n      \"emails\": 0,\r\n      \"formRequests\": 0,\r\n      \"submissionRequests\": 0\r\n    },\r\n    \"limit\": {\r\n      \"submissionRequests\": 2000000\r\n    },\r\n    \"reset\": \"2023-11-01T00:00:00Z\"\r\n  },\r\n  \"settings\": {\r\n    \"cors\": \"*\",\r\n    \"preview\": {\r\n      \"repository\": \"https://github.com/formio/formio-app-template\",\r\n      \"url\": \"http://formio.github.io/formio-app-template/\"\r\n    },\r\n    \"filetoken\": \"aJVYt4fi8mA5p0IgvIUsXQkzKbNCql\",\r\n    \"storage\": {\r\n      \"azure\": {\r\n        \"connectionString\": \"DefaultEndpointsProtocol=https;AccountName=cstd2forms;AccountKey=acountKeyT;EndpointSuffix=core.windows.net\",\r\n        \"container\": \"form\"\r\n      }\r\n    },\r\n    \"keys\": [\r\n      {\r\n        \"name\": \"PDF Server\",\r\n        \"key\": \"eNnPcMqUZhgY4Pz7bdiewTz8KlGQG4\"\r\n      },\r\n      {\r\n        \"name\": \"Constellation\",\r\n        \"key\": \"yVOk3cX991WqFs66qoyOdxuwmWoks4\"\r\n      },\r\n      {\r\n        \"name\": \"DevOps\",\r\n        \"key\": \"0nPi1PIbPlDYQ8xaPoHLMNbzA1WUt3\"\r\n      },\r\n      {\r\n        \"name\": \"Comptes Rendus\",\r\n        \"key\": \"2sarADUvCgrq7VM6Jec5GShADTgGwH\"\r\n      },\r\n      {\r\n        \"name\": \"Akisition\",\r\n        \"key\": \"3Q0LY6RKctF6djaIIn6liRSdLYGd2s\"\r\n      },\r\n      {\r\n        \"name\": \"SalesForce\",\r\n        \"key\": \"KLRaiXGlYr9ybkHWarUMh7dRssg6u4\"\r\n      }\r\n    ],\r\n    \"pdfserver\": \"https://cst-d2-formio-pdfserver.azurewebsites.net\",\r\n    \"allowConfig\": true,\r\n    \"tokenParse\": \"\",\r\n    \"email\": {\r\n      \"sendgrid\": {\r\n        \"auth\": {\r\n          \"api_user\": \"apikey\",\r\n          \"api_key\": \"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\"\r\n        }\r\n      }\r\n    },\r\n    \"secret\": \"bX7TOMUdD64QlBtzqc83oLDrdeucwYcB\",\r\n    \"formModule\": \"\",\r\n    \"remoteSecret\": \"Sauvage1+\"\r\n  },\r\n  \"public\": {\r\n    \"formModule\": \"\",\r\n    \"custom\": {}\r\n  }\r\n}";
            var data = JToken.Parse(dataString);

            var templateSchemaString = "{\r\n  \"_id\": \"5e2b07ba8c7108d7569c2d48\",\r\n  \"components\": [\r\n    {\r\n      \"type\": \"button\",\r\n      \"label\": \"Submit\",\r\n      \"key\": \"submit\",\r\n      \"disableOnInvalid\": true,\r\n      \"input\": true,\r\n      \"tableView\": false,\r\n      \"placeholder\": \"\",\r\n      \"prefix\": \"\",\r\n      \"customClass\": \"\",\r\n      \"suffix\": \"\",\r\n      \"multiple\": false,\r\n      \"defaultValue\": null,\r\n      \"protected\": false,\r\n      \"unique\": false,\r\n      \"persistent\": false,\r\n      \"hidden\": false,\r\n      \"clearOnHide\": true,\r\n      \"refreshOn\": \"\",\r\n      \"redrawOn\": \"\",\r\n      \"modalEdit\": false,\r\n      \"labelPosition\": \"top\",\r\n      \"description\": \"\",\r\n      \"errorLabel\": \"\",\r\n      \"tooltip\": \"\",\r\n      \"hideLabel\": false,\r\n      \"tabindex\": \"\",\r\n      \"disabled\": false,\r\n      \"autofocus\": false,\r\n      \"dbIndex\": false,\r\n      \"customDefaultValue\": \"\",\r\n      \"calculateValue\": \"\",\r\n      \"widget\": {\r\n        \"type\": \"input\"\r\n      },\r\n      \"attributes\": {},\r\n      \"validateOn\": \"change\",\r\n      \"validate\": {\r\n        \"required\": false,\r\n        \"custom\": \"\",\r\n        \"customPrivate\": false,\r\n        \"strictDateValidation\": false,\r\n        \"multiple\": false,\r\n        \"unique\": false\r\n      },\r\n      \"conditional\": {\r\n        \"show\": null,\r\n        \"when\": null,\r\n        \"eq\": \"\"\r\n      },\r\n      \"overlay\": {\r\n        \"style\": \"\",\r\n        \"left\": \"\",\r\n        \"top\": \"\",\r\n        \"width\": \"\",\r\n        \"height\": \"\"\r\n      },\r\n      \"allowCalculateOverride\": false,\r\n      \"encrypted\": false,\r\n      \"showCharCount\": false,\r\n      \"showWordCount\": false,\r\n      \"properties\": {},\r\n      \"allowMultipleMasks\": false,\r\n      \"size\": \"md\",\r\n      \"leftIcon\": \"\",\r\n      \"rightIcon\": \"\",\r\n      \"block\": false,\r\n      \"action\": \"submit\",\r\n      \"theme\": \"primary\",\r\n      \"dataGridLabel\": true,\r\n      \"id\": \"edswxyf\"\r\n    },\r\n    {\r\n      \"label\": \"signatoryFullName\",\r\n      \"hidden\": true,\r\n      \"tableView\": true,\r\n      \"key\": \"signatoryFullName\",\r\n      \"overlay\": {\r\n        \"page\": 1,\r\n        \"left\": 220.5881176470588,\r\n        \"top\": 217.11776470588237,\r\n        \"width\": 495,\r\n        \"height\": 21,\r\n        \"style\": \"\"\r\n      },\r\n      \"type\": \"textfield\",\r\n      \"input\": true,\r\n      \"placeholder\": \"\",\r\n      \"prefix\": \"\",\r\n      \"customClass\": \"\",\r\n      \"suffix\": \"\",\r\n      \"multiple\": false,\r\n      \"defaultValue\": null,\r\n      \"protected\": false,\r\n      \"unique\": false,\r\n      \"persistent\": true,\r\n      \"clearOnHide\": true,\r\n      \"refreshOn\": \"\",\r\n      \"redrawOn\": \"\",\r\n      \"modalEdit\": false,\r\n      \"labelPosition\": \"top\",\r\n      \"description\": \"\",\r\n      \"errorLabel\": \"\",\r\n      \"tooltip\": \"\",\r\n      \"hideLabel\": false,\r\n      \"tabindex\": \"\",\r\n      \"disabled\": false,\r\n      \"autofocus\": false,\r\n      \"dbIndex\": false,\r\n      \"customDefaultValue\": \"\",\r\n      \"calculateValue\": \"\",\r\n      \"widget\": {\r\n        \"type\": \"input\"\r\n      },\r\n      \"attributes\": {},\r\n      \"validateOn\": \"change\",\r\n      \"validate\": {\r\n        \"required\": false,\r\n        \"custom\": \"\",\r\n        \"customPrivate\": false,\r\n        \"strictDateValidation\": false,\r\n        \"multiple\": false,\r\n        \"unique\": false,\r\n        \"minLength\": \"\",\r\n        \"maxLength\": \"\",\r\n        \"pattern\": \"\"\r\n      },\r\n      \"conditional\": {\r\n        \"show\": null,\r\n        \"when\": null,\r\n        \"eq\": \"\"\r\n      },\r\n      \"allowCalculateOverride\": false,\r\n      \"encrypted\": false,\r\n      \"showCharCount\": false,\r\n      \"showWordCount\": false,\r\n      \"properties\": {},\r\n      \"allowMultipleMasks\": false,\r\n      \"mask\": false,\r\n      \"inputType\": \"text\",\r\n      \"inputFormat\": \"plain\",\r\n      \"inputMask\": \"\",\r\n      \"id\": \"e350qxk\"\r\n    },\r\n    {\r\n      \"label\": \"companyName\",\r\n      \"hidden\": true,\r\n      \"tableView\": true,\r\n      \"key\": \"companyName\",\r\n      \"overlay\": {\r\n        \"page\": 1,\r\n        \"left\": 207,\r\n        \"top\": 260,\r\n        \"height\": 20,\r\n        \"width\": 505,\r\n        \"style\": \"\"\r\n      },\r\n      \"type\": \"textfield\",\r\n      \"input\": true,\r\n      \"placeholder\": \"\",\r\n      \"prefix\": \"\",\r\n      \"customClass\": \"\",\r\n      \"suffix\": \"\",\r\n      \"multiple\": false,\r\n      \"defaultValue\": null,\r\n      \"protected\": false,\r\n      \"unique\": false,\r\n      \"persistent\": true,\r\n      \"clearOnHide\": true,\r\n      \"refreshOn\": \"\",\r\n      \"redrawOn\": \"\",\r\n      \"modalEdit\": false,\r\n      \"labelPosition\": \"top\",\r\n      \"description\": \"\",\r\n      \"errorLabel\": \"\",\r\n      \"tooltip\": \"\",\r\n      \"hideLabel\": false,\r\n      \"tabindex\": \"\",\r\n      \"disabled\": false,\r\n      \"autofocus\": false,\r\n      \"dbIndex\": false,\r\n      \"customDefaultValue\": \"\",\r\n      \"calculateValue\": \"\",\r\n      \"widget\": {\r\n        \"type\": \"input\"\r\n      },\r\n      \"attributes\": {},\r\n      \"validateOn\": \"change\",\r\n      \"validate\": {\r\n        \"required\": false,\r\n        \"custom\": \"\",\r\n        \"customPrivate\": false,\r\n        \"strictDateValidation\": false,\r\n        \"multiple\": false,\r\n        \"unique\": false,\r\n        \"minLength\": \"\",\r\n        \"maxLength\": \"\",\r\n        \"pattern\": \"\"\r\n      },\r\n      \"conditional\": {\r\n        \"show\": null,\r\n        \"when\": null,\r\n        \"eq\": \"\"\r\n      },\r\n      \"allowCalculateOverride\": false,\r\n      \"encrypted\": false,\r\n      \"showCharCount\": false,\r\n      \"showWordCount\": false,\r\n      \"properties\": {},\r\n      \"allowMultipleMasks\": false,\r\n      \"mask\": false,\r\n      \"inputType\": \"text\",\r\n      \"inputFormat\": \"plain\",\r\n      \"inputMask\": \"\",\r\n      \"id\": \"e4339ck\"\r\n    },\r\n    {\r\n      \"label\": \"SIRET\",\r\n      \"tableView\": true,\r\n      \"key\": \"SIRETNumber\",\r\n      \"overlay\": {\r\n        \"page\": 1,\r\n        \"left\": 743,\r\n        \"top\": 278,\r\n        \"height\": 20,\r\n        \"width\": 248,\r\n        \"style\": \"\"\r\n      },\r\n      \"type\": \"textfield\",\r\n      \"input\": true,\r\n      \"placeholder\": \"\",\r\n      \"prefix\": \"\",\r\n      \"customClass\": \"\",\r\n      \"suffix\": \"\",\r\n      \"multiple\": false,\r\n      \"defaultValue\": null,\r\n      \"protected\": false,\r\n      \"unique\": false,\r\n      \"persistent\": true,\r\n      \"hidden\": false,\r\n      \"clearOnHide\": true,\r\n      \"refreshOn\": \"\",\r\n      \"redrawOn\": \"\",\r\n      \"modalEdit\": false,\r\n      \"labelPosition\": \"top\",\r\n      \"description\": \"\",\r\n      \"errorLabel\": \"\",\r\n      \"tooltip\": \"\",\r\n      \"hideLabel\": false,\r\n      \"tabindex\": \"\",\r\n      \"disabled\": false,\r\n      \"autofocus\": false,\r\n      \"dbIndex\": false,\r\n      \"customDefaultValue\": \"\",\r\n      \"calculateValue\": \"\",\r\n      \"widget\": {\r\n        \"type\": \"input\"\r\n      },\r\n      \"attributes\": {},\r\n      \"validateOn\": \"change\",\r\n      \"validate\": {\r\n        \"required\": false,\r\n        \"custom\": \"\",\r\n        \"customPrivate\": false,\r\n        \"strictDateValidation\": false,\r\n        \"multiple\": false,\r\n        \"unique\": false,\r\n        \"minLength\": \"\",\r\n        \"maxLength\": \"\",\r\n        \"pattern\": \"\"\r\n      },\r\n      \"conditional\": {\r\n        \"show\": null,\r\n        \"when\": null,\r\n        \"eq\": \"\"\r\n      },\r\n      \"allowCalculateOverride\": false,\r\n      \"encrypted\": false,\r\n      \"showCharCount\": false,\r\n      \"showWordCount\": false,\r\n      \"properties\": {},\r\n      \"allowMultipleMasks\": false,\r\n      \"mask\": false,\r\n      \"inputType\": \"text\",\r\n      \"inputFormat\": \"plain\",\r\n      \"inputMask\": \"\",\r\n      \"id\": \"ecw26c\"\r\n    },\r\n    {\r\n      \"label\": \"address1\",\r\n      \"tableView\": true,\r\n      \"key\": \"address1\",\r\n      \"overlay\": {\r\n        \"page\": 1,\r\n        \"left\": 208,\r\n        \"top\": 299,\r\n        \"width\": 783,\r\n        \"height\": 20,\r\n        \"style\": \"\"\r\n      },\r\n      \"type\": \"textfield\",\r\n      \"input\": true,\r\n      \"placeholder\": \"\",\r\n      \"prefix\": \"\",\r\n      \"customClass\": \"\",\r\n      \"suffix\": \"\",\r\n      \"multiple\": false,\r\n      \"defaultValue\": null,\r\n      \"protected\": false,\r\n      \"unique\": false,\r\n      \"persistent\": true,\r\n      \"hidden\": false,\r\n      \"clearOnHide\": true,\r\n      \"refreshOn\": \"\",\r\n      \"redrawOn\": \"\",\r\n      \"modalEdit\": false,\r\n      \"labelPosition\": \"top\",\r\n      \"description\": \"\",\r\n      \"errorLabel\": \"\",\r\n      \"tooltip\": \"\",\r\n      \"hideLabel\": false,\r\n      \"tabindex\": \"\",\r\n      \"disabled\": false,\r\n      \"autofocus\": false,\r\n      \"dbIndex\": false,\r\n      \"customDefaultValue\": \"\",\r\n      \"calculateValue\": \"\",\r\n      \"widget\": {\r\n        \"type\": \"input\"\r\n      },\r\n      \"attributes\": {},\r\n      \"validateOn\": \"change\",\r\n      \"validate\": {\r\n        \"required\": false,\r\n        \"custom\": \"\",\r\n        \"customPrivate\": false,\r\n        \"strictDateValidation\": false,\r\n        \"multiple\": false,\r\n        \"unique\": false,\r\n        \"minLength\": \"\",\r\n        \"maxLength\": \"\",\r\n        \"pattern\": \"\"\r\n      },\r\n      \"conditional\": {\r\n        \"show\": null,\r\n        \"when\": null,\r\n        \"eq\": \"\"\r\n      },\r\n      \"allowCalculateOverride\": false,\r\n      \"encrypted\": false,\r\n      \"showCharCount\": false,\r\n      \"showWordCount\": false,\r\n      \"properties\": {},\r\n      \"allowMultipleMasks\": false,\r\n      \"mask\": false,\r\n      \"inputType\": \"text\",\r\n      \"inputFormat\": \"plain\",\r\n      \"inputMask\": \"\",\r\n      \"id\": \"e57oq0v\"\r\n    },\r\n    {\r\n      \"label\": \"address2\",\r\n      \"tableView\": true,\r\n      \"key\": \"address2\",\r\n      \"overlay\": {\r\n        \"page\": 1,\r\n        \"left\": 209,\r\n        \"top\": 322,\r\n        \"width\": 782.625,\r\n        \"height\": 18.125,\r\n        \"style\": \"\"\r\n      },\r\n      \"type\": \"textfield\",\r\n      \"input\": true,\r\n      \"placeholder\": \"\",\r\n      \"prefix\": \"\",\r\n      \"customClass\": \"\",\r\n      \"suffix\": \"\",\r\n      \"multiple\": false,\r\n      \"defaultValue\": null,\r\n      \"protected\": false,\r\n      \"unique\": false,\r\n      \"persistent\": true,\r\n      \"hidden\": false,\r\n      \"clearOnHide\": true,\r\n      \"refreshOn\": \"\",\r\n      \"redrawOn\": \"\",\r\n      \"modalEdit\": false,\r\n      \"labelPosition\": \"top\",\r\n      \"description\": \"\",\r\n      \"errorLabel\": \"\",\r\n      \"tooltip\": \"\",\r\n      \"hideLabel\": false,\r\n      \"tabindex\": \"\",\r\n      \"disabled\": false,\r\n      \"autofocus\": false,\r\n      \"dbIndex\": false,\r\n      \"customDefaultValue\": \"\",\r\n      \"calculateValue\": \"\",\r\n      \"widget\": {\r\n        \"type\": \"input\"\r\n      },\r\n      \"attributes\": {},\r\n      \"validateOn\": \"change\",\r\n      \"validate\": {\r\n        \"required\": false,\r\n        \"custom\": \"\",\r\n        \"customPrivate\": false,\r\n        \"strictDateValidation\": false,\r\n        \"multiple\": false,\r\n        \"unique\": false,\r\n        \"minLength\": \"\",\r\n        \"maxLength\": \"\",\r\n        \"pattern\": \"\"\r\n      },\r\n      \"conditional\": {\r\n        \"show\": null,\r\n        \"when\": null,\r\n        \"eq\": \"\"\r\n      },\r\n      \"allowCalculateOverride\": false,\r\n      \"encrypted\": false,\r\n      \"showCharCount\": false,\r\n      \"showWordCount\": false,\r\n      \"properties\": {},\r\n      \"allowMultipleMasks\": false,\r\n      \"mask\": false,\r\n      \"inputType\": \"text\",\r\n      \"inputFormat\": \"plain\",\r\n      \"inputMask\": \"\",\r\n      \"id\": \"e45x8ps\"\r\n    },\r\n    {\r\n      \"label\": \"bankCode\",\r\n      \"tableView\": true,\r\n      \"key\": \"bankCode\",\r\n      \"overlay\": {\r\n        \"page\": 1,\r\n        \"left\": 112,\r\n        \"top\": 521.4,\r\n        \"height\": 21,\r\n        \"width\": 215,\r\n        \"style\": \"\"\r\n      },\r\n      \"type\": \"textfield\",\r\n      \"input\": true,\r\n      \"placeholder\": \"\",\r\n      \"prefix\": \"\",\r\n      \"customClass\": \"\",\r\n      \"suffix\": \"\",\r\n      \"multiple\": false,\r\n      \"defaultValue\": null,\r\n      \"protected\": false,\r\n      \"unique\": false,\r\n      \"persistent\": true,\r\n      \"hidden\": false,\r\n      \"clearOnHide\": true,\r\n      \"refreshOn\": \"\",\r\n      \"redrawOn\": \"\",\r\n      \"modalEdit\": false,\r\n      \"labelPosition\": \"top\",\r\n      \"description\": \"\",\r\n      \"errorLabel\": \"\",\r\n      \"tooltip\": \"\",\r\n      \"hideLabel\": false,\r\n      \"tabindex\": \"\",\r\n      \"disabled\": false,\r\n      \"autofocus\": false,\r\n      \"dbIndex\": false,\r\n      \"customDefaultValue\": \"\",\r\n      \"calculateValue\": \"\",\r\n      \"widget\": {\r\n        \"type\": \"input\"\r\n      },\r\n      \"attributes\": {},\r\n      \"validateOn\": \"change\",\r\n      \"validate\": {\r\n        \"required\": false,\r\n        \"custom\": \"\",\r\n        \"customPrivate\": false,\r\n        \"strictDateValidation\": false,\r\n        \"multiple\": false,\r\n        \"unique\": false,\r\n        \"minLength\": \"\",\r\n        \"maxLength\": \"\",\r\n        \"pattern\": \"\"\r\n      },\r\n      \"conditional\": {\r\n        \"show\": null,\r\n        \"when\": null,\r\n        \"eq\": \"\"\r\n      },\r\n      \"allowCalculateOverride\": false,\r\n      \"encrypted\": false,\r\n      \"showCharCount\": false,\r\n      \"showWordCount\": false,\r\n      \"properties\": {},\r\n      \"allowMultipleMasks\": false,\r\n      \"mask\": false,\r\n      \"inputType\": \"text\",\r\n      \"inputFormat\": \"plain\",\r\n      \"inputMask\": \"\",\r\n      \"id\": \"ezzpl69\"\r\n    },\r\n    {\r\n      \"label\": \"bankSortCode\",\r\n      \"hidden\": true,\r\n      \"tableView\": true,\r\n      \"key\": \"bankSortCode\",\r\n      \"overlay\": {\r\n        \"page\": 1,\r\n        \"left\": 347,\r\n        \"top\": 522,\r\n        \"width\": 206,\r\n        \"height\": 20,\r\n        \"style\": \"\"\r\n      },\r\n      \"type\": \"textfield\",\r\n      \"input\": true,\r\n      \"placeholder\": \"\",\r\n      \"prefix\": \"\",\r\n      \"customClass\": \"\",\r\n      \"suffix\": \"\",\r\n      \"multiple\": false,\r\n      \"defaultValue\": null,\r\n      \"protected\": false,\r\n      \"unique\": false,\r\n      \"persistent\": true,\r\n      \"clearOnHide\": true,\r\n      \"refreshOn\": \"\",\r\n      \"redrawOn\": \"\",\r\n      \"modalEdit\": false,\r\n      \"labelPosition\": \"top\",\r\n      \"description\": \"\",\r\n      \"errorLabel\": \"\",\r\n      \"tooltip\": \"\",\r\n      \"hideLabel\": false,\r\n      \"tabindex\": \"\",\r\n      \"disabled\": false,\r\n      \"autofocus\": false,\r\n      \"dbIndex\": false,\r\n      \"customDefaultValue\": \"\",\r\n      \"calculateValue\": \"\",\r\n      \"widget\": {\r\n        \"type\": \"input\"\r\n      },\r\n      \"attributes\": {},\r\n      \"validateOn\": \"change\",\r\n      \"validate\": {\r\n        \"required\": false,\r\n        \"custom\": \"\",\r\n        \"customPrivate\": false,\r\n        \"strictDateValidation\": false,\r\n        \"multiple\": false,\r\n        \"unique\": false,\r\n        \"minLength\": \"\",\r\n        \"maxLength\": \"\",\r\n        \"pattern\": \"\"\r\n      },\r\n      \"conditional\": {\r\n        \"show\": null,\r\n        \"when\": null,\r\n        \"eq\": \"\"\r\n      },\r\n      \"allowCalculateOverride\": false,\r\n      \"encrypted\": false,\r\n      \"showCharCount\": false,\r\n      \"showWordCount\": false,\r\n      \"properties\": {},\r\n      \"allowMultipleMasks\": false,\r\n      \"mask\": false,\r\n      \"inputType\": \"text\",\r\n      \"inputFormat\": \"plain\",\r\n      \"inputMask\": \"\",\r\n      \"id\": \"emgo9el5\"\r\n    },\r\n    {\r\n      \"label\": \"bankAccountNumber\",\r\n      \"hideLabel\": true,\r\n      \"tableView\": true,\r\n      \"key\": \"bankAccountNumber\",\r\n      \"overlay\": {\r\n        \"page\": 1,\r\n        \"left\": 576,\r\n        \"top\": 521,\r\n        \"width\": 208,\r\n        \"height\": 21,\r\n        \"style\": \"\"\r\n      },\r\n      \"type\": \"textfield\",\r\n      \"input\": true,\r\n      \"placeholder\": \"\",\r\n      \"prefix\": \"\",\r\n      \"customClass\": \"\",\r\n      \"suffix\": \"\",\r\n      \"multiple\": false,\r\n      \"defaultValue\": null,\r\n      \"protected\": false,\r\n      \"unique\": false,\r\n      \"persistent\": true,\r\n      \"hidden\": false,\r\n      \"clearOnHide\": true,\r\n      \"refreshOn\": \"\",\r\n      \"redrawOn\": \"\",\r\n      \"modalEdit\": false,\r\n      \"labelPosition\": \"top\",\r\n      \"description\": \"\",\r\n      \"errorLabel\": \"\",\r\n      \"tooltip\": \"\",\r\n      \"tabindex\": \"\",\r\n      \"disabled\": false,\r\n      \"autofocus\": false,\r\n      \"dbIndex\": false,\r\n      \"customDefaultValue\": \"\",\r\n      \"calculateValue\": \"\",\r\n      \"widget\": {\r\n        \"type\": \"input\"\r\n      },\r\n      \"attributes\": {},\r\n      \"validateOn\": \"change\",\r\n      \"validate\": {\r\n        \"required\": false,\r\n        \"custom\": \"\",\r\n        \"customPrivate\": false,\r\n        \"strictDateValidation\": false,\r\n        \"multiple\": false,\r\n        \"unique\": false,\r\n        \"minLength\": \"\",\r\n        \"maxLength\": \"\",\r\n        \"pattern\": \"\"\r\n      },\r\n      \"conditional\": {\r\n        \"show\": null,\r\n        \"when\": null,\r\n        \"eq\": \"\"\r\n      },\r\n      \"allowCalculateOverride\": false,\r\n      \"encrypted\": false,\r\n      \"showCharCount\": false,\r\n      \"showWordCount\": false,\r\n      \"properties\": {},\r\n      \"allowMultipleMasks\": false,\r\n      \"mask\": false,\r\n      \"inputType\": \"text\",\r\n      \"inputFormat\": \"plain\",\r\n      \"inputMask\": \"\",\r\n      \"id\": \"e1t094\"\r\n    },\r\n    {\r\n      \"label\": \"bankCheckNumber\",\r\n      \"hideLabel\": true,\r\n      \"tableView\": true,\r\n      \"key\": \"bankCheckNumber\",\r\n      \"overlay\": {\r\n        \"page\": 1,\r\n        \"left\": 804,\r\n        \"top\": 521.6,\r\n        \"height\": 20,\r\n        \"width\": 201,\r\n        \"style\": \"\"\r\n      },\r\n      \"type\": \"textfield\",\r\n      \"input\": true,\r\n      \"placeholder\": \"\",\r\n      \"prefix\": \"\",\r\n      \"customClass\": \"\",\r\n      \"suffix\": \"\",\r\n      \"multiple\": false,\r\n      \"defaultValue\": null,\r\n      \"protected\": false,\r\n      \"unique\": false,\r\n      \"persistent\": true,\r\n      \"hidden\": false,\r\n      \"clearOnHide\": true,\r\n      \"refreshOn\": \"\",\r\n      \"redrawOn\": \"\",\r\n      \"modalEdit\": false,\r\n      \"labelPosition\": \"top\",\r\n      \"description\": \"\",\r\n      \"errorLabel\": \"\",\r\n      \"tooltip\": \"\",\r\n      \"tabindex\": \"\",\r\n      \"disabled\": false,\r\n      \"autofocus\": false,\r\n      \"dbIndex\": false,\r\n      \"customDefaultValue\": \"\",\r\n      \"calculateValue\": \"\",\r\n      \"widget\": {\r\n        \"type\": \"input\"\r\n      },\r\n      \"attributes\": {},\r\n      \"validateOn\": \"change\",\r\n      \"validate\": {\r\n        \"required\": false,\r\n        \"custom\": \"\",\r\n        \"customPrivate\": false,\r\n        \"strictDateValidation\": false,\r\n        \"multiple\": false,\r\n        \"unique\": false,\r\n        \"minLength\": \"\",\r\n        \"maxLength\": \"\",\r\n        \"pattern\": \"\"\r\n      },\r\n      \"conditional\": {\r\n        \"show\": null,\r\n        \"when\": null,\r\n        \"eq\": \"\"\r\n      },\r\n      \"allowCalculateOverride\": false,\r\n      \"encrypted\": false,\r\n      \"showCharCount\": false,\r\n      \"showWordCount\": false,\r\n      \"properties\": {},\r\n      \"allowMultipleMasks\": false,\r\n      \"mask\": false,\r\n      \"inputType\": \"text\",\r\n      \"inputFormat\": \"plain\",\r\n      \"inputMask\": \"\",\r\n      \"id\": \"ekbn008\"\r\n    }\r\n  ],\r\n  \"settings\": {\r\n    \"pdf\": {\r\n      \"src\": \"https://cst-d2-formio-pdfserver.azurewebsites.net/pdf/5ce57e521c42f37327ab37ab/file/43f5ca2a-3fef-5788-97e4-474555ce2916\",\r\n      \"id\": \"43f5ca2a-3fef-5788-97e4-474555ce2916\"\r\n    }\r\n  },\r\n  \"title\": \"Mandat BNP\",\r\n  \"path\": \"jdc-mandat-bnp\"\r\n}";
            var form = JToken.Parse(templateSchemaString);

            var downloadUrl = $"{pdfServerUrl}/pdf/{projectId}/download";

            var request = JToken.FromObject(new
            {
                form,
                submission = data,
            });

            var httpContent = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

            var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.Created)
            {
                Content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json"),
            };

            var client = new Mock<IHttpClient>(MockBehavior.Strict);
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

            var factory = new Mock<IFormioClientFactory>(MockBehavior.Strict);
            factory.Setup(f => f.Create())
                .Returns(client.Object)
                .Verifiable();

            var logger = new Mock<ILogger<HttpFormIoClient>>(MockBehavior.Strict);
            logger.Setup(x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsValueType>(),
                It.IsAny<Exception>(),
                (Func<It.IsValueType, Exception?, string>)It.IsAny<object>()));

            var jeDeclareClient = new HttpFormIoClient(logger.Object, factory.Object);

            var result = await jeDeclareClient.DownloadSubmissionAsPDFWithTemplate(form, data, downloadUrl, fileToken);

            result.Should().NotBeNull();
            result.Data.Should().NotBeNull();
            result.Id.Should().Be("5ce57e521c42f37327ab37ab");

            client.VerifyAll();
            factory.VerifyAll();
        }

        [Fact]
        public async Task GetProjectDefinitionAsync_CaseThrowExeption()
        {
            var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent("error message returned by formioApi"),
            };

            var client = new Mock<IHttpClient>(MockBehavior.Strict);
            client.Setup(c => c.GetAsync(It.IsAny<string>()))
                .Callback<string>(url =>
                {
                    url.Should().Be($"project/demandemandat");
                })
                .ReturnsAsync(httpResponseMessage)
                .Verifiable();

            client.Setup(c => c.Dispose())
                .Verifiable();

            var auth = new FormioAuthToken();

            var factory = new Mock<IFormioClientFactory>(MockBehavior.Strict);
            factory.Setup(f => f.Create(auth))
                .Returns(client.Object)
                .Verifiable();

            var logger = new Mock<ILogger<HttpFormIoClient>>(MockBehavior.Strict);
            logger.Setup(x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsValueType>(),
                It.IsAny<Exception>(),
                (Func<It.IsValueType, Exception?, string>)It.IsAny<object>()));

            var jeDeclareClient = new HttpFormIoClient(logger.Object, factory.Object);

            Func<Task> act = async () => await jeDeclareClient.GetProjectDefinitionAsync("demandemandat", auth);

            await act.Should().ThrowExactlyAsync<FormioApiException>()
                .WithMessage("Exception was thrown : status code : BadRequest - Message : 'error message returned by formioApi'");

            client.VerifyAll();
            factory.VerifyAll();
        }

        [Fact]
        public async Task GetSubmissionMandateAsync_CaseOK()
        {
            var submission1 = new FormioSubmission()
            {
                Id = "id1",
                Modified = "modified1",
                Owner = "owner1",
                Created = "created1",
                Data = new
                {
                    bankCode = "123",
                    bankSortCode = "456",
                    bankAccountNumber = "789",
                    bankCheckNumber = "46",
                },
            };

            var formioSubmissionCollection = new FormioSubmissionCollection()
            {
                Limit = 0,
                Skip = 0,
                Total = 0,
            };

            formioSubmissionCollection.Submissions.Add(submission1);

            var serializedFormioSubmissionCollection = JsonNode.Parse(JsonConvert.SerializeObject(formioSubmissionCollection.Submissions)) !.ToJsonString();

            var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(serializedFormioSubmissionCollection, Encoding.UTF8, "application/json"),
            };

            var client = new Mock<IHttpClient>(MockBehavior.Strict);
            client.Setup(c => c.GetAsync(It.IsAny<string>()))
                .Callback<string>(url =>
                {
                    url.Should().Be($"constellation/demandemandat/submission?data.bankCode=123&data.bankSortCode=456&data.bankAccountNumber=789&data.bankCheckNumber=46");
                })
                .ReturnsAsync(httpResponseMessage)
                .Verifiable();

            client.Setup(c => c.Dispose())
                .Verifiable();

            var auth = new FormioAuthToken();

            var factory = new Mock<IFormioClientFactory>(MockBehavior.Strict);
            factory.Setup(f => f.Create(auth))
                .Returns(client.Object)
                .Verifiable();

            var logger = new Mock<ILogger<HttpFormIoClient>>(MockBehavior.Strict);
            logger.Setup(x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsValueType>(),
                It.IsAny<Exception>(),
                (Func<It.IsValueType, Exception?, string>)It.IsAny<object>()));

            var jeDeclareClient = new HttpFormIoClient(logger.Object, factory.Object);

            var submissionMandate = await jeDeclareClient.GetSubmissionMandateAsync("123", "456", "789", "46", auth);

            submissionMandate.Limit.Should().Be(0);
            submissionMandate.Skip.Should().Be(0);

            submissionMandate.Submissions.Count.Should().Be(1);
            var submission = submissionMandate.Submissions[0];

            var dataJtoken = JToken.FromObject(submission.Data);

            ((string)dataJtoken["bankCode"] !).Should().Be("123");
            ((string)dataJtoken["bankSortCode"] !).Should().Be("456");
            ((string)dataJtoken["bankAccountNumber"] !).Should().Be("789");
            ((string)dataJtoken["bankCheckNumber"] !).Should().Be("46");

            client.VerifyAll();
            factory.VerifyAll();
        }
    }
}
