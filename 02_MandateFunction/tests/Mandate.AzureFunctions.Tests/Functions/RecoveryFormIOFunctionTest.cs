// <copyright file="RecoveryFormIOFunctionTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.AzureFunctions.Tests.Functions
{
    using KPMG.Pulse.Back.Accounting.Mandate.Function.Functions;
    using Microsoft.DurableTask.Client;
    using Microsoft.Extensions.Logging;

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