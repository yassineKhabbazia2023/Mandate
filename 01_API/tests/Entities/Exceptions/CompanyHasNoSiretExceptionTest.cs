// <copyright file="CompanyHasNoSiretExceptionTest.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Tests.Exceptions
{
    public class CompanyHasNoSiretExceptionTest
    {
        [Fact]
        public void Constructor_Message()
        {
            var ex = new CompanyHasNoSiretException("message");
            ex.Message.Should().Be("message");
        }
    }
}