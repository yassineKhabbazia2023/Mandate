// <copyright file="ServiceRegistration.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Sql.Implementation
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public static class ServiceRegistration
    {
        public static void AddMandateSql(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<MandateContext>(
                options =>
                    options

                        .UseSqlServer(configuration["DbConnectionString"], sqlOptions =>
                        {
                            sqlOptions.EnableRetryOnFailure(
                                maxRetryCount: 3,
                                maxRetryDelay: TimeSpan.FromSeconds(3),
                                errorNumbersToAdd: null);
                        }));
            services.AddScoped<IMandateRepository, SqlMandateRepository>();
            services.AddScoped<IPaymentPreferenceRepository, PaymentPreferenceRepository>();
        }
    }
}
