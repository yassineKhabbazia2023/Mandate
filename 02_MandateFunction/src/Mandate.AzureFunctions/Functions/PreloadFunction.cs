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
    using Microsoft.Azure.Functions.Worker;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;

    public class PreloadFunction
    {
        private readonly IPreloadManager preloadManager;
        private readonly ILogger<PreloadFunction> logger;

        public PreloadFunction(
            IPreloadManager preloadManager,
            ILogger<PreloadFunction> logger)
        {
            this.preloadManager = preloadManager;
            this.logger = logger;
        }

        [Function("PreloadFunction")]
        public async Task PreloadFunctionAsync([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req)
        {
            try
            {
                this.logger.LogInformation($"Start execution of the PreloadFunction function at: {DateTime.UtcNow}");
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                Bban rib = JsonConvert.DeserializeObject<Bban>(requestBody)!;
                await this.preloadManager.GetRecoveryAsync(rib!);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, ex.Message);
                throw;
            }
        }
    }
}
