// <copyright file="ServiceRegistrationTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Adapters.Tests
{
    using KPMG.Pulse.Back.Accounting.Mandate.Sql;
    using KPMG.Pulse.Back.Accounting.Mandate.Sql.Implementation;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;

    public class ServiceRegistrationTest
    {
        [Fact]
        public void Register()
        {
            var sc = new ServiceCollection();
            sc.AddMandateAdapters();
            sc.AddMandateSql(opt => opt.ConnectionString = "a");
            var sp = sc.BuildServiceProvider();

            // Make sure we don't forget services ; exclude services from Microsoft (IOption, ...)
            sc.Count(s => s.ServiceType.FullName?.StartsWith("KPMG") ?? false).Should().Be(2);

            // Test all services ; number of tests below should match the number of services above
            sp.GetService<IDatabaseService>().Should().NotBeNull();
            sp.GetService<IMandateRepository>().Should().NotBeNull();
        }
    }
}
