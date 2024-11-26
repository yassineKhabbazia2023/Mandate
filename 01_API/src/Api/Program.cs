// <copyright file="Program.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

using System.Diagnostics.CodeAnalysis;
using KPMG.Pulse.Back.Accounting.Mandate.Adapters;
using KPMG.Pulse.Back.Accounting.Mandate.Application;
using KPMG.Pulse.Back.Accounting.Mandate.JeDeclare.Client.Http;
using KPMG.Pulse.Back.Accounting.Mandate.Notifications;
using KPMG.Pulse.Back.Accounting.Mandate.Portal;
using KPMG.Pulse.Back.Accounting.Mandate.Sql.Implementation;
using Mandate.Messaging;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace KPMG.Pulse.Back.Accounting.Mandate.AspNetCore
{
    [ExcludeFromCodeCoverage]
    public static class Program
    {
        private static ClientCredential? clientCredential;

        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddApplicationInsightsTelemetry(options =>
            {
                if (!string.IsNullOrWhiteSpace(builder.Configuration["APPINSIGHTS_INSTRUMENTATIONKEY"]))
                {
                    options.ConnectionString = builder.Configuration["APPINSIGHTS_INSTRUMENTATIONKEY"];
                }
            })
            .AddLogging(logging =>
            {
                logging.AddApplicationInsights();
                if (Enum.TryParse<Microsoft.Extensions.Logging.LogLevel>(builder.Configuration["LogLevel"], out var logLevel))
                {
                    logging.AddFilter<Microsoft.Extensions.Logging.ApplicationInsights.ApplicationInsightsLoggerProvider>(string.Empty, logLevel);
                    logging.AddFilter<Microsoft.Extensions.Logging.ApplicationInsights.ApplicationInsightsLoggerProvider>("Microsoft.AspNetCore", Microsoft.Extensions.Logging.LogLevel.Warning);
                }
            });

            // Add services to the container.
            builder.Services
                .AddControllers()
                .AddNewtonsoftJson();

            builder.Services.AddHealthChecks();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();

            // Options
            if (string.IsNullOrWhiteSpace(builder.Configuration["DbConnectionString"]))
            {
                throw new InvalidOperationException($"Database connection string is not definied (Missing setting: DbConnectionString)");
            }

            builder.Services.AddConstellationHttpClient();
            builder.Services.AddSingleton<MandateAuthorizationFilterAttribute>();

            builder.Services.AddMandateSql(builder.Configuration);
            builder.Services.AddMandateJeDeclare(opt =>
            {
                opt.BaseUri = new Uri(builder.Configuration["JeDeclareBaseUri"]!);
                opt.Login = builder.Configuration["JeDeclareLogin"]!;
                opt.Password = builder.Configuration["JeDeclarePassword"]!;
                opt.JdcCompteId = builder.Configuration["JeDeclareCompteId"]!;
                opt.HistoryDateEnabledBanks = builder.Configuration["JeDeclareHistoryDateEnabledBanks"]!;
            });
            string mandateCancellationCC = builder.Configuration["MandateCancellationCcEmails"] ?? string.Empty;
            string uploadedCCEmails = builder.Configuration["MandateUploadedCcEmails"] ?? string.Empty;
            builder.Services.AddMandateApplication(opt =>
            {
                opt.MandateCancellationSubject = builder.Configuration["MandateCancellationSubject"]!;
                opt.MandateCancellationTemplateName = builder.Configuration["MandateCancellationTemplateName"]!;
                opt.MandateCancellationFromEmail = builder.Configuration["MandateCancellationFromEmail"]!;
                opt.MandateCancellationToEmail = builder.Configuration["MandateCancellationToEmail"]!;
                opt.MandateCancellationCcEmails = string.IsNullOrEmpty(mandateCancellationCC) ? new List<string>() : mandateCancellationCC.Split(";").ToList();
                opt.MandateUploadedSubject = builder.Configuration["MandateUploadedSubject"]!;
                opt.MandateUploadedTemplateName = builder.Configuration["MandateUploadedTemplateName"]!;
                opt.MandateUploadedFromEmail = builder.Configuration["MandateUploadedFromEmail"]!;
                opt.MandateUploadedToEmail = builder.Configuration["MandateUploadedToEmail"]!;
                opt.MandateUploadedCcEmails = string.IsNullOrEmpty(uploadedCCEmails) ? new List<string>() : uploadedCCEmails.Split(";").ToList();
            });
            builder.Services.AddMandateAdapters();
            builder.Services.AddPortailApi(builder.Configuration);
            builder.Services.AddNotificationsApi(option => option.BaseUrl = builder.Configuration["MANDATE_NOTIFICATION_V2_API_URL"]);
            builder.Services.AddRequestTimeouts(options =>
            {
                options.AddPolicy("TwoSecondsTimeOut", TimeSpan.FromSeconds(120));
            });

            // Configure Azure Service Bus
            builder.Services.AddServiceBusConfiguration(builder.Configuration);

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("api.json", "KPMG Pulse Mandate API");
                c.DocumentTitle = "KPMG Pulse Mandate API";
                c.RoutePrefix = "api";
                c.EnableTryItOutByDefault();
            });

            app.UseHttpsRedirection();

            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseCors("CorsPolicy");

            app.MapControllers();

            app.MapHealthChecks("/health", new HealthCheckOptions()
            {
                ResponseWriter = WriteResponse,
            });

            app.UseRequestTimeouts();
            app.Run();
        }

        private static Task WriteResponse(HttpContext context, HealthReport result)
        {
            context.Response.ContentType = "application/json";
            return context.Response.WriteAsync(HealthReportHelper.SerializeToJson(result));
        }
    }
}