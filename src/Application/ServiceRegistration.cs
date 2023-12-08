// <copyright file="ServiceRegistration.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Application
{
    using Microsoft.Extensions.DependencyInjection;

    public static class ServiceRegistration
    {
        public static IServiceCollection AddMandateApplication(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddScoped<IBankManager, BankManager>();
            services.AddScoped<IBbanManager, BbanManager>();
            services.AddScoped<IMandateManager, MandateManager>();
            services.AddScoped<ICompanyManager, CompanyManager>();
            services.AddScoped<IFakeDataManager, FakeDataManager>();
            services.AddScoped<IAsposeHelper, AsposeHelper>();
            services.AddScoped<IGuidGenerator, GuidGenerator>();
            services.AddScoped<IFormIoManager, FormIoManager>();

            return services;
        }
    }
}
