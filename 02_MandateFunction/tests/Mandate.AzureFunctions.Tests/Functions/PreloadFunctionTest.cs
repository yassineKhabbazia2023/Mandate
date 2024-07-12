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
        [Fact]
        public void Constructor()
        {
            var manager = new Mock<IPreloadManager>(MockBehavior.Strict);

            var preloadFunction = new PreloadFunction(manager.Object);

            preloadFunction.Should().NotBeNull();
        }

        [Fact]
        public async Task PreloadFunctionAsync()
        {
            var rib = new Client.Bban("bankCodeM", "branchCodeM", "accountNumberM", "checkDigitsM");

            var request = CreateHttpRequest(rib);

            var manager = new Mock<IPreloadManager>(MockBehavior.Strict);
            manager.Setup(m => m.GetRecoveryAsync(It.IsAny<Bban>()))
                .Callback<Bban>(b =>
                {
                    b.BankCode.Should().Be("bankCodeM");
                    b.BranchCode.Should().Be("branchCodeM");
                    b.AccountNumber.Should().Be("accountNumberM");
                    b.CheckDigits.Should().Be("checkDigitsM");
                })
                .Returns(Task.CompletedTask)
            .Verifiable();

            var logger = new Mock<ILogger>(MockBehavior.Strict);
            logger.Setup(x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsValueType>(),
                It.IsAny<Exception?>(),
                (Func<It.IsValueType, Exception?, string>)It.IsAny<object>())); // Ignore all logs

            var preloadFunction = new PreloadFunction(manager.Object);

            await preloadFunction.PreloadFunctionAsync(req: request, logger.Object);

            manager.VerifyAll();
            logger.VerifyAll();
        }

        [Fact]
        public void PreloadFunctionAsync_CaseThrowExecption()
        {
            var rib = new { jsonobject = string.Empty };

            var request = CreateHttpRequest(rib);

            var manager = new Mock<IPreloadManager>(MockBehavior.Strict);
            manager.Setup(m => m.GetRecoveryAsync(It.IsAny<Bban>()))
                .Callback<Bban>(b =>
                {
                    b.BankCode.Should().Be("bankCodeM");
                    b.BranchCode.Should().Be("branchCodeM");
                    b.AccountNumber.Should().Be("accountNumberM");
                    b.CheckDigits.Should().Be("checkDigitsM");
                })
                .Throws(new Exception())
            .Verifiable();

            var logger = new Mock<ILogger>(MockBehavior.Strict);
            logger.Setup(x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsValueType>(),
                It.IsAny<Exception?>(),
                (Func<It.IsValueType, Exception?, string>)It.IsAny<object>())); // Ignore all logs

            var preloadFunction = new PreloadFunction(manager.Object);

            Func<Task> action = async () => await preloadFunction.PreloadFunctionAsync(req: request, logger.Object);

            action.Should().ThrowAsync<Exception>();
            manager.VerifyAll();
            logger.VerifyAll();
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
