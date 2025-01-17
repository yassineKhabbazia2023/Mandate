// <copyright file="Program.cs" company="PULSE">
// Copyright (c) PULSE. All rights reserved.
// </copyright>

using Azure.Identity;
using KPMG.Pulse.Back.Accounting.Mandate;
using KPMG.Pulse.Back.Accounting.Mandate.Adapters;
using KPMG.Pulse.Back.Accounting.Mandate.AzureFunctions;
using KPMG.Pulse.Back.Accounting.Mandate.Client.Http;
using KPMG.Pulse.Back.Accounting.Mandate.Function;
using KPMG.Pulse.Back.Accounting.Mandate.JeDeclare.Client.Http;
using KPMG.Pulse.Back.Accounting.Mandate.Sql;
using KPMG.Pulse.Back.Accounting.Mandate.Sql.Implementation;
using Mandate.AzureFunctions;
using Mandate.AzureFunctions.Functions;
using Mandate.AzureFunctions.Interfaces;
using Mandate.AzureFunctions.Managers;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices((context, services) =>
    {
        var config = context.Configuration;
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        services.AddConstellationHttpClient();

        services.AddDbContext<MandateContext>(
    options =>
    {
#if DEBUG
        options.LogTo(Console.WriteLine)
            .EnableSensitiveDataLogging()
            .EnableDetailedErrors();
#endif
        options.UseSqlServer(config["DbConnectionString"]);
    },
    ServiceLifetime.Scoped);
        services.AddOptions<UpdateStatusesConfiguration>().BindConfiguration(UpdateStatusesConfiguration.SectionName);
        services.AddScoped<IMandateRepository, SqlMandateRepository>();
        services.AddMandateJeDeclare(opt =>
        {
            opt.BaseUri = new Uri(context.Configuration["JeDeclareBaseUri"]!);
            opt.Login = context.Configuration["JeDeclareLogin"]!;
            opt.Password = context.Configuration["JeDeclarePassword"]!;
            opt.JdcCompteId = context.Configuration["JeDeclareCompteId"]!;
            opt.HistoryDateEnabledBanks = context.Configuration["JeDeclareHistoryDateEnabledBanks"]!;
        });
        services.AddSingleton<IJeDeclareService, JeDeclareAdapter>();
        services.AddSingleton<IMandateProvider, MandateProvider>();
        services.AddSingleton<IUpdateMandateStatusesHandler, UpdateMandateStatusesHandler>();
        services.AddSingleton<IEventsFunctionManager, EventsFunctionManager>();
        services.AddSingleton<ISqlAdapter, KPMG.Pulse.Back.Accounting.Mandate.Function.SqlAdapter>();
        services.AddScoped<IDatabaseService, KPMG.Pulse.Back.Accounting.Mandate.Adapters.SqlAdapter>();

        services.AddAzureClients(builder =>
        {
            builder.AddServiceBusClientWithNamespace(config["serviceBusNameSpace:fullyQualifiedNamespace"]).WithCredential(new DefaultAzureCredential(new DefaultAzureCredentialOptions
            {
                ManagedIdentityClientId = config["serviceBusNameSpace:clientId"],
            }));
        });
    })
    .ConfigureLogging(logging =>
    {
        logging.Services.Configure<LoggerFilterOptions>(options =>
        {
            LoggerFilterRule? defaultRule = options.Rules
                .FirstOrDefault(rule => rule.ProviderName == "Microsoft.Extensions.Logging.ApplicationInsights.ApplicationInsightsLoggerProvider");
            if (defaultRule is not null)
            {
                options.Rules.Remove(defaultRule);
            }
        });
    })
    .Build();

host.Run();

[ExcludeFromCodeCoverage]
public partial class Program { }
