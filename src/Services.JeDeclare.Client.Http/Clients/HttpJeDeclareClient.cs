// <copyright file="HttpJeDeclareClient.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.JeDeclare.Client.Http
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;

    public class HttpJeDeclareClient : IJeDeclareClient
    {
        private readonly ILogger logger;
        private readonly IJeDeclareClientFactory factory;

        public HttpJeDeclareClient(ILogger<HttpJeDeclareClient> logger, IJeDeclareClientFactory factory)
        {
            this.logger = logger;
            this.factory = factory;
        }

        public async Task<ListeReleves> GetAllConfigurationFromFolderAsync(string jdcCompteId, string jdcFolderId)
        {
            using var client = this.factory.Create();
            var requestUri = $"compte/{jdcCompteId}/dossierClient/{jdcFolderId}/releve";

            var response = await client.GetAsync(requestUri);

            var responseBody = await response.Content.ReadAsStringAsync();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var listeReleves = responseBody.Deserialize<ListeReleves>();

                return listeReleves!;
            }

            var exception = new JeDeclareApiException($"Exception was thrown : status code : {response.StatusCode} - Message : '{responseBody}'");

            this.logger.LogError(
                exception,
                "{class} - '{method}': Exception was thrown : status code : '{statusCode}' - jdcCompteId : '{jdcCompteId}' - jdcFolderId : '{jdcFolderId}' - Message : '{eroorMessage}'",
                nameof(HttpJeDeclareClient),
                nameof(this.GetAllConfigurationFromFolderAsync),
                response.StatusCode,
                jdcCompteId,
                jdcFolderId,
                responseBody);

            throw exception;
        }

        public async Task<byte[]> GetSignedMandatPdfAsync(string jdcCompteId, string jdcFolderId, string jdcRibId)
        {
            using var client = this.factory.Create();

            var requestUri = $"compte/{jdcCompteId}/dossierClient/{jdcFolderId}/rib/{jdcRibId}/mandatSigne";

            var response = await client.GetAsync(requestUri);

            var result = await response.Content.ReadAsStringAsync();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                return Convert.FromBase64String(result);
            }

            var exception = new JeDeclareApiException($"Exception was thrown : status code : {response.StatusCode} - Message : '{result}'");

            this.logger.LogError(
                exception,
                "{class} - '{method}': Exception was thrown : status code : '{statusCode}' - jdcCompteId : '{jdcCompteId}' - jdcFolderId : '{jdcFolderId}' - jdcRibId : '{jdcRibId}' - Message : '{eroorMessage}'",
                nameof(HttpJeDeclareClient),
                nameof(this.GetSignedMandatPdfAsync),
                response.StatusCode,
                jdcCompteId,
                jdcFolderId,
                jdcRibId,
                result);

            throw exception;
        }

        // missing UT => to code it when finishing aspose.
        public async Task<byte[]> GetMandatPdfAsync(string jdcCompteId, string jdcFolderId, string jdcRibId)
        {
            using var client = this.factory.Create();

            var requestUri = $"compte/{jdcCompteId}/dossierClient/{jdcFolderId}/rib/{jdcRibId}/mandat";

            var response = await client.GetAsync(requestUri);

            var responseBody = await response.Content.ReadAsStringAsync();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                byte[] pdfBytes = Convert.FromBase64String(responseBody);

                using var stream = new MemoryStream(pdfBytes);
                var mandatBytes = this.DeleteFirstPageMandatPdf(stream);
                return mandatBytes;
            }

            var exception = new JeDeclareApiException($"Exception was thrown : status code : {response.StatusCode} - Message : '{responseBody}'");

            this.logger.LogError(
                exception,
                "{class} - '{method}': Exception was thrown : status code : '{statusCode}' - jdcCompteId : '{jdcCompteId}' - jdcFolderId : '{jdcFolderId}' - jdcRibId : '{jdcRibId}' - Message : '{eroorMessage}'",
                nameof(HttpJeDeclareClient),
                nameof(this.GetMandatPdfAsync),
                response.StatusCode,
                jdcCompteId,
                jdcFolderId,
                jdcRibId,
                responseBody);

            throw exception;
        }

        public async Task<DossierClient> CreateFolderAsync(string jdcCompteId, DossierClient folderClient)
        {
            using var client = this.factory.Create();

            var requestUri = $"compte/{jdcCompteId}/dossierClient";

            var serializedFolder = folderClient.Serialize();

            var content = new StringContent(
               serializedFolder,
               Encoding.UTF8,
               "text/xml");

            var response = await client.PostAsync(requestUri, content);

            var responseBody = await response.Content.ReadAsStringAsync();
            if (response.StatusCode == HttpStatusCode.Created)
            {
                var folder = responseBody.Deserialize<DossierClient>();
                return folder;
            }

            var exception = new JeDeclareApiException($"Exception was thrown : status code : {response.StatusCode} - Message : '{responseBody}'");

            this.logger.LogError(
            exception,
            "{class} - '{method}': Exception was thrown : status code : '{statusCode}' - jdcCompteId : '{jdcCompteId}' - serializedFolder : '{serializedFolder}' - Message : '{eroorMessage}'",
            nameof(HttpJeDeclareClient),
            nameof(this.CreateFolderAsync),
            response.StatusCode,
            jdcCompteId,
            serializedFolder,
            responseBody);

            throw exception;
        }

        public async Task<Rib> AddRibToFolderAsync(string jdcCompteId, string jdcFolderId, Rib ribClient)
        {
            using var client = this.factory.Create();

            var requestUri = $"compte/{jdcCompteId}/dossierClient/{jdcFolderId}/rib";

            var serializedRib = ribClient.Serialize();

            var content = new StringContent(
                serializedRib,
                Encoding.UTF8,
                "text/xml");

            var response = await client.PostAsync(requestUri, content);

            var responseBody = await response.Content.ReadAsStringAsync();
            if (response.StatusCode == HttpStatusCode.Created)
            {
                var ribSaved = responseBody.Deserialize<Rib>();
                return ribSaved;
            }

            var exception = new JeDeclareApiException($"Exception was thrown : status code : {response.StatusCode} - Message : '{responseBody}'");

            this.logger.LogError(
            exception,
            "{class} - '{method}': Exception was thrown : status code : '{statusCode}' - jdcCompteId : '{jdcCompteId}' - jdcFolderId : '{jdcFolderId}' - serializedrib : '{serializedRib}' - Message : '{eroorMessage}'",
            nameof(HttpJeDeclareClient),
            nameof(this.AddRibToFolderAsync),
            response.StatusCode,
            jdcCompteId,
            jdcFolderId,
            serializedRib,
            responseBody);

            throw exception;
        }

        public async Task<Releve> CreateCollecteConfigurationAsync(string jdcCompteId, string jdcFolderId, Releve releve)
        {
            using var client = this.factory.Create();

            var requestUri = $"compte/{jdcCompteId}/dossierClient/{jdcFolderId}/releve";

            var serializedReleve = releve.Serialize();

            var content = new StringContent(
               serializedReleve,
               Encoding.UTF8,
               "text/xml");

            var response = await client.PostAsync(requestUri, content);

            var responseBody = await response.Content.ReadAsStringAsync();

            if (response.StatusCode == HttpStatusCode.Created)
            {
                var releveSaved = responseBody.Deserialize<Releve>();
                return releveSaved;
            }

            var exception = new JeDeclareApiException($"Exception was thrown : status code : {response.StatusCode} - Message : '{responseBody}'");

            this.logger.LogError(
            exception,
            "{class} - '{method}': Exception was thrown : status code : '{statusCode}' - jdcCompteId : '{jdcCompteId}' - jdcFolderId : '{jdcFolderId}' - serializedreleve : '{serializedReleve}' - Message : '{eroorMessage}'",
            nameof(HttpJeDeclareClient),
            nameof(this.CreateCollecteConfigurationAsync),
            response.StatusCode,
            jdcCompteId,
            jdcFolderId,
            serializedReleve,
            responseBody);

            throw exception;
        }

        public async Task<bool> UpdateCollecteConfigurationAsync(string jdcCompteId, string jdcFolderId, Releve releve)
        {
            using var client = this.factory.Create();

            var requestUri = $"compte/{jdcCompteId}/dossierClient/{jdcFolderId}/releve/{releve.Id}";

            var serializedReleve = releve.Serialize();

            var content = new StringContent(
               serializedReleve,
               Encoding.UTF8,
               "text/xml");

            var response = await client.PutAsync(requestUri, content);

            if (response.StatusCode == HttpStatusCode.Created)
            {
                return true;
            }

            var responseBody = await response.Content.ReadAsStringAsync();

            var exception = new JeDeclareApiException($"Exception was thrown : status code : {response.StatusCode} - Message : '{responseBody}'");

            this.logger.LogError(
            exception,
            "{class} - '{method}': Exception was thrown : status code : '{statusCode}' - jdcCompteId : '{jdcCompteId}' - jdcFolderId : '{jdcFolderId}' - serializedreleve : '{serializedReleve}' - Message : '{eroorMessage}'",
            nameof(HttpJeDeclareClient),
            nameof(this.UpdateCollecteConfigurationAsync),
            response.StatusCode,
            jdcCompteId,
            jdcFolderId,
            serializedReleve,
            responseBody);

            throw exception;
        }

        public async Task<string> UploadSignedMandat(string jdcCompteId, string jdcFolderId, string jdcRibId, byte[] mandat)
        {
            using var client = this.factory.Create();

            var requestUri = $"compte/{jdcCompteId}/dossierClient/{jdcFolderId}/rib/{jdcRibId}/mandatSigne";

            var serializedMandatFile = Convert.ToBase64String(mandat);

            var content = new StringContent(
               serializedMandatFile,
               Encoding.UTF8);

            var response = await client.PostAsync(requestUri, content);

            var responseBody = await response.Content.ReadAsStringAsync();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                return responseBody;
            }

            var exception = new JeDeclareApiException($"Exception was thrown : status code : {response.StatusCode} - Message : '{responseBody}'");

            this.logger.LogError(
            exception,
            "{class} - '{method}': Exception was thrown : status code : '{statusCode}' - jdcCompteId : '{jdcCompteId}' - jdcFolderId : '{jdcFolderId}' - jdcRibId : '{jdcRibId}' - Message : '{eroorMessage}'",
            nameof(HttpJeDeclareClient),
            nameof(this.UploadSignedMandat),
            response.StatusCode,
            jdcCompteId,
            jdcFolderId,
            jdcRibId,
            responseBody);

            throw exception;
        }

        public async Task<bool> CheckSignedMandatExists(string jdcCompteId, string jdcFolderId, string jdcRibId)
        {
            using var client = this.factory.Create();

            var requestUri = $"compte/{jdcCompteId}/dossierClient/{jdcFolderId}/rib/{jdcRibId}/mandatSigne";

            var request = new HttpRequestMessage(HttpMethod.Get, requestUri);

            var response = await client.SendAsync(request);

            if (response.StatusCode == HttpStatusCode.NoContent)
            {
                // To verify algo in original jedeclareservice code
                return true;
            }

            var responseBody = await response.Content.ReadAsStringAsync();

            var exception = new JeDeclareApiException($"Exception was thrown : status code : {response.StatusCode} - Message : '{responseBody}'");

            this.logger.LogError(
            exception,
            "{class} - '{method}': Exception was thrown : status code : '{statusCode}' - jdcCompteId : '{jdcCompteId}' - jdcFolderId : '{jdcFolderId}' - jdcRibId : '{jdcRibId}' - Message : '{eroorMessage}'",
            nameof(HttpJeDeclareClient),
            nameof(this.CheckSignedMandatExists),
            response.StatusCode,
            jdcCompteId,
            jdcFolderId,
            jdcRibId,
            responseBody);

            throw exception;
        }

        private byte[] DeleteFirstPageMandatPdf(MemoryStream mandat)
        {
            // TO DO Aspose
           throw new NotImplementedException();
        }
    }
}
