// <copyright file="Helpers.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Client.Http
{
    public static class Helpers
    {
        public static string ConvertListToQueryString(List<int> statusCodes)
        {
            if (statusCodes == null || !statusCodes.Any())
            {
                return string.Empty;
            }

            // Utilisation de Select pour transformer chaque élément en une chaîne 'statusCodes=x'
            IEnumerable<string> queryStringParameters = statusCodes.Select(code => $"statusCodes={code}");

            // Utilisation de string.Join pour concaténer les paramètres avec '&'
            string queryString = string.Join("&", queryStringParameters);

            return queryString;
        }
    }
}
