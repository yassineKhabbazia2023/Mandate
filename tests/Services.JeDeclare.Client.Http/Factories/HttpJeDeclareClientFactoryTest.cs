// <copyright file="HttpJeDeclareClientFactoryTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.JeDeclare.Client.Http.Tests.Factories
{
    using System.Net.Http.Headers;
    using Kpmg.Constellation.Net.Http;
    using Microsoft.Extensions.Options;

    public class HttpJeDeclareClientFactoryTest
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task Create(bool allowAcceptXml)
        {
            var options = Options.Create(new JeDeclareOptions()
            {
                BaseUri = new Uri("https://recette.jedeclare.com/webservice/gestion/"),
                Login = "login",
                Password = "password",
                JdcCompteId = "JdcCompteId",
                HistoryDateEnabledBanks = "30000",
            });

            var client = new Mock<IHttpClient>();
            client.Setup(item => item.DefaultRequestHeaders)
                .Returns(() =>
                {
                    var httpClient = new HttpClient();
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(allowAcceptXml ? "application/xml" : "application/*"));
                    return httpClient.DefaultRequestHeaders;
                })
             .Verifiable();

            var factory = new Mock<IHttpClientFactory>(MockBehavior.Strict);
            factory.Setup(item => item.Create(
                It.Is<Uri>(item => item.AbsoluteUri == "https://recette.jedeclare.com/webservice/gestion/"),
                It.Is<BasicHttpClientAuthentication>(item => item.Login == "login" & item.Password == "password")))
                .Returns(client.Object)
                .Verifiable();

            HttpJeDeclareClientFactory jeDeclareClientFactory = new HttpJeDeclareClientFactory(options, factory.Object);

            IHttpClient httpClient = jeDeclareClientFactory.Create(allowAcceptXml);

            httpClient.Should().NotBeNull();

            client.VerifyAll();
            factory.VerifyAll();
        }
    }
}
