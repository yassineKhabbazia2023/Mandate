// <copyright file="ProgramTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.AzureFunctions.Tests;

using global::Mandate.AzureFunctions.Interfaces;
using global::Mandate.AzureFunctions.Managers;
using KPMG.Pulse.Back.Accounting.Mandate.Client.Http;
using KPMG.Pulse.Back.Accounting.Mandate.Function;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public class ProgramTest
{
    private readonly Mock<IConfiguration> configurationMock;

    public ProgramTest()
    {
        this.configurationMock = new Mock<IConfiguration>();
        this.configurationMock.SetupGet(c => c[It.IsAny<string>()]).Returns("test");
    }

    [Fact]
    public void Services_Should_Be_Registered_Correctly()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new[]
            {
                    new KeyValuePair<string, string?>("AuthClientId", "test-client-id"),
                    new KeyValuePair<string, string?>("AuthClientSecret", "test-client-secret"),
                    new KeyValuePair<string, string?>("AuthAudience", "test-audience"),
                    new KeyValuePair<string, string?>("AuthTenant", "test-tenant"),
                    new KeyValuePair<string, string?>("MANDATE_API_URL", "https://test.api.url"),
            })
            .Build();

        var memoryContext = new HostBuilderContext(new Dictionary<object, object>())
        {
            Configuration = configuration,
        };

        // Act
        var hostBuilder = new HostBuilder()
            .ConfigureServices((context, services) =>
            {
                var config = memoryContext.Configuration;
                services.AddApplicationInsightsTelemetryWorkerService();
                services.ConfigureFunctionsApplicationInsights();
                services.AddConstellationHttpClient();

                services.AddSingleton<IMandateProvider, MandateProvider>();
                services.AddSingleton<IMandateFunctionManager, MandateFunctionManager>();

                services.AddMandateClient(options =>
                {
                    options.BaseUri = new Uri(config["MANDATE_API_URL"]!);
                });
            });

        Action actBuild = () => hostBuilder.Build();
    }
}
