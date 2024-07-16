// <copyright file="PreloadFunctionTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.AzureFunctions.Tests
{
    using KPMG.Pulse.Back.Accounting.Mandate.Client;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;

    public class PreloadFunctionTest
    {
        private Mock<ILogger<PreloadFunction>> logger;
        private Mock<IPreloadManager> manager;
        private PreloadFunction preloadFunction;

        public PreloadFunctionTest()
        {
            this.logger = new Mock<ILogger<PreloadFunction>>(MockBehavior.Strict);
            this.logger.Setup(x => x.Log(
               It.IsAny<LogLevel>(),
               It.IsAny<EventId>(),
               It.IsAny<It.IsValueType>(),
               It.IsAny<Exception?>(),
               (Func<It.IsValueType, Exception?, string>)It.IsAny<object>())); // Ignore all logs

            this.manager = new Mock<IPreloadManager>(MockBehavior.Strict);
            this.preloadFunction = new PreloadFunction(this.manager.Object, this.logger.Object);

        }

        [Fact]
        public void Constructor()
        {
            this.preloadFunction.Should().NotBeNull();
        }

        [Fact]
        public async Task PreloadFunctionAsync()
        {
            var rib = new Client.Bban("bankCodeM", "branchCodeM", "accountNumberM", "checkDigitsM");

            var request = CreateHttpRequest(rib);

            this.manager.Setup(m => m.GetRecoveryAsync(It.IsAny<Bban>()))
                .Callback<Bban>(b =>
                {
                    b.BankCode.Should().Be("bankCodeM");
                    b.BranchCode.Should().Be("branchCodeM");
                    b.AccountNumber.Should().Be("accountNumberM");
                    b.CheckDigits.Should().Be("checkDigitsM");
                })
                .Returns(Task.CompletedTask)
            .Verifiable();

            await this.preloadFunction.PreloadFunctionAsync(req: request);

            this.manager.VerifyAll();
        }

        [Fact]
        public void PreloadFunctionAsync_CaseThrowExecption()
        {
            var rib = new { jsonobject = string.Empty };

            var request = CreateHttpRequest(rib);

            this.manager.Setup(m => m.GetRecoveryAsync(It.IsAny<Bban>()))
                .Callback<Bban>(b =>
                {
                    b.BankCode.Should().Be("bankCodeM");
                    b.BranchCode.Should().Be("branchCodeM");
                    b.AccountNumber.Should().Be("accountNumberM");
                    b.CheckDigits.Should().Be("checkDigitsM");
                })
                .Throws(new Exception())
            .Verifiable();

            Func<Task> action = async () => await this.preloadFunction.PreloadFunctionAsync(req: request);

            action.Should().ThrowAsync<Exception>();
            this.manager.VerifyAll();
        }

        private static HttpRequest CreateHttpRequest(object body)
        {
            var context = new DefaultHttpContext();
            var request = context.Request;
            request.Body = new MemoryStream(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(body)));
            return request;
        }
    }
}
