// <copyright file="MonitoringJdcStatusTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.AzureFunctions.Tests
{
    using System;
    using Microsoft.Extensions.Logging;

    public class MyProviderStub : IServiceProvider
    {
        public object? GetService(Type serviceType)
        {
            return new LoggerFactory();
        }
    }
}
