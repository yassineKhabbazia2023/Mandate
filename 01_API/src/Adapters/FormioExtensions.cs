// <copyright file="FormioExtensions.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Adapters
{
    using System.Globalization;
    using System.Text.Json.Nodes;
    using KPMG.Pulse.Back.Accounting.Mandate.Formio.Client;
    using Newtonsoft.Json;

    public static class FormioExtensions
    {
        public static Collection ToCollection(this FormioSubmission source)
        {
            var node = JsonNode.Parse(JsonConvert.SerializeObject(source))!.AsObject();
            var data = node["data"];
            var headOffice = data?["headOffice"];

            Signatory signatory = new(
                 data?["signatoryTitle"]?.ToString()!,
                 data?["signatoryFirstName"]?.ToString()!,
                 data?["signatoryLastName"]?.ToString()!,
                 data?["signatoryEmailAddress"]?.ToString()!);

            Address address = new(
                headOffice?["signatoryStreetAddress"]?.ToString()!,
                headOffice?["signatoryAddressComplements"]?.ToString()!,
                headOffice?["signatoryAddressZipCode"]?.ToString()!,
                headOffice?["signatoryAddressCity"]?.ToString()!,
                headOffice?["signatoryAddressCountry"]?.ToString()!);

            Company company = new(
                default,
                data?["companyName"]?.ToString()!,
                data?["SIRETNumber"]?.ToString()!,
                data?["accountNumber"]?.ToString()!,
                data?["jdcDossierId"]?.ToString()!,
                signatory,
                address);

            DateTime created, modified;
            const string format = "yyyy-MM-ddTHH:mm:ss.fffZ";

            DateTime.TryParseExact(
                node["created"]?.ToString(),
                format,
                CultureInfo.GetCultureInfo("fr-FR"),
                DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
                out created);

            DateTime.TryParseExact(
                node["modified"]?.ToString(),
                format,
                CultureInfo.GetCultureInfo("fr-FR"),
                DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
                out modified);

            Bban? bban = new Bban(
                data?["bankCode"]?.ToString()!,
                data?["bankSortCode"]?.ToString()!,
                data?["bankAccountNumber"]?.ToString()!,
                data?["bankCheckNumber"]?.ToString()!,
                data?["jdcRibId"]?.ToString()!,
                null);

            return new Collection(
               Guid.Empty,
               data?["jdcReleveId"]?.ToString(),
               company,
               bban,
               created,
               modified,
               new Status(CollectionStatus.ToDo, "En cours", Mandate.JdcCollectionStatus.Creation_InProgress));
        }
    }
}