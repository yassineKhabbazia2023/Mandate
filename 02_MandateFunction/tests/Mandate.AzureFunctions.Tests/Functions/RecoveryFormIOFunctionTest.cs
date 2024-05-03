// <copyright file="RecoveryFormIOFunctionTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.AzureFunctions.Tests.Functions
{
    using global::Mandate.AzureFunctions;
    using KPMG.Pulse.Back.Accounting.Mandate.Function.Functions;
    using Microsoft.Azure.WebJobs.Extensions.DurableTask;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;

    public class RecoveryFormIOFunctionTest
    {
        private readonly Mock<IDurableOrchestrationClient> mockStarter;
        private readonly Mock<ILogger> mockLog;
        private readonly Mock<IConfiguration> mockConfiguration;

        public RecoveryFormIOFunctionTest()
        {
            this.mockStarter = new Mock<IDurableOrchestrationClient>();
            this.mockLog = new Mock<ILogger>();
            this.mockConfiguration = new Mock<IConfiguration>();
        }

        [Fact]
        public void Constructor()
        {
            var recoveryFormIo = new RecoveryFormIOFunction();
            recoveryFormIo.Should().NotBeNull();
        }

        [Fact]
        public async Task RecoveryFormIOFunction_HttpStart_ShouldStartOrchestrationAndReturnResponse()
        {
            var content = new StringContent(JsonConvert.SerializeObject(new RecoveryOrchestratorInput() { Skip = 0 }), Encoding.UTF8, "application/json");

            var httpRequestMessage = new HttpRequestMessage()
            {
                Content = content,
            };
            this.mockStarter.Setup(s => s.StartNewAsync(
                    "RecoveryFormIo",
                    It.Is<RecoveryOrchestratorInput>(i => i.LimitConfig == 100)))
                .ReturnsAsync("instanceId");

            this.mockConfiguration.Setup(c => c["LimitHttp"]).Returns("100");

            var response = await RecoveryFormIOFunction.HttpStart(httpRequestMessage, this.mockStarter.Object, this.mockLog.Object);

            this.mockStarter.Verify(s => s.StartNewAsync("RecoveryFormIoOrchestrator", It.IsAny<object>()), Times.Once);
        }
    }
}
