// <copyright file="PreloadFunction.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.AzureFunctions
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using KPMG.Pulse.Back.Accounting.Mandate.Client;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.Http;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;

    public class PreloadFunction
    {
        private readonly IPreloadManager preloadManager;

        public PreloadFunction(
             IPreloadManager preloadManager)
        {
            this.preloadManager = preloadManager;
        }

        [FunctionName("PreloadFunction")]
        public async Task PreloadFunctionAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req,
            ILogger log)
        {
            try
            {
                log.LogInformation($"Start execution of the PreloadFunction function at: {DateTime.UtcNow}");
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                Bban rib = JsonConvert.DeserializeObject<Bban>(requestBody);
                await this.preloadManager.GetRecoveryAsync(rib);
            }
            catch (Exception ex)
            {
                log.LogError(ex, ex.Message);
                throw;
            }
        }
    }
}
