// <copyright file="PreloadFunction.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.AzureFunctions
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
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
        public async Task PreloadFunctionAsync([TimerTrigger("0 */5 * * * *")]TimerInfo myTimer, ILogger log)
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

        [FunctionName("MountFunction")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            string responseMessage = string.IsNullOrEmpty(name)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {name}. This HTTP triggered function executed successfully.";

            return new OkObjectResult(responseMessage);
        }
    }
}
