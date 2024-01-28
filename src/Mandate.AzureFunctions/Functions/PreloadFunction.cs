// <copyright file="PreloadFunction.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.AzureFunctions
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.Http;
    using Microsoft.Extensions.Logging;

    public class PreloadFunction
    {
        private readonly IPreloadManager preloadManager;

        public PreloadFunction(
             IPreloadManager preloadManager)
        {
            this.preloadManager = preloadManager;
        }

        [FunctionName("PreloadFunction")]
        public async Task PreloadFunctionAsync([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest myTimer, ILogger log)
        {
            try
            {
                log.LogInformation($"Start execution of the PreloadFunction function at: {DateTime.UtcNow}");

                var rib = new Client.Bban(
                    bankCode: "bankCodeM",
                    branchCode: "branchCodeM",
                    accountNumber: "accountNumberM",
                    checkDigits: "checkDigitsM");

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
