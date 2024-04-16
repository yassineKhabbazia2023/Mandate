// <copyright file="Program.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.AspNetCore
{
    using System.Diagnostics.CodeAnalysis;
    using Azure.Extensions.AspNetCore.Configuration.Secrets;
    using Azure.Identity;
    using Azure.Security.KeyVault.Secrets;
    using Kpmg.AspNetCore.Authentication.ConstellationIdentityService;
    using KPMG.Pulse.Back.Accounting.Mandate.Adapters;
    using KPMG.Pulse.Back.Accounting.Mandate.Application;
    using KPMG.Pulse.Back.Accounting.Mandate.Formio.Client.Http;
    using KPMG.Pulse.Back.Accounting.Mandate.JeDeclare.Client.Http;
    using KPMG.Pulse.Back.Accounting.Mandate.Notifications;
    using KPMG.Pulse.Back.Accounting.Mandate.Portal;
    using KPMG.Pulse.Back.Accounting.Mandate.Sql.Implementation;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Diagnostics.HealthChecks;
    using Microsoft.Data.SqlClient;
    using Microsoft.Data.SqlClient.AlwaysEncrypted.AzureKeyVaultProvider;
    using Microsoft.Extensions.Diagnostics.HealthChecks;
    using Microsoft.Extensions.Logging;
    using Microsoft.IdentityModel.Clients.ActiveDirectory;

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


            builder.Services
                .AddAuthentication()
                .AddConstellationIdentityService(
                new ConstellationIdentityServiceAuthenticationOptions
                {
                    ServerAddress = new Uri(builder.Configuration["identityserviceApiUrl"] !),
                    AzureActiveDirectoryClientCredentials =
                    {
                        ClientId = builder.Configuration["AuthClientId"],
                        ClientSecret = builder.Configuration["AuthClientSecret"],
                        Scope = builder.Configuration["AuthAudience"],
                        Tenant = builder.Configuration["AuthTenant"],
                    },
                }, out string[] schemeNames);
            builder.Services
                .AddAuthorization(options =>
                {
                    options.DefaultPolicy = new AuthorizationPolicyBuilder()
                        .RequireAuthenticatedUser()
                        .AddAuthenticationSchemes(schemeNames)
                        .Build();
                });

            builder.Services.AddConstellationHttpClient();
            builder.Services.AddSingleton<MandateAuthorizationFilterAttribute>();

            builder.Services.AddMandateSql(opt => opt.ConnectionString = builder.Configuration["DbConnectionString"]);
            builder.Services.AddMandateJeDeclare(opt =>
            {
                opt.BaseUri = new Uri(builder.Configuration["JeDeclareBaseUri"] !);
                opt.Login = builder.Configuration["JeDeclareLogin"] !;
                opt.Password = builder.Configuration["JeDeclarePassword"] !;
                opt.JdcCompteId = builder.Configuration["JeDeclareCompteId"] !;
                opt.HistoryDateEnabledBanks = builder.Configuration["JeDeclareHistoryDateEnabledBanks"] !;
            });

            builder.Services.AddMandateFormio(opt =>
            {
                opt.BaseUri = new Uri(builder.Configuration["FormioBaseUri"] !);
                opt.FormioApiKey = builder.Configuration["FormioApiKey"] !;
                opt.DemandeMandateFormId = builder.Configuration["DemandeMandateFormId"]!;
            });

            builder.Services.AddMandateApplication(opt =>
            {
                opt.MandateCancellationSubject = builder.Configuration["MandateCancellationSubject"] !;
                opt.MandateCancellationTemplateName = builder.Configuration["MandateCancellationTemplateName"] !;
                opt.MandateCancellationFromEmail = builder.Configuration["MandateCancellationFromEmail"] !;
                opt.MandateCancellationToEmail = builder.Configuration["MandateCancellationToEmail"] !;
                opt.MandateCancellationCcEmails = new List<string>(builder.Configuration["MandateCancellationCcEmails"] !.Split(";"));
                opt.MandateUploadedSubject = builder.Configuration["MandateUploadedSubject"] !;
                opt.MandateUploadedTemplateName = builder.Configuration["MandateUploadedTemplateName"] !;
                opt.MandateUploadedFromEmail = builder.Configuration["MandateUploadedFromEmail"] !;
                opt.MandateUploadedToEmail = builder.Configuration["MandateUploadedToEmail"] !;
                opt.MandateUploadedCcEmails = new List<string>(builder.Configuration["MandateUploadedCcEmails"] !.Split(";"));
            });
            builder.Services.AddMandateAdapters();
            builder.Services.AddPortailApi(builder.Configuration);
            builder.Services.AddNotificationsApi(builder.Configuration);

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("api.json", "KPMG Pulse Mandate API");
                c.DocumentTitle = "KPMG Pulse Mandate API";
                c.RoutePrefix = "api";
                c.EnableTryItOutByDefault();
            });

            app.UseJsonErrorExceptionHandler(app.Environment);

            app.UseHttpsRedirection();

            app.UseCors("CorsPolicy");

            app.UseAuthorization();

            app.MapControllers();

            app.MapHealthChecks("/api/health", new HealthCheckOptions()
            {
                ResponseWriter = WriteResponse,
            });

            app.Run();
        }

        private static void InitializeAzureKeyVaultProvider(string applicationId, string clientKey)
        {
            clientCredential = new ClientCredential(applicationId, clientKey);
            var azureKeyVaultProvider = new SqlColumnEncryptionAzureKeyVaultProvider(GetToken);
            var providers = new Dictionary<string, SqlColumnEncryptionKeyStoreProvider>();
            providers.Add(SqlColumnEncryptionAzureKeyVaultProvider.ProviderName, azureKeyVaultProvider);
            SqlConnection.RegisterColumnEncryptionKeyStoreProviders(providers);
        }

        private static async Task<string> GetToken(string authority, string resource, string scope)
        {
            var authContext = new Microsoft.IdentityModel.Clients.ActiveDirectory.AuthenticationContext(authority);
            var result = await authContext.AcquireTokenAsync(resource, clientCredential).ConfigureAwait(false);
            if (result == null)
            {
                throw new InvalidOperationException("Failed to obtain the access token");
            }

            return result.AccessToken;
        }

        private static Task WriteResponse(HttpContext context, HealthReport result)
        {
            context.Response.ContentType = "application/json";
            return context.Response.WriteAsync(HealthReportHelper.SerializeToJson(result));
        }
    }
}