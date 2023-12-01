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
    using Microsoft.Extensions.Options;

    public class HttpJeDeclareClient : IJeDeclareClient
    {
        private readonly ILogger logger;
        private readonly IJeDeclareClientFactory factory;
        private readonly IOptions<JeDeclareOptions> options;

        public HttpJeDeclareClient(ILogger<HttpJeDeclareClient> logger, IJeDeclareClientFactory factory, IOptions<JeDeclareOptions> options)
        {
            this.logger = logger;
            this.factory = factory;
            this.options = options ?? throw new ArgumentNullException(nameof(options));
            options.Value.Validate();
        }

        public async Task<ListeReleves> GetAllConfigurationFromFolderAsync(string jdcFolderId)
        {
            using var client = this.factory.Create();

            var jdcCompteId = this.options.Value.JdcCompteId;
            var requestUri = $"compte/{jdcCompteId}/dossierClient/{jdcFolderId}/releve";

            var response = await client.GetAsync(requestUri).ConfigureAwait(false);

            var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var listeReleves = responseBody.Deserialize<ListeReleves>();

                return listeReleves!;
            }

            var exception = new JeDeclareApiException($"Exception was thrown : status code : {response.StatusCode} - Message : '{responseBody}'");

            this.logger.LogError(
                exception,
                "{class} - '{method}': Exception was thrown : status code : '{statusCode}' - jdcCompteId : '{jdcCompteId}' - jdcFolderId : '{jdcFolderId}' - Message : '{errorMessage}'",
                nameof(HttpJeDeclareClient),
                nameof(this.GetAllConfigurationFromFolderAsync),
                response.StatusCode,
                jdcCompteId,
                jdcFolderId,
                responseBody);

            throw exception;
        }

        public async Task<byte[]> GetSignedMandatPdfAsync(string jdcFolderId, string jdcRibId)
        {
            using var client = this.factory.Create();

            var jdcCompteId = this.options.Value.JdcCompteId;
            var requestUri = $"compte/{jdcCompteId}/dossierClient/{jdcFolderId}/rib/{jdcRibId}/mandatSigne";

            var response = await client.GetAsync(requestUri).ConfigureAwait(false);

            var result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                return Convert.FromBase64String(result);
            }

            var exception = new JeDeclareApiException($"Exception was thrown : status code : {response.StatusCode} - Message : '{result}'");

            this.logger.LogError(
                exception,
                "{class} - '{method}': Exception was thrown : status code : '{statusCode}' - jdcCompteId : '{jdcCompteId}' - jdcFolderId : '{jdcFolderId}' - jdcRibId : '{jdcRibId}' - Message : '{errorMessage}'",
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
        public async Task<byte[]> GetMandatPdfAsync(string jdcFolderId, string jdcRibId)
        {
            using var client = this.factory.Create();

            var jdcCompteId = this.options.Value.JdcCompteId;
            var requestUri = $"compte/{jdcCompteId}/dossierClient/{jdcFolderId}/rib/{jdcRibId}/mandat";

            var response = await client.GetAsync(requestUri).ConfigureAwait(false);

            var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

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
                "{class} - '{method}': Exception was thrown : status code : '{statusCode}' - jdcCompteId : '{jdcCompteId}' - jdcFolderId : '{jdcFolderId}' - jdcRibId : '{jdcRibId}' - Message : '{errorMessage}'",
                nameof(HttpJeDeclareClient),
                nameof(this.GetMandatPdfAsync),
                response.StatusCode,
                jdcCompteId,
                jdcFolderId,
                jdcRibId,
                responseBody);

            throw exception;
        }

        public async Task<DossierClient> CreateFolderAsync(DossierClient folderClient)
        {
            using var client = this.factory.Create();

            var jdcCompteId = this.options.Value.JdcCompteId;
            var requestUri = $"compte/{jdcCompteId}/dossierClient";

            var serializedFolder = folderClient.Serialize();

            var content = new StringContent(
               serializedFolder,
               Encoding.UTF8,
               "text/xml");

            var response = await client.PostAsync(requestUri, content).ConfigureAwait(false);

            var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            if (response.StatusCode == HttpStatusCode.Created)
            {
                var folder = responseBody.Deserialize<DossierClient>();
                return folder;
            }

            var exception = new JeDeclareApiException($"Exception was thrown : status code : {response.StatusCode} - Message : '{responseBody}'");

            this.logger.LogError(
            exception,
            "{class} - '{method}': Exception was thrown : status code : '{statusCode}' - jdcCompteId : '{jdcCompteId}' - serializedFolder : '{serializedFolder}' - Message : '{errorMessage}'",
            nameof(HttpJeDeclareClient),
            nameof(this.CreateFolderAsync),
            response.StatusCode,
            jdcCompteId,
            serializedFolder,
            responseBody);

            throw exception;
        }

        public async Task<Rib> AddRibToFolderAsync(string jdcFolderId, Rib ribClient)
        {
            using var client = this.factory.Create();

            var jdcCompteId = this.options.Value.JdcCompteId;
            var requestUri = $"compte/{jdcCompteId}/dossierClient/{jdcFolderId}/rib";

            var serializedRib = ribClient.Serialize();

            var content = new StringContent(
                serializedRib,
                Encoding.UTF8,
                "text/xml");

            var response = await client.PostAsync(requestUri, content).ConfigureAwait(false);

            var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            if (response.StatusCode == HttpStatusCode.Created)
            {
                var ribSaved = responseBody.Deserialize<Rib>();
                return ribSaved;
            }

            var exception = new JeDeclareApiException($"Exception was thrown : status code : {response.StatusCode} - Message : '{responseBody}'");

            this.logger.LogError(
            exception,
            "{class} - '{method}': Exception was thrown : status code : '{statusCode}' - jdcCompteId : '{jdcCompteId}' - jdcFolderId : '{jdcFolderId}' - serializedrib : '{serializedRib}' - Message : '{errorMessage}'",
            nameof(HttpJeDeclareClient),
            nameof(this.AddRibToFolderAsync),
            response.StatusCode,
            jdcCompteId,
            jdcFolderId,
            serializedRib,
            responseBody);

            throw exception;
        }

        public async Task<Releve> CreateCollecteConfigurationAsync(string jdcFolderId, Releve releve)
        {
            using var client = this.factory.Create();

            var jdcCompteId = this.options.Value.JdcCompteId;
            var requestUri = $"compte/{jdcCompteId}/dossierClient/{jdcFolderId}/releve";

            var serializedReleve = releve.Serialize();

            var content = new StringContent(
               serializedReleve,
               Encoding.UTF8,
               "text/xml");

            var response = await client.PostAsync(requestUri, content).ConfigureAwait(false);

            var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (response.StatusCode == HttpStatusCode.Created)
            {
                var releveSaved = responseBody.Deserialize<Releve>();
                return releveSaved;
            }

            var exception = new JeDeclareApiException($"Exception was thrown : status code : {response.StatusCode} - Message : '{responseBody}'");

            this.logger.LogError(
            exception,
            "{class} - '{method}': Exception was thrown : status code : '{statusCode}' - jdcCompteId : '{jdcCompteId}' - jdcFolderId : '{jdcFolderId}' - serializedreleve : '{serializedReleve}' - Message : '{errorMessage}'",
            nameof(HttpJeDeclareClient),
            nameof(this.CreateCollecteConfigurationAsync),
            response.StatusCode,
            jdcCompteId,
            jdcFolderId,
            serializedReleve,
            responseBody);

            throw exception;
        }

        public async Task<bool> UpdateCollecteConfigurationAsync(string jdcFolderId, Releve releve)
        {
            using var client = this.factory.Create();

            var jdcCompteId = this.options.Value.JdcCompteId;
            var requestUri = $"compte/{jdcCompteId}/dossierClient/{jdcFolderId}/releve/{releve.Id}";

            var serializedReleve = releve.Serialize();

            var content = new StringContent(
               serializedReleve,
               Encoding.UTF8,
               "text/xml");

            var response = await client.PutAsync(requestUri, content).ConfigureAwait(false);

            if (response.StatusCode == HttpStatusCode.Created)
            {
                return true;
            }

            var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            var exception = new JeDeclareApiException($"Exception was thrown : status code : {response.StatusCode} - Message : '{responseBody}'");

            this.logger.LogError(
            exception,
            "{class} - '{method}': Exception was thrown : status code : '{statusCode}' - jdcCompteId : '{jdcCompteId}' - jdcFolderId : '{jdcFolderId}' - serializedreleve : '{serializedReleve}' - Message : '{errorMessage}'",
            nameof(HttpJeDeclareClient),
            nameof(this.UpdateCollecteConfigurationAsync),
            response.StatusCode,
            jdcCompteId,
            jdcFolderId,
            serializedReleve,
            responseBody);

            throw exception;
        }

        public async Task<string> UploadSignedMandat(string jdcFolderId, string jdcRibId, byte[] mandat)
        {
            using var client = this.factory.Create();

            var jdcCompteId = this.options.Value.JdcCompteId;
            var requestUri = $"compte/{jdcCompteId}/dossierClient/{jdcFolderId}/rib/{jdcRibId}/mandatSigne";

            var serializedMandatFile = Convert.ToBase64String(mandat);

            var content = new StringContent(
               serializedMandatFile,
               Encoding.UTF8);

            var response = await client.PostAsync(requestUri, content).ConfigureAwait(false);

            var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                return responseBody;
            }

            var exception = new JeDeclareApiException($"Exception was thrown : status code : {response.StatusCode} - Message : '{responseBody}'");

            this.logger.LogError(
            exception,
            "{class} - '{method}': Exception was thrown : status code : '{statusCode}' - jdcCompteId : '{jdcCompteId}' - jdcFolderId : '{jdcFolderId}' - jdcRibId : '{jdcRibId}' - Message : '{errorMessage}'",
            nameof(HttpJeDeclareClient),
            nameof(this.UploadSignedMandat),
            response.StatusCode,
            jdcCompteId,
            jdcFolderId,
            jdcRibId,
            responseBody);

            throw exception;
        }

        public async Task<bool> CheckSignedMandatExists(string jdcFolderId, string jdcRibId)
        {
            using var client = this.factory.Create();

            var jdcCompteId = this.options.Value.JdcCompteId;
            var requestUri = $"compte/{jdcCompteId}/dossierClient/{jdcFolderId}/rib/{jdcRibId}/mandatSigne";

            var request = new HttpRequestMessage(HttpMethod.Head, requestUri);

            var response = await client.SendAsync(request).ConfigureAwait(false);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                return true;
            }
            else if (response.IsSuccessStatusCode)
            {
                return false;
            }

            var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            var exception = new JeDeclareApiException($"Exception was thrown : status code : {response.StatusCode} - Message : '{responseBody}'");

            this.logger.LogError(
            exception,
            "{class} - '{method}': Exception was thrown : status code : '{statusCode}' - jdcCompteId : '{jdcCompteId}' - jdcFolderId : '{jdcFolderId}' - jdcRibId : '{jdcRibId}' - Message : '{errorMessage}'",
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
            // TODO Aspose
           throw new NotImplementedException();
        }
    }
}
