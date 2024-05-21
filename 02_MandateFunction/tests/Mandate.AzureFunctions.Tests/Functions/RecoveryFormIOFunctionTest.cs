// <copyright file="RecoveryFormIOFunctionTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

using Microsoft.Azure.Functions.Worker.Http;

namespace KPMG.Pulse.Back.Accounting.Mandate.AzureFunctions.Tests.Functions
{
    using global::Mandate.AzureFunctions;
    using KPMG.Pulse.Back.Accounting.Mandate.Function.Functions;
    using Microsoft.Azure.Functions.Worker;
    using Microsoft.Azure.Functions.Worker.Http;
    using Microsoft.DurableTask;
    using Microsoft.DurableTask.Client;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using System.Net;

    public class RecoveryFormIOFunctionTest
    {
        private readonly Mock<DurableTaskClient> mockStarter;
        private readonly Mock<ILogger> mockLog;

        public RecoveryFormIOFunctionTest()
        {
            this.mockStarter = new Mock<DurableTaskClient>("test");
            this.mockLog = new Mock<ILogger>();
        }

        [Fact]
        public void Constructor()
        {
            var recoveryFormIo = new RecoveryFormIOFunction();
            recoveryFormIo.Should().NotBeNull();
        }
    }
}