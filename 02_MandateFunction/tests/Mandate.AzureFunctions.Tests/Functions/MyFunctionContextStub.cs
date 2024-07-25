// <copyright file="MonitoringJdcStatusTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.AzureFunctions.Tests
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Azure.Functions.Worker;

    public class MyFunctionContextStub : FunctionContext
    {
        public override string InvocationId => throw new NotImplementedException();

        public override string FunctionId => throw new NotImplementedException();

        public override TraceContext TraceContext => throw new NotImplementedException();

        public override BindingContext BindingContext => throw new NotImplementedException();

        public override Microsoft.Azure.Functions.Worker.RetryContext RetryContext => throw new NotImplementedException();

        public override IServiceProvider InstanceServices
        {
            get
            {
                return new MyProviderStub();
            }
            set => throw new NotImplementedException();
        }

        public override FunctionDefinition FunctionDefinition => throw new NotImplementedException();

        public override IDictionary<object, object> Items { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override IInvocationFeatures Features => throw new NotImplementedException();
    }
}
