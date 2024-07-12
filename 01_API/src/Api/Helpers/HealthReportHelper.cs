// <copyright file="HealthReportHelper.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.AspNetCore
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.Json;
    using Microsoft.Extensions.Diagnostics.HealthChecks;

    public static class HealthReportHelper
    {
        public static string SerializeToJson(HealthReport result)
        {
            var options = new JsonWriterOptions
            {
                Indented = true,
            };

            using (var stream = new MemoryStream())
            {
                using (var writer = new Utf8JsonWriter(stream, options))
                {
                    writer.WriteStartObject();

                    SerializeEntry(writer, result.Status, result.Entries);

                    writer.WriteEndObject();
                }

                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }

        private static void SerializeEntry(Utf8JsonWriter writer, HealthStatus status, IEnumerable<KeyValuePair<string, HealthReportEntry>> entries)
        {
            writer.WriteString("status", Enum.GetName(typeof(HealthStatus), status));

            foreach (var entry in entries)
            {
                writer.WriteStartObject(entry.Key);
                SerializeEntry(writer, entry.Value.Status, entry.Value.Data.Select(kv => new KeyValuePair<string, HealthReportEntry>(kv.Key, (HealthReportEntry)kv.Value)));
                writer.WriteEndObject();
            }
        }
    }
}
