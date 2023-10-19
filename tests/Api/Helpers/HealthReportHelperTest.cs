// <copyright file="HealthReportHelperTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.AspNetCore.Tests
{
    using Microsoft.Extensions.Diagnostics.HealthChecks;

    public class HealthReportHelperTest
    {
        [Fact]
        public void SerializeToJson()
        {
            var entries = new Dictionary<string, HealthReportEntry>()
            {
                { "sqlDatabase", new HealthReportEntry(HealthStatus.Healthy, null, TimeSpan.Zero, null, null) },
            };

            var objectToWrite = new
            {
                status = "Healthy",
                sqlDatabase = new
                {
                    status = "Healthy",
                },
            };

            var report = new HealthReport(entries, TimeSpan.Zero);

            var result = HealthReportHelper.SerializeToJson(report);

            objectToWrite.Should().BeJsonSerializableTo(result);
        }
    }
}
