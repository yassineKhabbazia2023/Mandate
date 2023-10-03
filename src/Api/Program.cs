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
    using KPMG.Pulse.Back.Accounting.Mandate.JeDeclare.Client.Http;
    using KPMG.Pulse.Back.Accounting.Mandate.Sql.Implementation;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Diagnostics.HealthChecks;
    using Microsoft.Data.SqlClient;
    using Microsoft.Data.SqlClient.AlwaysEncrypted.AzureKeyVaultProvider;
    using Microsoft.Extensions.Diagnostics.HealthChecks;
    using Microsoft.IdentityModel.Clients.ActiveDirectory;

    [ExcludeFromCodeCoverage]
    public static class Program
    {
        private static ClientCredential? clientCredential;

        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            ConfigureKeyVault(builder);

            builder.Services.AddApplicationInsightsTelemetry(options =>
            {
                if (!string.IsNullOrWhiteSpace(builder.Configuration["ApplicationInsightsKey"]))
                {
                    options.InstrumentationKey = builder.Configuration["ApplicationInsightsKey"];
                }
            })
            .AddLogging(logging =>
            {
                logging.AddApplicationInsights();
                if (Enum.TryParse<Microsoft.Extensions.Logging.LogLevel>(builder.Configuration["MANDATE_LOG_LEVEL"], out var logLevel))
                {
                    logging.AddFilter<Microsoft.Extensions.Logging.ApplicationInsights.ApplicationInsightsLoggerProvider>(string.Empty, logLevel);
                }
            });

            if (string.IsNullOrWhiteSpace(builder.Configuration["MANDATE_FRONTENDS_URL"]))
            {
                throw new InvalidOperationException($"Allowed origins list is not definied (Missing setting: MANDATE_FRONTENDS_URL)");
            }

            var frontEndsUri = builder.Configuration["MANDATE_FRONTENDS_URL"].Split(';', StringSplitOptions.RemoveEmptyEntries);
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(
                    "CorsPolicy",
                    builder =>
                    {
                        builder
                            .WithOrigins(frontEndsUri)
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowCredentials();
                    });
            });

            // Add services to the container.
            builder.Services
                .AddControllers()
                .AddNewtonsoftJson();

            builder.Services.AddHealthChecks();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();

            // Options
            if (string.IsNullOrWhiteSpace(builder.Configuration["MandateDbConnectionString"]))
            {
                throw new InvalidOperationException($"Database connection string is not definied (Missing setting: MandateDbConnectionString)");
            }

            builder.Services
                .AddAuthentication()
                .AddConstellationIdentityService(
                new ConstellationIdentityServiceAuthenticationOptions
                {
                    ServerAddress = new Uri(builder.Configuration["MANDATE_IDENTITYSERVICE_API_URL"]),
                    AzureActiveDirectoryClientCredentials =
                    {
                        ClientId = builder.Configuration["ConstellationClientId"],
                        ClientSecret = builder.Configuration["ConstellationSecret"],
                        Scope = builder.Configuration["ConstellationAudience"],
                        Tenant = builder.Configuration["ConstellationTenant"],
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

            builder.Services.AddMandateSql(opt => opt.ConnectionString = builder.Configuration["MandateDbConnectionString"]);
            builder.Services.AddMandateJeDeclare(opt =>
            {
                opt.BaseUri = new Uri(builder.Configuration["MandateJeDeclareBaseUri"]);
                opt.Login = builder.Configuration["MandateJeDeclareLogin"];
                opt.Password = builder.Configuration["MandateJeDeclarePassword"];
            });

            builder.Services.AddMandateApplication();
            builder.Services.AddMandateAdapters();

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

        private static void ConfigureKeyVault(WebApplicationBuilder builder)
        {
            if (!builder.Environment.IsEnvironment("Local"))
            {
                builder.WebHost
                .ConfigureAppConfiguration((ctx, confbuilder) =>
                {
                    var config = confbuilder.Build();
                    if (string.IsNullOrWhiteSpace(config["MANDATE_KEYVAULT_CLIENT_ID"]))
                    {
                        throw new InvalidOperationException($"No client id has been specified to access to the Azure Vault (Missing setting: MANDATE_KEYVAULT_CLIENT_ID)");
                    }

                    if (string.IsNullOrWhiteSpace(config["MANDATE_KEYVAULT_CLIENT_SECRET"]))
                    {
                        throw new InvalidOperationException($"No client secret has been specified to access to the Azure Vault (Missing setting: MANDATE_KEYVAULT_CLIENT_SECRET)");
                    }

                    InitializeAzureKeyVaultProvider(config["MANDATE_KEYVAULT_CLIENT_ID"], config["MANDATE_KEYVAULT_CLIENT_SECRET"]);
                });

                if (builder.Environment.IsDevelopment() || builder.Environment.IsStaging())
                {
                    var secretClient = new SecretClient(
                                    new Uri($"https://{builder.Configuration["MANDATE_VAULT_NAME"]}.vault.azure.net/"),
                                    new ClientSecretCredential(
                                        builder.Configuration["MANDATE_IDENTITYSERVICE_AAD_TENANT"],
                                        builder.Configuration["MANDATE_KEYVAULT_CLIENT_ID"],
                                        builder.Configuration["MANDATE_KEYVAULT_CLIENT_SECRET"]));
                    builder.Configuration.AddAzureKeyVault(secretClient, new KeyVaultSecretManager());
                }
                else
                {
                    builder.Configuration.AddAzureKeyVault(
                        new Uri($"https://{builder.Configuration["MANDATE_VAULT_NAME"]}.vault.azure.net/"),
                        new DefaultAzureCredential());
                }
            }
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