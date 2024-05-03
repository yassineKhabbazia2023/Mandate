// <copyright file="RecoveryFormIOFunctionTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.AzureFunctions.Tests.Functions
{
    using global::Mandate.AzureFunctions;
    using KPMG.Pulse.Back.Accounting.Mandate.Function.Functions;
    using Microsoft.Azure.WebJobs.Extensions.DurableTask;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;

    public class RecoveryFormIOFunctionTest
    {
        //private readonly Mock<IDurableOrchestrationClient> mockStarter;
        private readonly Mock<ILogger> mockLog;

        public RecoveryFormIOFunctionTest()
        {
            //this.mockStarter = new Mock<IDurableOrchestrationClient>(MockBehavior.Strict);
            this.mockLog = new Mock<ILogger>();
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
            string inst = "instanceId";
            var content = new StringContent(JsonConvert.SerializeObject(new RecoveryOrchestratorInput() { Skip = 10 }), Encoding.UTF8, "application/json");
            var httpRequestMessage = new HttpRequestMessage()
            {
                Content = content,
            };

            Environment.SetEnvironmentVariable("LimitRecoveryFormIo", "100");

            var mockStarter = new Mock<IDurableOrchestrationClient>(MockBehavior.Strict);
            mockStarter.Setup(s => s.StartNewAsync(
                    "RecoveryFormIoOrchestrator",
                    It.Is<RecoveryOrchestratorInput>(i => i.LimitConfig == 100 && i.Skip == 10)))
                .ReturnsAsync(inst);

            HttpResponseMessage httpResponse = new HttpResponseMessage();
            mockStarter.Setup(s => s.CreateCheckStatusResponse(httpRequestMessage, inst, false))
                .Returns(httpResponse);

            var response = await RecoveryFormIOFunction.HttpStart(httpRequestMessage, mockStarter.Object, this.mockLog.Object);

            mockStarter.Verify(s => s.StartNewAsync("RecoveryFormIoOrchestrator", It.IsAny<object>()), Times.Once);
        }
    }
}
